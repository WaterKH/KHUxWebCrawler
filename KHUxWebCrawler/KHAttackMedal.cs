using System;
using System.IO;

namespace KHUxWebCrawler
{
    public class KHAttackMedal : KHMedal
    {
        public string BaseAttack { get; set; }
        public string MaxAttack { get; set; }
        public string BaseDefense { get; set; }
        public string MaxDefense { get; set; }
        public string TraitSlots { get; set; }
        public string BasePetPoints { get; set; }
        public string MaxPetPoints { get; set; }
        public string Ability { get; set; }
        public string AbilityDescription { get; set; }
        public string BaseMultiplier { get; set; }
        public string MaxMultiplier { get; set; }
        public string Target { get; set; }
        public string Gauge { get; set; }
        public string GuiltMaxMultiplier { get; set; }
        public string Tier { get; set; }

        public override void PrintReadableData()
        {
            Console.WriteLine();
            Console.WriteLine("Name: " + Name);
            Console.WriteLine("ImageURL: " + ImageURL);
            Console.WriteLine("Star: " + Star);
            Console.WriteLine("Class: " + Class);
            Console.WriteLine("Type: " + Type);
            Console.WriteLine("Attribute_PSM: " + Attribute_PSM);
            Console.WriteLine("Attribute_UD: " + Attribute_UD);
            Console.WriteLine("BaseAttack: " + BaseAttack);
            Console.WriteLine("MaxAttack: " + MaxAttack);
            Console.WriteLine("BaseDefense: " + BaseDefense);
            Console.WriteLine("MaxDefense: " + MaxDefense);
            Console.WriteLine("TraitSlots: " + TraitSlots);
            Console.WriteLine("BasePetPoints: " + BasePetPoints);
            Console.WriteLine("MaxPetPoints: " + MaxPetPoints);
            Console.WriteLine("Ability: " + Ability);
            Console.WriteLine("Description: " + AbilityDescription);
            Console.WriteLine("BaseMultiplier: " + BaseMultiplier);
            Console.WriteLine("MaxMultiplier: " + MaxMultiplier);
            Console.WriteLine("Target: " + Target);
            Console.WriteLine("Gauge: " + Gauge);
            Console.WriteLine("GuiltMaxMultiplier: " + GuiltMaxMultiplier);
            Console.WriteLine("Tier: " + Tier);
            Console.WriteLine();
        }

        public override void PrintWriteableData(StreamWriter writer)
        {
            writer.Write(Name + "," + ImageURL + "," + Star + "," + Class + "," + Type + "," + Attribute_PSM + "," + Attribute_UD + "," +
                         BaseAttack + "," + MaxAttack + "," + BaseDefense + "," + MaxDefense + "," + TraitSlots + "," + BasePetPoints + "," +
                         MaxPetPoints + "," + Ability + "," + AbilityDescription + "," + BaseMultiplier + "," + MaxMultiplier + "," +
                         Target + "," + Gauge + "," + GuiltMaxMultiplier + "," + Tier);
            writer.WriteLine();
        }
    }
}
