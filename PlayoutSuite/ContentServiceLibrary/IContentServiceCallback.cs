using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ContentServiceLibrary
{
    
    public interface IContentServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void OnUpdate(String name, String value);
        
        [OperationContract(IsOneWay = true)]
        void OnOnAirChange(String onairslidekey);
    }
}
