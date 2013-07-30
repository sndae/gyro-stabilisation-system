using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* For StructLayout*/
using System.Runtime.InteropServices;

/* Messages. */
using System.Windows.Forms;

/* Serial port. */
using System.IO.Ports;

namespace SeriaPortCommunication
{
    class SerialCommunication
    {
        #region Variables and enumerations
            public System.IO.Ports.SerialPort serialComPort;
/*
            private digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass filterAx = new digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass();
            private digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass filterAy = new digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass();
            private digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass filterAz = new digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass();
            private digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass filterMx = new digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass();
            private digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass filterMy = new digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass();
            private digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass filterMz = new digFilters.iir_Filters_6_Order_100Hz_Samples_4Hz_LowPass();
*/
            private digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel filterAx = new digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel();
            private digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel filterAy = new digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel();
            private digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel filterAz = new digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel();
            private digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel filterMx = new digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel();
            private digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel filterMy = new digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel();
            private digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel filterMz = new digFilters.iir_Filters_2_Order_100Hz_Samples_4Hz_LowPass_Bessel();

            // Each data should calc new body position and save it to quaternion
            public float[] bodyQuaternion = new float[] { 1.0f, 0.0f, 1.0f, .0f };
            
            // Here we save azimut calculated like  cross(cross (Grav Vector, Magn Vector),Grav Vector), cross is vector multiplication.
            public float[] MagnetAzimut   = new float[] {0, 0, 0 }; 
            // AHRS
            //static AHRS.MadgwickAHRS AHRS = new AHRS.MadgwickAHRS(1f / 256f, 0.1f);
            static AHRS.MahonyAHRS AHRS = new AHRS.MahonyAHRS(1f / 256f, 0.1f);
           
            public UInt32 deltaTime =0;
            private UInt32 OldTime = 0;
            private int firstTimeFlag =0;

            public enum WriteCommand { Ram = 0 , Flash = 1 };

            // Receive round buffer.
            private byte[] round_rx_buf;
        
            // Index of head of round buffer.
            private int round_rx_buf_head;
           
            // Index of tail of round buffer.
            private int round_rx_buf_tail;
           
            // Size of datas stored in round buffer.
            private int round_rx_buf_size;

            // Union structure
        [StructLayout(LayoutKind.Explicit)]
            public struct Union_int32
            {
                //Int 32
                [FieldOffset(0)]
                public Int32 i32;
                [FieldOffset(0)]
                public byte b0;
                [FieldOffset(1)]
                public byte b1;
                [FieldOffset(2)]
                public byte b2;
                [FieldOffset(3)]
                public byte b3;
            };
        [StructLayout(LayoutKind.Explicit)]
            public struct Union_float
            {
                //float
                [FieldOffset(0)]
                public float f32;
                [FieldOffset(0)]
                public byte b0;
                [FieldOffset(1)]
                public byte b1;
                [FieldOffset(2)]
                public byte b2;
                [FieldOffset(3)]
                public byte b3;
            };

        [StructLayout(LayoutKind.Explicit)]
            public struct Union_Uint32
            {
                //UInt 32
                [FieldOffset(0)]
                public UInt32 ui32;
                // array of byte[4]
                [FieldOffset(0)]
                public byte b0;
                [FieldOffset(1)]
                public byte b1;
                [FieldOffset(2)]
                public byte b2;
                [FieldOffset(3)]
                public byte b3;
            };

        [StructLayout(LayoutKind.Explicit)]
            public struct Union_int16
            {
                //Int 16
                [FieldOffset(0)]
                public Int16 i16;
                // array of byte[2]
                [FieldOffset(0)]
                public byte b0;
                [FieldOffset(1)]
                public byte b1;
            };

        [StructLayout(LayoutKind.Explicit)]
            public struct Union_Uint16
            {
                //Uint 16
                [FieldOffset(0)]
                public UInt16 u16;
                // array of byte[2]
                [FieldOffset(0)]
                public byte b0;
                [FieldOffset(1)]
                public byte b1;
            };
        
        
        // Structure for line sensors.
            [StructLayout(LayoutKind.Sequential)]
            public struct Sensor_3D_Vector
            {
                public Union_int16 Axis_X;
                public Union_int16 Axis_Y;
                public Union_int16 Axis_Z;
            };

