using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SaiGeDemo.UserView
{
    /// <summary>
    /// 变换方法列表的数据模型
    /// </summary>
    public class MethodModel
    {


        public string MethodName { get; set; }

        public BitmapImage BitmapImage { get; set; }

        public MethodModel(string methodName)
        {
            MethodName = methodName;
            BitmapImage = new BitmapImage();

        }

        /// <summary>
        /// 执行CV2变换方法
        /// </summary>
        /// <param name="uriString">需要变换的图片地址</param>
        public MethodModel CV2_Run(string uriString)
        {
            BitmapImage = new BitmapImage();
            Mat input = new Mat(uriString);
            Mat output = new Mat();
            switch (MethodName)
            {
                case "颜色空间转换":
                    Cv2.CvtColor(input, output, ColorConversionCodes.BGR2GRAY);
                    break;
                case "方框滤波":
                    Cv2.BoxFilter(input, output, MatType.CV_8UC3, new OpenCvSharp.Size(5, 5));
                    break;
                case "均值滤波":
                    Cv2.Blur(input, output, new OpenCvSharp.Size(5, 5));
                    break;
                case "高斯滤波":
                    Cv2.GaussianBlur(input, output, new OpenCvSharp.Size(5, 5), 0);
                    break;
                case "中值滤波":
                    Cv2.MedianBlur(input, output, 3);
                    break;
                case "双边滤波":
                    Cv2.BilateralFilter(input, output, 9, 80, 80);
                    break;
                case "圆形":
                    output = FindCircle(input);
                    break;
                case "找轮廓":
                    output = FindContours(input);
                    break;
                default:
                    break;
            }

            var memoryStream = output.ToMemoryStream();
            BitmapImage.BeginInit();
            BitmapImage.StreamSource = memoryStream;
            BitmapImage.EndInit();
            BitmapImage.Freeze();
            return this;

        }

        public Mat FindCircle(Mat input)
        {
            Mat pGray = new Mat(input.Size(), input.Type());

            Mat pBlur = new Mat(input.Size(), input.Type());

            Cv2.MedianBlur(input, pBlur, 3);

            Mat grayImg = new Mat(input.Size(), input.Type());

            Cv2.CvtColor(pBlur, grayImg, ColorConversionCodes.BGR2GRAY);

            Cv2.Threshold(grayImg, pGray, 100, 255, ThresholdTypes.Binary);

            CircleSegment[] circleSegments;
            circleSegments = Cv2.HoughCircles(~pGray, HoughModes.Gradient, 1, 30, 30, 26, 0, 0);

            Mat dstImg = new Mat(input.Size(), input.Type());

            pBlur.CopyTo(dstImg);

            Scalar pColor = new Scalar(0, 0, 255);

            for (int i = 0; i < circleSegments.Count(); i++)
            {
                Cv2.Circle(dstImg, (int)circleSegments[i].Center.X, (int)circleSegments[i].Center.Y, (int)circleSegments[i].Radius, pColor, 2, LineTypes.AntiAlias);
            }

            return dstImg;
        }

        public Mat FindContours(Mat input)
        {
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


            return dstImg;
        }
    }

}
