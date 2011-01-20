//#define MUSICMASTERSUPPORTED

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
using System.Data;
using System.Windows.Threading;
using System.ServiceModel;


namespace Playout
{
  
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable _LogData;
        public DataTable LogData
        { get { return _LogData; } }
        public LogList loglist { get; private set; }

        public static MainWindow Instance = null;


        public MainWindow()
        {

            //LogList log = new LogList(DateTime.Now);
            LogList log = null;
            MessageBoxResult msgboxres = MessageBox.Show("Would you like to load playlist from mysql [yes] or from MusicMaster DB [no]", "LogList loading...", MessageBoxButton.YesNoCancel);
            if (msgboxres == MessageBoxResult.Yes)
            {
                log = loadMysqlLogList();
            }
            else if (msgboxres == MessageBoxResult.No)
                log = loadMMLogList();
            else
                Environment.Exit(0);
            


            log.loadPlaylist();
            log.refreshAirdatetime();
            MessageBox.Show("PLAYLIST OK");
            _LogData = log.getDataTable();
            this.loglist = log;
            InitializeComponent();
            
            Instance=this;


            playoutFrame.loadLoglist(log);
            loggrid.Items.Refresh();
            DateTime startTime = DateTime.Now;//.Add(TimeSpan.FromMinutes(55).Add(TimeSpan.FromHours(11)));

            if(log.slices.Count != 0){
                for (int i = 0; i < loggrid.Items.Count; i++)
                {
                    if (loggrid.Items[i] is DataRowView)
                    {
                        DataRowView rowview = (DataRowView)loggrid.Items[i];
                        if (rowview != null)
                        {
                            DataRow row = rowview.Row;
                            if (row != null && row["slot"] != null && !row["slot"].Equals(DBNull.Value))
                            {
                                SlotItem slot = row["slot"] as SlotItem;
                                if (slot != null && slot.scheduleddatetime > startTime)
                                {
                                    loggrid.SelectedIndex = i;
                                    loggrid.ScrollIntoView(loggrid.SelectedItem);
                                    break;
                                }

                            }
                        }
                    }
                }
            }

            loggrid.SelectedCellsChanged += new SelectedCellsChangedEventHandler(grid_SelectedCellsChanged);
            
            contentFrame.assignNewOnAirHandler(playoutFrame);

            errorAdd("Playout Running...", "MAIN");
        }

        public LogList loadMysqlLogList()
        {
            DateTime startDateTime = DateTime.Now;//DateTime.Parse("2010-09-02 00:00:00");
            DateTime endDateTime = DateTime.Now.AddDays(1);  //Parse("2010-09-15 23:00:00");

            MysqlSchedule.DBlib.DBengineMySql dbengine = MysqlSchedule.DBlib.DBengineMySql.GetInstance();
            dbengine.connect("127.0.0.1", "root", "1234", "ebuplayout-dev2");
            LogListPrj.LogList log = dbengine.getLogList(startDateTime.Date);
            log.refreshAirdatetime();
            
            return log;
        }
        
        public LogList loadMMLogList()
        {
#if MUSICMASTERSUPPORTED
            DateTime startDateTime = DateTime.Now;//DateTime.Parse("2010-09-02 00:00:00");
            DateTime endDateTime = DateTime.Now.AddDays(1);
            try
            {
                SMPAG.MM.MMConnector.MusicMasterAPI mmAPI = new SMPAG.MM.MMConnector.MusicMasterAPI(System.Configuration.ConfigurationSettings.AppSettings["MMDataBasePath"]);
                SMPAG.Smartrix.BusinessLogic.ScheduleQuery q = new SMPAG.Smartrix.BusinessLogic.ScheduleQuery(startDateTime, endDateTime);
                SMPAG.Smartrix.BusinessLogic.ScheduleResult r = mmAPI.querySchedule(q);

                // SMPAG.Smartrix.BusinessLogic.

                LogList log = SMPAG.Smartrix.Converters.EBUConverters.SMLogListTOEBULogList(r.Log);
                log.refreshAirdatetime();
                return log;
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message + "\n" + e.StackTrace);
                System.Environment.Exit(1);
                return null;
            }
      
#else      
            MessageBox.Show("Music Master not supported.");
            System.Environment.Exit(1);
            return null;
#endif
        }

        public static void errorAdd(String msg, String module="undefined"){
            if(Instance != null){
                errorListAdd(msg, module);
                if (module == "CONTENTSERVICE")
                {
                    if (Instance.contentFrame.srv != null && Instance.contentFrame.srv.channel != null)
                    {
                        CommunicationState state = Instance.contentFrame.srv.channel.State;
                        errorListAdd("Connection status: " + state, module);
                        if (state == CommunicationState.Faulted || state == CommunicationState.Closed)
                        {
                            errorListAdd("Retrying to connect to ContentService...", module);
                            Instance.contentFrame.connectToService();
                        }
                    }
                }
            }
        }

        private static void errorListAdd(String msg, String module="undefined")
        {
            if (Instance != null)
            {
                Instance.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    Instance.errorListBox.Items.Add("[" + module + "] " + DateTime.Now.ToLongTimeString() + ": " + msg);
                    Instance.error_label.Content = "[" + module + "] " + DateTime.Now.ToLongTimeString() + ": " + msg;
                    Instance.error_btn.Content = "Warning ("+Instance.errorListBox.Items.Count+")";
                }));
            }
        }

        void grid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView src = ((DataRowView)((DataGrid)e.Source).CurrentItem);
            if (src == null) return;
            if (src.Row["slot"] == null || src.Row["slot"] is DBNull)
            {   //SLICE

                MessageBox.Show("SLICE OPTION " + ((Slice)src.Row["slice"]).label);
            }
            else
            {
                SlotItem slot = (SlotItem)src.Row["slot"];
                MessageBox.Show("SLOT " + slot.uniqID);
            }
        }

        private void grid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
            
        }

        private void grid_DragEnter(object sender, DragEventArgs e)
        {
            MessageBox.Show("Drag enter " + e.Data);
        }

        private void Header_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void error_btn_Click(object sender, RoutedEventArgs e)
        {
            if (errorListBox.Visibility == System.Windows.Visibility.Visible)
                errorListBox.Visibility = System.Windows.Visibility.Hidden;
            else
                errorListBox.Visibility = System.Windows.Visibility.Visible;
            
        }

    }
}