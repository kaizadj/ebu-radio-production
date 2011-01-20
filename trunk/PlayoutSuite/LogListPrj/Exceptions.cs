using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogListPrj.Exceptions
{
    public class LogException : Exception { }
    public class EmptySliceException : Exception { }
    public class NotFoundSlotException : Exception { }
    public class BadDataTypeException : Exception { }
}
