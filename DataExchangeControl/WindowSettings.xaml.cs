using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Shapes;

//using Microsoft.Windows.Controls;
using FileChangeChecker;
using System.Threading;
using Xceed.Wpf.Toolkit;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        private Peremennye peremennye = new Peremennye();
        public WindowSettings()
        {
            InitializeComponent();
        }


        private void ButtonMaxCountPoint_Spin(object sender, SpinEventArgs e)
        {
            string currentSpinValue = (string)ButtonMaxCountPoint.Content;

            int currentValue = String.IsNullOrEmpty(currentSpinValue) ? 0 : Convert.ToInt32(currentSpinValue);
            if (e.Direction == SpinDirection.Increase)
            {
                currentValue++;
                peremennye._maxCountPoint = currentValue;
                ButtonMaxCountPoint.Content = currentValue.ToString();
            }
            else
            {
                currentValue--;
                if (currentValue<1)
                {
                    currentValue = 1;
                }
                peremennye._maxCountPoint = currentValue; 
                ButtonMaxCountPoint.Content = currentValue.ToString();
            }

        }
     
      private void RadRibbonButtonOK_Click(object sender, RoutedEventArgs e)
      {

          Peremennye.Instance._maxCountPoint = peremennye._maxCountPoint;
          Peremennye.Instance._tableFilePath = peremennye._tableFilePath;
          DialogResult = true;
          this.Close();
          
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
          peremennye._maxCountPoint = Peremennye.Instance._maxCountPoint;
          peremennye._tableFilePath = Peremennye.Instance._tableFilePath;

          ButtonMaxCountPoint.Content = peremennye._maxCountPoint.ToString();

          foreach(StrPathToExange _element in Peremennye.Instance._tableFilePath)
          {
              AddRowToTablePath(_element);
          }
      }

      public void AddRowToTablePath(StrPathToExange _newStr)
      {
          Dispatcher.BeginInvoke(new ThreadStart(delegate
          {
              ArrayList _tableForm = (ArrayList)TableFilePath.Resources["teams"];
              ((ArrayList)_tableForm).Add(_newStr);
              TableFilePath.Items.Refresh();
          }));

      }

      private void ButtonNewExcange_Click(object sender, RoutedEventArgs e)
      {
          Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
          dlg.Title = "Выберите ";
          dlg.DefaultExt = ".zip";
          dlg.Filter = "архивы|*.zip|xml-файлы|*.xml";

          // Show open file dialog box
          Nullable<bool> result = dlg.ShowDialog(); // Открываем окно

          if (result != true)
          {
              return;
          }

          string _strFilePath = dlg.FileName; //присваиваем переменной путь к файлу
          int _lastChar = _strFilePath.LastIndexOf("\\");
          string[] _ArrayPath = _strFilePath.Split(new Char[] { '\\' });
          int _lastStr = _ArrayPath.Count();
          String _strNameExchange = _ArrayPath[_lastStr - 1];
          _strNameExchange = _strNameExchange.Substring(8, _strNameExchange.Length - 4 - 8);
          int _separator = _strNameExchange.LastIndexOf("_");
          String _strNameExchangeBack = _strNameExchange.Substring(_separator + 1, _strNameExchange.Length - _separator - 1) + "_" + _strNameExchange.Substring(0, _separator);
          String _strFilePathBack = _strFilePath.Substring(0, _lastChar + 9) + _strNameExchangeBack + _strFilePath.Substring(_strFilePath.Length - 4, 4);

          _strNameExchange = _strNameExchange.Replace("_", "->");
          _strNameExchangeBack = _strNameExchangeBack.Replace("_", "->");

          ArrayList _tableForm = (ArrayList)TableFilePath.Resources["teams"];

          StrPathToExange _newStr = new StrPathToExange();
          _newStr._strNameExchange = _strNameExchange;
          _newStr._strNameExchangeBack = _strNameExchangeBack;
          _newStr._strFilePath = _strFilePath;
          _newStr._strFilePathBack = _strFilePathBack;

          AddRowToTablePath(_newStr);
          peremennye._tableFilePath.Add(_newStr);
      }

    }
}