            // Structure for rotation sensors.
            [StructLayout(LayoutKind.Sequential)]
            public struct Sensor_3D_Rotation
            {
                public Union_int16 Rot_XY;
                public Union_int16 Rot_YZ;
                public Union_int16 Rot_XZ;
            };

            // Structure for PIDs parameters.
            [StructLayout(LayoutKind.Sequential)]
            public struct PIDs
            {
                public Union_Uint16 kP;
                public Union_Uint16 kP_dynamic;
                public Union_Uint16 kI;
                public Union_Uint16 kI_dynamic;
                public Union_Uint16 kD;
                public Union_Uint16 kD_dynamic;
            };   
    


            //Sensor data structure
            //[StructLayout(LayoutKind.Explicit)]
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public class ReceivedData
            {
             //   [FieldOffset(0)]
                public Sensor_3D_Vector     Accelerometer;
             //   [FieldOffset(6)]
                public Sensor_3D_Rotation   Gyro;
             //   [FieldOffset(12)]
                public Sensor_3D_Vector     Magnetometer;
             //   [FieldOffset(18)]
                public Union_float Quaternion_0;
             //   [FieldOffset(22)]
                public Union_float Quaternion_1;
             //   [FieldOffset(26)]
                public Union_float Quaternion_2;
             //   [FieldOffset(30)]
                public Union_float Quaternion_3;
             //   [FieldOffset(34)]
                public Union_float EulerAngle_0;
             //   [FieldOffset(38)]
                public Union_float EulerAngle_1;
             //   [FieldOffset(42)]
                public Union_float EulerAngle_2;
             //   [FieldOffset(46)] 
                public Union_Uint32 ExternalBoardSystemTime;
             //   [FieldOffset(50)]
                public PIDs Pitch;
             //   [FieldOffset(56)]
                public PIDs Roll;
             //   [FieldOffset(62)]
                public PIDs Yaw;
             //   [FieldOffset(68)]
                public Union_Uint16 CRC;
            }


            //Sensor data structure
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            //[StructLayout(LayoutKind.Explicit)]
            public class TxData
            {
                public WriteCommand Cmd;
                public PIDs Pitch;
                public PIDs Roll;
                public PIDs Yaw;
                public Union_Uint16 CRC;
            }

            // Received data variable.
            public ReceivedData currentReceivedData;
            public TxData SendData;



        #endregion

        #region Properties
            // RX round buffer size.
            // TODO: set value like input value. 
            private const int ROUND_BUFFER_SIZE = 128;

        #endregion

        #region Constructors
            public int ComPort  ( 
                                        string                      PortName, 
                                        System.IO.Ports.StopBits    Stop_Bit, 
                                        byte                        DataBits,
                                        int                         BaudRate,
                                        System.IO.Ports.Parity      Parity
                                )
            {
                //Create new serial communication port component.
                serialComPort = new System.IO.Ports.SerialPort();
                // create a Receiver structure.
                currentReceivedData = new ReceivedData();
                SendData = new TxData();
                // Create round  Rx buffer.
                round_rx_buf = new byte[ROUND_BUFFER_SIZE];

                //Set init value of round buffer server variables.
                round_rx_buf_head = 0;
                round_rx_buf_tail = 0;
                round_rx_buf_size = 0;  

                //open com port and connect
                try
                {
                    if (PortName == "")
                    {
                        // "PortName" error.
                        return 1;
                    }
                    // Setup port parameters.
                    serialComPort.StopBits = Stop_Bit;
                    serialComPort.DataBits = DataBits;
                    serialComPort.BaudRate = BaudRate;
                    serialComPort.PortName = PortName;
                    serialComPort.Parity   = Parity.None;
                    //Set DataReceived event function.
                    serialComPort.DataReceived += new SerialDataReceivedEventHandler(PortDataReceived);
                    // Opening port.
                    serialComPort.Open();
                    if (serialComPort.IsOpen)
                    {
                        // Port opened OK.
                        return 0;
                    }
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                    // Some error happend.
                    return 2;
                }
                // Just return :) .
                return 3;
            }

        #endregion

        #region Component events
            /******************************************************************************
            *  Read byte from RX to RX round buffer. 
            ******************************************************************************/
            private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                int BytesToRead;

