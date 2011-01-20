using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Un4seen.Bass;
using System.Timers;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;
using System.IO;

namespace DBMgrRibbon
{
    /// <summary>
    /// Interaction logic for AudioPlayer.xaml
    /// </summary>
    public partial class AudioPlayer : UserControl
    {
        int currentchannel;


        public AudioPlayer()
        {
            InitializeComponent();
            timerUpdate = new Timer();

            this.timerUpdate.Interval = 50;
            this.timerUpdate.Elapsed += new ElapsedEventHandler(timerUpdate_Elapsed);
            this.pictureBoxWaveForm.MouseDown += new MouseButtonEventHandler(pictureBoxWaveForm_MouseDown);
            this.wavePosition.MouseDown += new MouseButtonEventHandler(pictureBoxWaveForm_MouseDown);
            
            
            this.wavePosition.Width = 2;
            this.wavePosition.Height = this.pictureBoxWaveForm.Height;
            this.wavePosition.Opacity = 0.5;
            this.wavePosition.Fill = Brushes.Red;

           
            _mixer = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT);
            if (_mixer == 0)
            {
                MessageBox.Show("Could not create mixer!");
                Bass.BASS_Free();
                return;
            }

            _mixerStallSync = new SYNCPROC(OnMixerStall);
            Bass.BASS_ChannelSetSync(_mixer, BASSSync.BASS_SYNC_STALL, 0L, _mixerStallSync, IntPtr.Zero);

            timerUpdate.Start();
            Bass.BASS_ChannelPlay(_mixer, false);


            newtrack(@"D:\OAIBC\2-13 California.mp3", null);

        }

