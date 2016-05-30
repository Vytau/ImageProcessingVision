using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPV_assignment2
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> _imageFrame;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\ipv.bmp");
            imageBox1.Image = _imageFrame;
            imageBox2.Image = _imageFrame.Clone();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _imageFrame = new Image<Bgr, byte>(@"..\..\Resources\sudoku-original.jpg");
            imageBox1.Image = _imageFrame;
            imageBox2.Image = _imageFrame.Clone();
        }
    }
}
