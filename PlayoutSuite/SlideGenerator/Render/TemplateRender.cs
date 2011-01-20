using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlideGeneratorLib.Parser;
using System.Xml.Linq;
using SlideGeneratorLib.Exceptions;

namespace SlideGeneratorLib.Rendering
{
    class TemplateRender:ARender
    {
        private Dictionary<String, LinkedList<XElement>> templates = new Dictionary<string, LinkedList<XElement>>();
        public TemplateRender()
            : base("template")
        {
        }
        public override void draw(System.Xml.Linq.XElement e, System.Windows.Controls.Canvas c)
        {
            String id = e.Attribute("id").Value;
            try{
                LinkedList<XElement> list = templates[id];
                foreach(XElement l in list){
                    Console.WriteLine(l);
                    e.AddAfterSelf(l);
                }
            }
            catch (KeyNotFoundException err)
            {
                throw new XMLNotRecognizedElement("template "+id+" not found");
            }
        }

        public override void loadConfig(XElement config)
        {

            Console.WriteLine("****************\nParse config for Templates");
            try
            {
                VarParser.XMLrequire(config, "conf");
                
                IEnumerable<XElement> elements =
                from el in config.Elements()
                where el.Name == "includes"
                select el;
                Console.WriteLine("XML :" + elements.First().Name);

                XElement econf = elements.First();
                VarParser.XMLrequire(econf, "includes");
                
                elements =
                from el in econf.Elements()
                where el.Name == "template"
                select el;

                for (int i = 0; i < elements.Count(); i++)
                {
                    XElement t = XElement.Load(elements.ElementAt(i).Attribute("src").Value);
                    
                    VarParser.XMLrequire(t, "templates");
                    IEnumerable<XElement> tmpl =
                    from el in t.Elements()
                    where el.Name == "template"
                    select el;

                    Console.WriteLine("first:"+tmpl.Count());
                    for (int j = 0; j < tmpl.Count(); j++)
                    {
                        String key = tmpl.ElementAt(j).Attribute("id").Value;
                        Console.WriteLine("NEW TEMPLATE " + key + "");
                        LinkedList<XElement> l = new LinkedList<XElement>();
                        foreach (XElement h in tmpl.ElementAt(j).Elements())
                        {
                            l.AddFirst(h);
                        }
                        templates.Add(key, l);
                    }

                }

            }
            catch (Exception er)
            {
                Console.WriteLine("XMLNotRecognizedElement :" + er.Message);
                Console.WriteLine("FATAL ERROR: exit(1)");
                Environment.Exit(1);
            }

        }
    }
}
