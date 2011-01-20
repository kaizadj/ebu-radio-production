using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using System.Collections;
using System.Threading;
using Un4seen.Bass.AddOn.Enc;
using AudioEngineDll.AudioTrack;
namespace AudioEngineDll.AudioSystem
{
    class AudioSystemBass : Interfaces.IAudioSystem
    {

        //MIX
        private int samplerate = 44100;

        //WEB CASTING
        private bool SHOUTCAST_CASTING_ENABLE = false;
        private int shoutcastmixer;
        private int bitrate = 128;

        //SYNC
        private Hashtable synctable;
        private Hashtable syncendtable;
        private Hashtable idsyncendtable;
        private Hashtable idsynctable;

        public AudioSystemBass()
        {
            BassNet.Registration("mathieu@habegger-bros.ch", "2X22382431331");
            synctable = new Hashtable();
            syncendtable = new Hashtable();
            idsynctable = new Hashtable();
            idsyncendtable = new Hashtable();
        }
#region SYS
        public bool init(int audiocard, int sample)
        {
            //Init vars
            this.samplerate = sample;

            AudioEngine.DEBUGOUTPUT("[BASS] INIT(audiocard: " + audiocard.ToString() + ", sample: " + (samplerate.ToString()) + ")");


            bool ret = Bass.BASS_Init(audiocard, samplerate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            //Set volume @ max
            //Bass.BASS_SetVolume(1);

            //WEB CASTING
            /* if (SHOUTCAST_CASTING_ENABLE)
            {
                shoutcastmixer = BassMix.BASS_Mixer_StreamCreate(samplerate, 2, BASSFlag.BASS_MIXER_NONSTOP);
                if (shoutcastmixer == 0)
                {
                    AudioEngine.DEBUGOUTPUT("[BASS] ERROR, not able to create mixer channel for shoutcast)");
                }
                else
                {
                    Bass.BASS_ChannelPlay(shoutcastmixer, false);
                    casting(shoutcastmixer);
                }
            }*/

            return ret;
        }

        public void close()
        {
            Bass.BASS_Free();
        }

        public string getLastError()
        {
            throw new NotImplementedException();
        }
#endregion SYS
#region STREAM
        public int createFileStream(string fileName, int startCue, int duration)
        {
            AudioEngine.DEBUGOUTPUT("[BASS] NEW FILESTREAM(fileName: " + fileName + ")");
            int ch = 0;
            int loop = 0;
            while (ch == 0 && loop < 3)
            {

                //WITH SHOUTCAST OPTION
                if (SHOUTCAST_CASTING_ENABLE)
                {
                   // ch = Bass.BASS_StreamCreateFile(fileName, (Int64)startCue, (Int64)duration, BASSFlag.BASS_STREAM_DECODE);
                }
                else
                { //WITHOUT
                    ch = Bass.BASS_StreamCreateFile(fileName, (Int64)startCue, (Int64)duration, BASSFlag.BASS_STREAM_AUTOFREE);
                }
                Console.WriteLine("file opened on channel " + ch + " error code: " + Bass.BASS_ErrorGetCode().ToString());
                ++loop;
            }

            //ADD CHANNEL TO SHOUTCAST OUTPUT

           /* if (SHOUTCAST_CASTING_ENABLE && !BassMix.BASS_Mixer_StreamAddChannel(shoutcastmixer, ch, BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_MIXER_PAUSE))
                AudioEngine.DEBUGOUTPUT("[BASS] ERROR, unable to link the channel " + ch + " to mixer channel for shoutcast: " + Bass.BASS_ErrorGetCode() + ")");
            */
            return ch;
        }

        public void playStream(int stream)
        {
            if (SHOUTCAST_CASTING_ENABLE)
            {
                BassMix.BASS_Mixer_ChannelPlay(stream);
            }
            else
            {
                Bass.BASS_ChannelPlay(stream, false);
            }         
        }

        public void stopStream(int stream)
        {
            Bass.BASS_ChannelStop(stream);
        }

        public void closeStream(int stream)
        {
            this.stopStream(stream);
            syncendtable.Remove(stream);
            synctable.Remove(stream);
            if (SHOUTCAST_CASTING_ENABLE)
            {
                BassMix.BASS_Mixer_ChannelRemove(stream);
            }
            Bass.BASS_StreamFree(stream);
        }
#endregion
#region STATUS
        public bool isPlaying(int stream)
        {
            if (SHOUTCAST_CASTING_ENABLE)
            {
                return (BassMix.BASS_Mixer_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING);
            }
            else
            {
                return (Bass.BASS_ChannelIsActive(stream) == Un4seen.Bass.BASSActive.BASS_ACTIVE_PLAYING);
            }
        }

        public bool isReady(int stream)
        {
            return (stream != 0);
        }
        
#endregion
#region VOLUME

        public int getVolume(int stream)
        {
            float volume = 0.0F;
            Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, ref volume);
            //  AudioEngine.DEBUGOUTPUT("[BASS] GET VOLUME(" + (volume).ToString() + " => " + ((int)(volume * 1000)).ToString() + ")");
            return (int)(volume * 1000);
        }

