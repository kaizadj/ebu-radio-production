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
using LogListPrj;
using LogListPrj.DataItems;

namespace Playout.UI.Slots
{
    /// <summary>
    /// Interaction logic for USlot.xaml
    /// </summary>
    public partial class USlot : UserControl, IDisposable
    {
        public SlotItem slot { get; set; }
        public USlotSong slotsong = null; //TODO: Interface
        public USlotSync uisync = null;
        public USlotContent uicontent = null;
        public USlot(SlotItem slot)
        {
            this.slot = slot;

            InitializeComponent();
            this.Height = 20;

            if (slot.item.dataitemtype == DS.DataItemType.SONG)
            {
                this.slotsong = new USlotSong(slot);
                /*Grid g = (Grid)this.Content;
                g.Children.Add(slotsong);
                Grid.SetRow(slotsong,1);*/
                this.Content = slotsong;
                this.Height = this.slotsong.Height;
            }

            else if (slot.item.dataitemtype == DS.DataItemType.SYNC)
            {
                DataSyncItem sync = (DataSyncItem)slot.item;

                this.uisync = new USlotSync(sync.synctype, sync.scheduleddatetime, slot.uniqID);
                /*Grid g = (Grid)this.Content;
                g.Children.Add(slotsong);
                Grid.SetRow(slotsong,1);*/
                this.Content = uisync;
                this.Height = this.uisync.Height;
            }

            else if (slot.item.dataitemtype == DS.DataItemType.LOGNOTE && slot.item.label.IndexOf("CONTENTSEQUENCE")==0)
            {
                this.uicontent = new USlotContent(slot);
                /*Grid g = (Grid)this.Content;
                g.Children.Add(slotsong);
                Grid.SetRow(slotsong,1);*/
                this.Content = uicontent;
                this.Height = this.uicontent.Height;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Dispose()
        {
            this.slot = null;
            this.slotsong = null;
        }
    }
}
