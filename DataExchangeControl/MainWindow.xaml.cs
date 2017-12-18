using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;

using System.Collections.ObjectModel;
using System.ComponentModel;

using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
//using Microsoft.Windows.Controls;
using FileChangeChecker;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
//using System.Windows.Forms;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon; //трей

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            DataContext = wave;
            //+трей
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            //MyNotifyIcon.Icon = new Icon("/Resources/Icon1.ico");
            MyNotifyIcon.Icon = new Icon(Directory.GetCurrentDirectory() + @"\Resources\Icon1.ico");
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            MyNotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseClick);

            System.Windows.Forms.ContextMenuStrip MyNotifyIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            MyNotifyIconContextMenuStrip.Items.Add("Развернуть",null,EventHandlerContextMenu);
            MyNotifyIconContextMenuStrip.Items.Add("Закрыть", null, EventHandlerContextMenuClosing);
            MyNotifyIcon.ContextMenuStrip = MyNotifyIconContextMenuStrip;
            //MyNotifyIconContextMenuStrip.MouseClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIconContextMenuStrip_MouseClick);
           //MyNotifyIconContextMenuStrip.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIconContextMenuStrip_MouseClick);
            //-
        }

        //трей

        public void EventHandlerContextMenu(Object sender, EventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }

        public void EventHandlerContextMenuClosing(Object sender, EventArgs e)
        {
            this.Close();
        }

        void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }

        //трей
        void MyNotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            { 
                this.WindowState = System.Windows.WindowState.Normal; 
            }
        }

        //трей
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                MyNotifyIcon.BalloonTipText = "Minimized the app ";
                MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == System.Windows.WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }
        //-

        HarmonicSeries wave = new HarmonicSeries();

        private RadChart FindElementRadChart(UIElementCollection ChildrenCollection, string _elementName)
        {
            var e = ChildrenCollection.GetEnumerator();
            while (e.MoveNext())
            {

                UIElement _elementChildren = (UIElement)e.Current;
                if (_elementChildren.GetType().Name != "RadChart")
                {
                    continue;
                }
                Telerik.Windows.Controls.RadChart _element = (Telerik.Windows.Controls.RadChart)_elementChildren;
                if (_element.Name == _elementName)
                {
                    return _element;
                }
            }
            return null;
        }

        public void InstallationOfColorOfTheChart(int numTable,Boolean exchangeIsStopped)
        {

            String numTableString = numTable.ToString();
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {

                int numRadChart = numTable / 2;
                string _elementName = "RadChart" + numRadChart.ToString();
                RadChart _elementRadChart = FindElementRadChart(this.GridTable.Children, _elementName);


                System.Windows.Media.Brush br;
                if (exchangeIsStopped)
                {
                    br = new SolidColorBrush(System.Windows.Media.Color.FromArgb(40, 255, 0, 0));
                }
                else
                {
                    br = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 255, 255, 255));
                }

                if (!(_elementRadChart.Background is System.Windows.Media.SolidColorBrush))
                {
                    _elementRadChart.Background = br;
                }

                if (((System.Windows.Media.SolidColorBrush)(_elementRadChart.Background)).Color != 
                    ((System.Windows.Media.SolidColorBrush)(br)).Color)
                {
                    _elementRadChart.Background = br;
                    if (this.WindowState == System.Windows.WindowState.Minimized)
                    {
                        this.WindowState = System.Windows.WindowState.Normal;
                        MyNotifyIcon.Visible = false;
                        this.ShowInTaskbar = true;
                    }
                }
 
            }));

        }
        public void AddLineToChart(int numTable, TimeAndFileSize newStr)
        {

            String numTableString = numTable.ToString();
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {


                //график 
                string _mHour = newStr.Date.Hour.ToString();
                if (_mHour.Length==1)
                {
                    _mHour = "0" + _mHour;
                }

                string _mMinute = newStr.Date.Minute.ToString();
                if (_mMinute.Length == 1)
                {
                    _mMinute = "0" + _mMinute;
                }

                string _xCategory = _mHour + ":" + _mMinute;

                int numChart = 0;
                int _otherNumTable = 0;
                if ((numTable % 2) == 0)
                {
                    numChart = 0;
                    _otherNumTable = 1;
                }
                else
                {
                    numChart = 1;
                    _otherNumTable = 0;
                }

                int numRadChart = numTable / 2;

                string _elementName = "RadChart" + numRadChart.ToString();
                RadChart _elementRadChart = FindElementRadChart(this.GridTable.Children, _elementName);

                DataSeries lineSeries = _elementRadChart.DefaultView.ChartArea.DataSeries[numChart];
                AddPointInChart(lineSeries, _xCategory, newStr.SizeMegabyte, true);

                //другая линия

                lineSeries = _elementRadChart.DefaultView.ChartArea.DataSeries[_otherNumTable];
                AddPointInChart(lineSeries, _xCategory, 0, false);
                //-
            }));

        }

        private void AddPointInChart(DataSeries lineSeries, string _xCategory, double _newYValue, bool _assignXCategory)
        {
            int _countPoint = lineSeries.Count();
            if (_countPoint != 0)
            {
                DataPoint _lastPoint = lineSeries[_countPoint - 1];
                if (_lastPoint.XCategory != _xCategory)
                {
                    if (!_assignXCategory)
                    {
                        _newYValue = _lastPoint.YValue;
                    }         
                    lineSeries.Add(new DataPoint() { YValue = _newYValue, XCategory = _xCategory });
                        DeletePointInChart(lineSeries);            
                }
                else
                {
                    if (_assignXCategory)
                    {
                        _lastPoint.YValue = _newYValue;
                        _lastPoint.XCategory = _xCategory;
                    }
                }
            }
            else
            {
                if (_assignXCategory)
                {
                    lineSeries.Add(new DataPoint() { YValue = _newYValue, XCategory = _xCategory });
                    DeletePointInChart(lineSeries);
                }
            }
        }

        private void DeletePointInChart(DataSeries lineSeries)
        {
            int maxCountPoint = Peremennye.Instance._maxCountPoint;

            if (lineSeries.Count()>maxCountPoint)
            {
                lineSeries.RemoveAt(0);
                DeletePointInChart(lineSeries);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void ButtonOfSettings_Click(object sender, RoutedEventArgs e)
        {

            Nullable<bool> result = new WindowSettings().ShowDialog();

            if (result != true)
            {
                return;
            }

            //Максимальное количество точек
            //ButtonMaxCountPoint.Content = Peremennye.Instance._maxCountPoint.ToString();

            foreach (UIElement _elementChildren in this.GridTable.Children)
            {
                if (_elementChildren.GetType().Name != "RadChart")
                {
                    continue;
                }
                RadChart _elementRadChart = (RadChart)_elementChildren;

                foreach (DataSeries lineSeries in _elementRadChart.DefaultView.ChartArea.DataSeries)
                {
                    DeletePointInChart(lineSeries);
                }
            }
            //-

            //Добавление-удаление графиков
            int _beginI = FileChecker.Instance._filePathList.Count;
            int _endI = Peremennye.Instance._tableFilePath.Count;
            for (int i = _beginI; i < _endI; i++)
            {
                int numRadChart = i;
                StrPathToExange _newStr = Peremennye.Instance._tableFilePath[i];

                AddChartInGrid(numRadChart, _newStr._strNameExchange, _newStr._strNameExchangeBack, _newStr._strFilePath, _newStr._strFilePathBack);
            }
        }

        private void AddFileWindows_Click(object sender, RoutedEventArgs e)
        {
  
        }

        private void AddChartInGrid(int numRadChart,string _strNameExchange,string _strNameExchangeBack
            , string _strFilePath, string _strFilePathBack)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                string _elementName = "RadChart" + numRadChart.ToString();
                RadChart _elementRadChart = FindElementRadChart(this.GridTable.Children, _elementName);
                Telerik.Windows.Controls.RadChart telerikChart = _elementRadChart;

                //добавление в Grid новой строки
                System.Windows.Controls.RowDefinition newRow = new System.Windows.Controls.RowDefinition();
                GridLengthConverter myGridLengthConverter = new GridLengthConverter();
                newRow.Height = (GridLength)myGridLengthConverter.ConvertFromString("*");
                GridTable.RowDefinitions.Add(newRow);
                _elementRadChart.Visibility = System.Windows.Visibility.Visible;
                //-

                telerikChart.DefaultView.ChartTitle.HorizontalAlignment = HorizontalAlignment.Center;    //Chart Legend
                telerikChart.DefaultView.ChartLegend.UseAutoGeneratedItems = true;    //Line Chart
                telerikChart.DefaultView.ChartTitle.Content = _strNameExchange; // "Обмен " + _strNameExchange; //+" и " + _strNameExchangeBack;

                DataSeries lineSeries = new DataSeries();
                lineSeries.LegendLabel = _strNameExchange; // "Обмен " + _strNameExchange;
                lineSeries.Definition = new LineSeriesDefinition();
                telerikChart.DefaultView.ChartArea.DataSeries.Add(lineSeries);    //Bar Chart

                DataSeries barSeries = new DataSeries();
                barSeries.LegendLabel = _strNameExchangeBack; // "Обмен " + _strNameExchangeBack;
                barSeries.Definition = new LineSeriesDefinition();
                telerikChart.DefaultView.ChartArea.DataSeries.Add(barSeries);

            }));

            StrPathToExange _newStr = new StrPathToExange();
            _newStr._strNameExchange = _strNameExchange;
            _newStr._strNameExchangeBack = _strNameExchangeBack;
            _newStr._strFilePath = _strFilePath;
            _newStr._strFilePathBack = _strFilePathBack;

            if (FileChecker.Instance._filePathList == null)
            {
                FileChecker.Instance._filePathList = new List<StrPathToExange>();
            }
            FileChecker.Instance._filePathList.Add(_newStr);
            FileChecker.Instance.CreateFileWatcher();

            //CreateChart_DefaultView();
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            RadChart0.Visibility = System.Windows.Visibility.Collapsed;
            RadChart1.Visibility = System.Windows.Visibility.Collapsed;
            RadChart2.Visibility = System.Windows.Visibility.Collapsed;
            RadChart3.Visibility = System.Windows.Visibility.Collapsed;
            RadChart4.Visibility = System.Windows.Visibility.Collapsed;
            RadChart5.Visibility = System.Windows.Visibility.Collapsed;
            RadChart6.Visibility = System.Windows.Visibility.Collapsed;
            RadChart7.Visibility = System.Windows.Visibility.Collapsed;

            GridTable.RowDefinitions.RemoveAt(7);
            GridTable.RowDefinitions.RemoveAt(6);
            GridTable.RowDefinitions.RemoveAt(5);
            GridTable.RowDefinitions.RemoveAt(4);
            GridTable.RowDefinitions.RemoveAt(3);
            GridTable.RowDefinitions.RemoveAt(2);
            GridTable.RowDefinitions.RemoveAt(1);
            GridTable.RowDefinitions.RemoveAt(0);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            
        }

        public class iniSettings
        {
            public int X_pos;
            public int Y_pos;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SerializeObjectPeremennye(Directory.GetCurrentDirectory() + "\\Settings.xml");
        }

        private void SerializeObjectPeremennye(string filename)
        {
            XmlSerializer serializer =
            new XmlSerializer(typeof(Peremennye));

            OrderedItem i = new OrderedItem();

            // Set the public property values.
            i.ItemName = "Widget";
            i.Description = "Regular Widget";
            i.Quantity = 10;
            i.UnitPrice = (decimal)2.30;

            TextWriter writer = new StreamWriter(filename);

            serializer.Serialize(writer, Peremennye.Instance);
            writer.Close();
        }

        public void ReadDocument(string filename)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(Peremennye));
            FileStream fs = new FileStream(filename, FileMode.Open);
            Peremennye peremennye;
            peremennye = (Peremennye)serializer.Deserialize(fs);
            Peremennye.Instance._maxCountPoint = peremennye._maxCountPoint;
            //ButtonMaxCountPoint.Content = Peremennye.Instance._maxCountPoint.ToString();
            Peremennye.Instance._tableFilePath = peremennye._tableFilePath;
            fs.Close();

            int numRadChart = 0;
            if (Peremennye.Instance._tableFilePath == null)
            {
                Peremennye.Instance._tableFilePath = new List<StrPathToExange>();
            }
                
            foreach (StrPathToExange _strTable in peremennye._tableFilePath)
            {
                AddChartInGrid(numRadChart, _strTable._strNameExchange, _strTable._strNameExchangeBack,
                    _strTable._strFilePath, _strTable._strFilePathBack);
                numRadChart++;
            }

        }

        // This is the class that will be serialized.
        public class OrderedItem
        {
            [XmlElement(ElementName = "First_Name")]
            public string ItemName;
            [XmlElement(ElementName = "Description")]
            public string Description;
            [XmlElement(ElementName = "UnitPrice")]
            public decimal UnitPrice;
            [XmlElement(ElementName = "Quantity")]
            public int Quantity;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadDocument(Directory.GetCurrentDirectory() + "\\Settings.xml");

            //ButtonMaxCountPoint.Content = Peremennye.Instance._maxCountPoint.ToString();
            if (Peremennye.Instance._maxCountPoint == 0)
            {
                Peremennye.Instance._maxCountPoint = 20;
                //ButtonMaxCountPoint.Content = "20";
            }
        }
       
    }
    public class MyVisual : Visual
    {
        public MyVisual()
        {
            VisualXSnappingGuidelines = new DoubleCollection();
            double mm = 50;
            VisualXSnappingGuidelines.Add(mm);
            mm = -50;
            VisualXSnappingGuidelines.Add(mm);
        }

        //public DoubleCollection VisualXSnappingGuidelines { get; set; }

    }
}
        //private void Telerik.Windows.Controls.RadChart CreateChart_DefaultView()
        //{
        //    //Telerik.Windows.Controls.RadChart telerikChart = new Telerik.Windows.Controls.RadChart();    //Chart Title
        //    Telerik.Windows.Controls.RadChart telerikChart = RadChart0;
        //    telerikChart.DefaultView.ChartTitle.HorizontalAlignment = HorizontalAlignment.Center;    //Chart Legend
        //    telerikChart.DefaultView.ChartLegend.UseAutoGeneratedItems = true;    //Line Chart
        //    telerikChart.DefaultView.ChartTitle.Content = "График 2009";
           
        //    DataSeries lineSeries = new DataSeries();
        //    lineSeries.LegendLabel = "Обмен1";
        //    lineSeries.Definition = new LineSeriesDefinition(); 
        //    lineSeries.Add(new DataPoint() { YValue = 154, XCategory = "Jan" });
        //    lineSeries.Add(new DataPoint() { YValue = 138, XCategory = "Feb" });
        //    lineSeries.Add(new DataPoint() { YValue = 143, XCategory = "Mar" });
        //    lineSeries.Add(new DataPoint() { YValue = 120, XCategory = "Apr" });
        //    lineSeries.Add(new DataPoint() { YValue = 135, XCategory = "May" });
        //    lineSeries.Add(new DataPoint() { YValue = 125, XCategory = "Jun" });
        //    lineSeries.Add(new DataPoint() { YValue = 179, XCategory = "Jul" });
        //    lineSeries.Add(new DataPoint() { YValue = 170, XCategory = "Aug" });
        //    lineSeries.Add(new DataPoint() { YValue = 198, XCategory = "Sep" });
        //    lineSeries.Add(new DataPoint() { YValue = 187, XCategory = "Oct" });
        //    lineSeries.Add(new DataPoint() { YValue = 193, XCategory = "Nov" });
        //    lineSeries.Add(new DataPoint() { YValue = 176, XCategory = "Dec" }); 
        //    telerikChart.DefaultView.ChartArea.DataSeries.Add(lineSeries);    //Bar Chart

        //    DataSeries barSeries = new DataSeries();
        //    barSeries.LegendLabel = "Обмен2";
        //    barSeries.Definition = new LineSeriesDefinition(); //new BarSeriesDefinition(); 
        //    barSeries.Add(new DataPoint() { YValue = 45, XCategory = "Jan" });
        //    barSeries.Add(new DataPoint() { YValue = 48, XCategory = "Feb" });
        //    barSeries.Add(new DataPoint() { YValue = 53, XCategory = "Mar" });
        //    barSeries.Add(new DataPoint() { YValue = 41, XCategory = "Apr" });
        //    barSeries.Add(new DataPoint() { YValue = 32, XCategory = "May" });
        //    barSeries.Add(new DataPoint() { YValue = 28, XCategory = "Jun" });
        //    barSeries.Add(new DataPoint() { YValue = 63, XCategory = "Jul" });
        //    barSeries.Add(new DataPoint() { YValue = 74, XCategory = "Aug" });
        //}    