        public void setVolume(int stream, int volume)
        {            
            float v = volume;
            v = v / 1000;
            //AudioEngine.DEBUGOUTPUT("[BASS] SET VOLUME(" + (volume.ToString()) + " => " + ((float)v).ToString() + ")");
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, v);
        }
#endregion
#region TIME
        public int getPositionMs(int stream)
        {
            long pos = Bass.BASS_ChannelGetPosition(stream);//BASS_ChannelGetPosition(channel, Un4seen.Bass.BASSMode.BASS_POS_MUSIC_ORDERS);
            double seconds = Bass.BASS_ChannelBytes2Seconds(stream, pos) * 1000.0;
            return (int)seconds;
        }

        public int getLengthMs(int stream)
        {
            long pos = Bass.BASS_ChannelGetLength(stream);//BASS_ChannelGetPosition(channel, Un4seen.Bass.BASSMode.BASS_POS_MUSIC_ORDERS);
            double seconds = Bass.BASS_ChannelBytes2Seconds(stream, pos) * 1000.0;
            return (int)seconds;
        }
#endregion
#region CASTING
        public void casting(int channel)
        {
            Thread t = new Thread(delegate()
            {
                int encoder = BassEnc.BASS_Encode_Start(shoutcastmixer, "lame -r -s " + samplerate + " -b " + (bitrate).ToString() + " -", BASSEncode.BASS_ENCODE_NOHEAD | BASSEncode.BASS_ENCODE_FP_16BIT, null, IntPtr.Zero);
                if (encoder == 0)
                {
                    Console.WriteLine("[BASS]  Error Encoder:" + Bass.BASS_ErrorGetCode());
                }
                if (!BassEnc.BASS_Encode_CastInit(encoder, "localhost:8000/test.mp3", "", BassEnc.BASS_ENCODE_TYPE_MP3, "playout", null, null, null, null, bitrate, false))
                {
                    Console.WriteLine("[BASS] Can't start encoder errorcode:" + Bass.BASS_ErrorGetCode().ToString());
                }

                Console.WriteLine("[BASS] CASTING code:" + Bass.BASS_ErrorGetCode().ToString());
            });
            t.Start();


        }
#endregion
#region SYNC

        public int defineEndSync(int stream, AudioFileTrack.TimeSyncFct fct)
        {

            if (stream != 0)
            {
                Console.WriteLine("DEFINITION END SYNC MARKER");
                syncendtable.Add(stream, new SYNCPROC(fct));
                int id = Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_MIXTIME | BASSSync.BASS_SYNC_FREE,
                0, (SYNCPROC)syncendtable[stream], new IntPtr());
                this.idsyncendtable.Add(stream, id);
                return id;
            }
            else
                return 0;


        }

        public int defineSync(int stream, AudioTimeMarker markers, AudioFileTrack.TimeSyncFct fct)
        {

            if (stream != 0)
            {
                long nextpos = Bass.BASS_ChannelSeconds2Bytes(stream, markers.getNext().TotalSeconds); // set to whatever you need
                Console.WriteLine("DEFINITION SYNC MARKER:" + nextpos + " (" + markers.getNext().TotalSeconds + "sec)");
                synctable.Add(stream, new SYNCPROC(fct));
                int id = Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME,
                nextpos, (SYNCPROC)synctable[stream], new IntPtr());
                this.idsynctable.Add(stream, id);
                return id;

            }
            else
                return 0;


        }

        public void removeAllSync(int stream)
        {
            Bass.BASS_ChannelRemoveSync(stream, (int)idsyncendtable[stream]);
            idsyncendtable.Remove(stream);
            Console.WriteLine("###############!!!!!!!" + Bass.BASS_ErrorGetCode());
            Bass.BASS_ChannelRemoveSync(stream, (int)idsynctable[stream]);
            idsynctable.Remove(stream);
            Console.WriteLine("!!!!!!!###############" + Bass.BASS_ErrorGetCode());
        }
#endregion

    }
}