        void pictureBoxWaveForm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Point click = e.GetPosition(this.pictureBoxWaveForm);
                Bass.BASS_ChannelSetPosition(_currentTrack.Channel, _WF.GetBytePositionFromX((int)click.X, (int)pictureBoxWaveForm.ActualWidth, _zoomStart, _zoomEnd));

            }
        }

        void timerUpdate_Elapsed(object sender, ElapsedEventArgs e)
        {
            int level = Bass.BASS_ChannelGetLevel(_mixer);
            try
            {
                this.Dispatcher.Invoke((Action)delegate()
                {
                    double left = Utils.LowWord32(level);

                    if (progressBarLeft.Value - 1500 < left)
                        progressBarLeft.Value = left;
                    else
                        progressBarLeft.Value -= 1500;

                    if (progressBarLeft.Value > 0.9 * 32768)
                        progressBarLeft.Foreground = System.Windows.Media.Brushes.Red;
                    else if (progressBarLeft.Value > 0.7 * 32768)
                        progressBarLeft.Foreground = System.Windows.Media.Brushes.Orange;
                    else
                        progressBarLeft.Foreground = System.Windows.Media.Brushes.LimeGreen;

                    double right = Utils.HighWord32(level);
                    if (progressBarRight.Value - 1500 < right)
                        progressBarRight.Value = right;
                    else
                        progressBarRight.Value -= 1500;

                    if (progressBarRight.Value > 0.9 * 32768)
                        progressBarRight.Foreground = System.Windows.Media.Brushes.Red;
                    else if (progressBarRight.Value > 0.7 * 32768)
                        progressBarRight.Foreground = System.Windows.Media.Brushes.Orange;
                    else
                        progressBarRight.Foreground = System.Windows.Media.Brushes.LimeGreen;

                });
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            if (_currentTrack != null)
            {
                this.Dispatcher.Invoke((Action)delegate()
                {

                    long pos = BassMix.BASS_Mixer_ChannelGetPosition(_currentTrack.Channel);
                    labelTime.Text = Utils.FixTimespan(Bass.BASS_ChannelBytes2Seconds(_currentTrack.Channel, pos), "HHMMSS");
                    labelRemain.Text = Utils.FixTimespan(Bass.BASS_ChannelBytes2Seconds(_currentTrack.Channel, _currentTrack.TrackLength - pos), "HHMMSS");

                    DrawWavePosition(pos, _currentTrack.TrackLength);
                });

            }
        }
        private void PlayNextTrack()
        {
            lock (listBoxPlaylist)
            {
                // get the next track to play
                if (listBoxPlaylist.Items.Count > 0)
                {
                    _previousTrack = _currentTrack;
                    _currentTrack = listBoxPlaylist.Items[0] as Track;

                    if (_previousTrack != null)
                        Bass.BASS_ChannelSlideAttribute(_previousTrack.Channel, BASSAttribute.BASS_ATTRIB_VOL, -1f, 1000);

                    listBoxPlaylist.Items.RemoveAt(0);

                    // the channel was already added
                    // so for instant playback, we just unpause the channel
                    BassMix.BASS_Mixer_ChannelPlay(_currentTrack.Channel);
                    /*
                    labelTitle.Text = _currentTrack.Tags.title;
                    labelArtist.Text = _currentTrack.Tags.artist;
                    */

                    if (_currentTrack.item != null)
                    {
                        labelCue.Text = _currentTrack.item.timemarker.cue.ToString();
                        labelIntro.Text = _currentTrack.item.timemarker.intro.ToString();
                        labelNext.Text = _currentTrack.item.timemarker.next.ToString();
                    }
                    // get the waveform for that track
                    GetWaveForm();
                }
            }
        }
        private void OnMixerStall(int handle, int channel, int data, IntPtr user)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                // this code runs on the UI thread!
                if (data == 0)
                {
                    // mixer stalled
                    timerUpdate.Stop();
                    progressBarLeft.Value = 0;
                    progressBarRight.Value = 0;
                }
                else
                {
                    // mixer resumed
                    timerUpdate.Start();
                }
            });
        }

        private void OnTrackSync(int handle, int channel, int data, IntPtr user)
        {
            if (user.ToInt32() == 0)
            {
                // END SYNC
                this.Dispatcher.BeginInvoke(new Action(PlayNextTrack));
            }
            else
            {
                // POS SYNC
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    // this code runs on the UI thread!
                    PlayNextTrack();
                    // and fade out and stop the 'previous' track (for 4 seconds)
                    if (_previousTrack != null)
                        Bass.BASS_ChannelSlideAttribute(_previousTrack.Channel, BASSAttribute.BASS_ATTRIB_VOL, -1f, 4000);
                });
            }
        }
        private Timer timerUpdate;
        private int _mixer = 0;
        private SYNCPROC _mixerStallSync;
        private Track _currentTrack = null;
        private Track _previousTrack = null;

        
        
        private Track newtrack(string filename, LogListPrj.DataItems.DataSongItem item)
        {

            Track track = new Track(filename, item);
            listBoxPlaylist.Items.Add(track);

            // add the new track to the mixer (in PAUSED mode!)
            BassMix.BASS_Mixer_StreamAddChannel(_mixer, track.Channel, BASSFlag.BASS_MIXER_PAUSE | BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);

            // an BASS_SYNC_END is used to trigger the next track in the playlist (if no POS sync was set)
            track.TrackSync = new SYNCPROC(OnTrackSync);
            BassMix.BASS_Mixer_ChannelSetSync(track.Channel, BASSSync.BASS_SYNC_END, 0L, track.TrackSync, new IntPtr(0));

            if(_currentTrack == null)
                DrawWave();
            return track;
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            timerUpdate.Stop();
            // close bass
            //Bass.BASS_StreamFree(_mixer);
           // Bass.BASS_Free();
        }
        #region Wave Form

        // zoom helper varibales
        private bool _zoomed = false;
        private int _zoomStart = -1;
        private long _zoomStartBytes = -1;
        private int _zoomEnd = -1;
        private float _zoomDistance = 5.0f; // zoom = 5sec.

        private Un4seen.Bass.Misc.WaveForm _WF = null;
        private void GetWaveForm()
        {
            // unzoom...(display the whole wave form)
            _zoomStart = -1;
            _zoomStartBytes = -1;
            _zoomEnd = -1;
            _zoomed = false;

            // render a wave form
            _WF = new WaveForm(_currentTrack.Filename, new WAVEFORMPROC(MyWaveFormCallback), null);
            //_WF.NotifyHandler = new WAVEFORMPROC(MyWaveFormCallback);
            _WF.FrameResolution = 0.01f; // 10ms are nice
            _WF.CallbackFrequency = 30000; // every 5min.
            _WF.ColorBackground = System.Drawing.Color.FromArgb(255, 255, 255);
            _WF.ColorLeft = System.Drawing.Color.LightBlue;

            _WF.ColorLeftEnvelope = System.Drawing.Color.LightBlue;
            /*_WF.ColorRight = System.Drawing.Color.Gray;
            _WF.ColorRightEnvelope = System.Drawing.Color.LightGray;*/
            _WF.ColorMarker = System.Drawing.Color.Gold;
            //_WF.ColorBeat = System.Drawing.Color.LightSkyBlue;
            _WF.ColorVolume = System.Drawing.Color.White;
            _WF.DrawWaveForm = WaveForm.WAVEFORMDRAWTYPE.Mono;
            _WF.DrawEnvelope = true;
            _WF.DrawMarker = WaveForm.MARKERDRAWTYPE.Line | WaveForm.MARKERDRAWTYPE.Name | WaveForm.MARKERDRAWTYPE.NamePositionAlternate;
            _WF.MarkerLength = 0.75f;
            _WF.RenderStart(true, BASSFlag.BASS_DEFAULT);

        }

        private void MyWaveFormCallback(int framesDone, int framesTotal, TimeSpan elapsedTime, bool finished)
        {
            if (finished)
            {

                _WF.SyncPlayback(_currentTrack.Channel);

                // and do pre-calculate the next track position
                // in this example we will only use the end-position
                long startPos = 0L;
                long endPos = 0L;
                if (_WF.GetCuePoints(ref startPos, ref endPos, -24.0, -12.0, true))
                {
                    _currentTrack.NextTrackPos = endPos;
                    // if there is already a sync set, remove it first
                    if (_currentTrack.NextTrackSync != 0)
                        BassMix.BASS_Mixer_ChannelRemoveSync(_currentTrack.Channel, _currentTrack.NextTrackSync);

                    // set the next track sync automatically
                    _currentTrack.NextTrackSync = BassMix.BASS_Mixer_ChannelSetSync(_currentTrack.Channel, BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, _currentTrack.NextTrackPos, _currentTrack.TrackSync, new IntPtr(1));

                    _WF.AddMarker("Next", _currentTrack.NextTrackPos);
                }
            }
            // will be called during rendering...
            DrawWave();
        }

        private System.Drawing.Bitmap _bitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                // from System.Media.BitmapImage to System.Drawing.Bitmap 
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        public void DrawWave()
        {
            if (_WF != null)
            {
                this.Dispatcher.Invoke((Action)delegate()
                {
                    try
                    {
                        System.Drawing.Bitmap bitmap = _WF.CreateBitmap(500, 100, _zoomStart, _zoomEnd, true);
                       // bitmap.MakeTransparent(bitmap.GetPixel(0, 0));
                        BitmapSource im = BitmapSourceFromImage(bitmap);


                        this.pictureBoxWaveForm.Source = im;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                });
            }
            else
                this.pictureBoxWaveForm.Source = null;
        }
        private BitmapSource BitmapSourceFromImage(System.Drawing.Image img)
        {

            if (img != null)
            {

                System.IO.MemoryStream memStream = new System.IO.MemoryStream();

                //save the image to memStream as a png
                img.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);

                //gets a decoder from this stream
                System.Windows.Media.Imaging.PngBitmapDecoder decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(memStream, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.Default);

                return decoder.Frames[0];
            }
            else
                return null;
        }
        private BitmapSource _bitmapToSource(System.Drawing.Bitmap bitmap)
        {
            BitmapSource destination;
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            destination = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            destination.Freeze();

            return destination;
        }
        private void DrawWavePosition(long pos, long len)
        {
            if (_WF == null || len == 0 || pos < 0)
            {
                this.wavePosition.Width = 0;
                this.pictureBoxWaveForm.Source = null;
                return;
            }

            double bpp = 0;

            try
            {
                if (_zoomed)
                {
                    // total length doesn't have to be _zoomDistance sec. here
                    len = _WF.Frame2Bytes(_zoomEnd) - _zoomStartBytes;

                    int scrollOffset = 10; // 10*20ms = 200ms.
                    // if we scroll out the window...(scrollOffset*20ms before the zoom window ends)
                    if (pos > (_zoomStartBytes + len - scrollOffset * _WF.Wave.bpf))
                    {
                        // we 'scroll' our zoom with a little offset
                        _zoomStart = _WF.Position2Frames(pos - scrollOffset * _WF.Wave.bpf);
                        _zoomStartBytes = _WF.Frame2Bytes(_zoomStart);
                        _zoomEnd = _zoomStart + _WF.Position2Frames(_zoomDistance) - 1;
                        if (_zoomEnd >= _WF.Wave.data.Length)
                        {
                            // beyond the end, so we zoom from end - _zoomDistance.
                            _zoomEnd = _WF.Wave.data.Length - 1;
                            _zoomStart = _zoomEnd - _WF.Position2Frames(_zoomDistance) + 1;
                            if (_zoomStart < 0)
                                _zoomStart = 0;
                            _zoomStartBytes = _WF.Frame2Bytes(_zoomStart);
                            // total length doesn't have to be _zoomDistance sec. here
                            len = _WF.Frame2Bytes(_zoomEnd) - _zoomStartBytes;
                        }
                        // get the new wave image for the new zoom window
                        DrawWave();
                    }
                    // zoomed: starts with _zoomStartBytes and is _zoomDistance long
                    pos -= _zoomStartBytes; // offset of the zoomed window

                    bpp = len / (double)this.pictureBoxWaveForm.ActualWidth;  // bytes per pixel
                }
                else
                {
                    // not zoomed: width = length of stream
                    bpp = len / (double)this.pictureBoxWaveForm.ActualWidth;  // bytes per pixel
                }
                int x = (int)Math.Round(pos / bpp);  // position (x) where to draw the line
                this.Dispatcher.Invoke((Action)delegate()
                {
                    if (x > 0)
                        Canvas.SetLeft(this.wavePosition, x);



                });

            }
            catch
            {
                this.Dispatcher.Invoke((Action)delegate()
                    {
                        this.wavePosition.Width = 0;
                    });
            }


        }
        #endregion

        private void Button_Play(object sender, RoutedEventArgs e)
        {

            PlayNextTrack();
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {

            DrawWave();
        }

        private void pictureBoxWaveForm_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
        public void stopAudio()
        {
            this.listBoxPlaylist.Items.Clear();
            Bass.BASS_StreamFree(_mixer);
        }
        internal void load(LogListPrj.DataItems.DataSongItem item)
        {
            this.listBoxPlaylist.Items.Clear();
            newtrack(@"D:\OAIBC\" + item.filename, item);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            long pos = BassMix.BASS_Mixer_ChannelGetPosition(_currentTrack.Channel);
            if (((String)btn.Content).Contains("cue"))
            {

                this.newCue = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(_currentTrack.Channel, pos));
                labelCue.Text = TimeSpanToStr(this.newCue);
            }
            else if (((String)btn.Content).Contains("intro"))
            {

                this.newIntro = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(_currentTrack.Channel, pos));
                labelIntro.Text = TimeSpanToStr(this.newIntro);
            }
            else if (((String)btn.Content).Contains("next"))
            {

                this.newNext = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(_currentTrack.Channel, pos));
                labelNext.Text = TimeSpanToStr(this.newNext);
            }
        }
        TimeSpan newCue = TimeSpan.MinValue;
        TimeSpan newIntro = TimeSpan.MinValue;
        TimeSpan newNext = TimeSpan.MinValue;

        public string TimeSpanToStr(TimeSpan t)
        {
            String h = checkZero(t.Hours);
            String m = checkZero(t.Minutes);
            String s = checkZero(t.Seconds);
            String ms = checkZero(t.Milliseconds);

            if (h != "00")
                return h + ":" + m + ":" + s + "." + ms;
            else
                return m + ":" + s + "." + ms;
        }
        private string checkZero(int n)
        {
            if (n < 10) return "0" + n;
            else return "" + n;
        }
    }




    public class Track
    {
        public Track(string filename, LogListPrj.DataItems.DataSongItem item = null)
        {
            Filename = filename;
            Tags = BassTags.BASS_TAG_GetFromFile(Filename);
            if (Tags == null)
                //throw new ArgumentException("File not valid!");
                MessageBox.Show("File not found");

            this.item = item;
            // we already create a stream handle
            // might not be the best place here (especially when having a larger playlist), but for the demo this is okay ;)
            CreateStream();
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", Tags, Utils.FixTimespan(Tags.duration, "HHMMSS"));
        }

        // member
        public string Filename = String.Empty;
        public TAG_INFO Tags = null;
        public int Channel = 0;
        public long TrackLength = 0L;
        public SYNCPROC TrackSync;
        public int NextTrackSync = 0;
        public long NextTrackPos = 0L;
        public LogListPrj.DataItems.DataSongItem item;

        private bool CreateStream()
        {
            Channel = Bass.BASS_StreamCreateFile(Filename, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN);
            if (Channel != 0)
            {
                TrackLength = Bass.BASS_ChannelGetLength(Channel);
                return true;
            }
            return false;
        }
    }

}
