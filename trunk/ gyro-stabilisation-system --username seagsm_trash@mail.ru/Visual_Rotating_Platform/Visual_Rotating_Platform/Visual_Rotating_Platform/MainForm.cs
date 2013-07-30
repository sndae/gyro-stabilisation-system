#define Gyro
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/* It is for using OpenGL lib. */ 
using Tao.OpenGl;
/* It is for using FreeGLUT lib. */
using Tao.FreeGlut;
/* It is for using SimpleOpenGLControl control. */
using Tao.Platform.Windows;

/* Serialisation and files processing. */
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

/* Work with XML. */
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;

/* Serial port. */
using System.IO.Ports;



namespace Visual_Rotating_Platform
{
    public partial class MainForm : Form
    {
        Form_3D Form_3D_DA, Form_3D_DB;
        Graph_2D Graph_2D_DA, Graph_2D_DB, Graph_2D_DC;
        //Log file
        StreamWriter file_magnetometer = new System.IO.StreamWriter(@"..\magnetometer.txt");
        int zGMaxAmountOfPoints = 600;
        x_IMU_API.QuaternionData qw = new x_IMU_API.QuaternionData();
        x_IMU_API.QuaternionData qt = new x_IMU_API.QuaternionData();
        float[] Quaternion_1 = new float[] { 1.0f, 0.0f, 1.0f, .0f };
        float a = 0, b = 0, alpha = 0;
        float[] angl_midl = new float[10];

        #region Serial potr variables
        /*------- Serial communication  port ----*/
        const int ROUND_BUFFER_SIZE = 128;/* RX round buffer size. */
        byte[] round_rx_buf = new byte[ROUND_BUFFER_SIZE];  /* Receive round buffer. */
        int round_rx_buf_head = 0;                          /* Index of head of round buffer. */
        int round_rx_buf_tail = 0;                          /* Index of tail of round buffer. */
        int round_rx_buf_size = 0;                          /* Size of datas stored in round buffer. */
        //int[] API_frame_RX_buffer = new int[round_buffer_size];
        //byte[] API_frame_TX_buffer = new byte[round_buffer_size];
        string CurrentComPortName;

        //class Port
        SeriaPortCommunication.SerialCommunication Port_1;
        /*---------------------------------------*/
        #endregion

        public MainForm()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false; // disable checking of unsafe
        }

        private void ButtonStart_Click(object sender, EventArgs e)
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
                Form_3D_A.Text = "Rotating Object 1";
                Form_3D_A.MinimizeInsteadOfClose = true;
                /* Create new Form B and create cube with textures. */
                Form_3D Form_3D_B = new Form_3D();
                Form_3D_B.Text = "Rotating Object 2";
                Form_3D_B.MinimizeInsteadOfClose = true;
                /* Create zGraph Form  and define all paramerers for 6 curves.*/
                Graph_2D Graph_2D_A = new Graph_2D(
                                                    new string[] {  
                                                                    "X", 
                                                                    "Y", 
                                                                    "Z", 
                                                                 },
                                                    new Color[]  {
                                                                    Color.Aqua,
                                                                    Color.Black,
                                                                    Color.Blue,
                                                                 },
                                                                "Graph Accel",
                                                                " Time , milliSec ",
                                                                " Value of parameters"
                                                  );
                /* Create zGraph Form  and define all paramerers for 2 curves.*/
                Graph_2D Graph_2D_B = new Graph_2D(
                                                    new string[] {  
                                                                    "X", 
                                                                    "Y", 
                                                                    "Z", 

                                                    },
                                                    new Color[]  {
                                                                    Color.Aqua,
                                                                    Color.Red,
                                                                    Color.Green
                                                                 },
                                                                "Graph Magnetometer",
                                                                " Time , milliSec ",
                                                                " Value of parameters"
                                                  );
                /* Create zGraph Form  and define all paramerers for 2 curves.*/
                Graph_2D Graph_2D_C = new Graph_2D(
                                                    new string[] {  
                                                                    "Pitch", 
                                                                    "Roll", 
                                                                    "Yaw", 

                                                    },
                                                    new Color[]  {
                                                                    Color.Aqua,
                                                                    Color.Red,
                                                                    Color.Green
                                                                 },
                                                                "Graph C",
                                                                " Time , milliSec ",
                                                                " Degree"
                                                  );