                BytesToRead = ReadDataFromRxBuffer(); 
 
                // TODO:
                // Here we should found data string in buffer array.
                // First of all lets found start string "DATA".
                // Second, if round_rx_buf_size >= size of "ReceivedData" class,
                // we will try to read data to ReceivedData class, if not,
                // just return round_rx_buf_head back and return till to next time.
                // It means that we did not receive all data packet yet.
                ParsingRxBuffer();

            }
        
        #endregion

        #region Control methods

            // Convertation degrees to radians.
            private float deg2rad(float degrees)
            {
                return (float)(Math.PI / 180) * degrees;
            }

            /* Structure of packet :
             *  0x0B     [0] header 
             *  'W'      [1] header
             *  'R'      [2] header
             *  'T'      [3] header
             *  
             *  [0] LSB DataPacketSize
             *  [1] MSB DataPacketSize
             *  
             * start of data packet:
             *  [0] LSB write-command
             *  [1]
             *  [2]
             *  [3] MSB write-command
             *  Roll
             *  [0] LSB kP
             *  [1] MSB kP
             *  [0] LSB kP dyn
             *  [1] MSB kP dyn
             *  [0] LSB kI
             *  [1] MSB kI
             *  [0] LSB kD
             *  [1] MSB kD
             *  Pitch
             *  [0] LSB kP
             *  [1] MSB kP
             *  [0] LSB kI
             *  [1] MSB kI
             *  [0] LSB kD
             *  [1] MSB kD
             *  Yaw
             *  [0] LSB kP
             *  [1] MSB kP
             *  [0] LSB kI
             *  [1] MSB kI
             *  [0] LSB kD
             *  [1] MSB kD
             *  CRC
             *  [0] LSB CRC
             *  [1] MSB CRC
             */


            public void WriteDataToBoard()
            {   
                int str_size=0; /* Size of data packet. */
                int tmp;
                Union_Uint16 PacketSize= new Union_Uint16();
                UInt16 ByteShift = 6;/* Size of header and "size" fields. */

                SendData.CRC.u16 = CalcTxCRC(SendData);     /* Calculation of control sum of data packet Write_command+PID_Roll+PID_Pitch_PID_Yaw. */
                str_size = Marshal.SizeOf(SendData);        /* Take size of data packet. */
                
                /* Create buffer for transmitting data. */
                byte[] SendBuffer = new byte[str_size + ByteShift]; /* 0x0B + 'W' + 'R' + 'T' + UInt16 sizeof(data_packet) = 6 bytes */
                
                /* Tx header. */
                SendBuffer[0] = 0x0B;
                SendBuffer[1] = (byte)'W';
                SendBuffer[2] = (byte)'R';
                SendBuffer[3] = (byte)'T';
                
                PacketSize.u16 = (UInt16)str_size;
                SendBuffer[4]  = PacketSize.b0; /* write size of data part, LSB. */
                SendBuffer[5]  = PacketSize.b1; /* write size of data part, LSB. */
                tmp = 0;
                while (tmp < str_size)
                {
                    SendBuffer[tmp + ByteShift] = Marshal.ReadByte(SendData, tmp);
                    tmp++;
                }
                serialComPort.Write(SendBuffer, 0, str_size + ByteShift);
            }

