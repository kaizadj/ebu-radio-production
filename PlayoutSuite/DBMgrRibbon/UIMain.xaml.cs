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
using Microsoft.Windows.Controls.Ribbon;
using System.Data;
using LogListPrj.DataItems;
using MysqlSchedule.DBlib;
using Un4seen.Bass;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using MysqlSchedule;

namespace DBMgrRibbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {


        private DataTable _UIData;
        public DataTable UIData { get { return _UIData; } set { _UIData = value; } }
        DataItems items;
        private int displayID = 1;
        private string currentSlice;
        private string currentPeriod;
        private  ITEMFILTER currentDisplay = ITEMFILTER.NONE;
        public MainWindow()
        {

            InitializeComponent();
            loadDataGrid();
            loadCategories();


            

            BassNet.Registration("mathieu@habegger-bros.ch", "2X22382431331");

            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                MessageBox.Show("Bass_Init error!");

                return;
            }
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 200);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 20);


        }


        public void loadCategories()
        {

            Dictionary<String, DataItemCategory> list = items.getCategoryList();

            in_category.Items.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                ComboBoxItem citem = new ComboBoxItem();
                citem.Content = list.ElementAt(i).Value.name;
                citem.Tag = list.ElementAt(i).Value.shortname;
                
                in_category.Items.Add(citem);
            }
            
        }

        public void loadDataGrid(ITEMFILTER filter=ITEMFILTER.NONE, String param="")
        {
            items = new DataItems(this);
            items.loadDataFromDB(filter, param);
            if (filter == ITEMFILTER.CANVASPERIOD)
            {
                this.currentDisplay = ITEMFILTER.CANVASPERIOD;
                this.grid.Columns.Clear();
                this.grid.Columns.Add(newTextCol("id", "id"));
                this.grid.Columns.Add(newTextCol("label", "label"));
                
                this.UIData = items.getUIDataTableCanvasPeriod();
            }
            else if (filter == ITEMFILTER.CANVASSLICE)
            {
                this.currentDisplay = ITEMFILTER.CANVASSLICE;
                this.grid.Columns.Clear();

                this.grid.Columns.Add(newTextCol("id", "id"));
                this.grid.Columns.Add(newTextCol("type", "type"));
                this.grid.Columns.Add(newTextCol("sliceposition", "sliceposition"));
                this.grid.Columns.Add(newTextCol("label", "label"));
                this.grid.Columns.Add(newTextCol("param1", "param1"));
                this.grid.Columns.Add(newTextCol("param2", "param2"));
                this.grid.Columns.Add(newTextCol("param3", "param3"));

                this.UIData = items.getUIDataTableCanvasSlice();
            }
                //CanvasPeriod
            else
            {
                this.currentDisplay = ITEMFILTER.CATEGORY;
                this.grid.Columns.Clear();
                this.grid.Columns.Add(newTextCol("data.ID", "id"));
                this.grid.Columns.Add(newTextCol("data.label", "label"));
                this.grid.Columns.Add(newTextCol("data.runtime", "runtime"));
                this.grid.Columns.Add(newTextCol("data.category", "category"));
                
                this.UIData = items.getUIDataTable();
            }
                
        }


        private void reloadTree()
        {
            libraryTree.Items.Clear();
            if (displayID == 1)
                generateLibraryDisplay();
            else if (displayID == 2)
                generateCanvasDisplay();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            reloadTree();

        }

        private static DataGridTextColumn newTextCol(String bindingValue, String header)
        {
            DataGridTextColumn col = new DataGridTextColumn();
            col.Binding = new Binding(bindingValue);
            col.Header = header;
            return col;
        }

        #region ribbon
        private void RibbonButton_Click_Cancel(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            int i = r.Next(50)+1;
            ((DataSongItem)items.dataitems[i]).artist = "test";
            Console.WriteLine(i);
        }

        private void RibbonButton_Click_Save(object sender, RoutedEventArgs e)
        {

            int selectedID = (int)((DataRowView)grid.SelectedItem)["id"];
            ((DataSongItem)items.dataitems[selectedID]).artist = in_artist.Text;
            ((DataSongItem)items.dataitems[selectedID]).title = in_title.Text;

            if (in_category.SelectedItem != null)
            {
                var categ = ((ComboBoxItem)in_category.SelectedItem).Tag;
                if (categ != null)
                    ((DataSongItem)items.dataitems[selectedID]).category = categ.ToString();
            }
            
        }
        #endregion

        #region grid
        private void grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grid.SelectedItem != null)
            {

                if (displayID == 1)
                {
                    int selectedID = (int)((DataRowView)grid.SelectedItem)["id"];
                    String shortname = ((DataSongItem)items.dataitems[selectedID]).category;
                    this.Title = shortname;

                    DataSongItem item = ((DataSongItem)items.dataitems[selectedID]);

                    this.in_category.SelectedIndex = getCategoryItem(shortname);

                    UIAudio winaudio = new UIAudio();
                    winaudio.audioplayer.load(item);
                    winaudio.ShowDialog();
                     
                }
                else if (displayID == 2)
                {
                    if(currentSlice != "-1"){
                        int selectedID = (int)((DataRowView)grid.SelectedItem)["id"];
                        if (grid.SelectedIndex == 0 && grid.SelectedIndex == grid.Items.Count - 1)
                        {

                            moveSlotUpBtn.IsEnabled = false;
                            moveSlotDownBtn.IsEnabled = false;
                        }
                        else if (grid.SelectedIndex == 0)
                        {
                            moveSlotUpBtn.IsEnabled = false;
                            moveSlotDownBtn.IsEnabled = true;
                        }
                        else if (grid.SelectedIndex == grid.Items.Count - 1)
                        {
                            moveSlotDownBtn.IsEnabled = false;
                            moveSlotUpBtn.IsEnabled = true;
                        }
                        else
                        {
                            moveSlotDownBtn.IsEnabled = true;
                            moveSlotUpBtn.IsEnabled = true;
                        }
                    }
                    else if (currentPeriod != "-1")
                    {
                        int selectedID = Int32.Parse(((DataRowView)grid.SelectedItem)["id"].ToString());
                        if(grid.SelectedIndex == 0 && grid.SelectedIndex == grid.Items.Count - 1){
                            
                            moveSlotUpBtn.IsEnabled = false;
                            moveSlotDownBtn.IsEnabled = false;
                        }
                        else if (grid.SelectedIndex == 0)
                        {
                            moveSlotUpBtn.IsEnabled = false;
                            moveSlotDownBtn.IsEnabled = true;
                        }
                        else if (grid.SelectedIndex == grid.Items.Count - 1)
                        {
                            moveSlotDownBtn.IsEnabled = false;
                            moveSlotUpBtn.IsEnabled = true;
                        }
                        else
                        {
                            moveSlotDownBtn.IsEnabled = true;
                            moveSlotUpBtn.IsEnabled = true;
                        }
                    }
                }
            }
        }

