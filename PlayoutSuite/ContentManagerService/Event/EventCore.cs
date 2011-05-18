using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerService
{
    public class EventCore
    {
        public event InputRefreshVariables inputRefreshVariables;
        public delegate void InputRefreshVariables(object sender, InputEventArgs eventArgs);



    }
    public class InputEventArgs : EventArgs{}
}
