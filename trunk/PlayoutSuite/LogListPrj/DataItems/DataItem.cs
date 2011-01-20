using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS;
using System.ComponentModel;

namespace LogListPrj.DataItems
{

    /// <summary>
    /// DataItem gather every metadata of an ITEM. Abstract because the structure depends on the type of the ITEM
    /// </summary>



    public abstract class DataItem : IDataItem, INotifyPropertyChanged
    {

        private long iID;
        private String ilabel;
        private TimeSpan iruntime;
        private DataItemType idataitemtype;
        Dictionary<String, String> fields;

        public DataItem(long ID, String label, TimeSpan runtime)
        {
            this.iID = ID;
            this.ilabel = label;
            this.iruntime = runtime;
            this.fields = new Dictionary<string, string>();
        }

#region Properties

        public long ID
        {
            get{ return this.iID; }
            set { this.iID = value; onPropertyChanged("id"); }
        }

        public string label
        {
            get { return this.ilabel; }
            set { this.ilabel = value; onPropertyChanged("label"); }
        }
        public abstract DataItemType dataitemtype
        {
            get;
            set;
        }
        public virtual TimeSpan runtime
        {
            get { return this.iruntime; }
            set { this.iruntime = value; onPropertyChanged("runtime"); }
        }
        public string getField(string name)
        {
                return fields[name];            
        }
        public void setField(string name, string value)
        {
            if (fields.ContainsKey(name))
            {
                fields[name] = value;
            }
            else
            {
                fields.Add(name, value);
            }
            onPropertyChanged(name);

        }

#endregion Properties

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void onPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        #endregion
    }
}
