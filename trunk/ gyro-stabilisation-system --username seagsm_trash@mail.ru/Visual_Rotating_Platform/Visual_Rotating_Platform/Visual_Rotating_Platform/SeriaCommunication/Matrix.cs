using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matrix
{
    class rotMatrix
    {
        #region Variables and enumerations

            public float X;
        
            // output vector 1x3
            public float[] A1x3 = new float[]   { 
                                                    0,
                                                    0,
                                                    0
                                                };
            // output matrix 3x3
            public float[] A3x3 = new float[]   {
                                                    0, 0, 0, 
                                                    0, 0, 0, 
                                                    0, 0, 0
                                                }; //3x3 matrix
            // output matrix 4x4
            public float[] A4x4 = new float[]   {    
                                                    0, 0, 0, 0, 
                                                    0, 0, 0, 0, 
                                                    0, 0, 0, 0,
                                                    0, 0, 0, 0
                                                }; //4x4 matrix
        #endregion

        #region Properties

        #endregion



            public void Scalar(float[] A, float[] B)
            {
                if (A.Length == B.Length)
                {
                    if (A.Length == 3)
                    {
                        X = A[0] * B[0] + A[1] * B[1] + A[2] * B[2];
                    }
                }
                else
                {
                    throw new Exception("Wrong vectors size.");
                }

            }
            public void ScalarMultiplication(float A, float[] B)
            {
                if (B.Length == 3)
                {
                    A1x3[0] = A * B[0];
                    A1x3[1] = A * B[1];
                    A1x3[2] = A * B[2];
                }
                else
                {
                    throw new Exception("Wrong vectors size.");
                }

            }
          public void Cross(float[] A, float[] B)
            {
                if (A.Length == B.Length)
                {
                    if (A.Length == 3)
                    {
                        A1x3[0] = A[2] * B[3] - A[3] * B[2];
                        A1x3[1] = A[3] * B[1] - A[1] * B[3];
                        A1x3[2] = A[1] * B[2] - A[2] * B[1];
                    }
                }
                else
                {
                    throw new Exception("Wrong vectors size.");
                }

            }
            public void MatrixMultiplication(float[] A, float[] B)
            {
                if (A.Length == B.Length)
                {
                    if (A.Length == 9)
                    {
                        A3x3[0] = A[0] * B[0] + A[1] * B[3] + A[2] * B[6];
                        A3x3[3] = A[3] * B[0] + A[4] * B[3] + A[5] * B[6];
                        A3x3[6] = A[6] * B[0] + A[7] * B[3] + A[8] * B[6];

                        A3x3[1] = A[0] * B[1] + A[1] * B[4] + A[2] * B[7];
                        A3x3[4] = A[3] * B[1] + A[4] * B[4] + A[5] * B[7];
                        A3x3[7] = A[6] * B[1] + A[7] * B[4] + A[8] * B[7];

                        A3x3[2] = A[0] * B[2] + A[1] * B[5] + A[2] * B[8];
                        A3x3[5] = A[3] * B[2] + A[4] * B[5] + A[5] * B[8];
                        A3x3[8] = A[6] * B[2] + A[7] * B[5] + A[8] * B[8];
                    }
                    else if (A.Length == 16)
                    {
                        A4x4[0] = A[0] * B[0] + A[1] * B[4] + A[2] * B[8] + A[3] * B[12];
                        A4x4[4] = A[4] * B[0] + A[5] * B[4] + A[6] * B[8] + A[7] * B[12];
                        A4x4[8] = A[8] * B[0] + A[9] * B[4] + A[10] * B[8] + A[11] * B[12];
                        A4x4[12] = A[12] * B[0] + A[13] * B[4] + A[14] * B[8] + A[15] * B[12];

                        A4x4[1] = A[0] * B[1] + A[1] * B[5] + A[2] * B[9] + A[3] * B[13];
                        A4x4[5] = A[4] * B[1] + A[5] * B[5] + A[6] * B[9] + A[7] * B[13];
                        A4x4[9] = A[8] * B[1] + A[9] * B[5] + A[10] * B[9] + A[11] * B[13];
                        A4x4[13] = A[12] * B[1] + A[13] * B[5] + A[14] * B[9] + A[15] * B[13];

                        A4x4[2] = A[0] * B[2] + A[1] * B[6] + A[2] * B[10] + A[3] * B[14];
                        A4x4[6] = A[4] * B[2] + A[5] * B[6] + A[6] * B[10] + A[7] * B[14];
                        A4x4[10] = A[8] * B[2] + A[9] * B[6] + A[10] * B[10] + A[11] * B[14];
                        A4x4[14] = A[12] * B[2] + A[13] * B[6] + A[14] * B[10] + A[15] * B[14];

                        A4x4[3] = A[0] * B[3] + A[1] * B[7] + A[2] * B[11] + A[3] * B[15];
                        A4x4[7] = A[4] * B[3] + A[5] * B[7] + A[6] * B[11] + A[7] * B[15];
                        A4x4[11] = A[8] * B[3] + A[9] * B[7] + A[10] * B[11] + A[11] * B[15];
                        A4x4[15] = A[12] * B[3] + A[13] * B[7] + A[14] * B[11] + A[15] * B[15];
                    }
                    else
                    {
                        throw new Exception("Wrong matrix size.");
                    }
                }
                else
                {
                    throw new Exception("Different matrix size.");
                }


            }













    }
}
