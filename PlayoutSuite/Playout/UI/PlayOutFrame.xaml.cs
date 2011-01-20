using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AudioServiceLibrary;
using System.Windows.Threading;
using LogListPrj;
using LogListPrj.DataItems;
using System.ServiceModel;
using Playout.UI.Slots;

namespace Playout.UI
{
    /// <summary>
    /// Interaction logic for PlayListFrame.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false,IncludeExceptionDetailInFaults= true)]
    public partial class PlayOutFrame : UserControl, IAudioServiceCallBack
    {
        AudioServiceConnection audioconnection;
        DispatcherTimer timer1;
        LogList loglist;

        Dictionary<long, UIElement> UIslots = new Dictionary<long, UIElement>();
        Dictionary<long, SlotItem> AEslots = new Dictionary<long, SlotItem>();
        SlotItem _lastLoaded = null;
        private SlotItem _nextPlay = null;

        public event newOnAirStatusHandler newOnAirStatus;
        public delegate void newOnAirStatusHandler(object sender, OnairEventArgs e);

        
        public SlotItem lastLoaded
        {
            get { 
                if (_lastLoaded == null) { 
                    try { nextPlay = loglist.getScheduledSlot(DateTime.Now); } 
                    catch { nextPlay = loglist.slices.First.Value.slots.First.Value; } 
                    return nextPlay; 
                } 
                else return _lastLoaded; 
            } // !!!!!! START AND END OF LOGLIST
            private set { this._lastLoaded = value; } 
        }

        public SlotItem nextPlay
        {
            get { return _nextPlay; } // !!!!!! START AND END OF LOGLIST
            private set { this._nextPlay = value; }
        }

        public PlayOutFrame()
        {
            InitializeComponent();
            timer1 = new DispatcherTimer();
            audioconnection = new AudioServiceConnection();
            
            try
            {
                audioconnection.connect("net.tcp://"+System.Configuration.ConfigurationSettings.AppSettings["audioengineAddress"]+":8080/AudioService", this, channel_Opened, channel_Faulted);
                audioconnection.hello("AudioTestMonitor", "key");
            }catch{
                MessageBox.Show("AudioEngine unreachable. Closing...");
                //Environment.Exit(1);
            }
           
            timer1.Interval = TimeSpan.FromMilliseconds(100);
            timer1.Start();

        }

        void channel_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("[PLAYOUT] Connection opened\n");
        }

