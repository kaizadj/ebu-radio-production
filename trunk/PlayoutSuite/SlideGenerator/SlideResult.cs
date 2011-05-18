using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SlideGeneratorLib
{
    public class SlideResult
    {
        public SlideResult(String name, String xmlpath, Canvas image, String text, String link)
        {
            this.name = name;
            this.xmlpath = xmlpath;
            this.image = image;
            this.text = text;
            this.link = link;
        }

        public string name { get; set; }

        public string xmlpath { get; set; }

        public Canvas image { get; set; }

        public string text { get; set; }
        
        public string link { get; set; }
    }
}
