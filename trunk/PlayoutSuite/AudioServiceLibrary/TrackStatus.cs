using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using AudioEngineDll.AudioTrack;

namespace AudioServiceLibrary
{

    public enum VolumeStatus { FADEIN, FADEOUT, VTIN, VTRUN, VTOUT, MUTE, DEFAULT, NOSOUND };
    public enum PlayType { FILE, STREAM, SILENCE }
    public enum TrackInfoType { EVENT, INFO, ERROR, SYNC }
    public enum TrackPlayStatus { READY, ERROR, ONAIR, PLAYED, EJECT, PAUSED, CLOSING, XFADE } //FADED?
    [DataContract]
    public class TrackStatus
    {

        [DataMember]
        public TrackInfoType infotype = TrackInfoType.INFO;

        [DataMember]
        public TrackPlayStatus playstatus { get; set; }

        private bool iloaded = false;
        private bool ipaused = false;

        [DataMember]
        public TimeSpan realelapsedtime = TimeSpan.Zero;

        [DataMember]
        public TimeSpan realremaintime = TimeSpan.Zero;

        [DataMember]
        public TimeSpan realdurationtime = TimeSpan.Zero;

        [DataMember]
        public long logid = 0;

        [DataMember]
        VolumeStatus volumestatus = VolumeStatus.NOSOUND;
        [DataMember]
        int volume = 1000;
        private int lap;
        private KeyValuePair<int, AudioEngineDll.AudioTrack.AUDIOTRACK> track;

        public TrackStatus()
        {
        
        }

        public TrackStatus(KeyValuePair<long, AudioEngineDll.AudioTrack.AUDIOTRACK> track)
        {
            
            if (!track.Value.isReady())
                this.infotype = TrackInfoType.ERROR;
            else if (track.Value.isClosing())
            {
                this.playstatus = TrackPlayStatus.CLOSING;
            }
            else if (track.Value.isPlaying())
            {
                this.playstatus = TrackPlayStatus.ONAIR;
            }
            else
            {
                this.playstatus = TrackPlayStatus.READY;
            }
            this.realelapsedtime = track.Value.getPosition();
            this.realdurationtime = track.Value.getLength();
            this.realremaintime = track.Value.getLength().Subtract(track.Value.getPosition());
            this.logid = track.Key;
        }

    }
    [DataContract]
    public class TrackPlayProperties
    {
        [DataMember]
        public PlayType type{get;set;}
        

        [DataMember]
        public TimeSpan markerCue = TimeSpan.Zero;

        [DataMember]
        public TimeSpan markerNext = TimeSpan.Zero;
        [DataMember]
        public TimeSpan markerIntro = TimeSpan.Zero;
        [DataMember]
        public long logid = 0;

        [DataMember]
        public String path = "";


        //private KeyValuePair<int, AudioEngineDll.AudioTrack.AUDIOTRACK> track;

        public TrackPlayProperties(PlayType type, String path="")
        {
            this.type = type;
            this.path = path;
        }

    /*    public TrackPlayProperties(KeyValuePair<int, AudioEngineDll.AudioTrack.AUDIOTRACK> track)
        {
            this.elapsedtime = track.Value.getPosition();
            this.durationtime = track.Value.getLength();
            this.remaintime = track.Value.getLength().Subtract(track.Value.getPosition());
        }*/

    }
}

