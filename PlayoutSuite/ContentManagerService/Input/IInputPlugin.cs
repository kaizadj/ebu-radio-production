using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerService
{
    public interface IInputPlugin
    {
        void setup(Dictionary<String, String> variableList);
        Boolean start();
        Boolean stop();
        String getPluginType();
        
    }
}