            /* Function calculate Magnet azimut vector it body coordinate. */
            public void MagnetAzimutCalculation(float Ax, float Ay, float Az, float Mx, float My, float Mz)
            {  
                float norm;

                float[] A = new float[] { 0, 0, 0 };
                float[] B = new float[] { 0, 0, 0 };
                float[] C = new float[] { 0, 0, 0 };
                // Correction matrix, it calculated by http://sailboatinstruments.blogspot.cz/2011/08/improved-magnetometer-calibration.html
                // MagCal
                float[] E = new float[] { 
                                             6.1223f, -0.0505f, -0.0721f,         
                                            -0.0505f,  6.1163f, -0.1209f,         
                                            -0.0721f, -0.1209f,  5.6381f 
                                        };

                A[0] = Ax;
                A[1] = Ay;
                A[2] = Az;
                // calculate correction by muctiplication matrix E and vector M
                B[0] = E[0] * Mx + E[1] * My + E[2] * Mz;
                B[1] = E[3] * Mx + E[4] * My + E[5] * Mz;
                B[2] = E[6] * Mx + E[7] * My + E[8] * Mz;

               // B[0] = Mx;
               // B[1] = My;
               // B[2] = Mz;

                /*TODO:
                 * We need carefully check is it neccessary to calculate proection at all or not.
                 * */
                // normalising accelerometer vector
                norm =(float)Math.Sqrt(A[0] * A[0] + A[1] * A[1] + A[2] * A[2]);
                A[0] = A[0]/norm;
                A[1] = A[1]/norm;
                A[2] = A[2]/norm;
                // normalising magnetometer vector
                norm =(float)Math.Sqrt(B[0] * B[0] + B[1] * B[1] + B[2] * B[2]);
                B[0] = B[0]/norm;
                B[1] = B[1]/norm;
                B[2] = B[2]/norm;

                // Calculate cross of Accel vector and Magnet vector.
                // Result is normal vector to AccelMagnet plane.
                C[0] = A[1] * B[2] - B[1] * A[2];
                C[1] = A[2] * B[0] - B[2] * A[0];
                C[2] = A[0] * B[1] - B[0] * A[1];

                // Calculate cross of Accel vector and Magnet vectornormal vector to AccelMagnet plane.
                // Result is Magnet azimut vector in rotated coordinats.

                MagnetAzimut[0] = C[1] * A[2] - A[1] * C[2];
                MagnetAzimut[1] = C[2] * A[0] - A[2] * C[0];
                MagnetAzimut[2] = C[0] * A[1] - A[0] * C[1];
            }