                /* Make pointer to Form. */
                Form_3D_DA  = Form_3D_A;
                Form_3D_DB  = Form_3D_B;
                Graph_2D_DA = Graph_2D_A;
                Graph_2D_DB = Graph_2D_B;
                Graph_2D_DC = Graph_2D_C;

                /* Start 3D form. */
                Form_3D_A.Show();
                Form_3D_B.Show();
                /* We should give different name for each form to make possible save form parameters to file.*/
                Graph_2D_A.Text = "zGraph N1";
               // Graph_2D_A.MinimizeInsteadOfClose = true;
                Graph_2D_A.Show();
                Graph_2D_B.Text = "zGraph N2";
               // Graph_2D_B.MinimizeInsteadOfClose = true;
                Graph_2D_B.Show();

                Graph_2D_C.Text = "Angle";
                // Graph_2D_B.MinimizeInsteadOfClose = true;
                Graph_2D_C.Show();


                //serialPort1.WriteLine("T");

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
            float[] quaternion = new float[] { 1, 0.0f, 0.0f, 0.0f };
            float[] eulerAngles = new float[] { 0.0f, 0.0f, 0.0f };

            Quaternion_1[0] = a;
            Quaternion_1[1] = 0.0f;
            Quaternion_1[2] = b;
            Quaternion_1[3] = 0.0f;
          
            qw.Quaternion = Quaternion_1;

            /* Refresh rotate matrix on the form object. */
            //Form_3D_DA.RotationMatrix = qw.ConvertToRotationMatrix();
#if DEMO
            Quaternion_1[0] = a;
            Quaternion_1[1] = b;
            Quaternion_1[2] = 0.0f;
            Quaternion_1[3] = 0.0f;

            qw.Quaternion = Quaternion_1;
            Form_3D_DB.RotationMatrix = qw.ConvertToRotationMatrix();
#endif

#if ! ACCEL
            if (Graph_2D_DA != null)
            {
                Graph_2D_DA.DrawGraph(
                                    (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                    (double)Port_1.currentReceivedData.Accelerometer.Axis_X.i16,
                                            0,
                                        10000.0,
                                          200,
                                          zGMaxAmountOfPoints
                                 );


                Graph_2D_DA.DrawGraph(
                         (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                         (double)Port_1.currentReceivedData.Accelerometer.Axis_Y.i16,
                                    1,
                                    10000.0,
                                    200,
                                    zGMaxAmountOfPoints
                        );
                Graph_2D_DA.DrawGraph(
                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                        (double)Port_1.currentReceivedData.Accelerometer.Axis_Z.i16,
                                2,
                            10000.0,
                              200,
                              zGMaxAmountOfPoints
                        );
            }
#else //GYRO
            if (Graph_2D_DA != null)
            {
                Graph_2D_DA.DrawGraph(
                                    (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                    (double)Port_1.currentReceivedData.Gyro.Rot_XY.i16 -2 ,
                                            0,
                                        10000.0,
                                          200,
                                          zGMaxAmountOfPoints
                                 );


                Graph_2D_DA.DrawGraph(
                         (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                         (double)Port_1.currentReceivedData.Gyro.Rot_XZ.i16,
                                    1,
                                    10000.0,
                                    200,
                                    zGMaxAmountOfPoints
                        );
                Graph_2D_DA.DrawGraph(
                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                        (double)Port_1.currentReceivedData.Gyro.Rot_YZ.i16,
                                2,
                            10000.0,
                              200,
                              zGMaxAmountOfPoints
                        );
            }
#endif
          

#if ! Gyro            
            
            if (Graph_2D_DB != null)
            {
                Graph_2D_DB.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)Port_1.currentReceivedData.Magnetometer.Axis_X.i16,
                                                0,
                                            10000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );

                Graph_2D_DB.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)Port_1.currentReceivedData.Magnetometer.Axis_Y.i16,
                                                1,
                                            10000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );
                Graph_2D_DB.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)Port_1.currentReceivedData.Magnetometer.Axis_Z.i16,
                                                2,
                                            10000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );

             }
