using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using SlideGeneratorLib.Parser;

namespace SlideGeneratorLib.Rendering
{
    class BackgroundRender:ARender
    {
        public BackgroundRender() : base("background") { }

        Brush bgcolor = Brushes.Transparent;
        int cWidth = 320;
        int cHeight = 320;
        public override void draw(System.Xml.Linq.XElement e, System.Windows.Controls.Canvas c)
        {
            if (e.Attribute("color") != null)
            {
                String col = e.Attribute("color").Value;
                Console.WriteLine("BGCOL : " + col);
                Rectangle rect = new Rectangle();
                rect.Height = cHeight;//HARD CODED BUG
                rect.Width = cWidth;
                rect.Fill = new SolidColorBrush(ColorParser.parse(col));
                addToCanvas(e, rect, c);
            }
        }

        public override void loadConfig(System.Xml.Linq.XElement config)
        {
            IEnumerable<XElement> elements =
            from el in config.Elements()
            where el.Name == "render"
            select el;
           
            XElement erender = elements.First();
            VarParser.XMLrequire(erender, "render");

            if (erender.Attribute("width") != null)
                this.cWidth = (int)erender.Attribute("width");
            if (erender.Attribute("height") != null)
                this.cHeight = (int)erender.Attribute("height");
        }
    }
}
