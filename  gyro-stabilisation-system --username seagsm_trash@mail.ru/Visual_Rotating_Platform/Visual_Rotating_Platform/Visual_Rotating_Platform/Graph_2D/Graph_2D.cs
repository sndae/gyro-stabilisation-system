using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/* It is for 2D graph. */
using ZedGraph;

namespace Visual_Rotating_Platform
{
    public partial class Graph_2D : Form
    {
        #region Variables and enumerations
  /*
        /// <summary>
        /// Form update timer.
        /// </summary>
        private Timer formUpdateTimer;

        /// <summary>
        /// Array of image file paths.
        /// </summary>
        private string[] imageFiles;

        /// <summary>
        /// Array of textures
        /// </summary>
        private uint[] textures;

        /// <summary>
        /// Dimensions of cuboid.
        /// </summary>
        private float halfXdimension, halfYdimension, halfZdimension;

        /// <summary>
        /// Transformation matrix describing translation and orientation of cuboid.
        /// </summary>
        private float[] transformationMatrix;

        /// <summary>
        /// Camera views of the cuboid.
        /// </summary>
        public enum CameraViews
        {
            Right,
            Left,
            Back,
            Front,
            Top,
            Bottom
        };
*/
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the form should minimise when closed by the user.
        /// </summary>
        public bool MinimizeInsteadOfClose { get; set; }
 /*
        /// <summary>
        /// Gets or sets a value indicating whether the form should minimise when closed by the user.
        /// </summary>
        public bool MinimizeInsteadOfClose { get; set; }

        /// <summary>
        /// Gets or sets a value describing the camera view of the cuboid. See Form_3D.CameraViews.
        /// </summary>
        public CameraViews CameraView { get; set; }

        /// <summary>
        /// Gets or sets the distance of the camera from the world origin.
        /// </summary>
        public float CameraDistance { get; set; }

        /// <summary>
        /// Gets or sets the translation vector describing the position of the cuboid relative to world origin.
        /// </summary>
        public float[] TranslationVector
        {
            get
            {
                return new float[] { transformationMatrix[12],
                                     transformationMatrix[13],
                                     transformationMatrix[14] };
            }
            set
            {
                if (value.Length != 3) throw new Exception("Array must be of length 3.");
                transformationMatrix[12] = value[0];
                transformationMatrix[13] = value[1];
                transformationMatrix[14] = value[2];
            }
        }

        /// <summary>
        /// Gets or sets the rotation matrix describing the orientation of the cuboid relative to world.
        /// </summary>
        /// <remarks>
        /// Index order is row major. See http://en.wikipedia.org/wiki/Row-major_order
        /// </remarks> 
        public float[] RotationMatrix
        {
            get
            {
                return new float[] {transformationMatrix[0], transformationMatrix[4], transformationMatrix[8],
                                    transformationMatrix[1], transformationMatrix[5], transformationMatrix[9],
                                    transformationMatrix[2], transformationMatrix[6], transformationMatrix[10]};
            }
            set
            {
                if (value.Length != 9) throw new Exception("Array must be of length 9.");
                transformationMatrix[0] = value[0]; transformationMatrix[4] = value[1]; transformationMatrix[8] = value[2];
                transformationMatrix[1] = value[3]; transformationMatrix[5] = value[4]; transformationMatrix[9] = value[5];
                transformationMatrix[2] = value[6]; transformationMatrix[6] = value[7]; transformationMatrix[10] = value[8];
            }
        }
*/
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Graph_2D"/> class.
        /// </summary>
        public Graph_2D()
            : this(new string[]{"line"},new Color[]{Color.Red},"MonoLine","X-axis","Y-axis")
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Graph_2D"/> class.
        /// </summary>
        /// <param name="zG_Curve_Name">
        /// Array of Name of Curves.Like {"Curve_N1","Curve_N2","Curve_N3",..."Curve_Nn"} .
        /// </param>
        /// <param name="zG_Curve_Color">
        /// Color of each curves.
        /// </param>
        /// <param name="zG_Title">
        /// Title of zGraphControl.
        /// </param>
        /// <param name="zG_X_value_Title">
        /// Title of X axis.
        /// </param>
        /// <param name="zG_Y_value_Title">
        /// Title of Y axis.
        /// </param>
        public Graph_2D(
                            string[] zG_Curve_Name,
                            Color[] zG_Curve_Color,
                            string zG_Title,
                            string zG_X_value_Title,
                            string zG_Y_value_Title
                       )
        {
            InitializeComponent();
            /* Init zGraphControll */
            CreateGraph(
                            zedGraph, 
                            zG_Curve_Name,
                            zG_Curve_Color,
                            zG_Title,
                            zG_X_value_Title,
                            zG_Y_value_Title
                       );
        }

        #endregion

