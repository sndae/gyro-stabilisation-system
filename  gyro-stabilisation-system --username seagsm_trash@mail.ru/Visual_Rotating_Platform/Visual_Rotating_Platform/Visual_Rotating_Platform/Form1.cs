using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


// It is for using OpenGL lib 
using Tao.OpenGl;
// It is for using FreeGLUT lib
using Tao.FreeGlut;
//  It is for using SimpleOpenGLControl control
using Tao.Platform.Windows;


namespace Visual_Rotating_Platform
{
    public partial class MainForm : Form
    {
        Form_3D Form_3D_DA, Form_3D_DB;
        x_IMU_API.QuaternionData qw = new x_IMU_API.QuaternionData();
        float[] Quaternion_1 = new float[] { 1.0f, 0.0f, 1.0f, .0f };
        float a=0, b=0, alpha=0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {   
                /* Create new Form A and create cube with textures. */
                Form_3D Form_3D_A = new Form_3D(new string[] {  "Form_3D/RightInv.png", 
                                                                "Form_3D/LeftInv.png", 
                                                                "Form_3D/BackInv.png", 
                                                                "Form_3D/FrontInv.png", 
                                                                "Form_3D/TopInv.png", 
                                                                "Form_3D/BottomInv.png" 
                                                            });
                /* Create new Form B and create cube with textures. */
                Form_3D Form_3D_B = new Form_3D();

                /* Make pointer to Form. */
                Form_3D_DA = Form_3D_A;
                Form_3D_DB = Form_3D_B;

                /* Start 3D form. */
                Form_3D_A.Show();
                Form_3D_B.Show();
                /* Start timer to refresh rotate matrix of the object on the form. */
                timer1.Enabled = true;
            }
            catch
            {
                MessageBox.Show(" Error .");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            a = (float)Math.Cos((alpha * Math.PI) / (180 * 2));
            b = (float)Math.Sin((alpha * Math.PI) / (180 * 2));
            float[] Quaternion_1 = new float[] { a, 0.0f, b, .0f };

            Quaternion_1[0] = a;
            Quaternion_1[1] = 0.0f;
            Quaternion_1[2] = b;
            Quaternion_1[3] = 0.0f;
          
            qw.Quaternion = Quaternion_1;
            /* Refresh rotate matrix on the form object. */
            Form_3D_DA.RotationMatrix = qw.ConvertToRotationMatrix();
            Form_3D_DB.RotationMatrix = qw.ConvertToRotationMatrix();

            alpha = alpha + 0.5f;

        }
    }
}
