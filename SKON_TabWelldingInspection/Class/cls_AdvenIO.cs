using Automation.BDaq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKON_TabWelldingInspection
{
    public class AdvanDevice
    {
        public static InstantDiCtrl Device_Di = new InstantDiCtrl();
        public static InstantDoCtrl Device_Do = new InstantDoCtrl();
        public static int DeviceNumber = 0;
        public static string DeviceName;
        public static bool Initialized = false;
        //public static bool[] bitArr_0 = new bool[8];
        //public static bool[] bitArr_1 = new bool[8];
        //public static bool[] bitArrOutGet_0 = new bool[8];
        //public static bool[] bitArrOutGet_1 = new bool[8];
        public static bool[] bitArrOut_0 = new bool[8];
        public static bool[] bitArrOut_1 = new bool[8];
        //private static Thread StartThread;
        private static Thread OutputThread;

        private static string strOutput_0 = "";
        private static string strOutput_0_old = "";
        private static string strOutput_1 = "";
        private static string strOutput_1_old = "";

        private static bool OutputThreadStop = true;

        public static bool Init(int deviceNo)
        {
            try
            {
                DeviceNumber = deviceNo;
                Device_Di.SelectedDevice = new DeviceInformation(DeviceNumber);
                Device_Do.SelectedDevice = new DeviceInformation(DeviceNumber);
                DeviceName = Device_Di.SelectedDevice.Description;
                if (Device_Di.Initialized && Device_Do.Initialized)
                {

                    OutputThread = new Thread(new ThreadStart(OutputThreadProc));
                    OutputThread.IsBackground = true;
                    if (!OutputThread.IsAlive)
                    {
                        OutputThread.Start();
                    }
                    Initialized = true;

                }
            }
            catch
            {
                Initialized = false;
            }
            return Initialized;
        }

        public static void GET32_IO(out bool[] bitArr_0, out bool[] bitArr_1, out bool[] bitArrOutGet_0, out bool[] bitArrOutGet_1)
        {

            //while (true)
            //{
            //    Thread.Sleep(1);
            bitArr_0 = new bool[8];
            bitArr_1 = new bool[8];
            bitArrOutGet_0 = new bool[8];
            bitArrOutGet_1 = new bool[8];
            if (Initialized)
            {

                byte portData_in = 0;
                byte portData_Out = 0;

                for (int i = 0; 2 > i; ++i)
                {
                    if (Read(i, out portData_in, out portData_Out))
                    {
                        if (i == 0)
                        {
                            for (int j = 0; j < 8; ++j)
                            {
                                int temp = ((portData_in >> j) & 0x1);
                                if (temp == 1)
                                {
                                    bitArr_0[j] = true;
                                }
                                else
                                {
                                    bitArr_0[j] = false;
                                }
                                temp = ((portData_Out >> j) & 0x1);
                                if (temp == 1)
                                {
                                    bitArrOutGet_0[j] = true;
                                }
                                else
                                {
                                    bitArrOutGet_0[j] = false;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 8; ++j)
                            {
                                if (((portData_in >> j) & 0x1) == 1)
                                {
                                    bitArr_1[j] = true;
                                }
                                else
                                {
                                    bitArr_1[j] = false;
                                }
                                if (((portData_Out >> j) & 0x1) == 1)
                                {
                                    bitArrOutGet_1[j] = true;
                                }
                                else
                                {
                                    bitArrOutGet_1[j] = false;
                                }
                            }
                        }
                    }

                }
            }
            //}
        }

        private static void OutputThreadProc()
        {
            while (OutputThreadStop)
            {
                if (Initialized)
                {
                    if (strOutput_0 != strOutput_0_old)
                    {

                        if (Write(0, Convert.ToByte(strOutput_0, 2)))
                        {
                            strOutput_0_old = strOutput_0;
                        }
                    }
                    if (strOutput_1 != strOutput_1_old)
                    {

                        if (Write(1, Convert.ToByte(strOutput_1, 2)))
                        {
                            strOutput_1_old = strOutput_1;
                        }

                    }

                }
                Thread.Sleep(10);

            }
        }

        static void Output_ThreadProc(int output, bool data, int delaytime, int Pulsetime, int portNum)
        {

            if (delaytime != 0)
            {
                Thread.Sleep(delaytime);
            }
            if (portNum == 0)
            {
                bitArrOut_0[output] = data;
                strOutput_0 = Convert_BoolArrTostring(bitArrOut_0);

                if (Pulsetime != 0)
                {
                    Thread.Sleep(Pulsetime);
                    bitArrOut_0[output] = false;
                    strOutput_0 = Convert_BoolArrTostring(bitArrOut_0);
                }
            }
            else
            {
                bitArrOut_1[output] = data;
                strOutput_1 = Convert_BoolArrTostring(bitArrOut_1);

                if (Pulsetime != 0)
                {
                    Thread.Sleep(Pulsetime);
                    bitArrOut_1[output] = false;
                    strOutput_1 = Convert_BoolArrTostring(bitArrOut_1);
                }
            }
        }

        public static void OutputStart(int output, bool data, int delaytime, int Pulsetime, int portNum)
        {
            if (output == -1) { return; }  // pin 번호가 -1이 입력되면 배선이 안되어 있는것이라서 리턴함. 2024-06-18
            Thread th = new Thread(() => Output_ThreadProc(output, data, delaytime, Pulsetime, portNum));
            th.IsBackground = true;
            th.Start();
        }

        private static string Convert_BoolArrTostring(bool[] arr)
        {
            string temp = "";
            for (int i = 7; -1 < i; i--)
            {
                temp += arr[i] ? "1" : "0";
            }
            return temp;
        }

        static bool Read(int portNum, out byte indata, out byte outdata)
        {
            bool flag = true;
            ErrorCode err = Device_Di.Read(portNum, out indata);
            if (err != ErrorCode.Success)

                flag = false;
            err = Device_Do.Read(portNum, out outdata);
            if (err != ErrorCode.Success)

                flag = false;
            return flag;
        }

        static bool Write(int portNum, byte data)
        {
            ErrorCode err = Device_Do.Write(portNum, data);
            if (err == ErrorCode.Success)
                return true;
            else
                return false;
        }

        public static void StopOutputThread()
        {
            if (OutputThread != null && OutputThread.IsAlive)
            {
                OutputThreadStop = false;

                // 스레드 종료 대기
                OutputThread.Join();
                OutputThread = null;
            }
        }

        static void HandleError(ErrorCode err)
        {
            if (err != ErrorCode.Success)
            {
                //MessageBox.Show("Sorry ! Some errors happened, the error code is: " + err.ToString(), "Static DI");
            }
        }
    }
}
