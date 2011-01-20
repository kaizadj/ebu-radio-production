using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;

namespace SlideGeneratorLib.Rendering
{
    interface IRenderable
    {
        void draw(XElement e, Canvas c);
        String getTag();
        void loadConfig(XElement config);
    }
}
