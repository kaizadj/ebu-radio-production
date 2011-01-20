using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Windows.Controls;

namespace ContentServiceLibrary
{

    [ServiceContract(CallbackContract = typeof(IContentServiceCallback))]
    public interface IContentService
    {
        /** BROADCAST / PREVIEW ACTIONS **/

        [OperationContract]
        String broadcast(String slidekey);

        [OperationContract]
        String getPreview(String slidekey);

        /** AVAILABLE SLIDES **/
        [OperationContract(Name="getAvailableSlides1")]
        List<String> getAvailableSlides();

        [OperationContract(Name="getAvailableSlides2")]
        List<String> getAvailableSlides(String prefix);
        
        /** AUTO BROADCAST ENABLED **/
        [OperationContract]
        void setAutoBroadcast(Boolean isEnabled);

        [OperationContract]
        Boolean getAutoBroadCastEnabled();

        /** AUTO BROADCAST OPTIONS **/
        [OperationContract]
        void setAutoBroadcastParameters(TimeSpan Interval, List<String> slidekeys);

        [OperationContract]
        List<String> getAutoBroadcastSlides();

        [OperationContract]
        TimeSpan getAutoBroadcastInterval();

        /** DYNAMIC VARS **/
        [OperationContract]
        void setVar(String key, String content);

        [OperationContract]
        String getVar(String key);
        
        /** STATIC VARS **/
        [OperationContract]
        String getTmpFolder();


    }
}