        #region Form events
        /// <summary>
        /// Form closing event to minimise form instead of close.
        /// </summary>
        private void Graph_2D_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MinimizeInsteadOfClose)
            {
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

/*
        /// <summary>
        /// Form visible changed event to start/stop form update formUpdateTimer.
        /// </summary>
        private void Form_3D_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                formUpdateTimer.Start();
            }
            else
            {
                formUpdateTimer.Stop();
            }
        }

        /// <summary>
        /// Form closing event to minimise form instead of close.
        /// </summary>
        private void Form_3D_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MinimizeInsteadOfClose)
            {
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Timer tick event to refresh graphics.
        /// </summary>
        private void formUpdateTimer_Tick(object sender, EventArgs e)
        {
            lock (this)
            {
                simpleOpenGlControl1.Refresh();
            }
        }
*/
        #endregion

        #region zGraphControl methods

        /// <summary>
        /// Create zGraph this input parameters.
        /// </summary>
        /// <param name="zGraph">
        /// ZedGraphControl object.
        /// </param>
        /// <param name="zG_Curve_Name">
        /// Array of Name of Curves.Like {"Curve_N1","Curve_N2","Curve_N3",..."Curve_Nn"} .
        /// </param>
        /// <param name="zG_Curve_Color">
        /// Color of each curves.
        /// </param>
        /// <param name="zG_Title">
        /// Title of zGraphControl.
        /// </param>
        /// <param name="zG_X_value_Title">
        /// Title of X axis.
        /// </param>
        /// <param name="zG_Y_value_Title">
        /// Title of Y axis.
        /// </param> 
        private void CreateGraph(ZedGraphControl zGraph, 
                                        string[] zG_Curve_Name,
                                         Color[] zG_Curve_Color,
                                          string zG_Title,
                                          string zG_X_value_Title,
                                          string zG_Y_value_Title
                                )
        {

            /* Create place on zGraph wthere we will add curves. */

            GraphPane myPane = zGraph.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text       = zG_Title;// "My Test Graph";
            myPane.XAxis.Title.Text = zG_X_value_Title;// "X Value";
           myPane.YAxis.Title.Text = zG_Y_value_Title;//"My Y Axis";

            /* Calculate how match curve name we have */
            int numOf_zg_Curve_Name = zG_Curve_Name.Length;
            /* Create same amount of List to add it to ZGraph */
            PointPairList[] zG_List_Of_Curve = new PointPairList[numOf_zg_Curve_Name];
            /* Here we add one by one curves till to end of name index "numOf_zg_Curve_Name". */
            for (int curve_index = 0; curve_index < numOf_zg_Curve_Name; curve_index++)
            {
                myPane.AddCurve(    
                                zG_Curve_Name[curve_index], 
                                zG_List_Of_Curve[curve_index],
                                zG_Curve_Color[curve_index], 
                                SymbolType.None
                               );  
            }

            /* Apply changes to zGraphControl component. */
            zGraph.AxisChange();
         }

        /// <summary>
        /// Add point to curve.
        /// </summary>
        /// <param name="double zG_X_Value">
        /// New value of curve for X axis.
        /// </param>
        /// <param name="zG_Y_Value">
        /// New value of curve for Y axis.
        /// </param>
        /// <param name="zG_Curve_Number">
        /// Number of curve from zGraph.
        /// </param>
        /// <param name="zG_Scale_Min">
        /// Shift to left from current value of X axis.
        /// </param>
        /// <param name="zG_Scale_Max">
        /// Shift to right from current value of X axis.
        public void DrawGraph(
                                double zG_X_Value, 
                                double zG_Y_Value, 
                                int zG_Curve_Number,
                                double zG_Scale_Min,
                                double zG_Scale_Max,
                                int    zG_MaxAmountOfPoint
                             )
        {
            PointPair new_point = new PointPair();
            /* Set value of new point to input value zG_X_Value and zG_Y_Value. */
            new_point.X = zG_X_Value;
            new_point.Y = zG_Y_Value;

            int index_p;

            /* Add new point to curve. */
            if (zedGraph.GraphPane != null)
            {
                zedGraph.GraphPane.CurveList[zG_Curve_Number].AddPoint(new_point);
                /* Read how match  points have been added. */
                index_p = zedGraph.GraphPane.CurveList[zG_Curve_Number].NPts;
                /*
                 * If we add more points to list that value of zG_MaxAmountOfPoint
                 * we should remove first one point.
                 */
                if (index_p > zG_MaxAmountOfPoint)
                {
                    zedGraph.GraphPane.CurveList[zG_Curve_Number].RemovePoint(0);
                }
                /* Set min and max for  axis X. */
                zedGraph.GraphPane.XAxis.Scale.Min = zG_X_Value - zG_Scale_Min;
                zedGraph.GraphPane.XAxis.Scale.Max = zG_X_Value + zG_Scale_Max;
            }
            /* Redraw graph. */
            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }
       
        #endregion




        private void Graph_2D_Load(object sender, EventArgs e)
        {
            FormSetup.SFP my_setup = new FormSetup.SFP();
            FormSetup.SFP.FormParameters sfp = new FormSetup.SFP.FormParameters();
            /* Set setup file name. */
            my_setup.SetupFileFileName = "setup.xml";
            /* Read setup file. */
            sfp = my_setup.ReadSetupFile(this);
            if (sfp != null)
            {
                this.Left   = sfp.FormPosition_Left;
                this.Top    = sfp.FormPosition_Top;
                this.Height = sfp.FormHeight;
                this.Width  = sfp.FormWidth;
            }
        }

        private void Graph_2D_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormSetup.SFP my_setup = new FormSetup.SFP();
            my_setup.SetupFileFileName = "setup.xml";
            my_setup.WriteSetupFile(this);
        }
    }
}
