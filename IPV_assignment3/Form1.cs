using System;
using System.Windows.Forms;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace IPV_assignment3
{
    public partial class Form1 : Form
    {
        private Capture _cap;
        private CascadeClassifier _haarFace;
        private CascadeClassifier _haarEye;
        private CascadeClassifier _haarEye2;
        private CascadeClassifier _haarSmile;

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Image<Bgr, byte> nextFrame = _cap.QueryFrame().ToImage<Bgr, byte>();
            {
                if (nextFrame != null)
                {
                    Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();
                    // TODO: Add code to detect faces, using the CascadeClassifier
                    ;
                    ;
                    Rectangle[] rect1 = _haarFace.DetectMultiScale(grayframe, 1.4, 4);
                    Rectangle[] rect2 = _haarEye.DetectMultiScale(grayframe, 1.4, 4);
                    Rectangle[] rect4 = _haarEye2.DetectMultiScale(grayframe, 1.4, 4);
                    Rectangle[] rect3 = _haarSmile.DetectMultiScale(grayframe, 1.4, 4);

                    //TODO: Add code top draw rectangles around the faces
                    ;
                    ;
                    //Draw head rectangle
                    if (rect1 != null && rect1.Length != 0)
                    {
                        nextFrame.Draw(rect1[0], new Bgr(0, 255, 0), 3);
                        //Draw eye rectangle
                        if (rect2 != null && rect2.Length != 0 && rect1[0].Contains(rect2[0]))
                        {
                            nextFrame.Draw(rect2[0], new Bgr(255, 0, 0), 3);
                        }
                        if (rect4 != null && rect4.Length != 0 && rect1[0].Contains(rect4[0]))
                        {
                            nextFrame.Draw(rect4[0], new Bgr(255, 0, 0), 3);
                        }
                        if (rect3 != null && rect3.Length != 0 && rect1[0].Contains(rect3[0]))
                        {
                            nextFrame.Draw(rect3[0], new Bgr(0, 0, 255), 3);
                        }
                    }


                    imageBox1.Image = nextFrame;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // passing 0 gets zeroth webcam
            _cap = new Capture(0);
            timer1.Enabled = true;
            // TODO: Add code to initialize the CascadeClassifier with the file haarcascade_frontalface_default.xml
            ;
            ;
            _haarFace = new CascadeClassifier(@"../../Resources/haarcascade_frontalface_default.xml");
            _haarEye = new CascadeClassifier(@"../../Resources/haarcascade_eye.xml");
            _haarEye2 = new CascadeClassifier(@"../../Resources/haarcascade_eye.xml");
            _haarSmile = new CascadeClassifier(@"../../Resources/haarcascade_smile.xml");
        }
    }
}