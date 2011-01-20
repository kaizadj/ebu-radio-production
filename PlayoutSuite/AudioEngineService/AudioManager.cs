using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioEngineDll;
using AudioEngineDll.AudioTrack;
using AudioServiceLibrary;
using System.Collections;
using System.Timers;

namespace AudioEngineService
{
    class AudioManager : IAudioManager
    {
	    private static AudioManager instance = null;
        private static readonly object instancelock = new object();

        private AudioServiceHost host;
        private AudioEngine audioengine;
        private Dictionary<long, AUDIOTRACK> tracks = new Dictionary<long, AUDIOTRACK>();

        /**
         * Events
         **/

        public event EventHandler periodicUpdate;
        public event EventHandler instantUpdate;

        private Timer timer;
        

        private AudioManager(){
            //Init audio engine
            audioengine = new AudioEngine();
            

            //Init timer for periodic Updates
            timer = new Timer();
            timer.Interval = 2000;
            timer.Elapsed += new ElapsedEventHandler(delegate(object o, ElapsedEventArgs e)
            {
                this.periodicUpdate(o, e);
            });
            this.periodicUpdate += AudioManager_periodicUpdate;

            this.instantUpdate += AudioManager_instantUpdate;

            //Init service
            Console.WriteLine("Opening AudioEngineService..");
            host = new AudioServiceHost(this);
            host.open("net.tcp://"+System.Configuration.ConfigurationSettings.AppSettings["hostAddress"]+":8080/AudioService", AudioManager_hostOpened);
            
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }


        #region EVENT
        void AudioManager_periodicUpdate(object sender, EventArgs e)
        {
            Console.WriteLine("Send new periodic Update to " + this.periodicUpdate.GetInvocationList().Count()+ " listener(s) (" + this.audioengine.tracks.Count +" track loaded, "+this.tracks.Count+" tracks registred)");
        }


        void AudioManager_instantUpdate(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("INSTANT Update to " + this.periodicUpdate.GetInvocationList().Count() + " listener(s)");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }

        void AudioManager_hostOpened(object o, EventArgs e) 
        { 
            timer.Start(); 
        }

        #endregion
        #region track
        public void newTrack(long id, TrackPlayProperties p)
        {
            String filename = p.path;//"D:\\UER\\"+id+".mp3";
            Console.WriteLine("loading " + filename);
            AudioFileTrack track;
            track = (AudioFileTrack)audioengine.newFileTrack(filename, new AudioTimeMarker(p.markerCue, p.markerNext));
            track.defineEndSync(TSF);
            
            Console.WriteLine("*************");
            Console.WriteLine("TIME MARKER DEF");
            Console.WriteLine("Cue:" + track.markers.getStart());
            Console.WriteLine("Next:" + track.markers.getNext());
            track.defineSync(track.markers, TSF2);
            
            if (tracks.ContainsKey(id))
                ejectTrack(id);
            tracks.Add(id, track);
        }

        void TSF(int syncHandle, int channel, int data, IntPtr user)
        {

            KeyValuePair<long, AUDIOTRACK> track = tracks.Where(tt => tt.Value.stream == channel).First();

            EndSyncEventArgs e = new EndSyncEventArgs(track, TrackPlayStatus.PLAYED);
                  
            instantUpdate(this, e);


        }

        void TSF2(int syncHandle, int channel, int data, IntPtr user)
        {
            KeyValuePair<long, AUDIOTRACK> track = tracks.Where(tt => tt.Value.stream == channel).First();

            EndSyncEventArgs e = new EndSyncEventArgs(track, TrackPlayStatus.XFADE);

            instantUpdate(this, e);
        }

        public void launchUpdate(EndSyncEventArgs e)
        {

            instantUpdate(this, e);
        }
        public void ejectTrack(long id)
        {
            AUDIOTRACK track = getTrack(id);
            
            if (track !=null)
            {
                track.close();
                tracks.Remove(id);
                
            }
        }

        public AUDIOTRACK getTrack(long id)
        {
            if (tracks.ContainsKey(id))
                return tracks[id];
            else return null;
        }

        public Dictionary<long, AUDIOTRACK> getAllTracks()
        {
            return tracks;
        }



        public void playTrack(long logid)
        {
            try
            {
                AUDIOTRACK track = this.tracks[logid];
                if (track != null)
                    track.play();
               // this.instantUpdate(track, null);
                KeyValuePair<long, AUDIOTRACK> t = new KeyValuePair<long, AUDIOTRACK>(logid, track);

                EndSyncEventArgs e = new EndSyncEventArgs(t, TrackPlayStatus.ONAIR);

                instantUpdate(this, e);
            }
            catch
            {
                Console.WriteLine("error when playing");
            }
        }

        public void stopTrack(long logid)
        {
            try
            {
                AUDIOTRACK track = this.tracks[logid];
                if (track != null)
                {
                    track.stop();
                    ejectTrack(logid);
                }
            }
            catch { Console.WriteLine("error when stoping"); }
        }
        #endregion
        #region Singleton
        public static AudioManager GetInstance()
	    {
            lock ((instancelock))
            {
			    if (instance == null)
			    {
                    instance = new AudioManager();
			    }

			    return instance;
		    }
        }
        #endregion

        
        public Boolean stopAll()
        {
            while(this.tracks.Count()>0)
            {
                long k = this.tracks.First().Key;
                //ejectTrack(this.tracks.First().Key);
                //ejectTrack(k);
                this.stopTrack(k);
                Console.WriteLine(k + " ejected");
                
            }
            return true;
        }


        public void closeTrack(long p)
        {
            try
            {
                this.tracks[p].close();
                this.tracks.Remove(p);
            }
            catch { }
        }
    }

   
}
