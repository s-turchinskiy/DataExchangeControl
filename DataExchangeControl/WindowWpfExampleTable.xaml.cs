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
using FileChangeChecker;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class WindowWpfExampleTable : Window
    {
        public static WindowWpfExampleTable Instance { get; private set; }

        private string mFilePath; //Переменная, хранящая путь к файлу проверки.

        public WindowWpfExampleTable()
        {
            Instance = this;
            InitializeComponent();
            DataContext = wave;
        }

        HarmonicSeries wave = new HarmonicSeries();

        private Label FindElementLabel(UIElementCollection ChildrenCollection, string _elementName)
        {
            var e = ChildrenCollection.GetEnumerator();
            while (e.MoveNext())
            {

                UIElement _elementChildren = (UIElement)e.Current;
                if (_elementChildren.GetType().Name != "Label")
                {
                    continue;
                }
                System.Windows.Controls.Label _element = (System.Windows.Controls.Label)_elementChildren;
                if (_element.Name == _elementName)
                {
                    return _element;
                }
            }
            return null;
        }

        private ListView FindElementListView(UIElementCollection ChildrenCollection, string _elementName) 
        {
            var e = ChildrenCollection.GetEnumerator();
            while (e.MoveNext())
            {
                
                UIElement _elementChildren = (UIElement)e.Current;
                if (_elementChildren.GetType().Name != "ListView")
                {
                    continue;
                }
                System.Windows.Controls.ListView _element = (System.Windows.Controls.ListView)_elementChildren;
                if (_element.Name == _elementName)
                {
                    return _element;
                }
            }
            return null;
        }

        private DockPanel FindElementDockPanel(UIElementCollection ChildrenCollection, string _elementName)
        {
            var e = ChildrenCollection.GetEnumerator();
            while (e.MoveNext())
            {

                UIElement _elementChildren = (UIElement)e.Current;
                if (_elementChildren.GetType().Name != "DockPanel")
                {
                    continue;
                }
                System.Windows.Controls.DockPanel _element = (System.Windows.Controls.DockPanel)_elementChildren;
                if (_element.Name == _elementName)
                {
                    return _element;
                }
            }
            return null;
        }

        public void AddRowToTable(int numTable, TimeAndFileSize newStr)
        {
            
            String numTableString = numTable.ToString();
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                string _elementName = "DockPanel" + numTableString;
                DockPanel _elementDockPanel = FindElementDockPanel(this.GridTable.Children, _elementName);
                _elementName = "TableExchange" + numTableString;
                ListView _elementTable = FindElementListView(_elementDockPanel.Children,_elementName);
                ArrayList _tableForm = (ArrayList)_elementTable.Resources["teams"];
                ((ArrayList)_tableForm).Add(newStr);
               _elementTable.Items.Refresh();

                if (numTable==0)
                {
                    AddPointInPoints(numTableString, newStr, _tableForm);
                }
            }));
            
        }

        private void AddPointInPoints(string numTableString, TimeAndFileSize newStr,ArrayList _tableForm)
        {

            //Polyline3.Points.Clear();
            double maxY = newStr.SizeMegabyte;

            foreach (var item in _tableForm)
            {

                double tekY = ((FileChangeChecker.TimeAndFileSize)(item)).SizeMegabyte;

                if (tekY > maxY)
                {
                    maxY = tekY;
                }
            }

            int newCountPoints = _tableForm.Count;
            double widthField = ((System.Windows.FrameworkElement)(Canvas3)).Width;
            double stepX;
            if (newCountPoints==1)
            {
                stepX = widthField; 
            }
            else
            {
               stepX = widthField / (newCountPoints-1);
            }

            double heightField = ((System.Windows.FrameworkElement)(Canvas3)).Height;
            double koeffizientY;
            if (maxY == 0)
            { koeffizientY = 1; }
            else
            {
                koeffizientY = heightField / maxY;
            }

           //gg HarmonicSeries wave = new HarmonicSeries();
            WpfApplication1.MyVisual class_Polyline = new MyVisual();
            //class_Polyline.VisualXSnappingGuidelines.Add(stepX);
            //class_Polyline.Test();
            Polyline polyline1 = new Polyline();
            //polyline1.v
            
            Polyline3.Points.Clear();
            for (int i = 0; i <= newCountPoints-1; ++i)
            {

                double x = i * stepX;
                double y = -1 * koeffizientY * ((FileChangeChecker.TimeAndFileSize)(_tableForm[i])).SizeMegabyte;

                //Polyline3.GuidelineSet = true; 
                Polyline3.Points.Add(new Point(x, y));

            }

        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void Tes1_Click(object sender, RoutedEventArgs e)
        {

            Button nw = new Button();
            nw.Name = "test2";
            nw.Margin = new Thickness(0, 0, 50, 0);
            nw.Content = "кнопка, созданная программно";
            PanelLeft.Children.Add(nw);

            GridTable.ColumnDefinitions.Clear();
        }

        private void AddFileWindows_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".zip";
            dlg.Filter = "архивы|*.zip|xml-файлы|*.xml";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog(); // Открываем окно

            // Process open file dialog box results
            if (result == true)
            {
                mFilePath = dlg.FileName; //присваиваем переменной путь к файлу
                int _lastChar = mFilePath.LastIndexOf("\\");
                string[] _ArrayPath = mFilePath.Split(new Char[] { '\\' });
                int _lastStr = _ArrayPath.Count();
                String _strNameExchange = _ArrayPath[_lastStr - 1];
                _strNameExchange = _strNameExchange.Substring(8, _strNameExchange.Length - 4 - 8);
                int _separator = _strNameExchange.LastIndexOf("_");
                String _strNameExchangeBack = _strNameExchange.Substring(_separator + 1, _strNameExchange.Length - _separator - 1) + "_" + _strNameExchange.Substring(0, _separator);
                String _newFilePath = mFilePath.Substring(0, _lastChar + 9) + _strNameExchangeBack + mFilePath.Substring(mFilePath.Length - 4, 4);

                int CountStr = ((ArrayList)ListPath.Resources["teams"]).Count;
                    Dispatcher.BeginInvoke(new ThreadStart(delegate
                    {
                        for (int i = 0; i <= 1; i++)
                        {
                            int num = CountStr + i;
                        string _elementName = "DockPanel" + num.ToString();
                        DockPanel _elementDockPanel = FindElementDockPanel(this.GridTable.Children, _elementName);
                        _elementName = "LabelPathFile" + num.ToString();
                        Label _elementLabel = FindElementLabel(_elementDockPanel.Children, _elementName);
                        if (i == 0)
                        {
                            _strNameExchange = _strNameExchange.Replace("_", "->");
                            _elementLabel.Content = "Обмен: " + _strNameExchange;
                        }
                        else
                        {
                            _strNameExchangeBack = _strNameExchangeBack.Replace("_", "->");
                            _elementLabel.Content = "Обмен: " + _strNameExchangeBack;
                        }
                        }
                    }));

                ((ArrayList)ListPath.Resources["teams"]).Add(mFilePath);
                ((ArrayList)ListPath.Resources["teams"]).Add(_newFilePath);
                FileChecker.Instance._filePathList = ((List<StrPathToExange>)ListPath.Resources["teams"]);

                FileChecker.Instance.CreateFileWatcher();

                Dispatcher.BeginInvoke(new ThreadStart(delegate { ListPath.Items.Refresh(); }));
            }

            Console.WriteLine(result); // <-- For debugging use.
        }
    }

    public class WindowWpfExampleTableMyVisual : Visual
    {
        public WindowWpfExampleTableMyVisual()
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
