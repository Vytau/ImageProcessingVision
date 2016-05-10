using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace IPV_assignment1
{
    public partial class Form1 : Form
    {
        private Capture _capture;        //takes images from camera as image frames
        private bool _captureInProgress; // checks if capture is executing
        private Image<Bgr, byte> _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\lena.jpg");

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
                    color.Blue = color.Blue * 0.114;
                    color.Green = color.Green * 0.587;
                    color.Red = color.Red * 0.299;
                    tempCloneImage[x, y] = color;
                }
            }
            imageBox3.Image = tempCloneImage;

            //or with method from emgu cv

            //Image<Gray, byte> grayImage = tempCloneImage.Convert<Gray, byte>();
            //imageBox6.Image = grayImage;
        }

        private void ProcessFrameC(object sender, EventArgs arg)
        {
            Image<Bgr, byte> tempCloneImage = _imageFrame.Clone();
            Image<Gray, byte> grayImage = tempCloneImage.Convert<Gray, byte>();

            for (int x = 0; x < grayImage.Rows; x++)
            {
                for (int y = 0; y < grayImage.Cols; y++)
                {
                    if (grayImage[x, y].Intensity > 125)
                    {
                        grayImage.Data[x, y, 0] = 255;
                    }
                    else if (grayImage[x, y].Intensity <= 125)
                    {
                        grayImage.Data[x, y, 0] = 0;
                    }
                }
            }

            imageBox4.Image = grayImage;

            //or with emgu cv threshold function

            //imageBox4.Image = grayImage.ThresholdBinary(new Gray(125), new Gray(200));
        }

        private void ProcessFrameD(object sender, EventArgs arg)
        {
            Image<Bgr, byte> tempCloneImage = _imageFrame.Clone();
            Image<Gray, byte> origine = tempCloneImage.Convert<Gray, byte>();
            Image<Gray, byte> newImage = tempCloneImage.Convert<Gray, byte>();
            List<byte> tempValues = new List<byte>();

            for (int x = 0; x < tempCloneImage.Rows; x++)
            {
                for (int y = 0; y < tempCloneImage.Cols; y++)
                {
                    tempValues.Clear();
                    tempValues.Add(origine.Data[x, y, 0]);
                    if (x - 1 >= 0)
                    {
                        tempValues.Add(origine.Data[x - 1, y, 0]);
                    }
                    if (y - 1 >= 0)
                    {
                        tempValues.Add(origine.Data[x, y - 1, 0]);
                    }
                    if (x + 1 > tempCloneImage.Rows)
                    {
                        tempValues.Add(origine.Data[x + 1, y, 0]);
                    }
                    if (y + 1 > tempCloneImage.Cols)
                    {
                        tempValues.Add(origine.Data[x, y + 1, 0]);
                    }

                    newImage.Data[x, y, 0] = tempValues.Max();
                }
            }

            imageBox5.Image = newImage;
            //to compare with oroginal picture
            imageBox6.Image = origine;

            //or with emgu function
            //imageBox6.Image = origine.Dilate(1);
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
            Application.Idle += ProcessFrameC;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Idle += ProcessFrameD;
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
                    button5.Text = "Start!";
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

        private void thresholdHistBtn_Click(object sender, EventArgs e)
        {
            Image<Bgr, byte> tempCloneImage = _imageFrame.Clone();
            Image<Gray, byte> grayImage = tempCloneImage.Convert<Gray, byte>();

            histogramBox1.ClearHistogram();
            histogramBox1.GenerateHistograms(grayImage, 256);
            histogramBox1.Refresh();
        }
    }
}
