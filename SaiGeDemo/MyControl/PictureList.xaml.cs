using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SaiGeDemo.MyControl
{
    /// <summary>
    /// 图片列表的数据模型
    /// </summary>
    public class MyBitmapImage
    {
        public BitmapImage BitmapImage { get; set; }

        public string Label { get; set; }

        public MyBitmapImage(Uri uri, string label)
        {
            BitmapImage = new BitmapImage(uri);
            Label = label;
        }
    }

    /// <summary>
    /// PictureList.xaml 的交互逻辑
    /// </summary>
    public partial class PictureList : UserControl
    {
        public delegate void SelectionChanged(object sender, SelectionChangedEventArgs e);

        /// <summary>
        /// 图片列表选项变更委托
        /// </summary>
        public SelectionChanged selectionChanged = (object sender, SelectionChangedEventArgs e) => { };

        public PictureList()
        {
            InitializeComponent();

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectionChanged.Invoke(sender,e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                listBox.ItemsSource = LoadImages(dialog.FileName);
        }

        /// <summary>
        /// 读取文件夹内的图片数据
        /// </summary>
        /// <param name="path">文件夹地址</param>
        /// <returns>图片列表数据</returns>
        public List<MyBitmapImage> LoadImages(string path)
        {
            List<MyBitmapImage> images = new List<MyBitmapImage>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (var item in directoryInfo.GetFiles().Where(x => x.Name.ToLower().EndsWith("png") || x.Name.ToLower().EndsWith("jpg")))
            {
                Uri uri = new Uri(item.FullName);
                images.Add(new MyBitmapImage(uri, item.Name));
            }
            return images;
        }
    }
}