            //Read data from RX buffer of serial port
            private int ParsingRxBuffer()
            {   
                int ReadByte = 0;
                int temp_size,temp_head;
                UInt16 tmp;
                while (ReadByte >= 0)
                {
                    temp_size = round_rx_buf_size;
                    temp_head = round_rx_buf_head;
                    ReadByte = ReadByteFromRXBuff();

                    if (ReadByte == 'D')
                    {
                        ReadByte = ReadByteFromRXBuff();
                        if (ReadByte == 'A')
                        {
                            ReadByte = ReadByteFromRXBuff();
                            if (ReadByte == 'T')
                            {
                                ReadByte = ReadByteFromRXBuff();
                                if (ReadByte == 'A')
                                {
                                    int size = Marshal.SizeOf(currentReceivedData);
                                    if (round_rx_buf_size < (size + 2))
                                    {
                                        // Return size and pointer of received buffer back and return.
                                        round_rx_buf_size = temp_size;
                                        round_rx_buf_head = temp_head;
                                        return (2);
                                    }
                                    else
                                    {
                                        //TODO: Lets here to read sizeof byte and set it to "currentReceivedData"

                                        // Get data to currentReceivedData
                                        //Accelerometer
                                        currentReceivedData.Accelerometer.Axis_X.b0 =(byte)ReadByteFromRXBuff();
                                        currentReceivedData.Accelerometer.Axis_X.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Accelerometer.Axis_Y.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Accelerometer.Axis_Y.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Accelerometer.Axis_Z.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Accelerometer.Axis_Z.b1 = (byte)ReadByteFromRXBuff();
                                        //Gyro
                                        currentReceivedData.Gyro.Rot_XY.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Gyro.Rot_XY.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Gyro.Rot_YZ.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Gyro.Rot_YZ.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Gyro.Rot_XZ.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Gyro.Rot_XZ.b1 = (byte)ReadByteFromRXBuff();
                                        //Magnetometer
                                        currentReceivedData.Magnetometer.Axis_X.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Magnetometer.Axis_X.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Magnetometer.Axis_Y.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Magnetometer.Axis_Y.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Magnetometer.Axis_Z.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Magnetometer.Axis_Z.b1 = (byte)ReadByteFromRXBuff();
                                        
                                        //read quaternion
                                        currentReceivedData.Quaternion_0.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_0.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_0.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_0.b3 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Quaternion_1.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_1.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_1.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_1.b3 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Quaternion_2.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_2.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_2.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_2.b3 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Quaternion_3.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_3.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_3.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Quaternion_3.b3 = (byte)ReadByteFromRXBuff();


                                        currentReceivedData.EulerAngle_0.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_0.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_0.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_0.b3 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.EulerAngle_1.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_1.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_1.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_1.b3 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.EulerAngle_2.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_2.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_2.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.EulerAngle_2.b3 = (byte)ReadByteFromRXBuff();


                                        /* System time */
                                        currentReceivedData.ExternalBoardSystemTime.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.ExternalBoardSystemTime.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.ExternalBoardSystemTime.b2 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.ExternalBoardSystemTime.b3 = (byte)ReadByteFromRXBuff();

                                        /* Pitch PID */
                                        currentReceivedData.Pitch.kP.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kP.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kP_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kP_dynamic.b1 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Pitch.kI.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kI.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kI_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kI_dynamic.b1 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Pitch.kD.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kD.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kD_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Pitch.kD_dynamic.b1 = (byte)ReadByteFromRXBuff();

                                        /* Roll PID */
                                        currentReceivedData.Roll.kP.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kP.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kP_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kP_dynamic.b1 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Roll.kI.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kI.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kI_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kI_dynamic.b1 = (byte)ReadByteFromRXBuff();
                                        
                                        currentReceivedData.Roll.kD.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kD.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kD_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Roll.kD_dynamic.b1 = (byte)ReadByteFromRXBuff();

                                        /* Yaw PID */
                                        currentReceivedData.Yaw.kP.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kP.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kP_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kP_dynamic.b1 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Yaw.kI.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kI.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kI_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kI_dynamic.b1 = (byte)ReadByteFromRXBuff();

                                        currentReceivedData.Yaw.kD.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kD.b1 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kD_dynamic.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.Yaw.kD_dynamic.b1 = (byte)ReadByteFromRXBuff();
                                        
                                        /* CRC */
                                        currentReceivedData.CRC.b0 = (byte)ReadByteFromRXBuff();
                                        currentReceivedData.CRC.b1 = (byte)ReadByteFromRXBuff();
                                        tmp = CalcRxCRC(currentReceivedData);
                                        if (tmp != currentReceivedData.CRC.u16)
                                        {
                                            return 0;
                                        }
                                        else
                                        {   
                                            /* Calc delta time from last data packet. */
                                            deltaTime = currentReceivedData.ExternalBoardSystemTime.ui32 - OldTime;
                                            // Save current packet time.
                                            OldTime = currentReceivedData.ExternalBoardSystemTime.ui32;
                                            if (firstTimeFlag != 0)
                                            {
                                                float Gx, Gy, Gz;
                                                float Ax, Ay, Az;
                                                
                                                float Mx, My, Mz;

                                                float GyroCoef = 0.00875f;
                                                //float GyroCoef = 0.01750f;
                                                //float GyroCoef = 0.280f;//0.07 * 4 
                                                //float GyroCoef = 0.070f; //angular speed value of one bit  in degree/sec.
                                                Gx = -((float)(currentReceivedData.Gyro.Rot_XY.i16 - 0) * GyroCoef);
                                                Gy = -((float)(currentReceivedData.Gyro.Rot_XZ.i16) * GyroCoef);
                                                Gz = (float)(currentReceivedData.Gyro.Rot_YZ.i16) * GyroCoef;
                                                Ax = -(float)(currentReceivedData.Accelerometer.Axis_Y.i16);
                                                Ay = -(float)(currentReceivedData.Accelerometer.Axis_X.i16);
                                                Az = (float)(currentReceivedData.Accelerometer.Axis_Z.i16);
                                                Mx = (float)(currentReceivedData.Magnetometer.Axis_X.i16 - 104);
                                                My = (float)(currentReceivedData.Magnetometer.Axis_Y.i16 - 41);
                                                Mz = -(float)(currentReceivedData.Magnetometer.Axis_Z.i16 - 350);

                                                //This function calc proection of real magnet vector to 
                                                // plane normaled to gravity vector. So,
                                                // I hope, the body frame will be corrected to horizontal position.
                                                MagnetAzimutCalculation(Ax, Ay, Az, Mx, My, Mz);
                                                Mx = MagnetAzimut[0];
                                                My = MagnetAzimut[1];
                                                Mz = MagnetAzimut[2];

                                               // Mx = filterMx.Filter(Mx);
                                               // My = filterMy.Filter(My);
                                               // Mz = filterMz.Filter(Mz);
                                               // Ax = filterAx.Filter(Ax);
                                               // Ay = filterAy.Filter(Ay);
                                               // Az = filterAz.Filter(Az);
                                                
                                                AHRS.SamplePeriod = deltaTime / 1000f;
                                                //AHRS.Beta = 0.1f;
                                                AHRS.Kp   = 5.1f;
                                                AHRS.Update(
                                                                    deg2rad(Gx),
                                                                    deg2rad(Gy),
                                                                    deg2rad(Gz),
                                                                    Ax,
                                                                    Ay,
                                                                    Az,
                                                                    Mx,
                                                                    My,
                                                                    Mz
                                                            );
                                                    bodyQuaternion = AHRS.Quaternion;
                                            }
                                            firstTimeFlag = 1;
                                
                                        }
                                        // Call visualisation function and send data to main Form.
                                    }
                                }
                            }
                        }
                    }//if(ReadByte == 'D')
                }

                return 1;
            }

