using System;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace KHUxWebCrawler
{
    class MainClass
    {
        static Dictionary<string, List<KHMedal>> Medals = new Dictionary<string, List<KHMedal>>();
        static List<string> Medal_Exclusion_List = new List<string>()
        {
            "Valentine Chocolate", "Moogle & Sora", ""
        };

        public static void Main(string[] args)
        {
            AsyncCrawl();
            Console.ReadLine();
        }

        public static async Task AsyncCrawl()
        {
            var url = "http://www.khunchainedx.com/wiki/";
            var httpClient = new HttpClient();

            // 3rd star medals start at 1
            // 4 star medals 2
            // 5 star medals 3
            // 6 star medals 5 and contain max mult
            // 7 star medals do not have base mult
            var html = await httpClient.GetStringAsync(url + "Album");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            //var tbody = htmlDocument.DocumentNode.SelectNodes("//tbody");
            var div = htmlDocument.DocumentNode.Descendants("table")
                                  .Where(node => node.GetAttributeValue("border", "").Equals("1")).FirstOrDefault();
            var nodes = div.Descendants("tr").ToList();

            foreach(var td in nodes)
            {
                var td_nodes = td.Descendants("td").ToList();//.ChildAttributes("href").FirstOrDefault().Value;
                if (td_nodes.Count == 0)
                    continue;

                //var name = td_nodes[1].InnerText.Substring(1, td_nodes[1].InnerText.Length - 2);
                var name = "";
                try
                {
                    name = td_nodes[1].Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Substring(6);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                var link = url + name;

                if(name == "Darkside" || name == "Behemoth")
                {
                    link += "_(Medal)";
                }
                //Console.WriteLine(link);

                try
                {
                    var medal = await AsyncMedalCrawl(link, name);

                    if (medal.Count > 0)
                    {
                        if (!Medals.ContainsKey(medal[0].Name))
                        {
                            //Console.WriteLine(name);
                            Medals.Add(medal[0].Name, medal);
                        }
                    }
                    else
                    {
                        Console.WriteLine(link);
                    }

                    using(StreamWriter writer = new StreamWriter("KHUx_Medal_Data.txt", true))
                    {
                        medal.ForEach(x => x.PrintWriteableData(writer));
                    }
                    //Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine(name);
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    Console.ReadLine();
                }
            }
            Console.WriteLine("Finished!");
        }

        public static async Task<List<KHMedal>> AsyncMedalCrawl(string link, string name)
        {
            var medals = new List<KHMedal>();

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(link);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var div = htmlDocument.DocumentNode.Descendants("div")
                                  .Where(node => node.GetAttributeValue("class", "").Equals("tabber")).FirstOrDefault();

            var sub_div = div.Descendants("div")
                             .Where(node => node.GetAttributeValue("class", "").Equals("tabbertab"));


            foreach(var d in sub_div)
            {
                int offset = 0;
                //Console.WriteLine(sub_div.ToList().Count);
                var star = d.GetAttributeValue("title", "")[0];
                //Console.WriteLine(d.GetAttributeValue("title", ""));
                if(star == 'E' || star == 'J')
                {
                    //offset = 1;
                    continue;
                }
                if(star == '★')
                {
                    star = '1';
                }

                var tr = d.Descendants("tr").ToList();
                var offset_calc = new List<string>();

                var readable_name = tr[0].Descendants("td")
                                         .Where(node => node.GetAttributeValue("align", "").Equals("center"))
                                         .FirstOrDefault().InnerHtml
                                         .Split(new string[] { "<br>" }, StringSplitOptions.None)[0]
                                         .Replace("<b>", "").Replace("</b>", "")
                                         .Replace(",", "");
                
                for (int i = 3; i < 7; ++i)
                {
                    var t = RemoveWhitespace(tr[i].Descendants("td").ToList()[0].InnerText);

                    if(t == "Kana" || t == "Romaji" || t == "JPSet" || t == "ENSet" || t == "NASet")
                    {
                        offset_calc.Add(t);
                    }
                }

                offset = 4 - offset_calc.Count;

                var imageURL = "NA";
                if (tr[2].Descendants("img").FirstOrDefault() != null)
                {
                    imageURL = tr[2].Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
                }
                var class_type_attributes = tr[8 - offset].Descendants("td").ToList();

                var medal = new KHMedal();
                var class_type = RemoveWhitespace(class_type_attributes[0].InnerText);
                var type = RemoveWhitespace(class_type_attributes[1].InnerText);
                //Console.WriteLine(class_type);
                if (type == "Attack" || type == "Rare" || type == "SuperRare")
                {
                    medal = CreateAttackMedal(tr, readable_name, star, imageURL, class_type_attributes, offset);
                }
                else if(type == "Sell" || type == "Evolve" || type == "EXP" || type == "Boost" || type == "Cost")
                {
                    medal = CreateSellEvolveEXPMedal(tr, readable_name, star, imageURL, class_type_attributes, offset);
                }
                if(class_type == "CommonEvolve")
                {
                    medal = CreateDalmationMedal(tr, readable_name, star, imageURL);
                }

                medals.Add(medal);
            }

            return medals;
        }

        public static KHMedal CreateAttackMedal(List<HtmlNode> tr, string name, char star, string imageURL, List<HtmlNode> class_type_attributes, int offset)
        {
            var str = RemoveWhitespace(tr[9 - offset].Descendants("td").ToList()[0].InnerText);
            var updated_offset = offset;

            var atk_def_trait_petpts = new List<HtmlNode>();
            var base_max_attack = new string[] { "TBD", "TBD" };
            var base_max_defense = new string[] { "TBD", "TBD" };
            var base_max_petpts = new string[] { "TBD", "TBD" };
            var trait_slots = "";

            if (str != "STR")
            {
                updated_offset += 2;
            }
            else
            {
                updated_offset = offset;

                atk_def_trait_petpts = tr[10 - updated_offset].Descendants("td").ToList();

                base_max_attack = Parser.ATKDEF(atk_def_trait_petpts[0].InnerText);
                base_max_defense = Parser.ATKDEF(atk_def_trait_petpts[1].InnerText);
                trait_slots = atk_def_trait_petpts[2].InnerText;
                base_max_petpts = Parser.PetPoints(atk_def_trait_petpts[3].InnerText);
            }

            // TODO Fix ability to include entire ability
            var ability = tr[11 - updated_offset].Descendants("a").FirstOrDefault().GetAttributeValue("title", "");
            //Console.WriteLine(ability);
            var ability_descr = tr[12 - updated_offset].Descendants("td").FirstOrDefault().InnerText;
            //Console.WriteLine(ability_descr);
            var target = RemoveWhitespace(tr[13 - updated_offset].Descendants("td").ToList()[3].InnerText);

            var base_damage_mult = "NA";
            var max_damage_mult = "NA";
            var guilt_damage_mult = "NA";
            var tier = "NA";
            var gauges = "NA";

            if(star == '1' || star == '2')
            {
                gauges = RemoveWhitespace(tr[14 - updated_offset].Descendants("td").ToList()[1].InnerText);
            }
            else
            {
                gauges = RemoveWhitespace(tr[14 - updated_offset].Descendants("td").ToList()[3].InnerText);
            }

            if (star >= '1' && star <= '6')
            {
                base_damage_mult = tr[13 - updated_offset].Descendants("td").ToList()[1].InnerText;
            }
            if (star >= '3' && star <= '6')
            {
                max_damage_mult = tr[14 - updated_offset].Descendants("td").ToList()[1].InnerText;
            }
            if (star == '6')
            {
                guilt_damage_mult = tr[15 - updated_offset].Descendants("td").ToList()[1].InnerText;
                tier = tr[15 - offset].Descendants("td").ToList()[3].InnerText;
            }
            if (star == '7')
            {
                max_damage_mult = tr[13 - updated_offset].Descendants("td").ToList()[1].InnerText;
                guilt_damage_mult = tr[14 - updated_offset].Descendants("td").ToList()[1].InnerText;
                //Console.WriteLine(guilt_damage_mult);
                tier = tr[15 - updated_offset].Descendants("td").ToList()[1].InnerText;
                //Console.WriteLine(tier);
            }

            var medal = new KHAttackMedal
            {
                Name = RemoveWhitespace(name).Replace("_", " "),
                Star = RemoveWhitespace(star.ToString()),
                ImageURL = RemoveWhitespace(imageURL),
                Class = RemoveWhitespace(class_type_attributes[0].InnerText),
                Type = RemoveWhitespace(class_type_attributes[1].InnerText),
                Attribute_PSM = RemoveWhitespace(class_type_attributes[2].InnerText),
                Attribute_UD = RemoveWhitespace(class_type_attributes[3].InnerText),
                BaseAttack = RemoveWhitespace(base_max_attack[0]).Replace("&#160;", ""),
                MaxAttack = RemoveWhitespace(base_max_attack[1]).Replace("&#160;", ""),
                BaseDefense = RemoveWhitespace(base_max_defense[0]).Replace("&#160;", ""),
                MaxDefense = RemoveWhitespace(base_max_defense[1]).Replace("&#160;", ""),
                TraitSlots = RemoveWhitespace(trait_slots),
                BasePetPoints = RemoveWhitespace(base_max_petpts[0]),
                MaxPetPoints = RemoveWhitespace(base_max_petpts[1]),
                Ability = ability,
                AbilityDescription = ability_descr.Remove(ability_descr.Length - 1).Replace("&#91;1&#93;", "").Replace(",", ""),
                BaseMultiplier = RemoveWhitespace(base_damage_mult).Replace("&#160;", ""),
                MaxMultiplier = RemoveWhitespace(max_damage_mult).Replace("&#160;", ""),
                Target = RemoveWhitespace(target),
                Gauge = RemoveWhitespace(gauges),
                GuiltMaxMultiplier = RemoveWhitespace(guilt_damage_mult).Replace("&#160;", ""),
                Tier = RemoveWhitespace(tier),
            };

            return medal;
        }

        public static KHMedal CreateSellEvolveEXPMedal(List<HtmlNode> tr, string name, char star, string imageURL, List<HtmlNode> class_type_attributes, int offset)
        {
            var effect = tr[9 - offset].Descendants("td").FirstOrDefault().InnerText;
            var effect_descr = tr[10 - offset].Descendants("td").FirstOrDefault().InnerText;

            var medal = new KHMiscMedal
            {
                Name = RemoveWhitespace(name).Replace("_", " "),
                Star = RemoveWhitespace(star.ToString()),
                ImageURL = RemoveWhitespace(imageURL),
                Class = RemoveWhitespace(class_type_attributes[0].InnerText),
                Type = RemoveWhitespace(class_type_attributes[1].InnerText),
                Attribute_PSM = RemoveWhitespace(class_type_attributes[2].InnerText),
                Attribute_UD = RemoveWhitespace(class_type_attributes[3].InnerText),
                Effect = RemoveWhitespace(effect),
                Effect_Description = effect_descr.Remove(effect_descr.Length - 1).Replace("&#91;1&#93;", "").Replace(",", ""),
            };

            return medal;
        }

        private static KHMedal CreateDalmationMedal(List<HtmlNode> tr, string name, char star, string imageURL)
        {
            var class_type = tr[8].Descendants("td").ToList()[0].InnerText.Split('\n');
            var attribute = tr[8].Descendants("td").ToList()[1].InnerText.Split('\n');
            var effect = tr[9].Descendants("td").FirstOrDefault().InnerText;
            var effect_descr = tr[10].Descendants("td").FirstOrDefault().InnerText;

            var medal = new KHMiscMedal
            {
                Name = RemoveWhitespace(name).Replace("_", " "),
                Star = RemoveWhitespace(star.ToString()),
                ImageURL = RemoveWhitespace(imageURL),
                Class = RemoveWhitespace(class_type[0]),
                Type = RemoveWhitespace(class_type[1]),
                Attribute_PSM = RemoveWhitespace(attribute[0]),
                Attribute_UD = RemoveWhitespace(attribute[1]),
                Effect = RemoveWhitespace(effect),
                Effect_Description = effect_descr.Remove(effect_descr.Length - 1).Replace("&#91;1&#93;", ""),
            };

            return medal;
        }

        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
