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
//using System.Threading;
using System.IO;
using System.Timers;

namespace FileChangeChecker
{
    public class Peremennye
    {
        private static Peremennye _instance = null;
        public static Peremennye Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Peremennye();
                }
                return _instance;
            }
        }

        public int _maxCountPoint { get; set; }

        public List<StrPathToExange> _tableFilePath { get; set; }

    }

    public class StrPathToExange
    {
        public string _strNameExchange { get; set; }
        public string _strNameExchangeBack { get; set; }
        public string _strFilePath { get; set; }
        public string _strFilePathBack { get; set; }
    }

    public class TimeAndFileSize
    {
        public int Numer { get; set; }
        public DateTime Date { get; set; }
        public long sizeBite { get; set; }
        public double SizeMegabyte { get; set; }
        public int dateInt { get; set; }
        public string ChangeType { get; set; }
    }

    class FileChecker
    {
        private static FileChecker _instance = null;
        public static FileChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileChecker();
                }
                return _instance;
            }
        }
        
        private ArrayList _arrayFileSizeInfoList  = new ArrayList();
        public List<StrPathToExange> _filePathList { get; set; }
        private string _fileToWatch = string.Empty; //в данной переменной хранится путь к файлу в классе 


        private FileChecker() { }

        //метод выполняется при нажатии на кнопку "Подписаться"
        public void CreateFileWatcher()
        {
            
            for (int i = 0; i < _filePathList.Count; i++)
            {

            string _filePath = (string)_filePathList[i]._strFilePath;
            int numTable = i * 2;
            AddFileSizeInfoList(_filePath, numTable);

            _filePath = (string)_filePathList[i]._strFilePathBack;
            numTable = i * 2 + 1;
            AddFileSizeInfoList(_filePath, numTable);
            }

            Timer myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(MyMethod);
            myTimer.Interval = 20*1000;
            myTimer.Start();

        }

        private void AddFileSizeInfoList(string _filePath, int i)
        {
            FileInfo File = new FileInfo(_filePath);
            long fileSize = 0;
            string chandeType = "-";
            if (File.Exists)
            {
                fileSize = File.Length;
            }
            else
            {
                chandeType = "deleted";
            }

            TimeAndFileSize newStr = GetStrList(fileSize, chandeType);
            newStr.Numer = 1;

             List<TimeAndFileSize> _fileSizeInfoList = new List<TimeAndFileSize>();
            _fileSizeInfoList.Add(newStr);
            _arrayFileSizeInfoList.Add(_fileSizeInfoList);

            WpfApplication1.MainWindow.Instance.AddLineToChart(i, newStr);
        }

        private void MyMethod(object source, ElapsedEventArgs e)
        {

            for (int i = 0; i < _filePathList.Count; i++)
            {
                string _filePath = (string)_filePathList[i]._strFilePath;
                int numTable = i * 2;
                MyMetodStr(_filePath, numTable);

                _filePath = (string)_filePathList[i]._strFilePathBack;
                numTable = i * 2 + 1;
                MyMetodStr(_filePath, numTable);
                
            }
        }

        private void MyMetodStr(string _filePath, int numTable)
        {
            FileInfo File = new FileInfo(_filePath);

            long fileSize = 0;
            string chandeType = "-";
            if (File.Exists)
            { fileSize = File.Length; }
            else
            { chandeType = "deleted"; }

            List<TimeAndFileSize> _fileSizeInfoList = (List<TimeAndFileSize>)_arrayFileSizeInfoList[numTable];

            TimeAndFileSize lastStr = _fileSizeInfoList.Last();
            if (lastStr.ChangeType == "deleted" && fileSize != 0)
            { chandeType = "created"; }
            else if (fileSize != 0 && lastStr.sizeBite != 0 && fileSize != lastStr.sizeBite)
            { chandeType = "changed"; }

            TimeAndFileSize newStr = GetStrList(fileSize, chandeType);
            if (newStr.sizeBite - lastStr.sizeBite < -10 || newStr.sizeBite - lastStr.sizeBite > 10) //иногда программа по-разному округляет одно и то же значение, разница в 1-2 байта
            {
                if (_filePath=="\\\\10.202.0.14\\Obmen\\Тест обмена\\Message_УМ_РО.zip")
                {
                Console.WriteLine("Файл " + _filePath + "изменения " + DateTime.UtcNow + ". старый размер : " + lastStr.SizeMegabyte.ToString() + ". Новый размер: " + newStr.SizeMegabyte.ToString());
                }
                newStr.Numer = lastStr.Numer + 1;
                WpfApplication1.MainWindow.Instance.AddLineToChart(numTable, newStr);
                _fileSizeInfoList.Add(newStr);
                _arrayFileSizeInfoList[numTable] = _fileSizeInfoList;

                ProverkaErrorObmen();
            }
            //else
            //{
            //    Console.WriteLine("без изменений " + DateTime.UtcNow + ". старый размер : " + lastStr.sizeBite.ToString() + ". Новый размер: " + fileSize.ToString());
            //}           
        }

        public TimeAndFileSize GetStrList(long fileSize, string chandeType)
        {
            TimeAndFileSize Str = new TimeAndFileSize();

            Str.Date = DateTime.Now;
            Str.dateInt = _GetCurrentTimeStamp();
            Str.sizeBite = fileSize;
            double _doubleFileSize = (double)fileSize;
            _doubleFileSize = _doubleFileSize / 1024 / 1024;
            _doubleFileSize = Math.Round(_doubleFileSize, 3); // 2-количество знаков после запятой
            Str.SizeMegabyte =_doubleFileSize;
            Str.ChangeType = chandeType;

            return Str;
        }

        private void ProverkaErrorObmen()
        {
            for (int i = 0; i < _filePathList.Count; i++)
            {
                string _filePath = (string)_filePathList[i]._strFilePath;
                int numTable = i * 2;
                Boolean exchange1IsStopped = ExchangeIsStopped(_filePath, numTable);

                _filePath = (string)_filePathList[i]._strFilePathBack;
                numTable = i * 2 + 1;
                Boolean exchange2IsStopped = ExchangeIsStopped(_filePath, numTable);

                WpfApplication1.MainWindow.Instance.InstallationOfColorOfTheChart(numTable, exchange1IsStopped | exchange2IsStopped);
            }
        }

        private Boolean ExchangeIsStopped(string nameTable, int numTable)
        {
            List<TimeAndFileSize> _fileSizeInfoList = (List<TimeAndFileSize>)_arrayFileSizeInfoList[numTable];
            //проверим, что размер файла увеличивается. его размер больше 20 мегабайт и 3 последних изменения только росли
            int countList = _fileSizeInfoList.Count() - 1;
            if (countList <= 2)
            {
                return false;
            }

            long prevSize = 0;
            for (int j = countList - 3; j <= countList; j++)
            {
                TimeAndFileSize tekStr = _fileSizeInfoList[j];
                if (prevSize >= tekStr.sizeBite)
                {
                    return false;
                }
                prevSize = tekStr.sizeBite;

            }
            //Form1.Instance.ShowBalloonTip(3000, "Внимание!", string.Format("Файл {0} растет подряд 3 раза!", _filePathList[i]), System.Windows.Forms.ToolTipIcon.Warning);

            Console.WriteLine(string.Format("Файл {0} растет подряд 3 раза!", nameTable));
            return true;
        }

        //данная функция возвращает число, содержащее количество секунд ка кразницу между текущей датой и 01.01.1970
        private int _GetCurrentTimeStamp()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
    //Form1.Instance.ShowBalloonTip(3000, "Внимание!", string.Format("Файл {0} был изменен!", _fileToWatch), System.Windows.Forms.ToolTipIcon.Warning);


}
