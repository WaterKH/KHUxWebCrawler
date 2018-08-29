using System;
using System.IO;

namespace KHUxWebCrawler
{
    public class KHMiscMedal : KHMedal
    {
        public string Effect { get; set; }
        public string Effect_Description { get; set; }

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
            Console.WriteLine("Effect: " + Effect);
            Console.WriteLine("Description: " + Effect_Description);
            Console.WriteLine();
        }

        public override void PrintWriteableData(StreamWriter writer)
        {
            writer.Write(Name + "," + ImageURL + "," + Star + "," + Class + "," + Type + "," + Attribute_PSM + "," + Attribute_UD + "," +
                         Effect + "," + Effect_Description);
            writer.WriteLine();
        }
    }
}
