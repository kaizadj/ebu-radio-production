using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioEngineDll.AudioSystem;
using AudioEngineDll.AudioTrack;
using AudioEngineDll.Interfaces;
using System.IO;

namespace AudioEngineDll
{

    public class AudioEngine
    {

        public LinkedList<AUDIOTRACK> tracks;
        public IAudioSystem audioSystem;

        public static void DEBUGOUTPUT(String msg)
        {
            Console.WriteLine(msg);
        }

        public AudioEngine()
        {
            audioSystem = new AudioSystemBass();
            tracks = new LinkedList<AUDIOTRACK>();
            init();
        }


        private Boolean init()
        {

            if (audioSystem.init(1, 48000))
            {
                AudioEngine.DEBUGOUTPUT("SOUNDSYSTEM INIT ok");
                return true;

            }
            else
            {
                AudioEngine.DEBUGOUTPUT("ERROR INITIALIZATION audio library");
                return false;
            }
        }


        public AUDIOTRACK newFileTrack(String filename, AudioTimeMarker markers)
        {
            AudioFileTrack track = new AudioFileTrack(this, filename, markers);
            tracks.AddLast(track);
            return track;
        }

        public void FadeAll()
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                tracks.ElementAt(i).FadeOut();
            }
        }

        //USED IN AUDIOTRACK
        public void killTrack(AUDIOTRACK track)
        {
            tracks.Remove(track);
            track = null;
        }
    }
}
