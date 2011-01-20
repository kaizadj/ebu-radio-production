using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlideGeneratorLib.Exceptions
{
    class XMLNotRecognizedElement : Exception
    {
        public XMLNotRecognizedElement(){ }
        public XMLNotRecognizedElement(String msg):base(msg) { }
    }
}