#else  //GYRO
            if (Graph_2D_DB != null)
            {
                Graph_2D_DB.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)Port_1.currentReceivedData.Gyro.Rot_XY.i16 - 2,
                                                0,
                                            10000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );

                Graph_2D_DB.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)Port_1.currentReceivedData.Gyro.Rot_XZ.i16,
                                                1,
                                            10000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );
                Graph_2D_DB.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)Port_1.currentReceivedData.Gyro.Rot_YZ.i16,
                                                2,
                                            10000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );
            }
#endif

            qw.Quaternion = Port_1.bodyQuaternion;
            quaternion = Port_1.bodyQuaternion;
            label2.Text = Convert.ToString(Port_1.deltaTime);
            
            eulerAngles  = qw.ConvertToEulerAngles();

            
            /******** My Euler Angle  ****************************/
            float Pitch_Real=0;
            float Roll_Real=0;
            float Yaw_Real;

            Pitch_Real = (float)Math.Atan2 (2*(quaternion[0]*quaternion[1]+quaternion[2]*quaternion[3]),1-2*(quaternion[1]*quaternion[1]+quaternion[2]*quaternion[2]));
            Roll_Real = (float)Math.Asin (2*(quaternion[0]*quaternion[2]-quaternion[3]*quaternion[1]));
            Yaw_Real = (float)Math.Atan2(2 * (quaternion[0] * quaternion[3] + quaternion[1] * quaternion[2]), 1 - 2 * (quaternion[2] * quaternion[2] + quaternion[3] * quaternion[3]));
            label76.Text = Convert.ToString(Roll_Real * 180 / (float)Math.PI); // Roll
            label77.Text = Convert.ToString(Pitch_Real * 180 / (float)Math.PI); // Pitch
            label78.Text = Convert.ToString(Yaw_Real * 180 / (float)Math.PI); // Yaw
            /********************************************************************************/


            /****** One more My Euler Angle ************/
            float heading;
		    float attitude;
		    float bank;


          	double test = quaternion[1]*quaternion[2] + quaternion[3]*quaternion[0];
	        if (test > 0.499) 
            { // singularity at north pole
		        heading = 2 * (float)Math.Atan2(quaternion[1],quaternion[0]);
		        attitude = (float)Math.PI/2;
		        bank = 0;
		    }
	        if (test < -0.499) 
            { // singularity at south pole
		        heading = -2 * (float)Math.Atan2(quaternion[1],quaternion[0]);
		        attitude = - (float)Math.PI/2;
		        bank = 0;
			}
            double sqx = quaternion[1]*quaternion[1];
            double sqy = quaternion[2]*quaternion[2];
            double sqz = quaternion[3]*quaternion[3];
            heading = (float)Math.Atan2(2*quaternion[2]*quaternion[0]-2*quaternion[1]*quaternion[3] , 1 - 2*sqy - 2*sqz);
	        attitude = (float)Math.Asin(2*test);
            bank = (float)Math.Atan2(2 * quaternion[1] * quaternion[0] - 2 * quaternion[2] * quaternion[3], 1 - 2 * sqx - 2 * sqz);

            label80.Text = Convert.ToString(heading * 180 / (float)Math.PI); // Pitch
            label81.Text = Convert.ToString(bank * 180 / (float)Math.PI); // Roll
            label82.Text = Convert.ToString(attitude * 180 / (float)Math.PI); // Yaw



            /*********************************************/



            /********* Angle Axis ***********************/
            float angle;

            float x,y,z;

            angle = 2 * (float)Math.Acos(Port_1.bodyQuaternion[0]);
            double s = Math.Sqrt(1 - Port_1.bodyQuaternion[0] * Port_1.bodyQuaternion[0]);
            if (s < 0.001)
            {
                x = Port_1.bodyQuaternion[1]; // if it is important that axis is normalised then replace with x=1; y=z=0;
                y = Port_1.bodyQuaternion[2];
                z = Port_1.bodyQuaternion[3];
            } 
            else 
            {
                x = Port_1.bodyQuaternion[1]/(float)s; 
                y = Port_1.bodyQuaternion[2]/(float)s;
                z = Port_1.bodyQuaternion[3]/(float)s;
            }

            angle = angle * 180 / (float)Math.PI;
            label71.Text = Convert.ToString(angle); 
            label72.Text = Convert.ToString(x); 
            label73.Text = Convert.ToString(y); 
            label74.Text = Convert.ToString(z); 
            /****** End Angle Axis *********************************/




            /* Quaternion PC. */
            
            label16.Text = Convert.ToString(eulerAngles[0]); // Roll
            label17.Text = Convert.ToString(eulerAngles[1]); // Pitch
            label18.Text = Convert.ToString(eulerAngles[2]); // Yaw

            label26.Text = Convert.ToString(Port_1.currentReceivedData.EulerAngle_0.f32);
            label27.Text = Convert.ToString(Port_1.currentReceivedData.EulerAngle_1.f32);
            label28.Text = Convert.ToString(Port_1.currentReceivedData.EulerAngle_2.f32);


            /* Add PIDs */
            /* Static PID */
            /* Roll. */
            label39.Text = Convert.ToString(Port_1.currentReceivedData.Roll.kP.u16);
            label40.Text = Convert.ToString(Port_1.currentReceivedData.Roll.kI.u16);
            label41.Text = Convert.ToString(Port_1.currentReceivedData.Roll.kD.u16);

            /* Pitch. */
            label42.Text = Convert.ToString(Port_1.currentReceivedData.Pitch.kP.u16);
            label43.Text = Convert.ToString(Port_1.currentReceivedData.Pitch.kI.u16);
            label44.Text = Convert.ToString(Port_1.currentReceivedData.Pitch.kD.u16);

            /* Yaw. */
            label45.Text = Convert.ToString(Port_1.currentReceivedData.Yaw.kP.u16);
            label46.Text = Convert.ToString(Port_1.currentReceivedData.Yaw.kI.u16);
            label47.Text = Convert.ToString(Port_1.currentReceivedData.Yaw.kD.u16);

            /* Dynamic PID */

            /* Roll. */
            label48.Text = Convert.ToString(Port_1.currentReceivedData.Roll.kP_dynamic.u16);
            label49.Text = Convert.ToString(Port_1.currentReceivedData.Roll.kI_dynamic.u16);
            label50.Text = Convert.ToString(Port_1.currentReceivedData.Roll.kD_dynamic.u16);

            /* Pitch. */
            label51.Text = Convert.ToString(Port_1.currentReceivedData.Pitch.kP_dynamic.u16);
            label52.Text = Convert.ToString(Port_1.currentReceivedData.Pitch.kI_dynamic.u16);
            label53.Text = Convert.ToString(Port_1.currentReceivedData.Pitch.kD_dynamic.u16);

            /* Yaw. */
            label54.Text = Convert.ToString(Port_1.currentReceivedData.Yaw.kP_dynamic.u16);
            label55.Text = Convert.ToString(Port_1.currentReceivedData.Yaw.kI_dynamic.u16);
            label56.Text = Convert.ToString(Port_1.currentReceivedData.Yaw.kD_dynamic.u16);


