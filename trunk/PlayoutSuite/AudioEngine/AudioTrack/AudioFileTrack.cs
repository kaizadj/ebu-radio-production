using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AudioEngineDll.AudioTrack
{
    public class AudioFileTrack : AUDIOTRACK
    {

        public object reference;
        private Boolean isFree;

        private String filename;
        public AudioTimeMarker markers;
        public int timesync = 0; //synchro

        public delegate void TimeSyncFct(int syncHandle, int channel, int data, IntPtr user);


        public AudioFileTrack(AudioEngine audioengine, String filename, AudioTimeMarker markers, object reference=null)
            : base(audioengine)
        {
            this.reference = null;
            this.isFree = false;
            this.filename = filename;
            this.markers = markers;
            Init();

        }

        public override void play()
        {

            if (this.stream != 0)
            {
                audioEngine.audioSystem.playStream(stream);
                audioEngine.audioSystem.setVolume(stream, 1000);
                AudioEngine.DEBUGOUTPUT("BASS playing");
            }
            else
                AudioEngine.DEBUGOUTPUT("BASS error playing" + audioEngine.audioSystem.getLastError());
        }


        public override void stop()
        {
            if (this.stream != 0)
            {
                audioEngine.audioSystem.stopStream(this.stream);
            }
        }
        public override void pause()
        {
            new NotImplementedException();
        }

        public override void close()
        {
            audioEngine.audioSystem.closeStream(this.stream);
                        
            audioEngine.killTrack(this);
        }


        public void defineSync(AudioTimeMarker markers, TimeSyncFct fct)
        {

            timesync = this.audioEngine.audioSystem.defineSync(this.stream, markers, fct);
        }

        public void defineEndSync(TimeSyncFct fct)
        {

            timesync = this.audioEngine.audioSystem.defineEndSync(this.stream, fct);
        }

        public void removeAllSync()
        {
            this.audioEngine.audioSystem.removeAllSync(this.stream);
        }

        internal void Init()
        {
            this.isFree = false;
            String path = this.filename;
            try
            {
                String[] files = Directory.GetFiles(System.IO.Path.GetDirectoryName(path));
                path = System.IO.Path.GetFullPath(path);
                String file = Array.Find(files, s => (s.ToLower() == path.ToLower()));
                if (file != "" && file != null) this.filename= file;
                else this.filename = "-1";
            }
            catch { 
                Console.WriteLine("File not found:" + path);
                this.filename= "-1"; 
            }
            this.stream = this.audioEngine.audioSystem.createFileStream(this.filename, (int)this.markers.getStart().TotalMilliseconds, 0);
        }

        public Boolean free
        {
            get
            {
                return isFree;
            }
        }

        public override AUDIOTRACKTYPE getTrackType()
        {
            return AUDIOTRACKTYPE.FILE;
        }
    }
}
