using System;
using System.IO;

namespace KHUxWebCrawler
{
    public class KHMedal
    {
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Star { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public string Attribute_PSM { get; set; }
        public string Attribute_UD { get; set; }

        public virtual void PrintReadableData()
        {
            Console.WriteLine("Base Class");
        }

        public virtual void PrintWriteableData(StreamWriter writer)
        {
            Console.WriteLine("Base Class");
        }
    }
}
