using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SemaphoreAppKeyb
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = Program.press_count.ToString();
            label2.Text = Program.Length.ToString();

            if (Program.press_count <= 100)
            {
                progressBar1.Value = Program.press_count;
            }
            else
            { 
                progressBar1.Value = 100; 
            }

            if(Program.press_count > 0)
            {
                Program.press_count--;
            }

            label3.Text = Program.ActiveWindowName;
            this.Update();
        }
    }
}

