using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SlideGeneratorLib.Exceptions;

namespace SlideGeneratorLib.Parser
{
    public class VarParser
    {
        public static String parseText(String str, Dictionary<String, String> dic)
        {
            while (str.IndexOf("@@@") != -1)
            {

                int pstart = str.IndexOf("@@@") + 3;
                int pend = str.IndexOf("@@@", pstart);
                if (pend == -1)
                {
                    throw new XMLSynthaxErrorException("@@@{varname}@@@ tag error. Check text element {content='" + str + "'}");
                }
                String var = str.Substring(pstart, pend - pstart);
               // Console.WriteLine("VAR: " + var);
                String val = "--Error--";
                if (var == "DATE")
                {
                    val = DateTime.Now.ToLongDateString();
                }
                else if (var == "SHORTDATE")
                {
                    val = DateTime.Now.ToShortDateString();
                }
                else if (var == "TIME")
                {
                    val = DateTime.Now.ToLongTimeString();
                }
                else if (var == "SHORTTIME")
                {
                    val = DateTime.Now.ToShortTimeString();
                }
                else
                {
                    try
                    {
                        val = dic[var.ToUpper()];
                       // Console.WriteLine("VAL: " + val);
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine("Key " + var + " not found: " + e.Message);
                    }
                }
                str = str.Replace("@@@" + var + "@@@", val);
            }


            return str;
        }

        internal static void XMLrequire(XElement xe, string p)
        {
            if (xe == null)
            {
                throw new XMLNotRecognizedElement("XML ERROR required:" + p + " found:xe == null");
            }
            if (xe.Name == null)
            {
                throw new XMLNotRecognizedElement("XML ERROR required:" + p + " required found:xe.Name == null");

            }
            if (xe.Name != p)
            {
                throw new XMLNotRecognizedElement("XML ERROR required:" + p + " found:" + xe.Name.ToString() + "");

            }
        }
    }
}
