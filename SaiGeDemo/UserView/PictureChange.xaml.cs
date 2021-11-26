using MahApps.Metro.Controls;
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
using OpenCvSharp;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using SaiGeDemo.MyControl;

namespace SaiGeDemo.UserView
{


   
   

    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class PictureChange : UserControl
    {

        private readonly static string[] methodNamesDefault = new string[] { "颜色空间转换", "方框滤波", "均值滤波", "高斯滤波", "中值滤波", "双边滤波","圆形", "找轮廓" };

        private readonly static string[] methodNamesA= new string[] { "颜色空间转换", "方框滤波", "均值滤波", "高斯滤波", "中值滤波", "双边滤波" };

        private readonly static string[] methodNamesB = new string[] { "圆形", "找轮廓" };

        private bool MethodRun { get; set; } = false;

        public delegate void ShowLog(string LogText);

        public ShowLog ShowLogHandle=(string LogText)=> { };

        public delegate MethodModel CV2_RUN(string uriString);

        public CV2_RUN CV2_RUN_Handle;

        public PictureChange()
        {
            InitializeComponent();

            picturelist.selectionChanged += listBox_SelectionChanged;

            listBox_method.ItemsSource = LoadMethod();

        }
       

        /// <summary>
        /// 读取变换方法
        /// </summary>
        /// <returns>返回变换模型数据</returns>
        public List<MethodModel> LoadMethod()
        {
            string[] methodNames = methodNamesDefault;
            List<MethodModel> methodModels = new List<MethodModel>();
            foreach (var itemName in methodNames)
            {
                MethodModel model = new MethodModel(itemName);
                methodModels.Add(model);
            }
            return methodModels;
        }
      
        /// <summary>
        /// 图片列表选项变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            MyBitmapImage SelectedItem = (sender as ListBox).SelectedItem as MyBitmapImage;
            picture.DataContext = SelectedItem;
            picture_CV2.DataContext = null;
            
            listBox_method.Invoke(()=> { listBox_method.ItemsSource = LoadMethod(); });
            
            ShowLogHandle($"图片变更为 {SelectedItem.Label}");


        }

        /// <summary>
        /// 变换方法列表选项变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //判断转换是否还在进行
            if (MethodRun)
            {
                ShowLogHandle($"有在进行的转换！");
                return;
            }

            //获取模型
            MethodModel model = (sender as ListBox).SelectedItem as MethodModel;

            if (model == null)
            {
                ShowLogHandle($"转换模型为NULL!");
                return;
            }
            
           

            string uriString = (picturelist.listBox.SelectedItem as MyBitmapImage).BitmapImage.UriSource.LocalPath;

            ShowLogHandle($"图片 {model.MethodName} 开始执行！");

            MethodRun = true;
            CV2_RUN_Handle = model.CV2_Run;
            //执行图像变更

            CV2_RUN_Handle.BeginInvoke(uriString, ReturnAsync,CV2_RUN_Handle);

           
        }


        public void ReturnAsync(IAsyncResult ar)
        {
            //获得调用委托实例的引用
            CV2_RUN del = (CV2_RUN)ar.AsyncState;
            MethodModel result = del.EndInvoke(ar);

            //输出执行结果 
            picture_CV2.Dispatcher.BeginInvoke((Action)delegate () { picture_CV2.DataContext = result; }) ;

            ShowLogHandle($"图片 {result.MethodName} 执行成功！");

            MethodRun = false;
        }

        /// <summary>
        /// 保存图片按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MethodModel model = picture_CV2.DataContext as MethodModel;
           
           BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(model.BitmapImage));

            var dialog = new SaveFileDialog();
     
            dialog.Filter = @"(*.png)|*.png|(*.jpg)|*.jpg|所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                using (var fileStream = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                ShowLogHandle.BeginInvoke($"图片保存成功！", null, null);
            }
            else
                return;

        }
    }
}