            private UInt16 CalcRxCRC(ReceivedData RData)
            {
                const UInt16 polinom = 0xa001;
                UInt16 code = 0xffff;
                byte ml;
                byte tmp;
                
                //amount of byte in ReceivedData structure.
                //After size of ReceivedData reached 70, Marshal.SizeOf() return 72,
                //so,just work around is change 2 to 4.
                int length = Marshal.SizeOf(RData) -2;// 2 is sizeof CRC in ReceivedData
                for (int i = 0; i < length; i++)
                {
                    //For each byte from array

                    /* Put LSB of 16 bit code(in the future a СRС16) to ml. */
                    ml = (byte)(code);

                    /* Do m1 XOR msg[i] and put result to m1. */
                    tmp = Marshal.ReadByte(RData, i);
                    ml ^= tmp;

                    /* Set LSB of code to zero. */
                    code &= 0xff00;
                    code += ml;
                    for (int j = 0; j < 8; j++)
                    {
                        /* Check LSB bit of code. */
                        if ((code & 0x0001) == 1)
                        {   /* LSB bit is 1. */
                            code >>= 1;
                            /* Do code XOR polinom and put result to code. */
                            code ^= polinom;
                        }
                        else
                        {   /* LSB bit is 0*/
                            code >>= 1;
                        }
                    }
                }
                return code;
            }

            private UInt16 CalcTxCRC(TxData RData)
            {
                const UInt16 polinom = 0xa001;
                UInt16 code = 0xffff;
                byte ml;
                byte tmp;

                //amount of byte in ReceivedData structure.
                //After size of ReceivedData reached 70, Marshal.SizeOf() return 72,
                //so,just work around is change 2 to 4.
                int length = Marshal.SizeOf(RData) - 2;// 2 is sizeof CRC in ReceivedData
                for (int i = 0; i < length; i++)
                {
                    //For each byte from array

                    /* Put LSB of 16 bit code(in the future a СRС16) to ml. */
                    ml = (byte)(code);

                    /* Do m1 XOR msg[i] and put result to m1. */
                    tmp = Marshal.ReadByte(RData, i);
                    ml ^= tmp;

                    /* Set LSB of code to zero. */
                    code &= 0xff00;
                    code += ml;
                    for (int j = 0; j < 8; j++)
                    {
                        /* Check LSB bit of code. */
                        if ((code & 0x0001) == 1)
                        {   /* LSB bit is 1. */
                            code >>= 1;
                            /* Do code XOR polinom and put result to code. */
                            code ^= polinom;
                        }
                        else
                        {   /* LSB bit is 0*/
                            code >>= 1;
                        }
                    }
                }
                return code;
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

            //Read data from RX buffer of serial port
            private int ReadDataFromRxBuffer()
            {
                int BytesToRead;
                
                //Check how match bytes in buffer
                BytesToRead = serialComPort.BytesToRead;  
                // Read size of bytes from Serial port buffer.
                for (int count = 0; count < BytesToRead; count++)
                {
                    // This version of buffer read buffer size and drop all another bytes
                    // before it will be read.
                    if (round_rx_buf_size < ROUND_BUFFER_SIZE)
                    {
                        /* Put byte to the end of round buffer */
                        round_rx_buf[round_rx_buf_tail] = (Byte)serialComPort.ReadByte();
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







        #endregion

    }
}