#endregion

        #region dataitem
        public int getCategoryItem(String shortname)
        {
            ItemCollection items = in_category.Items;
            ComboBoxItem it = null;
            foreach (ComboBoxItem item in items)
            {
                if (item.Tag.ToString().Equals(shortname))
                {
                    it = item;
                }
            }

            return items.IndexOf(it);
        }
        #endregion
        private void This_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Bass.BASS_Free();
        }



        private void generateCanvasDisplay()
        {

            TreeViewItem days = newTreeItem("Days");
            TreeViewItem slices = newTreeItem("Slices");
            TreeViewItem minilists = newTreeItem("Mini-List");

            List<PeriodCanvas> dayCanvasList = items.listPeriodCanvas();
            for (int i = 0; i < dayCanvasList.Count; i++)
            {
                days.Items.Add(newCanvasTreeItem(dayCanvasList.ElementAt(i).name, dayCanvasList.ElementAt(i).id, CANVASTYPE.PERIOD));
            }
            
            List<SliceCanvas> sliceCanvasList = items.listSliceCanvas();
            for (int i = 0; i < sliceCanvasList.Count; i++)
            {
                slices.Items.Add(newCanvasTreeItem(sliceCanvasList.ElementAt(i).name, sliceCanvasList.ElementAt(i).id, CANVASTYPE.SLICE));
            }
            
            libraryTree.Items.Add(days);
            libraryTree.Items.Add(slices);
            
        }

        private void generateLibraryDisplay()
        {
            TreeViewItem root = newTreeItem("EBU RADIO");
            TreeViewItem music = newTreeItem("MUSIC");
            TreeViewItem news = newTreeItem("NEWS");
            TreeViewItem misc = newTreeItem("MISC.");
            TreeViewItem other = newTreeItem("OTHER");
            TreeViewItem commercial = newTreeItem("COMMERCIAL");

            Dictionary<String, DataItemCategory> list = items.getCategoryList();

            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i).Value.categoryType == "MISC")
                {
                    misc.Items.Add(newCategTreeItem(list.ElementAt(i).Value.name, list.ElementAt(i).Value.shortname));
                }
                else if (list.ElementAt(i).Value.categoryType == "NEWS")
                {
                    news.Items.Add(newCategTreeItem(list.ElementAt(i).Value.name, list.ElementAt(i).Value.shortname));
                }
                else if (list.ElementAt(i).Value.categoryType == "MUSIC")
                {
                    music.Items.Add(newCategTreeItem(list.ElementAt(i).Value.name, list.ElementAt(i).Value.shortname));
                }
                else if (list.ElementAt(i).Value.categoryType == "COMMERCIAL")
                {
                    commercial.Items.Add(newCategTreeItem(list.ElementAt(i).Value.name, list.ElementAt(i).Value.shortname));
                }
                else
                {
                    other.Items.Add(newCategTreeItem(list.ElementAt(i).Value.name, list.ElementAt(i).Value.shortname));
                }
            }

            libraryTree.Items.Add(root);
            root.Items.Add(music);
            root.Items.Add(misc);
            root.Items.Add(news);
            root.Items.Add(commercial);
            root.Items.Add(other);
            root.IsExpanded = true;
            
        }

        private TreeViewItem newCategTreeItem(string label, string tag)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = label;
            item.Tag = tag;
            item.MouseDoubleClick += new MouseButtonEventHandler(item_MouseDoubleClick);
            
            return item;
        }

        //Categ Tree Event
        void item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            String shortname = item.Tag.ToString();
            
            
            grid.ItemsSource = null;
            this.loadDataGrid(ITEMFILTER.CATEGORY, shortname);
            grid.ItemsSource = UIData.DefaultView;
            
        }

        enum CANVASTYPE {PERIOD, SLICE}
        private TreeViewItem newCanvasTreeItem(string label, string tag, CANVASTYPE type)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = label;
            item.Tag = tag;
            if (type == CANVASTYPE.PERIOD)
            {
                item.MouseDoubleClick += new MouseButtonEventHandler(canvasperiodItem_MouseDoubleClick);
               /* item.MouseRightButtonDown += new MouseButtonEventHandler(delegate(Object sender, MouseButtonEventArgs e)
                {
                    item.ContextMenu = new ContextMenu();
                    //Remove item
                    MenuItem mi = new MenuItem();
                    mi.Header = "Remove item";
                    mi.Click += new RoutedEventHandler(delegate(Object s, RoutedEventArgs evt)
                    {

                        items.removePeriod(Int32.Parse(item.Tag.ToString()));
                        reloadTree();
                    });
                    item.ContextMenu.Items.Add(mi);


                });*/
            }
            else if (type == CANVASTYPE.SLICE)
            {
                item.MouseDoubleClick += new MouseButtonEventHandler(canvassliceItem_MouseDoubleClick);
                item.MouseRightButtonDown += new MouseButtonEventHandler(delegate(Object sender, MouseButtonEventArgs e)
                {
                    item.ContextMenu = new ContextMenu();
                    //Remove item
                    MenuItem mi = new MenuItem();
                    mi.Header = "Remove item";
                    mi.Click += new RoutedEventHandler(delegate(Object s, RoutedEventArgs evt)
                    {

                        items.removeSlice(Int32.Parse(item.Tag.ToString()));
                        reloadTree();
                    });
                    item.ContextMenu.Items.Add(mi);


                });

            }
            return item;
        }

        void canvasperiodItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            String id = item.Tag.ToString();

            this.currentPeriod = id;
            this.currentSlice = "-1";
            grid.ItemsSource = null;
            this.loadDataGrid(ITEMFILTER.CANVASPERIOD, id);
            grid.ItemsSource = UIData.DefaultView;
            
        }

        void canvassliceItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            String id = item.Tag.ToString();


            grid.ItemsSource = null;
            this.currentSlice = id;
            this.currentPeriod = "-1";
            this.loadDataGrid(ITEMFILTER.CANVASSLICE, id);
            grid.ItemsSource = UIData.DefaultView;
        }

        private TreeViewItem newTreeItem(String name)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = name;

            return item;
        }

        private void in_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            Console.WriteLine(btn.Name);
            String tag = btn.Content.ToString();
            Console.WriteLine(tag);

            int tbcheckedid = -1;
            switch (btn.Name)
            {
                case "tb1":
                       tbcheckedid = 1;
                       break;
                case "tb2":
                       tbcheckedid = 2;
                       break;
                case "tb3":
                       tbcheckedid = 3;
                       break;
            }
            
            tb1.IsChecked = (tbcheckedid == 1);
            tb2.IsChecked = (tbcheckedid == 2);
            tb3.IsChecked = (tbcheckedid == 3);

            swapDisplay(tbcheckedid);
        }

        private void swapDisplay(int id)
        {
            displayID = id;
            if (id == 3)
            {
                libraryTree.Visibility = System.Windows.Visibility.Hidden;
                scheduleCalendar.Visibility = System.Windows.Visibility.Visible;
                loggrid.Visibility = System.Windows.Visibility.Visible;
                grid.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                loggrid.Visibility = System.Windows.Visibility.Hidden;
                grid.Visibility = System.Windows.Visibility.Visible;
                libraryTree.Visibility = System.Windows.Visibility.Visible;
                scheduleCalendar.Visibility = System.Windows.Visibility.Hidden;
                reloadTree();
            }
        }

        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
          /*  if (((TreeViewItem)libraryTree.SelectedItem).Tag == "slice")
            {
                removeSliceBtn.IsEnabled = true;
            }*/
        }



        private void newSliceBtnClick(object sender, RoutedEventArgs e)
        {
            items.newSlice(this.newSliceName.Text);

            reloadTree();
        }

        private void GridRightBtnDown(object sender, MouseButtonEventArgs e)
        {
            if (grid.SelectedItem != null)
            {
                if (currentDisplay == ITEMFILTER.CATEGORY)
                {
                   // MessageBox.Show("CATEGORY");
                }
                else if (currentDisplay == ITEMFILTER.CANVASSLICE)
                {
                  //  MessageBox.Show("CANVASSLICE");

                    int selectedId = (int)((DataRowView)grid.SelectedItem)["id"];
                    int position = (int)((DataRowView)grid.SelectedItem)["sliceposition"];
                    SlotCanvasType type = (SlotCanvasType)((DataRowView)grid.SelectedItem)["type"];
                    String param1 = ((DataRowView)grid.SelectedItem)["param1"].ToString();
                    String param2 = ((DataRowView)grid.SelectedItem)["param2"].ToString();
                    String param3 = ((DataRowView)grid.SelectedItem)["param3"].ToString();

                   // MessageBox.Show(param1);


                    UICanvasItem uicanvasitem = new UICanvasItem();
                    uicanvasitem.setModifValues(Int32.Parse(currentSlice), selectedId, position, type, param1, param2, param3);


                    uicanvasitem.ShowDialog();



                    int selectedI = grid.SelectedIndex;
                    this.loadDataGrid(ITEMFILTER.CANVASSLICE, currentSlice);
                    grid.ItemsSource = UIData.DefaultView;
                    grid.SelectedIndex = selectedI;
                }
                else if (currentDisplay == ITEMFILTER.CANVASPERIOD)
                {
                   // MessageBox.Show("CANVASPERIOD");

                }
            }
        }

        private void newCanvasSliceItem(object sender, RoutedEventArgs e)
        {
            if (displayID == 2)
                {
                    if (currentSlice != "-1")
                    {
                        int n = 0;
                        if(((DataView)grid.ItemsSource).Count>0)
                            n = (int)((DataView)grid.ItemsSource)[((DataView)grid.ItemsSource).Count - 1].Row["sliceposition"] + 1;
                        UICanvasItem uicanvasitem = new UICanvasItem();
                        uicanvasitem.setCanvasId(Int32.Parse(currentSlice), n);
                        uicanvasitem.ShowDialog();




                        this.loadDataGrid(ITEMFILTER.CANVASSLICE, currentSlice);
                        grid.ItemsSource = UIData.DefaultView;
                    }
                    else if (currentPeriod != "-1")
                    {
                        int n = 0;
                        if (((DataView)grid.ItemsSource).Count > 0)
                            n = (int)((DataView)grid.ItemsSource)[((DataView)grid.ItemsSource).Count - 1].Row["position"] + 1;
                        UIPeriodItem uiperioditem = new UIPeriodItem();
                        uiperioditem.setPeriodId(Int32.Parse(currentPeriod), n);
                        uiperioditem.ShowDialog();




                        this.loadDataGrid(ITEMFILTER.CANVASPERIOD, currentPeriod);
                        grid.ItemsSource = UIData.DefaultView;
                    }

                }

        }

        private void moveDownCanvasSliceItem(object sender, RoutedEventArgs e)
        {
            int selectedI = grid.SelectedIndex;
            if (selectedI!=-1)
            {
                if (this.currentSlice != "-1")
                {
                    //!!!DISTRIBUTED OPERATION NOT SUPPORTED
                    int selectedId = (int)((DataRowView)grid.SelectedItem)["id"];

                    int nextId = (int)((DataView)grid.ItemsSource).Table.Rows[grid.SelectedIndex + 1]["id"];
                    int currentPos = (int)((DataRowView)grid.SelectedItem)["sliceposition"];

                    //!!! Hide behind DataItems Class
                    DBengineMySql dbengine = DBengineMySql.GetInstance();
                    dbengine.moveDownCanvasSlot(Int32.Parse(currentSlice), selectedId, nextId, currentPos);

                    this.loadDataGrid(ITEMFILTER.CANVASSLICE, currentSlice);
                    grid.ItemsSource = UIData.DefaultView;
                    grid.SelectedIndex = selectedI + 1;
                }
                else if (this.currentPeriod != "-1")
                {
                    //!!!DISTRIBUTED OPERATION NOT SUPPORTED
                    int selectedId = Int32.Parse(((DataRowView)grid.SelectedItem)["id"].ToString());

                    int nextId = Int32.Parse(((DataView)grid.ItemsSource).Table.Rows[grid.SelectedIndex + 1]["id"].ToString());
                    int currentPos = (int)((DataRowView)grid.SelectedItem)["position"];

                    //!!! Hide behind DataItems Class
                    DBengineMySql dbengine = DBengineMySql.GetInstance();
                    dbengine.moveDownCanvasSlice(Int32.Parse(currentPeriod), selectedId, nextId, currentPos);

                    this.loadDataGrid(ITEMFILTER.CANVASPERIOD, currentPeriod);
                    grid.ItemsSource = UIData.DefaultView;
                    grid.SelectedIndex = selectedI + 1;
                }
            }
        }

        private void moveUpCanvasSliceItem(object sender, RoutedEventArgs e)
        {
            int selectedI = grid.SelectedIndex;
            if (selectedI != -1)
            {

                if (this.currentSlice != "-1")
                {

                    //!!!DISTRIBUTED OPERATION NOT SUPPORTED
                    int selectedId = (int)((DataRowView)grid.SelectedItem)["id"];

                    int nextId = (int)((DataView)grid.ItemsSource).Table.Rows[grid.SelectedIndex - 1]["id"];
                    int currentPos = (int)((DataRowView)grid.SelectedItem)["sliceposition"];

                    //!!! Hide behind DataItems Class
                    DBengineMySql dbengine = DBengineMySql.GetInstance();
                    dbengine.moveUpCanvasSlot(Int32.Parse(currentSlice), selectedId, nextId, currentPos);

                    this.loadDataGrid(ITEMFILTER.CANVASSLICE, currentSlice);
                    grid.ItemsSource = UIData.DefaultView;
                    grid.SelectedIndex = selectedI - 1;
                }
                else if (this.currentPeriod != "-1")
                {
                    //!!!DISTRIBUTED OPERATION NOT SUPPORTED
                    int selectedId = Int32.Parse(((DataRowView)grid.SelectedItem)["id"].ToString());

                    int nextId = Int32.Parse(((DataView)grid.ItemsSource).Table.Rows[grid.SelectedIndex - 1]["id"].ToString());
                    int currentPos = (int)((DataRowView)grid.SelectedItem)["position"];

                    //!!! Hide behind DataItems Class
                    DBengineMySql dbengine = DBengineMySql.GetInstance();
                    dbengine.moveUpCanvasSlice(Int32.Parse(currentPeriod), selectedId, nextId, currentPos);

                    this.loadDataGrid(ITEMFILTER.CANVASPERIOD, currentPeriod);
                    grid.ItemsSource = UIData.DefaultView;
                    grid.SelectedIndex = selectedI - 1;
                }
            }
        }

        private void removeCanvasSliceItem(object sender, RoutedEventArgs e)
        {
            if (this.currentSlice != "-1")
            {
                //!!!DISTRIBUTED OPERATION NOT SUPPORTED
                int selectedId = (int)((DataRowView)grid.SelectedItem)["id"];

                //!!! Hide behind DataItems Class
                DBengineMySql dbengine = DBengineMySql.GetInstance();
                dbengine.removeFromSlice(selectedId);

                this.loadDataGrid(ITEMFILTER.CANVASSLICE, currentSlice);
                grid.ItemsSource = UIData.DefaultView;
            }
            else if (this.currentPeriod != "-1")
            {
                //HERE

                int selectedId = Int32.Parse(((DataRowView)grid.SelectedItem)["id"].ToString());
                //MessageBox.Show(selectedId.ToString());


                //!!! Hide behind DataItems Class
                DBengineMySql dbengine = DBengineMySql.GetInstance();
                dbengine.removeFromSlicePeriod(selectedId, this.currentPeriod);


                this.loadDataGrid(ITEMFILTER.CANVASPERIOD, currentPeriod);
                grid.ItemsSource = UIData.DefaultView;
            }
        }
        BackgroundWorker scheduleWorker;
        private void scheduleCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            this.IsEnabled = false;
            Calendar cal = (Calendar)sender;

            //!!! Hide behind DataItems Class
            DBengineMySql dbengine = DBengineMySql.GetInstance();
            DateTime date = DateTime.Parse(cal.SelectedDate.ToString());
            if (!dbengine.isScheduled(date))
            {
                if (MessageBox.Show("Would you like to schedule " + date.ToShortDateString() + " with NORMALDAY?", "Day not scheduled", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ScheduleEngine sengine = new ScheduleEngine(date, "1");
                    
                    sengine.ProgressChanged += new ProgressChangedEventHandler(delegate(object s2, ProgressChangedEventArgs evt2)
                    {
                        this.Dispatcher.Invoke(new Action(delegate()
                        {
                            this.ProgressBar.Value = evt2.ProgressPercentage;
                            this.ProgressBar.InvalidateVisual();
                        }), null);
                    });

                    scheduleWorker = new BackgroundWorker();
                    scheduleWorker.ProgressChanged += new ProgressChangedEventHandler(scheduleWorker_ProgressChanged);
                    
                    scheduleWorker.DoWork += new DoWorkEventHandler(delegate(object s, DoWorkEventArgs evt)
                    {
 
                        this.Dispatcher.Invoke(new Action(delegate()
                        {
                            this.ProgressBar.Maximum = 100;
                            this.ProgressBar.Visibility = System.Windows.Visibility.Visible;
                        }), null);
                        
                        sengine.startScheduling();

                    });

                    scheduleWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object s, RunWorkerCompletedEventArgs evt)
                    {
                        this.IsEnabled = true;
                        MessageBox.Show("Schedule done");
                        LogListPrj.LogList log = dbengine.getLogList(date);
                        loggrid.ItemsSource = log.getDataTable().DefaultView;

                        this.Dispatcher.Invoke(new Action(delegate()
                        {
                            this.ProgressBar.Visibility = System.Windows.Visibility.Hidden;
                        }), null);
                        
                    });

                    scheduleWorker.RunWorkerAsync();

                }
                else
                    this.IsEnabled = true;
            }
            else
            {
                LogListPrj.LogList log = dbengine.getLogList(date);
                
                loggrid.ItemsSource = log.getDataTable().DefaultView;
                this.IsEnabled = true;
            }

        }

        void scheduleWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressBar.Value = e.ProgressPercentage;
        }


        private void loadCalendar(){

            DBengineMySql dbengine = DBengineMySql.GetInstance();
            List<DateTime> dts = dbengine.getCalendarScheduledRanges();
        }

        private void libraryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                if (displayID == 2)
                {
                    if (currentSlice !="-1")
                    {

                        int id = Int32.Parse(((DataRowView)grid.SelectedItem)["id"].ToString());
                        SlotCanvasType type = (SlotCanvasType)((DataRowView)grid.SelectedItem)["type"];
                        string label = ((DataRowView)grid.SelectedItem)["label"].ToString();
                        string param1 = ((DataRowView)grid.SelectedItem)["param1"].ToString();
                        string param2 = ((DataRowView)grid.SelectedItem)["param2"].ToString();
                        string param3 = ((DataRowView)grid.SelectedItem)["param3"].ToString();

                        int n = 0;
                        if (((DataView)grid.ItemsSource).Count > 0)
                            n = (int)((DataView)grid.ItemsSource)[((DataView)grid.ItemsSource).Count - 1].Row["sliceposition"] + 1;

                        DBengineMySql dbengine = DBengineMySql.GetInstance();
                        dbengine.addNewCanvasItem(Int32.Parse(currentSlice), n, type, param1, param2, param3, label);


                        this.loadDataGrid(ITEMFILTER.CANVASSLICE, currentSlice);
                        
                    }
                }
            }
        }

    }
}
