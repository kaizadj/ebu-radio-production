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
    /// Interaction logic for UICanvasItem.xaml
    /// </summary>
    public partial class UICanvasItem : Window
    {
        SlotCanvasType DISPLAYTYPE = SlotCanvasType.CATEGORYITEM;
        DBengine dbengine;
        public int itemId = -1;
        private int canvasId = -1;
        private int canvasPos = -1;
        private Dictionary<SlotCanvasType, ComboBoxItem> ctypeItems = new Dictionary<SlotCanvasType, ComboBoxItem>();
        private Dictionary<String, ComboBoxItem> ccategItems = new Dictionary<String, ComboBoxItem>();
        public void setCanvasId(int id, int pos) { this.canvasId = id; this.canvasPos = pos; this.Title = "Add new item to canvas: " + id + " at pos:" + canvasPos; }

        public UICanvasItem()
        {
            this.dbengine = DBengineMySql.GetInstance();
            InitializeComponent();


            this.comboType.Items.Add(newComboItem("SLIDE CHANGE", SlotCanvasType.SLIDESLOAD));
            this.comboType.Items.Add(newComboItem("CATEGORY ITEM", SlotCanvasType.CATEGORYITEM));
            this.comboType.Items.Add(newComboItem("SPECIFIC ITEM", SlotCanvasType.SPECIFICITEM));
            this.comboType.Items.Add(newComboItem("SYNCHRONIZATION", SlotCanvasType.SYNC));
            fillSyncType(this.param1SyncCombo);
            fillCategory(this.param1CategCombo);
            this.comboType.SelectedIndex = 2;

           // refreshLayout();
        }
        
        public ComboBoxItem newComboItem(String label, SlotCanvasType tag){
            ComboBoxItem c = new ComboBoxItem();
            c.Content = label;
            c.Tag = tag;
            ctypeItems.Add(tag, c);
            return c;
        }

        public void refreshLayout(){


            
                    this.param1Txt.Text = "";
                    this.param1Value.Visibility         = Visibility.Hidden;
                    this.param1Value.Text = "";
                    this.param1CategCombo.Visibility    = Visibility.Hidden;
                    this.param1SyncCombo.Visibility     = Visibility.Hidden;
                    this.param1Label.Visibility = Visibility.Hidden;
                    this.param1Label.Text = "";

                    this.param2Txt.Text = "";
                    this.param2Txt.Visibility = Visibility.Hidden;
                    this.param2Value.Text = "";
                    this.param2Value.Visibility         = Visibility.Hidden;
                    this.param2Clock.Visibility         = Visibility.Hidden;


                    this.param3Txt.Text = "";
                    this.param3Txt.Visibility = Visibility.Hidden;
                    this.param3Value.Text = "";
                    this.param3Value.Visibility = Visibility.Hidden;

           
                if (DISPLAYTYPE == SlotCanvasType.SPECIFICITEM)
                {
                    this.param1Txt.Text = "ID :";
                    this.param1Value.Visibility = Visibility.Visible; 
                    param1Label.Visibility = Visibility.Visible;
                    

                }
                else if (DISPLAYTYPE == SlotCanvasType.CATEGORYITEM)
                {
                    this.param1Txt.Text = "Category :";
                    this.param1CategCombo.Visibility    = Visibility.Visible;
                }
                else if (DISPLAYTYPE == SlotCanvasType.SYNC)
                {
                    this.param1Txt.Text = "Synchronization type :";
                    this.param1SyncCombo.Visibility = Visibility.Visible;
                    this.param2Txt.Visibility = Visibility.Visible;
                    this.param2Txt.Text = "Relative Time from begining\nof the slice :\n(hh:mm:ss)";
                    this.param2Clock.Visibility = Visibility.Visible;
                }
                else if (DISPLAYTYPE == SlotCanvasType.SLIDESLOAD)
                {
                    this.param1Txt.Text = "SlideID 1 :";
                    this.param2Txt.Text = "SlideID 2 :";
                    this.param3Txt.Text = "SlideID 3 :";
                    this.param1Txt.Visibility = Visibility.Visible;
                    this.param2Txt.Visibility = Visibility.Visible;
                    this.param3Txt.Visibility = Visibility.Visible;
                    this.param1Value.Visibility = Visibility.Visible;
                    this.param2Value.Visibility = Visibility.Visible;
                    this.param3Value.Visibility = Visibility.Visible;
                }
        }

        

        private void fillCategory(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            Dictionary<String, DataItemCategory> categ = dbengine.listCategories();
            for (int i = 0; i < categ.Count; i++)
            {
                ComboBoxItem c = new ComboBoxItem();
                c.Content = categ.ElementAt(i).Value.name;
                c.Tag = categ.ElementAt(i).Value.shortname;
                this.ccategItems.Add(categ.ElementAt(i).Value.shortname, c);
                comboBox.Items.Add(c);
            }

        }

        private void fillSyncType(ComboBox comboBox)
        {

            comboBox.Items.Clear();
            comboBox.Items.Add("HARD");
            comboBox.Items.Add("SOFT");
            
        }

        private void comboTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DISPLAYTYPE = ((SlotCanvasType)((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag);
            refreshLayout();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String param1="";
            String param2="";
            String param3="";
            String label = "";
            if (DISPLAYTYPE == SlotCanvasType.SPECIFICITEM)
            {
                param1 = this.param1Value.Text;
                label = this.param1Label.Text;
            }
            else if (DISPLAYTYPE == SlotCanvasType.CATEGORYITEM)
            {
                param1 = ((ComboBoxItem)this.param1CategCombo.SelectedValue).Tag.ToString();
                label = this.param1CategCombo.Text;
            }
            else if (DISPLAYTYPE == SlotCanvasType.SYNC)
            {
                param1 = this.param1SyncCombo.SelectedValue.ToString();
                param2 = this.param2Clock.Value;
                label = "SYNC " + param1 + " : " + param2;
            }
            else if (DISPLAYTYPE == SlotCanvasType.SLIDESLOAD)
            {
                param1 = this.param1Value.Text+","+this.param2Value.Text+","+this.param3Value.Text;
                label = "SLIDES CHANGE : " + param1;
            }

          //  MessageBox.Show("canvasId:" + canvasId + " - itemId:" + itemId + "\n=> " + param1 + "\n=> " + param2 + "\n=> " + param3+"\nlabel: "+label+"\ncanvasId:"+canvasId+"\nitemID:"+itemId);
            if (this.itemId == -1)
            {
                dbengine.addNewCanvasItem(canvasId, canvasPos, this.DISPLAYTYPE, param1, param2, param3, label);
                this.Close();
            }
            else
            {
                dbengine.modifyCanvasItem(canvasId, canvasPos, itemId, this.DISPLAYTYPE, param1, param2, param3, label);
                this.Close();
            }
            


            //
        }

        private void param1Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DISPLAYTYPE == SlotCanvasType.SPECIFICITEM)
            {
                this.param1Label.Dispatcher.Invoke(new Action(delegate()
                {
                    try{
                        Int32.Parse(param1Value.Text);
                    
                        DataTable table = dbengine.listItems(ITEMFILTER.DATAITEMID, param1Value.Text);
                        if (table.Rows.Count > 0)
                            param1Label.Text = table.Rows[0]["title"].ToString() + " - " + table.Rows[0]["artist"].ToString();
                        else
                            param1Label.Text = "not found";
                    }
                    catch
                    {
                        if(param1Value.Text=="")
                            param1Label.Text = "insert an id";
                        else
                            param1Label.Text = "bad format";
                    }
                }));
            }

        }

        internal void setModifValues(int sliceId, int selectedId, int position, SlotCanvasType type, string param1, string param2, string param3)
        {
            this.canvasId = sliceId;
            this.itemId = selectedId;
            this.canvasPos = position;
            this.DISPLAYTYPE = type;
            
            this.comboType.SelectedItem = ctypeItems[type];


            
            if (DISPLAYTYPE == SlotCanvasType.SPECIFICITEM)
            {
                this.param1Value.Text = param1;
                param1Value_TextChanged(null,null);
            }
            else if (DISPLAYTYPE == SlotCanvasType.CATEGORYITEM)
            {
                param1CategCombo.SelectedItem = this.ccategItems[param1];
            }
            else if (DISPLAYTYPE == SlotCanvasType.SYNC)
            {
                param1SyncCombo.SelectedItem = param1;
                param2Clock.Value = param2;
            }
            else if (DISPLAYTYPE == SlotCanvasType.SLIDESLOAD)
            {
                String[] s = param1.Split(',');
                Queue<String> q = new Queue<string>(s);
                param1Value.Text = q.Dequeue();
                param2Value.Text = q.Dequeue();
                param3Value.Text = String.Join(",", q.ToArray());
            }
        }
    }
}

