using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlideGeneratorLib.Exceptions
{
    class XMLSynthaxErrorException : Exception
    {
        public XMLSynthaxErrorException(String msg) : base(msg) {  }
    }
}