#if ! MAGNETOMETER            
            if (Graph_2D_DC != null)
            {
                Graph_2D_DC.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)eulerAngles[0],
                                                0,
                                            100000.0,
                                              200,
                                              3000//zGMaxAmountOfPoints
                                    );

                Graph_2D_DC.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                        (double)eulerAngles[1],
                                                1,
                                            100000.0,
                                              200,
                                              3000//zGMaxAmountOfPoints
                                    );

                /* midle filter.*/
                float angl_sum=0;
                int angl_ind=50;
                angl_sum = 0;
                angl_ind = 0;

                while(angl_ind < 10 )
                {
                    angl_sum = angl_sum + angl_midl[angl_ind];
                    angl_ind++;
                }
                angl_sum = angl_sum /(float) 10.0;
                /******************/
                Graph_2D_DC.DrawGraph(
                                        (double)Port_1.currentReceivedData.ExternalBoardSystemTime.ui32,
                                     //   ((double)eulerAngles[2] + angl_sum) / 51.0,
                                        ((double)angl_sum),
                                       
                                                2,
                                            100000.0,
                                              200,
                                              3000//zGMaxAmountOfPoints
                                    );
                /* midle filter.*/
                angl_sum = 0;
                angl_ind = 9;
                while (angl_ind > 0)
                {
                    angl_midl[angl_ind] = angl_midl[angl_ind-1];
                    angl_ind--;
                }
                //angl_midl[0] = eulerAngles[2];
                angl_midl[0] = Port_1.currentReceivedData.EulerAngle_2.f32;
                /******************/
             }
