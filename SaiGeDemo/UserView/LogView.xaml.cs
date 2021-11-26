using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SaiGeDemo.UserView
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : UserControl
    {
        public ObservableCollection<LogModel> LogModels { get; set; }
        public LogView()
        {
            InitializeComponent();

            LogModels = new ObservableCollection<LogModel>();

            ListLog.ItemsSource = LogModels;

           
        }

        public void AddLog(string text)
        {
            ListLog.Dispatcher.BeginInvoke(
                (Action)delegate (){
                    LogModels.Add(new LogModel(text));
                    ListLog.ScrollIntoView(ListLog.Items[ListLog.Items.Count - 1]);
                });
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            
            dialog.Filter = @"(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(dialog.FileName))
                {
                    foreach (var log in LogModels)
                    {
                        sw.WriteLine(log.Text);
                    }
                    
                }
                AddLog("导出日志成功！");
            }
            else
                return;
        }
    }
}
