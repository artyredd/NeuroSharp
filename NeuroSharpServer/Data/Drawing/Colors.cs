using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroSharpServer
{
    public static class Colors
    {
        public const string Black = "rgb(0,0,0)";
        public const string White = "rgb(255,255,255)";
        public const string Grey = "rgb(200,200,200)";
        public const string Red = "rgb(255,0,0)";
        public const string Orange = "rgb(255,165,0)";
        public const string Yellow = "rgb(255,255,0)";
        public const string Green = "rgb(0,128,0)";
        public const string Blue = "rgb(0,0,255)";
        public const string Ididgo = "rgb(75,0,130)";
        public const string Violet = "rgb(238,130,238)";

        public static string[] Rainbow { get; } = { Red, Orange, Yellow, Green, Blue, Ididgo, Violet };

        public static string Color(int r, int g, int b)
        {
            return $"rgb({r},{g},{b})";
        }

        public static string Color(float r, float g, float b)
        {
            return $"rgb({r},{g},{b})";
        }
    }
}
