using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace IPV_assignments
{
    public partial class Form1 : Form
    {
        private Capture _capture;        //takes images from camera as image frames
        private bool _captureInProgress; // checks if capture is executing
        private Image<Bgr, byte> _imageFrame = new Image<Bgr, byte>(@"lena.jpg");

        public Form1()
        {
            InitializeComponent();

            imageBox1.Image = _imageFrame;
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            _imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>();   //line 1

            imageBox1.Image = _imageFrame;                              //line 2
        }

        private void ProcessFrameA(object sender, EventArgs arg)
        {
            Image<Bgr, byte> tempCloneImage = _imageFrame.Clone();

            for (int i = 50; i < 200; i++)
            {
                tempCloneImage.Data[200, i, 0] = 255;
                tempCloneImage.Data[200, i, 1] = 255;
                tempCloneImage.Data[200, i, 2] = 255;
                //double line
                tempCloneImage.Data[200, i + 1, 0] = 255;
                tempCloneImage.Data[200, i + 1, 1] = 255;
                tempCloneImage.Data[200, i + 1, 2] = 255;
            }
            for (int i = 50 + 75; i < 275; i++)
            {
                tempCloneImage[i, 125] = new Bgr(0, 0, 0);
                //double line
                tempCloneImage[i, 125 + 1] = new Bgr(0, 0, 0);
            }

            imageBox2.Image = tempCloneImage;
        }

        private void ProcessFrameB(object sender, EventArgs arg)
        {
            Image<Bgr, byte> tempCloneImage = _imageFrame.Clone();

            for (int x = 0; x < tempCloneImage.Rows; x++)
            {
                for (int y = 0; y < tempCloneImage.Cols; y++)
                {
                    Bgr color = tempCloneImage[x, y];
                    color.Green = color.Green * 0.114;
                    color.Blue = color.Blue * 0.587;
                    color.Red = color.Red * 0.299;
                    tempCloneImage[x, y] = color;
                }
            }
            imageBox3.Image = tempCloneImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Idle += ProcessFrameA;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Idle += ProcessFrameB;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ToDo
            //implement C part on this buttondasdas
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ToDo
            //implement D part on this button
        }

        private void cameraBtn_Click(object sender, EventArgs e)
        {
            #region if capture is not created, create it now
            if (_capture == null)
            {
                try
                {
                    _capture = new Capture();
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            #endregion

            if (_capture != null)
            {
                if (_captureInProgress)
                {  //if camera is getting frames then stop the capture and set button Text
                    // "Start" for resuming capture
                    button5.Text = "Start!"; //
                    Application.Idle -= ProcessFrame;
                }
                else
                {
                    //if camera is NOT getting frames then start the capture and set button
                    // Text to "Stop" for pausing capture
                    button5.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }

                _captureInProgress = !_captureInProgress;
            }
        }
    }
}
