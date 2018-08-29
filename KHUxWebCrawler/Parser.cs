using System;
using System.Collections.Generic;

namespace KHUxWebCrawler
{
    public static class Parser
    {
        public static string[] ATKDEF(string AtkDef)
        {
            var parsed_AtkDef = AtkDef.Split('/');

            if (parsed_AtkDef.Length != 2)
                return new string[] { AtkDef, AtkDef };
            //Console.WriteLine(baseAtkDef[0] + " " + baseAtkDef[1]);
            parsed_AtkDef[0] = parsed_AtkDef[0].Replace("&nbsp;", "").Replace(" ", "");
            parsed_AtkDef[1] = parsed_AtkDef[1].Replace(" ", "");

            return parsed_AtkDef;
        }

        public static string[] PetPoints(string petPts)
        {
            var parsed_petPts = petPts.Split('/');

            if (parsed_petPts.Length != 2)
                return new string[] { petPts, petPts };
            //Console.WriteLine(baseAtkDef[0] + " " + baseAtkDef[1]);
            parsed_petPts[0] = parsed_petPts[0].Replace("&nbsp;", "").Replace(" ", "");
            parsed_petPts[1] = parsed_petPts[1].Replace(" ", "");

            return parsed_petPts;
        }
    }
}
