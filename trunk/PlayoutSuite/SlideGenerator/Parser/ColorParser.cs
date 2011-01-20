using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SlideGeneratorLib.Parser
{
    class ColorParser
    {
        public static Color parse(String col)
        {
            byte a = 255;
            byte r = (byte)(Convert.ToUInt32(col.Substring(1, 2), 16));
            byte g = (byte)(Convert.ToUInt32(col.Substring(3, 2), 16));
            byte b = (byte)(Convert.ToUInt32(col.Substring(5, 2), 16));
            return Color.FromArgb(a, r, g, b);
        }
    }
}
