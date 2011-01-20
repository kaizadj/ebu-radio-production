using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS;

namespace LogListPrj.DataItems
{
    public class DataLogNoteItem : DataItem
    {
        public DataLogNoteItem(long ID, String label, TimeSpan runtime)
            : base(ID, label, runtime){}
        
        public override DataItemType dataitemtype
        {
            get { return DataItemType.LOGNOTE;  }
            set { }
        }
    }
}
