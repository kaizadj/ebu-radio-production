using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using AudioEngineDll.AudioTrack;
using System.Collections.Generic;
using System.Timers;


namespace AudioServiceLibrary
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class AudioService : IAudioService, IDisposable
    {
        private Dictionary<long, IAudioServiceCallBack> listeners = new Dictionary<long, IAudioServiceCallBack>();
        private IAudioServiceCallBack callback;

        public AudioService()
        {
            Console.WriteLine("New AudioService Connection");
            try
            {

                this.callback = OperationContext.Current.GetCallbackChannel<IAudioServiceCallBack>();
                /*if (!listeners.ContainsValue(callback))
                {
                    listeners.Add(logid, callback);
                }
                */

                AudioServiceHost.audiomgr.periodicUpdate += new EventHandler(audiomgr_periodicUpdate);
                AudioServiceHost.audiomgr.instantUpdate += new EventHandler(audiomgr_instantUpdate);

            }
            catch { }
        }


        public Boolean stopAll()
        {
           return  AudioServiceHost.audiomgr.stopAll();
        }

        public void playTrack(long logid)
        {
            AudioServiceHost.audiomgr.playTrack(logid);
            
        }

        public void stopTrack(long logid)
        {

            AudioServiceHost.audiomgr.stopTrack(logid);
            Console.WriteLine("STOP");
            
        }

        public void pauseTrack(long logid)
        {
            throw new NotImplementedException();
        }

        public void loadTrack(long logid, TrackPlayProperties p)
        {
            AudioServiceHost.audiomgr.newTrack(logid, p);
        }

        public bool subscribeTrackInfo(long logid)
        {/*
            try
            {

                this.callback = OperationContext.Current.GetCallbackChannel<IAudioServiceCallBack>();
                

                AudioServiceHost.audiomgr.periodicUpdate += new EventHandler(audiomgr_periodicUpdate);

                return true;
            }
            catch { return false; }*/
            return false;
        }

        void audiomgr_periodicUpdate(object sender, EventArgs e)
        {

            Dictionary<long, AUDIOTRACK> tracks = AudioServiceHost.audiomgr.getAllTracks();
            foreach (KeyValuePair<long, AUDIOTRACK> track in tracks.ToArray())
            {
                TrackStatus status = new TrackStatus(track);
                try
                {
                    this.callback.OnTrackInfo(status, DateTime.Now);
                }
                catch
                {
                    
                }
            }

        }
        void audiomgr_instantUpdate(object sender, EventArgs e)
        {
                EndSyncEventArgs ee = (EndSyncEventArgs)e;
                

                TrackStatus status = new TrackStatus(ee.track);
                status.playstatus = ee.status;
                   try
                   {

                       this.callback.OnTrackInfo(status, DateTime.Now);

                   }
                   catch
                   {

                   }

                   if (status.playstatus == TrackPlayStatus.PLAYED) 
                   AudioServiceHost.audiomgr.closeTrack(ee.track.Key);

        }
        public void unsubscribeTrackInfo(long logid)
        {
            try
            {
                IAudioServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IAudioServiceCallBack>();
                if (listeners.ContainsValue(callback))
                {
                    listeners.Remove(logid);
                }
            }
            catch { }
        }

        public void setVolume(long logid, int volume)
        {
            throw new NotImplementedException();
        }

        public void setPosition(long logid, int position)
        {
            throw new NotImplementedException();
        }


        public void fadeStop(long logid, int milliseconds)
        {
            AUDIOTRACK track = AudioServiceHost.audiomgr.getTrack(logid);
            if (track != null)
            {
                track.FadeOut(TimeSpan.FromMilliseconds(milliseconds));

                KeyValuePair<long, AUDIOTRACK> t = new KeyValuePair<long, AUDIOTRACK>(logid, track);
                EndSyncEventArgs e = new EndSyncEventArgs(t, TrackPlayStatus.CLOSING);

                AudioServiceHost.audiomgr.launchUpdate(e);
            }
        }

        public void Dispose()
        {
            AudioServiceHost.audiomgr.periodicUpdate -= this.audiomgr_periodicUpdate;
            Console.WriteLine("["+name+"] Connection close");
        }

        String name="none";

        public Boolean hello(string name, string key)
        {
            Console.WriteLine("HELLO from " + name + " with key: " + key);
            this.name = name;
            return true;
        }
    }
    public class EndSyncEventArgs : EventArgs
    {
        public EndSyncEventArgs(KeyValuePair<long,AUDIOTRACK> track, TrackPlayStatus status)
        {
            this.track = track;
            this.status = status;
            this.timestamp = DateTime.Now;
        }
        public DateTime timestamp;
        public override string ToString()
        {
            return "id:" + track.Key + " track:" + track.Value.getTrackType() + " isplaying?" + track.Value.isPlaying() + " status:" + status;
        }
        public int syncHandle;
        public int channel;
        public int data;
        public IntPtr user;
        public KeyValuePair<long,AUDIOTRACK> track;
        public TrackPlayStatus status;

    } 
}
