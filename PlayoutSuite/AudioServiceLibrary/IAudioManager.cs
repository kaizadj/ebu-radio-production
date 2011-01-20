using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioEngineDll.AudioTrack;

namespace AudioServiceLibrary
{
    public interface IAudioManager
    {
        event EventHandler periodicUpdate;
        event EventHandler instantUpdate;
        void newTrack(long logid, TrackPlayProperties p);
        AUDIOTRACK getTrack(long logid);
        void ejectTrack(long logid);
        Dictionary<long, AUDIOTRACK> getAllTracks();

        void playTrack(long logid);

        void stopTrack(long logid);

        Boolean stopAll();

        void closeTrack(long p);

        void launchUpdate(EndSyncEventArgs e);
    }
}
