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
using System.Windows.Shapes;
using MysqlSchedule.DBlib;
using MysqlSchedule;
using System.Data;

namespace DBMgrRibbon
{
    /// <summary>
    /// Interaction logic for UIPeriodItem.xaml
    /// </summary>
    public partial class UIPeriodItem : Window
    {
        private int periodId;
        private int position;
        public UIPeriodItem()
        {
            InitializeComponent();
            DBengineMySql db = DBengineMySql.GetInstance();
            List<SliceCanvas> slices = db.listSliceCanvas();
            for (int i = 0; i < slices.Count; i++)
            {
                listSlices.Items.Add(newLBitem(slices.ElementAt(i).name, slices.ElementAt(i).id));
            }
        }
        private ListBoxItem newLBitem(String name, String tag)
        {
            ListBoxItem c = new ListBoxItem();
            c.Content = name;
            c.Tag = tag;            
            return c;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)listSlices.SelectedItem;
            String name = lbi.Content.ToString();
            String tag = lbi.Tag.ToString();

            DBengineMySql dbengine = DBengineMySql.GetInstance();
            dbengine.addSliceToPeriod(periodId, position, tag);
            this.Close();
        }

        internal void setPeriodId(int p, int n)
        {
            this.periodId = p;
            this.position = n;
        }
    }
}
