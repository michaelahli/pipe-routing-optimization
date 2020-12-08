using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Routing_Optimized_with_GA
{
    public partial class Form1 : Form
    {
        Graphics gra;
        public Form1()
        {
            InitializeComponent();
        }

        Dijkstra test; bool diagonalallowance;
        private void Button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                diagonalallowance = true;
            }
            else
            {
                diagonalallowance = false;
            }
            gra = pictureBox1.CreateGraphics();
            test = new Dijkstra(textBox1.Text, textBox2.Text, textBox3.Text);
            if (textBox4.Text.Length == 0)
            {
                test.Search(gra, listBox1, diagonalallowance);
            }
            else
            {
                test.Search(gra, listBox1, diagonalallowance, textBox4.Text);
            }
        }

        private void TextBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
            }
        }

        private void TextBox2_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;
            }
        }

        private void TextBox3_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = ofd.FileName;
            }
        }

        GA genetic_algorithm;
        private void Button2_Click(object sender, EventArgs e)
        {
            string name = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                name = fbd.SelectedPath;
            }
            gra = pictureBox1.CreateGraphics();
            Random r = new Random();
            genetic_algorithm = new GA(textBox1.Text, textBox2.Text, textBox3.Text, Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox6.Text));
            this.Enabled = false;
            genetic_algorithm.Run(gra, listBox1, r, progressBar1, chart1, name);
            this.Enabled = true;
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TextBox4_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = ofd.FileName;
            }
        }
    }
}