#endif

            label8.Text  = Convert.ToString(Port_1.bodyQuaternion[0]);
            label9.Text  = Convert.ToString(Port_1.bodyQuaternion[1]);
            label10.Text = Convert.ToString(Port_1.bodyQuaternion[2]);
            label11.Text = Convert.ToString(Port_1.bodyQuaternion[3]);


            label20.Text = Convert.ToString(Port_1.currentReceivedData.Quaternion_0.f32);
            label21.Text = Convert.ToString(Port_1.currentReceivedData.Quaternion_1.f32);
            label22.Text = Convert.ToString(Port_1.currentReceivedData.Quaternion_2.f32);
            label23.Text = Convert.ToString(Port_1.currentReceivedData.Quaternion_3.f32);

            
            qt.Quaternion[0] = Port_1.currentReceivedData.Quaternion_0.f32;
            qt.Quaternion[1] = Port_1.currentReceivedData.Quaternion_1.f32;
            qt.Quaternion[2] = Port_1.currentReceivedData.Quaternion_2.f32;
            qt.Quaternion[3] = Port_1.currentReceivedData.Quaternion_3.f32;


            //Save magnetometer data to file for using in calibration algoritm.
            string text = Convert.ToString(((float)Port_1.currentReceivedData.Magnetometer.Axis_X.i16)) + "\t" +
              Convert.ToString(((float)Port_1.currentReceivedData.Magnetometer.Axis_Y.i16)) + "\t" +
              Convert.ToString(((float)Port_1.currentReceivedData.Magnetometer.Axis_Z.i16));

            file_magnetometer.WriteLine(text);

/*
 * This is test for rotation by rotation matrix
 */

            float alpha_rad = 0.03f / 57.2957795130823f;
            float[] rot_z = new float[] 
            {
                (float)Math.Cos(alpha_rad) , (float)-Math.Sin(alpha_rad),  0,
                (float)Math.Sin(alpha_rad) ,(float) Math.Cos(alpha_rad) ,  0,
                                  0 ,                   0 ,                 1
            };

            float[] rot_x = new float[] 
            {   
                1,0,0,
                0,(float)Math.Cos(alpha_rad) , (float)-Math.Sin(alpha_rad),
                0,(float)Math.Sin(alpha_rad) ,(float) Math.Cos(alpha_rad) 
            };

            Matrix.rotMatrix G = new Matrix.rotMatrix();
            //rotate body by rotation matrix.
            G.MatrixMultiplication(rot_z, Form_3D_DA.RotationMatrix);
            //G.MatrixMultiplication(rot_x, G.A3x3);
            //Rotate body by rotation matrix.
        ///    Form_3D_DA.RotationMatrix = G.A3x3;



/*
 * End of test 
 */
            x_IMU_API.QuaternionData q = new x_IMU_API.QuaternionData();
            q.Quaternion =new float[] { 
                                        (float)Math.Cos(alpha_rad/2.0),
                                        (float)Math.Sin(alpha_rad/2.0)*1.0f,        //Z
                                        (float)Math.Sin(alpha_rad/2.0)*0.0f,        //Y
                                        (float)Math.Sin(alpha_rad/2.0)*0.0f         //Z
                                      };
            //Rotate body by quaternion.
          //  qw.QuaternionMultiplication(q.Quaternion);

            alpha_rad = -0.02f / 57.2957795130823f;
            q.Quaternion[0] = (float)Math.Cos(alpha_rad / 2.0);
            q.Quaternion[1] = (float)Math.Sin(alpha_rad / 2.0) * 0.0f;        //Z
            q.Quaternion[2] = (float)Math.Sin(alpha_rad / 2.0) * 1.0f;        //Y
            q.Quaternion[3] = (float)Math.Sin(alpha_rad / 2.0) * 0.0f;         //Z

          //  qw.QuaternionMultiplication(q.Quaternion);

            Form_3D_DA.RotationMatrix = qw.ConvertToConjugate().ConvertToRotationMatrix();


           // Form_3D_DA.RotationMatrix = qw.ConvertToConjugate().ConvertToRotationMatrix();
            Form_3D_DB.RotationMatrix = qt.ConvertToConjugate().ConvertToRotationMatrix();



