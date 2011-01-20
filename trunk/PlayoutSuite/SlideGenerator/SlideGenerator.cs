using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using SlideGeneratorLib.Exceptions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Collections.Generic;
using SlideGeneratorLib.Rendering;
using SlideGeneratorLib.Parser;
using System.Threading;

namespace SlideGeneratorLib
{
    /// <summary>
    /// init Synthax:
    ///        slideGen = new SlideGenerator("D:\\VSPRJ\\EBU\\EBUPrj\\EBUtests\\bin\\Debug\\conf.xml");
    ///        slideGen.newTag("text", new TextRender());
    ///        slideGen.newTag("img", new ImgRender());
    ///        slideGen.newTag("background", new BackgroundRender());
    ///        slideGen.newTag("template", new TemplateRender());
    /// </summary>
    public class SlideGenerator
    {
        Dictionary<String, IRenderable> tags;
        XElement config;
        int cWidth = 320;
        int cHeight = 240;
        String slidefolder = "";
        Dictionary<String,String> slides;
        public Dictionary<String, String> cstlist;
        public string tmpfolder;
        

        public SlideGenerator(string urlconfig)
        {
            cstlist = new Dictionary<string, string>();
            tags = new Dictionary<String, IRenderable>();
            slides = new Dictionary<string, string>();
            Console.WriteLine("XML: Loading config file " + urlconfig);
            
            try
            {
                this.config = XElement.Load(urlconfig);
                this.tmpfolder = config.Attribute("tmpfolder").Value;
                Console.WriteLine("TMP FOLDER: " + this.tmpfolder);
                //loadRenderingConfig();
                init();

                loadSlides();
            }
            catch (Exception er)
            {
                Console.WriteLine("Unable to open " + urlconfig + "");
            }
        }

        private void init()
        {
            loadConstants();
            Console.WriteLine("Loading text tag");
            this.newTag("text", new TextRender(this));
            Console.WriteLine("Loading img tag");
            this.newTag("img", new ImgRender(this.cstlist));
            Console.WriteLine("Loading background tag");
            this.newTag("background", new BackgroundRender());
            Console.WriteLine("Loading template tag");
            this.newTag("template", new TemplateRender());
        }

        private void loadConstants()
        {

            try
            {

                VarParser.XMLrequire(config, "conf");
                IEnumerable<XElement> elements =
                from el in config.Elements()
                where el.Name == "constants"
                select el;
                XElement econf = elements.First();
                VarParser.XMLrequire(econf, "constants");


                List<XElement> vars = econf.Elements().ToList();
                Console.WriteLine(vars.Count + " elements");
                for (int i = 0; i < vars.Count; i++)
                {
                    String key = vars.ElementAt(i).Name.ToString().ToUpper();
                    String val = vars.ElementAt(i).Value;
                    Console.WriteLine(key + " added to cstList with val=" + val);
                    cstlist.Add(key, val);
                }
            }
            catch (XMLNotRecognizedElement er)
            {
                Console.WriteLine("XMLNotRecognizedElement :" + er.Message);
                Console.WriteLine("FATAL ERROR: exit(1)");
                Environment.Exit(1);
            }
        }

        private void loadSlides()
        {
            loadSlideFolder();
            DirectoryInfo i = new DirectoryInfo(slidefolder);
            Console.WriteLine("Available slides:");
            slides.Clear();
            foreach(FileInfo f in i.EnumerateFiles("*.xml"))
            {
                String slidename = f.Name.Substring(0, f.Name.Length - 4);
                String err = "";
                if (slidename.Contains("."))
                    err = "=>ERROR FILENAME CONTAINS A POINT -> not loaded";
                else
                    slides.Add(slidename, f.FullName);
                Console.WriteLine(slidename+err);
                
            }
        }

