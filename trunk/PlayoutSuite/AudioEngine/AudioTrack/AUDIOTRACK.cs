using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AudioEngineDll.AudioTrack
{
    public abstract class AUDIOTRACK
    {
            public int stream{ get; protected set;}

            abstract public void play();
            abstract public void pause();
            abstract public void stop();
            abstract public void close();

            /******************
             * FADE OUT SECTION
             * ----------------
             * Use an affine approximation to make the fade out curve.
             * To increase precision, decrease the FADE_RATE_TIME
             * @ 100 : error is approximately +- 100 ms
             * @ 50 : error is approximately +- 50 ms
             * ****************/
            const int FADE_RATE_TIME = 50;
            const int FADE_DEFAULT_FADEOUT = 1500; //in ms
            const int FADE_UPPERBOUND_MUTE = 3;//Volume at which we consider it's possible to mute a track.

            private DateTime fadeStartTime;
            private double fadeFactor = 1;
            private Boolean fadingOut = false;

            protected AudioEngine audioEngine;

            public AUDIOTRACK(AudioEngine audioEngine)
            {
                this.audioEngine = audioEngine;
                this.fadingOut = false;
            }


            /**
             * FadeOut trigger without specified fadeout duration
             * use the FADE_DEFAULT_FADEOUT value
             * */
            public void FadeOut()
            {
                this.FadeOut(TimeSpan.FromMilliseconds(FADE_DEFAULT_FADEOUT));
            }

            /**
             * FadeOut trigger with time specification
             * 1. Compute the Affine factor for the curve of the fade.
             * 2. Create and run the thread which will make the fade.
             * 
             * */
            public void FadeOut(TimeSpan duration)
            {

                fadeStartTime = DateTime.Now;
                DateTime fadeEndTime = fadeStartTime.Add(duration);
                fadeFactor = 1000.0 / (double)duration.TotalMilliseconds;


                Console.WriteLine("FADING: FACTOR=" + fadeFactor + " Duration=" + duration + "");
                if (!fadingOut)
                {
                    fadingOut = true;
                    Thread thread = new Thread(new ThreadStart(this.FadeOutProcess));
                    thread.Start();
                }

            }

            /**
             * FadeOutProcess is the function called by the thread.
             * 1. Loop for the fade
             *      - decrease the volume depending on the time and the curve of the fade
             * 2. Set the volume of the track @ 0
             * 3. Close the track.
             * 
             * */
            private void FadeOutProcess()
            {
                DateTime s = DateTime.Now;

                int volume = audioEngine.audioSystem.getVolume(stream);

                while (volume >= FADE_UPPERBOUND_MUTE)
                {
                    volume = (int)(-DateTime.Now.Subtract(fadeStartTime).TotalMilliseconds * fadeFactor + 1000.0);
                    audioEngine.audioSystem.setVolume(stream, (int)volume);

                    if (volume >= FADE_UPPERBOUND_MUTE)
                        Thread.Sleep(FADE_RATE_TIME);
                }

                audioEngine.audioSystem.setVolume(stream, 0);
                AudioEngine.DEBUGOUTPUT("TOTAL FADE TIME (with safe to 0 check):" + DateTime.Now.Subtract(s));
                this.close();
            }

            /**
             * Return the position
             * */
            public TimeSpan getPosition()
            {
                return TimeSpan.FromMilliseconds(audioEngine.audioSystem.getPositionMs(this.stream));
            }
            /**
             * Return the length of the track
             * */
            public TimeSpan getLength()
            {
                return TimeSpan.FromMilliseconds(audioEngine.audioSystem.getLengthMs(this.stream));
            }


            public bool isPlaying()
            {
                return audioEngine.audioSystem.isPlaying(this.stream);
            }

            //Return if stream is open and ready
            public bool isReady()
            {
                return audioEngine.audioSystem.isReady(this.stream);
            }

            public abstract AUDIOTRACKTYPE getTrackType();




            public bool isClosing()
            {
                return this.fadingOut;
            }
    }
    public  enum AUDIOTRACKTYPE{ FILE, EXTERNAL, URL };
}
