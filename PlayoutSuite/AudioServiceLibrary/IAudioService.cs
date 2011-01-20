using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace AudioServiceLibrary
{

    public interface IAudioServiceCallBack
    {
        [OperationContract(IsOneWay = true)]
        void OnTrackInfo(TrackStatus status, DateTime delay);
        [OperationContract(IsOneWay = true)]
        void OnTrackEnd(int logid);
    }

    [ServiceContract(CallbackContract = typeof(IAudioServiceCallBack))]
    public interface IAudioService
    {

        /* [OperationContract]
         string GetData(int value);

         [OperationContract]
         CompositeType GetDataUsingDataContract(CompositeType composite);
         */
        // TODO: Add your service operations here
        [OperationContract]
        Boolean stopAll();

        [OperationContract]
        void stopTrack(long logid);

        [OperationContract]
        void pauseTrack(long logid);

        [OperationContract]
        void playTrack(long logid);

        [OperationContract]
        void loadTrack(long logid, TrackPlayProperties p);


        [OperationContract]
        void setVolume(long logid, int volume);


        [OperationContract]
        void setPosition(long logid, int position);

        [OperationContract]
        Boolean subscribeTrackInfo(long logid);

        [OperationContract]
        void unsubscribeTrackInfo(long logid);

        [OperationContract]
        void fadeStop(long logid, int milliseconds);

        [OperationContract]
        Boolean hello(string name, string key);
    }
}
