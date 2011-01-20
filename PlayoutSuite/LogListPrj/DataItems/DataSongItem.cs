using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS;

namespace LogListPrj.DataItems
{

    public class DataSongItem : DataItem
    {

        private String ifilename;
        private String ititle;
        private String iartist;
        private TimeMarker itimemarker;
        private String icategory;

        public DataSongItem(long ID, String artist, String title, String filename, TimeMarker timemarker)
            : base(ID, /*"SONG(" + ID + "): " + */artist + " - " + title, timemarker.duration)
        {
            this.iartist = artist;
            this.ititle = title;
            this.ifilename = filename;
            this.itimemarker = timemarker;
        }

        public override DataItemType dataitemtype
        {
            get { return DataItemType.SONG; }
            set { }
        }

        public TimeMarker timemarker
        {
            get { return this.itimemarker; }
            set { this.itimemarker = value; onPropertyChanged("runtime"); }
        }

        public override TimeSpan runtime
        {
            get { return this.timemarker.duration; }
            set { this.timemarker.next = this.timemarker.cue + value; onPropertyChanged("runtime"); }
        }

        public String filename
        {
            get { return this.ifilename; }
            set { this.ifilename = value; onPropertyChanged("filename"); }
        }

        public String artist
        {
            get { return this.iartist; }
            set { this.iartist = value; onPropertyChanged("artist"); this.label = this.artist + " - " + this.title; }
        }

        public String title
        {
            get { return this.ititle; }
            set { this.ititle = value; onPropertyChanged("title"); this.label = this.artist + " - " + this.title; }
        }

        public String category
        {
            get { return this.icategory; }
            set { this.icategory = value; onPropertyChanged("category"); }
        }
    }
}
