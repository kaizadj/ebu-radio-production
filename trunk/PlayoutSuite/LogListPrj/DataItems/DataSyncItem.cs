using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS;

namespace LogListPrj.DataItems
{
    public class DataSyncItem : DataItem
    {

        private SyncType isynctype;
        private DateTime ischeduleddatetime;

        public DataSyncItem(long ID, DS.SyncType synctype, DateTime scheduleddatetime)
            : base(ID, synctype + " synchronization", TimeSpan.Zero)
        {
            this.isynctype = synctype;
            this.ischeduleddatetime = scheduleddatetime;
        }
        
        public override DataItemType dataitemtype
        {
            get { return DataItemType.SYNC; }
            set { }
        }

        public DS.SyncType synctype
        {
            get{ return this.isynctype;}
            set { this.isynctype = value;}
        }

        public DateTime scheduleddatetime
        {
            get {return this.ischeduleddatetime;}
            set {this.ischeduleddatetime = value;}
        }

    }
}
