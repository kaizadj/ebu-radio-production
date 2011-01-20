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

namespace Playout.UI.Slots
{
    /// <summary>
    /// Interaction logic for USlotContent.xaml
    /// </summary>
    public partial class USlotContent : UserControl
    {
        private LogListPrj.SlotItem slot;


        public USlotContent(LogListPrj.SlotItem slot)
        {
            // TODO: Complete member initialization
            this.slot = slot;

            InitializeComponent();
            String[] slides = parseSlidesFromLog(slot.item.label);
            foreach (String s in slides)
                slidenames.Text += s;

        }

        public String[] parseSlidesFromLog(String loglabel){
            loglabel = loglabel.Substring(("CONTENTSEQUENCE").Count());
            return loglabel.Split('|');
            

            
        }
    }
}