        void channel_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine("[PLAYOUT] Connection lost ***********\n");
             
        }

        public void fillPlayout()
        {
            try
            {
                while (UIslots.Count < 8 && lastLoaded.NextSlot != null)
                {
                    SlotItem current = updateLastLoaded();
                    if (current.item.dataitemtype != DS.DataItemType.LOGNOTE)
                        addUSlot(current);
                }
            }
            catch { MainWindow.errorAdd("End of playlist", "PLAYLIST"); }
        }

        public void addUSlot(SlotItem slot, Boolean isForeign = false)
        {
            if (!UIslots.ContainsKey(slot.uniqID))
            {
                USlot Uslot = new USlot(slot);
                if (!isForeign) AudioLoadSlot(slot);
                this.UIplayoutStack.Children.Add(Uslot);
                UIslots.Add(slot.uniqID, Uslot);
                AEslots.Add(slot.uniqID, slot);
            }
        }

        public void removeUSlot(long id)
        {
            AEslots.Remove(id);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.UIplayoutStack.Children.Remove(UIslots[id]);
                if (UIslots[id] is USlot && ((USlot)UIslots[id]).slotsong is USlotSong)
                {
                    ((USlot)UIslots[id]).slotsong.Dispose();
                }
                else if (UIslots[id] is USlot && ((USlot)UIslots[id]).uicontent is USlotContent)
                {
                    //Nothing
                }
                else if (UIslots[id] is USlotError)
                {
                    //Nothing
                }
                else
                {

                }
                UIslots[id] = null;
                UIslots.Remove(id);
            }));
        }

        private void ReplaceByErrorSlot(long id)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                USlotError e = new USlotError(id, "Error with " + ((USlot)UIslots[id]).slot.item.label);
                this.UIplayoutStack.Children.Insert(this.UIplayoutStack.Children.IndexOf(UIslots[id]), e);
                removeUSlot(id);
                this.UIslots.Add(id, e);                
            }));
        }

        public SlotItem updateLastLoaded(){
            SlotItem current = this.lastLoaded;
            this.lastLoaded = this.lastLoaded.NextSlot;
            return current;
        }

        public SlotItem updateNextPlay()
        {
            SlotItem current = this.nextPlay;

            this.nextPlay = this.nextPlay.NextSlot;
            this.CurrentSlot = current;
            return current;
        }

        public void AudioLoadSlot(SlotItem slot)
        {
            Console.WriteLine("CURRENT SLOT:" + slot);
            if (slot.item.dataitemtype == DS.DataItemType.SONG)
            {
                try
                {
                    this.audioconnection.service.loadTrack(slot.uniqID, slotToTrackPlayProperties(slot));
                }
                catch
                {
                    MainWindow.errorAdd("Unable to load track: " + slot.uniqID + " (maybe AudioEngine closed)", "AUDIOENGINE");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fillPlayout();
        }

        private TrackPlayProperties slotToTrackPlayProperties(SlotItem slot){
            if(slot.item.dataitemtype == DS.DataItemType.SONG){
                DataSongItem song = (DataSongItem)slot.item;
                TrackPlayProperties p = new TrackPlayProperties(PlayType.FILE, song.filename);
                p.markerCue = song.timemarker.cue;
                p.markerIntro = song.timemarker.intro;
                p.markerNext = song.timemarker.next;
                p.logid = slot.uniqID;
                return p;
            }
            else{
                return new TrackPlayProperties(PlayType.SILENCE);
            }
        }
        private int init = 0; // 0 = init | 1 = onair sync | 2 = fill playout
        public void OnTrackInfo(TrackStatus status, DateTime delay)
        {

            try
            {
                SlotItem slot = this.AEslots[status.logid];
                UIElement uislot = this.UIslots[status.logid];
                if (status.infotype == TrackInfoType.ERROR)
                {

                    slot.status = DS.SlotStatus.ERROR;
                    ReplaceByErrorSlot(status.logid);
                    Console.WriteLine("ERROR with slot id :" + status.logid);
                }
                else if(status.infotype == TrackInfoType.INFO)
                {
                    if (status.playstatus == TrackPlayStatus.READY)
                    {
                        slot.status = DS.SlotStatus.READY;
                    }
                    else if (status.playstatus == TrackPlayStatus.XFADE)
                    {
                        Console.WriteLine("[AUDIOENGINE] "+DateTime.Now.Subtract(delay).TotalMilliseconds+" ms delay");
                        //if(slot.status == DS.SlotStatus.ONAIR)
                        this.PlayNextSlot();                     
                    }
                    else if (status.playstatus == TrackPlayStatus.CLOSING)
                    {
                        slot.status = DS.SlotStatus.CLOSING;
                    }
                    else if (status.playstatus == TrackPlayStatus.ONAIR)
                    {
                        slot.status = DS.SlotStatus.ONAIR;
                        if (slot.item.dataitemtype == DS.DataItemType.SONG)
                        {
                            DataSongItem songitem = (DataSongItem)slot.item;
                            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                            {
                                USlotSong uislotsong = (USlotSong)((USlot)uislot).Content;
                              /*  Console.WriteLine(songitem.timemarker.next);
                                Console.WriteLine(status.realelapsedtime);
                                Console.WriteLine(songitem.timemarker.cue);*/
                                
                                uislotsong.currenttime = songitem.timemarker.next.Subtract(status.realelapsedtime);

                            }));

                        }
                    }
                        //Clean Audioengine Tracks
                    else if (status.playstatus == TrackPlayStatus.PLAYED)
                    {
                        removeUSlot(status.logid);
                        slot.status = DS.SlotStatus.PLAYED;

                        this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        {
                                fillPlayout();
                        }));
                    }
                }
            }
            catch (KeyNotFoundException e){ 
                Console.WriteLine("NOT FROM THIS PLAYER "+status.playstatus);
                //Console.WriteLine(e.Message);
                try
                {
                    //Recover ONAIR Tracks
                    if (status.playstatus == TrackPlayStatus.ONAIR && init==0)
                    {

                        this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        {
                            try
                            {
                                SlotItem current = loglist.getSlotById(status.logid);

                                if (current != null && current.item.dataitemtype != DS.DataItemType.LOGNOTE)
                                {
                                    if (this.init==0)
                                    {
                                        

                                        CurrentSlot = current;
                                        this.nextPlay = current;
                                        this.lastLoaded = current;
                                        current =updateLastLoaded();
                                        updateNextPlay();
                                        init++;
                                                                                
                                    }                                    
                                    addUSlot(current, true);
                                    loglist.skipUntilSlot(current);
                                }
                            }
                            catch{
                                Console.WriteLine("NOT FOUND SLOT");
                            }
                        }));
                        this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        {
                            if (init == 1)
                            {
                                fillPlayout();
                                init++;
                            }
                        }));
                    }
                    else
                    {
                        this.audioconnection.service.stopTrack(status.logid);
                    }
                }
                catch { Console.WriteLine("[OnTrackInfo] error: " + e.Message);  }
            
            }
        }

        public void OnTrackEnd(int logid)
        {
            throw new NotImplementedException();
        }

        internal void loadLoglist(LogList log)
        {
            this.loglist = log;
            Console.WriteLine(this.loglist.slices.Count() + " slices in log");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
                this.audioconnection.service.stopAll();
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PlayNextSlot();
        }

        public void PlayNextSlot()
        {
            if (nextPlay != null)
            {
                SlotItem currentslot;
                try
                {
                     currentslot = this.nextPlay.PreviousSlot;
                }
                catch
                {
                    currentslot = null;
                }

                if (currentslot != null)
                    this.audioconnection.service.fadeStop(currentslot.uniqID, 2000);

                while (nextPlay != null && (nextPlay.status == DS.SlotStatus.ERROR || nextPlay.item.dataitemtype != DS.DataItemType.SONG))
                {
                    if (nextPlay.status == DS.SlotStatus.ERROR)
                    {
                        try
                        {
                            ((USlotError)this.UIslots[nextPlay.uniqID]).Hide();
                           // removeUSlot(nextPlay.uniqID);
                        }
                        catch
                        {
                           // MessageBox.Show("Error with Playout slot in PlayoutFrame");
                        }
                    }
                    else if (nextPlay.item.dataitemtype == DS.DataItemType.SYNC)
                    {
                        DataSyncItem item = (DataSyncItem)nextPlay.item;
                        nextPlay.status = DS.SlotStatus.PLAYED;
                        if (item.synctype == DS.SyncType.SOFT)
                        {
                            if(((USlot)this.UIslots[nextPlay.uniqID]).uisync!=null)
                            ((USlot)this.UIslots[nextPlay.uniqID]).uisync.StartHideSequence();
                        }
                        else if (item.synctype == DS.SyncType.HARD)
                        {

                        }
                        else
                        {
                            //skip
                        }

                    }
                    else if (nextPlay.item.dataitemtype == DS.DataItemType.LOGNOTE)
                    {
                        nextPlay.status = DS.SlotStatus.SKIPPED;

                        this.newOnAirStatus(this, new OnairEventArgs(nextPlay));
                    }
                    nextPlay = nextPlay.NextSlot;
                }

                if (nextPlay != null)
                {
                    SlotItem next = this.updateNextPlay();
                    this.audioconnection.service.playTrack(next.uniqID);
                    this.newOnAirStatus(this, new OnairEventArgs(next));
                }
            }
        }

        
        public SlotItem CurrentSlot { get; set; }
    }
}
