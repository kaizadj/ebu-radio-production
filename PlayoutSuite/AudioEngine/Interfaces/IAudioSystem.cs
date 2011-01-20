using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioEngineDll.AudioTrack;

namespace AudioEngineDll.Interfaces
{
    public interface IAudioSystem
    {
        //SYS
        Boolean init(int audiocard, int sample);
        void close();
        String getLastError();
        //STREAM
        int createFileStream(String fileName, int startCue, int duration); 
        void playStream(int stream);
        void stopStream(int stream);
        void closeStream(int stream);
        //STATUS
        bool isPlaying(int stream);
        bool isReady(int stream);
        //VOLUME
        int getVolume(int stream);
        void setVolume(int stream, int volume);
        //TIME
        int getPositionMs(int stream);
        int getLengthMs(int stream);
        //SYNC
        int defineSync(int stream, AudioTimeMarker markers, AudioFileTrack.TimeSyncFct fct);
        int defineEndSync(int channel, AudioFileTrack.TimeSyncFct fct);
        void removeAllSync(int stream);
    }
}