#if DEMO  
            if (Graph_2D_DA != null)
            {
                Graph_2D_DA.DrawGraph(
                                    (double)alpha,
                                    (double)a,
                                            0,
                                        1000.0,
                                          200,
                                          zGMaxAmountOfPoints
                                 );


                Graph_2D_DA.DrawGraph(
                                        (double)alpha,
                                        (double)b,
                                                1,
                                            1000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );
            }

            if (Graph_2D_DB != null)
            {
                Graph_2D_DB.DrawGraph(
                                        (double)alpha,
                                        (double)b,
                                                0,
                                            1000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );

                Graph_2D_DB.DrawGraph(
                                        (double)alpha+10.3,
                                        (double)b,
                                                1,
                                            1000.0,
                                              200,
                                              zGMaxAmountOfPoints
                                    );
            }
#endif
            alpha = alpha + 0.05f;
     //       label2.Text = Convert.ToString(alpha);




        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Load Form Parameters from setup file.
            /* Load setup parameters for this form. */
            FormSetup.SFP my_setup = new FormSetup.SFP();
            FormSetup.SFP.FormParameters sfp = new FormSetup.SFP.FormParameters();
            /* Set setup file name. */

            my_setup.SetupFileFileName = "setup.xml";
            /* Read setup file. */
            sfp = my_setup.ReadSetupFile(this);
            if (sfp != null)
            {
                this.Left = sfp.FormPosition_Left;
                this.Top = sfp.FormPosition_Top;
                this.Height = sfp.FormHeight;
                this.Width = sfp.FormWidth;
                this.CurrentComPortName = sfp.FormComPortName;
            }
            #endregion

            #region Scan current Com ports and set Com Port name.
            /* Scan all serial ports. */
            try
            {
                /* Get list of avialabled com-ports.*/
                string[] availablePorts = SerialPort.GetPortNames();
                /* Add list of com-ports to combo box. */
                int PortIndex = 0;
                foreach (string port in availablePorts)
                {
                    if (this.CurrentComPortName == port)
                    {
                        PortIndex = comboBox1.Items.Count;
                    }
                    comboBox1.Items.Add(port);
                }
                comboBox1.Text = comboBox1.Items[PortIndex].ToString();
            }
            catch (Exception e9)
            {
                MessageBox.Show(e9.Message);
            }
            #endregion
        }


        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            #region Save current Form Parameters to setup file.

            FormSetup.SFP my_setup = new FormSetup.SFP();
            my_setup.SetupFileFileName = "setup.xml";
            my_setup.ComPortName = CurrentComPortName;
            my_setup.WriteSetupFile(this);
            
            #endregion

            #region Close opened file.

            file_magnetometer.Close();

            #endregion
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            #region Save child Form Parameters to setup file 
            if (Form_3D_DA != null)
            {
                FormSetup.SFP my_setup_a = new FormSetup.SFP();
                my_setup_a.SetupFileFileName = "setup.xml";
                my_setup_a.WriteSetupFile(Form_3D_DA);
            }
            if (Form_3D_DB != null)
            {
                FormSetup.SFP my_setup_b = new FormSetup.SFP();
                my_setup_b.SetupFileFileName = "setup.xml";
                my_setup_b.WriteSetupFile(Form_3D_DB);
            }
            if (Graph_2D_DA != null)
            {
                FormSetup.SFP my_setup_a = new FormSetup.SFP();
                my_setup_a.SetupFileFileName = "setup.xml";
                my_setup_a.WriteSetupFile(Graph_2D_DA);
            }
            if (Graph_2D_DB != null)
            {
                FormSetup.SFP my_setup_b = new FormSetup.SFP();
                my_setup_b.SetupFileFileName = "setup.xml";
                my_setup_b.WriteSetupFile(Graph_2D_DB);
            }
            if (Graph_2D_DC != null)
            {
                FormSetup.SFP my_setup_b = new FormSetup.SFP();
                my_setup_b.SetupFileFileName = "setup.xml";
                my_setup_b.WriteSetupFile(Graph_2D_DC);
            }
            #endregion

        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            CurrentComPortName = comboBox1.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int result;

            //open com port and connect
            Port_1 = new SeriaPortCommunication.SerialCommunication();

            result = Port_1.ComPort (
                                        comboBox1.Text,
                                        StopBits.One,
                                        8,
                                        115200,
                                        Parity.None
                                    );

            if (result ==0)
            {
                button6.Visible = false;
                button7.Visible = true;
            }

        }


        /******************************************************************************
         *  Read byte from RX to RX round buffer. 
         ******************************************************************************/
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {   
            int ReadbufferSize;
            int ReadByte=0;
            // Read received data from serial RX buffer
            // to round RX buffer.
            ReadbufferSize = ReadRXBuffer(sender,e);
           // ReadbufferSize = (Byte)serialPort1.ReadByte();

            // Read round buffer and parsing it.
            while (ReadByte >= 0)
            {
                ReadByte = ReadByteFromRXBuff();
            }
        }

        // Read received data from serial buffer to round RX buffer 
        // return size of read data.
        private int ReadRXBuffer(object sender, SerialDataReceivedEventArgs e)
        {
            int BytesToRead;

            BytesToRead = serialPort1.BytesToRead;
            // Read size of bytes from Serial port buffer.
            for (int count = 0; count < BytesToRead; count++)
            {
                // This version of buffer read buffer size and drop all another bytes
                // before it will be read.
                if (round_rx_buf_size < ROUND_BUFFER_SIZE)
                {
                    /* Put byte to the end of round buffer */
                    round_rx_buf[round_rx_buf_tail] = (Byte)serialPort1.ReadByte();
                    /* Increase byte counter */
                    round_rx_buf_size++;
                    /* Increase pounter to the tail of round buffer */
                    round_rx_buf_tail++;
                    /* Check if pointer reach end of buffer  */
                    if (round_rx_buf_tail >= ROUND_BUFFER_SIZE)
                    {
                        /* Round the pointer */
                        round_rx_buf_tail = 0;
                    }
                }
            }
            return BytesToRead;
        }
        
        // Read byte fron buffer, return Byte of -1
        private int ReadByteFromRXBuff() 
        {
            int ReadByte = 0;
        
            if (round_rx_buf_size > 0)
            {
                /* Put byte to the ReadByte from serial round buffer */
                ReadByte = round_rx_buf[round_rx_buf_head];
                /* Dicrease byte counter */
                round_rx_buf_size--;
                /* Increase pounter to the head of round buffer */
                round_rx_buf_head++;
                /* Check if pointer reach end of buffer */
                if (round_rx_buf_head >= ROUND_BUFFER_SIZE)
                {
                    /* Round the pointer */
                    round_rx_buf_head = 0;
                }
                return ReadByte;
            }
            else 
            {
                return (-1);
            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            //disconnect and close com port
            try
            {
                Port_1.serialComPort.Close();
                if (!Port_1.serialComPort.IsOpen)
                {
                    button7.Visible = false;
                    button6.Visible = true;
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Port_1.MagnetAzimutCalculation(0, 0, -1, -1, 0, 3);

            x_IMU_API.QuaternionData qtest = new x_IMU_API.QuaternionData();
            qtest.Quaternion     = new float [] {1f,0f,0f,0f};
            float alpha_degree = 10;

            float alpha_rad = alpha_degree / 57.2957795130823f;

            float[] q = new float[] { 
                                        (float)Math.Cos(alpha_rad / 2.0), 
                                        (float)Math.Sin(alpha_rad / 2.0), 
                                        (float)Math.Sin(alpha_rad / 2.0), 
                                        (float)Math.Sin(alpha_rad / 2.0) 
                                    };

            qtest.QuaternionMultiplication(q);
        }

        private void ReadPIDs_Click(object sender, EventArgs e)
        {
            /* Add PIDs */
            /* Static PID */
            /* Roll. */
            numericUpDown1.Value = Convert.ToInt16(label39.Text);
            numericUpDown2.Value = Convert.ToInt16(label40.Text);
            numericUpDown3.Value = Convert.ToInt16(label41.Text);

            /* Pitch. */
            numericUpDown4.Value = Convert.ToInt16(label42.Text);
            numericUpDown5.Value = Convert.ToInt16(label43.Text);
            numericUpDown6.Value = Convert.ToInt16(label44.Text);

            /* Yaw. */
            numericUpDown7.Value = Convert.ToInt16(label45.Text);
            numericUpDown8.Value = Convert.ToInt16(label46.Text);
            numericUpDown9.Value = Convert.ToInt16(label47.Text);

            /* Dynamic PID */
            /* Roll. */
            numericUpDown10.Value = Convert.ToInt16(label48.Text);
            numericUpDown11.Value = Convert.ToInt16(label49.Text);
            numericUpDown12.Value = Convert.ToInt16(label50.Text);

            /* Pitch. */
            numericUpDown13.Value = Convert.ToInt16(label51.Text);
            numericUpDown14.Value = Convert.ToInt16(label52.Text);
            numericUpDown15.Value = Convert.ToInt16(label53.Text);

            /* Yaw. */
            numericUpDown16.Value = Convert.ToInt16(label54.Text);
            numericUpDown17.Value = Convert.ToInt16(label55.Text);
            numericUpDown18.Value = Convert.ToInt16(label56.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /* Add PIDs */

            /* Static PID */
            /* Roll. */
            Port_1.SendData.Cmd = SeriaPortCommunication.SerialCommunication.WriteCommand.Ram;
            Port_1.SendData.Roll.kP.u16 = (UInt16)numericUpDown1.Value;
            Port_1.SendData.Roll.kI.u16 = (UInt16)numericUpDown2.Value;
            Port_1.SendData.Roll.kD.u16 = (UInt16)numericUpDown3.Value;

            /* Pitch. */
            Port_1.SendData.Pitch.kP.u16 = (UInt16)numericUpDown4.Value;
            Port_1.SendData.Pitch.kI.u16 = (UInt16)numericUpDown5.Value;
            Port_1.SendData.Pitch.kD.u16 = (UInt16)numericUpDown6.Value;

            /* Yaw. */
            Port_1.SendData.Yaw.kP.u16 = (UInt16)numericUpDown7.Value;
            Port_1.SendData.Yaw.kI.u16 = (UInt16)numericUpDown8.Value;
            Port_1.SendData.Yaw.kD.u16 = (UInt16)numericUpDown9.Value;

            /* Dynamic PID */
            /* Roll. */
            Port_1.SendData.Roll.kP_dynamic.u16 = (UInt16)numericUpDown10.Value;
            Port_1.SendData.Roll.kI_dynamic.u16 = (UInt16)numericUpDown11.Value;
            Port_1.SendData.Roll.kD_dynamic.u16 = (UInt16)numericUpDown12.Value;

            /* Pitch. */
            Port_1.SendData.Pitch.kP_dynamic.u16 = (UInt16)numericUpDown13.Value;
            Port_1.SendData.Pitch.kI_dynamic.u16 = (UInt16)numericUpDown14.Value;
            Port_1.SendData.Pitch.kD_dynamic.u16 = (UInt16)numericUpDown15.Value;

            /* Yaw. */
            Port_1.SendData.Yaw.kP_dynamic.u16 = (UInt16)numericUpDown16.Value;
            Port_1.SendData.Yaw.kI_dynamic.u16 = (UInt16)numericUpDown17.Value;
            Port_1.SendData.Yaw.kD_dynamic.u16 = (UInt16)numericUpDown18.Value;

            Port_1.WriteDataToBoard();
        }


    }
}
