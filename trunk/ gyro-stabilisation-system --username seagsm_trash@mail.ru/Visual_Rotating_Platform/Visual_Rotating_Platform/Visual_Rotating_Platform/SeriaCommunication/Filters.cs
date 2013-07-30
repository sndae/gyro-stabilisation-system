using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace digFilters
{
    class iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass
    {
        #region Variables and enumerations
            
           
            private float[] y = new float[] {0, 0, 0, 0, 0, 0, 0}; //output samples
            private float[] x = new float[] {0, 0, 0, 0, 0, 0, 0}; //input samples


        #endregion

        #region Properties
            private int NCoef = 6;
            private double [] ACoef = new double[]{
                                                    0.00000227167980216140,
                                                    0.00001363007881296841,
                                                    0.00003407519703242103,
                                                    0.00004543359604322805,
                                                    0.00003407519703242103,
                                                    0.00001363007881296841,
                                                    0.00000227167980216140
                                                  };
            private double [] BCoef = new double[]{
                                                    1.00000000000000000000,
                                                    -5.02943835142160900000,
                                                    10.60704218377968500000,
                                                    -11.99931581621669400000,
                                                    7.67547454820019940000,
                                                    -2.63105512847394830000,
                                                    0.37745238637408868000
                                                  };


        #endregion

            public float Filter(float input)
            {
                int n = 0;

                //shift the old samples
                for (n = NCoef; n > 0; n--)
                {
                    x[n] = x[n - 1];
                    y[n] = y[n - 1];
                }

                //Calculate the new output
                x[0] = input;
                y[0] = (float)ACoef[0] * x[0];
                for (n = 1; n <= NCoef; n++)
                    y[0] += (float)ACoef[n] * x[n] - (float)BCoef[n] * y[n];

                return y[0];
            }
    }

    class iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel
    {
        #region Variables and enumerations


        private float[] y = new float[] { 0, 0, 0, 0, 0, 0, 0 }; //output samples
        private float[] x = new float[] { 0, 0, 0, 0, 0, 0, 0 }; //input samples


        #endregion

        #region Properties
        private int NCoef = 2;
        private double[] ACoef = new double[]{
                                                0.01289887410332486500,
                                                0.02579774820664973100,
                                                0.01289887410332486500
                                             };
        private double[] BCoef = new double[]{
                                                1.00000000000000000000,
                                               -1.59388788360400200000,
                                                0.64558702605023732000
                                             };
        #endregion

        public float Filter(float input)
        {
            int n = 0;

            //shift the old samples
            for (n = NCoef; n > 0; n--)
            {
                x[n] = x[n - 1];
                y[n] = y[n - 1];
            }

            //Calculate the new output
            x[0] = input;
            y[0] = (float)ACoef[0] * x[0];
            for (n = 1; n <= NCoef; n++)
                y[0] += (float)ACoef[n] * x[n] - (float)BCoef[n] * y[n];
            return y[0];
        }
    }


}
