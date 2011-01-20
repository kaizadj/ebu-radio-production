using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using SlideGeneratorLib.Exceptions;

namespace SlideGeneratorLib.Rendering 
{
    abstract class ARender : IRenderable
    {
        private String tag;

        public ARender(string tag)
        {
            this.tag = tag;
        }
        
        abstract public void draw(XElement e, Canvas c);
        abstract public void loadConfig(XElement config);

        public String getTag()
        {
            return tag;
        }

        protected void addToCanvas(XElement e, FrameworkElement uielem, Canvas c)
        {

            /** Size **/
            if (e.Attribute("width") != null)
                uielem.Width = Double.Parse(e.Attribute("width").Value.ToString());
            if (e.Attribute("height") != null)
                uielem.Height = Double.Parse(e.Attribute("height").Value.ToString());

            /** Opacity **/
            if (e.Attribute("opacity") != null)
                uielem.Opacity = Double.Parse(e.Attribute("opacity").Value.ToString()) / 100.0;
            
            c.Children.Add(uielem);
            
            if (e.Attribute("top") != null)
                Canvas.SetTop(uielem, Double.Parse(e.Attribute("top").Value.ToString()));
            else if (e.Attribute("bottom") != null)
                Canvas.SetBottom(uielem, Double.Parse(e.Attribute("bottom").Value.ToString()));

            if (e.Attribute("left") != null)
                Canvas.SetLeft(uielem, Double.Parse(e.Attribute("left").Value.ToString()));
            else if (e.Attribute("right") != null)
                Canvas.SetRight(uielem, Double.Parse(e.Attribute("right").Value.ToString()));
            

            
        }



    }
}