        private void loadRenderingConfig(){
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

        private void loadSlideFolder()
        {
            VarParser.XMLrequire(config, "conf");

            IEnumerable<XElement> elements =
            from el in config.Elements()
            where el.Name == "includes"
            select el;

            XElement econf = elements.First();
            VarParser.XMLrequire(econf, "includes");

            elements =
            from el in econf.Elements()
            where el.Name == "slidefolder"
            select el;

            if(elements.Count() ==1)
                if (elements.First().Attribute("src") != null)
                {
                    slidefolder = elements.First().Attribute("src").Value.ToString();
                    if (slidefolder.Last() != '\\')
                        slidefolder = slidefolder + "\\";
                    Console.WriteLine("NEW SlideFolder: " + slidefolder);
                }

        }

        private void newTag(String tag, IRenderable renderengine)
        {
            tags.Add(tag, renderengine);
            renderengine.loadConfig(config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">slide key for slides Dictionary (if name contains a "." interpreted as an url)</param>
        /// <returns></returns>
        public Canvas loadXMLSlide(String name)
        {
            DateTime n = DateTime.Now;
            Canvas c = new Canvas();
            String url="";
            Console.WriteLine("search for :" + name);
            try
            {
                if (name.Contains("."))
                    url = name;
                else
                    url = slides[name];
            }
            catch { return null;  }
            Console.WriteLine("search ok!");
            c.Width = this.cWidth;
            c.Height = this.cHeight;
            c.Background = System.Windows.Media.Brushes.LightGray;
            Console.WriteLine("url:"+url);
            XElement e = null;
            try
            {
                e = XElement.Load(url);
            }
            catch (Exception er)
            {
                Console.WriteLine("=> Unable to open " + url + "");
                return null;
            }

            VarParser.XMLrequire(e, "sequence");


            Console.WriteLine(e.Elements().Count());
            List<XElement> slideList = e.Elements().ToList();

            for (int i = 0; i < slideList.Count; i++)
            {

                List<XElement> elementList = slideList.ElementAt(i).Elements().ToList();
                Console.WriteLine("slide " + i + " has " + elementList.Count + " elements");
                for (int j = 0; j < elementList.Count; j++)
                {
                    try
                    {
                        XElement elem = elementList.ElementAt(j);
                        Console.WriteLine("rendering element " + j + " of slide " + i + " " + elem.Name.ToString());
                        drawXElement(elem, c);
                    }
                    catch (XMLNotRecognizedElement er)
                    {
                        Console.WriteLine("XMLNotRecognizedElement : " + er.Message);
                    }
                    elementList = slideList.ElementAt(i).Elements().ToList();
                }

            }
            
            RectangleGeometry r = new RectangleGeometry(new Rect(0, 0, 320, 240));
            c.Clip = r;
            Console.WriteLine("Slide " + name + " generated in "+DateTime.Now.Subtract(n).TotalMilliseconds+"ms");

            return c;
        }

        private void drawXElement(XElement e, Canvas c)
        {

            String tag = e.Name.ToString();
            try
            {
                IRenderable engine = tags[tag];
                engine.draw(e, c);

            }
            catch (KeyNotFoundException err)
            {
                throw new XMLNotRecognizedElement("Engine for tag:{" + tag + "} not found");
            }

        }



        /// <summary>
        /// Save an UIElement to a png file
        /// </summary>
        /// <param name="source">Element to save</param>
        /// <param name="filename">png filename</param>
        public void saveToPng(UIElement source, String filename)
        {
            source.Measure(new System.Windows.Size(320.0, 240.0));
            source.Arrange(new System.Windows.Rect(0.0, 0.0, 320.0, 240.0));
            RenderTargetBitmap rtbImage = new RenderTargetBitmap(320,
               240,
               96,
               96,
               PixelFormats.Pbgra32);
            rtbImage.Render(source);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtbImage));

            using (Stream stm = File.Create(filename))
            {
                png.Save(stm);
            }

        }

        public void saveToJpg(UIElement source, String filename)
        {
            
            source.Measure(new System.Windows.Size(320.0, 240.0));
            source.Arrange(new System.Windows.Rect(0.0, 0.0, 320.0, 240.0));
            RenderTargetBitmap rtbImage = new RenderTargetBitmap(320,
               240,
               96,
               96,
               PixelFormats.Pbgra32);
            rtbImage.Render(source);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            
            BitmapFrame outputFrame = BitmapFrame.Create(rtbImage);
            encoder.Frames.Add(outputFrame);
            encoder.QualityLevel = 100;
            try
            {
                using (FileStream file = File.OpenWrite(filename))
                {
                    encoder.Save(file);
                }
            }
            catch
            {
                Console.WriteLine("Error writting files");
            }
        }


        public List<string> getAvailableSlides()
        {
            this.loadSlides();
            return this.slides.Keys.ToList();
        }
        public List<string> getAvailableSlides(String prefix)
        {
            this.loadSlides();
            return this.slides.Keys.TakeWhile(key => key.IndexOf(prefix) == 0).ToList();
        }

        public void setVar(string key, string content)
        {
            key = key.ToUpper();
            try
            {
                Console.WriteLine("SET " + key + " = " + content);
                if (!cstlist.ContainsKey(key))
                    cstlist.Add(key, content);
                else
                    cstlist[key] = content;
            }
            catch { Console.WriteLine(key + " not found"); }
        }
       
        public string getVar(string key)
        {

            key = key.ToUpper();
            try
            {
                return cstlist[key];
            }
            catch { Console.WriteLine(key +" not found"); }
            return "-- error --";
        }
    }
}


