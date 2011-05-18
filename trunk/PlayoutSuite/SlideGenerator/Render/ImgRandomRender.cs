using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.IO;
using SlideGeneratorLib.Parser;
using System.Threading;
using System.Net;

namespace SlideGeneratorLib.Rendering
{
    class ImgRandomRender:ARender
    {
        public ImgRandomRender(Dictionary<string, string> dic) : base("imgrdm") { this.dictionary = dic; this.rand = new Random(DateTime.Now.Second); }
        private Dictionary<string, string> dictionary;
        private Random rand;
        public override void draw(XElement field, Canvas c)
        {
            DateTime n = DateTime.Now;
            if (field.Attribute("folder") != null)
            {
                System.Windows.Controls.Image box = new System.Windows.Controls.Image();
                RenderOptions.SetBitmapScalingMode(box, BitmapScalingMode.Fant);
                String path = VarParser.parseText(field.Attribute("folder").Value, dictionary);
                String exactpath = folderexists(path);
                Console.WriteLine("PATH : --- " + exactpath);
                if (exactpath != "-1")
                {
                    Console.WriteLine("FILE OK");
                    try
                    {
                        Console.WriteLine("RDMIMG: " + exactpath + " path:" + path);
                        if (exactpath.StartsWith("http://"))
                        {
                            String filename=System.Configuration.ConfigurationSettings.AppSettings["TmpFolder"] + "test-"+DateTime.Now.ToFileTime()+".jpg";
                            try
                            {
                                WebClient Client = new WebClient();
                                
                                Client.DownloadFile(new Uri(exactpath), filename);
                                Boolean ok = false;
                                while (!ok)
                                {
                                    try
                                    {
                                        Uri u = new Uri(filename, UriKind.RelativeOrAbsolute);
                                        BitmapImage i = new BitmapImage(u);
                                        
                                        box.Source = i;
                                        box.Stretch = System.Windows.Media.Stretch.Fill;

                                        

                                        addToCanvas(field, box, c);
                                        File.Delete(filename);
                                        Console.WriteLine("ok");
                                        ok = true;
                                    }
                                    catch(Exception e)
                                    {
                                     if(!e.Message.EndsWith("because it is being used by another process."))
                                         ok = true;
                                         Console.WriteLine("Error downloading file " + exactpath + " to " + filename);
                                         Console.WriteLine(e.Message);
                                    }
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Error downloading file " + exactpath + " to " + filename);
                                Console.WriteLine(e.Message);
                            }


                        }
                        else
                        {

                            Uri u = new Uri(exactpath, UriKind.RelativeOrAbsolute);
                            BitmapImage i = new BitmapImage(u);
                            box.Source = i;

                            box.Stretch = System.Windows.Media.Stretch.Fill;

                            addToCanvas(field, box, c);
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[IMG TAG ERROR] Unable to load src={" + exactpath + "}");
                    }
                }
                else
                {
                    Console.WriteLine("File " + path + " not found!!!");
                }


                Console.WriteLine("Component " + field.Value + " generated in " + DateTime.Now.Subtract(n).TotalMilliseconds + "ms");
            }
        }
        private String folderexists(string path)
        {
            if (Directory.Exists(path))
            {
                
                String[] files = Directory.GetFiles(System.IO.Path.GetFullPath(path));
                //path = System.IO.Path.GetFullPath(path);
                if (files.Length > 0)
                {
                    int n = rand.Next(files.Length);

                    String file = files[n];
                    if (file != "" && file != null) return file;
                    else return "-1";
                }
                else
                {
                    Console.WriteLine("[IMGRDM] folder empty");
                    return "-1";
                }
            }
            else
            {
                Console.WriteLine("[IMGRDM] folder not found");
                return "-1";
            }

           
        }
        private String fileexists(string path)
        {
            if (path.IndexOf("http://") == 0)
            {
                if (path.Contains("?"))
                    return path;
                else
                    return path + "?" + DateTime.Now.ToFileTime();
            }


            try
            {
                String[] files = Directory.GetFiles(System.IO.Path.GetDirectoryName(path));
                path = System.IO.Path.GetFullPath(path);
                String file = Array.Find(files, s => (s.ToLower() == path.ToLower()));
                if (file != "" && file != null) return file;
                else return "-1";
            }
            catch { 
                Console.WriteLine("File not found:" + path);
                return "-1"; 
            }
        }

        public override void loadConfig(XElement config)
        {
            //Nothing to load
        }
    }
}
