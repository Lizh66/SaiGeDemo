using AForge.Controls;
using AForge.Video.DirectShow;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Threading;

namespace SaiGeDemo.UserView
{
    /// <summary>
    /// VideoPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPlayer : UserControl
    {


        private string MethodName = "Defalut";

        public delegate void ShowLog(string LogText);

        public ShowLog ShowLogHandle = (string LogText) => { };

        public VideoPlayer()
        {

            InitializeComponent();

            

        }

        private void VideoPlayerStop()
        {
            Player.NewFrame -= VideoPlayerNewFrame;
            Player.SignalToStop();
            Player.WaitForStop();
            
        }

        private void VideoPlayerNewFrame(object sender, ref Bitmap image)
        {
            switch (MethodName)
            {
                case "颜色空间转换":
                    image=CvtColor(image);
                    break;
                
                case "圆形":
                    
                    break;
                case "找轮廓":
                    image = FindContours(image);
                    break;
                default:
                    break;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            VideoPlayerStop();

            var dialog = new OpenFileDialog();

            dialog.Filter = @"(*.gif)|*.gif|所有文件(*.*)|*.*";
            if (dialog.ShowDialog()==true)
            {
                FileVideoSource source = new FileVideoSource(dialog.FileName);

                // 设置VideoSourcePlayer的VideoSource     
                Player.VideoSource = source;

                Player.NewFrame += VideoPlayerNewFrame;

                Player.Start();

                ShowLogHandle("导入成功，开始播放!");
            }
            else
                return;

            

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            VideoPlayerStop();
            // 获取视频输入设备列表
            FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (devices.Count > 0)
            {
                VideoCaptureDevice source = new VideoCaptureDevice(devices[0].MonikerString);

                Player.VideoSource = source;

                Player.NewFrame += VideoPlayerNewFrame;

                Player.Start();
            }
            else
                ShowLogHandle("未找到视频输入设备");

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            VideoPlayerStop();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MethodName = (sender as Button).Content.ToString();
            ShowLogHandle($"视频已设置用{MethodName}方法处理");
        }

        private Bitmap CvtColor(Bitmap image)
        {
            Mat input = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);
            Mat output_1 = new Mat();
            Mat output_2 = new Mat();
            Cv2.CvtColor(input, output_1, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(output_1, output_2, 120, 0xff, ThresholdTypes.Binary);

            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(output_2);
        }

        public Bitmap FindContours(Bitmap image)
        {
            Mat input = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);
            Mat dstImg = new Mat(input.Size(), input.Type());

            Mat pGray = new Mat(input.Size(), input.Type());

            Mat pBlur = new Mat(input.Size(), input.Type());

            Cv2.MedianBlur(input, pBlur, 3);

            Mat grayImg = new Mat(input.Size(), input.Type());

            Cv2.CvtColor(pBlur, grayImg, ColorConversionCodes.BGR2GRAY);

            Cv2.Threshold(grayImg, pGray, 100, 255, ThresholdTypes.Binary);

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchly;
            Cv2.FindContours(~pGray, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new OpenCvSharp.Point(0, 0));

            pBlur.CopyTo(dstImg);
            Random rnd = new Random();
            for (int i = 0; i < contours.Length; i++)
            {
                Scalar color = new Scalar(255, 255, 255);//白色
                Cv2.DrawContours(dstImg, contours, i, color, 1, LineTypes.Link8, hierarchly);
            }


            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dstImg);
        }
    }

}

