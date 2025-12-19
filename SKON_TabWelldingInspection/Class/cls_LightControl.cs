using System;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_LightControl
    {
        private cls_Ini ini;
        private System.IO.Ports.SerialPort lightControl;
        private const int CAM_CATHODE = 0;
        private const int CAM_ANODE = 1;
        private string mLightControllerPort;
        private string mLightControllerMaker;   
        private string mCathode_Light;
        private string mAnode_Light;

        public cls_LightControl(string iniFilename)
        {
            ini = new cls_Ini(iniFilename);

            // INI 파일에서 포트 설정을 로드
            LoadIni();
        }

        public void LoadIni()
        {
            mLightControllerPort = ini.ReadIniString("Camera", "LightControllerPort", "COM1");
            mLightControllerMaker = ini.ReadIniString("Camera", "LightControllerMaker", "LFINE");
            mCathode_Light = ini.ReadIniString("CameraSetup", "Cathode_Light", "360");
            mAnode_Light = ini.ReadIniString("CameraSetup", "Anode_Light", "480");
        }

        public void SaveIniValue(string light, string value)
        {
            ini.WriteIniValue("CameraSetup", light, value);
        }

        public bool IsConnected()
        {
            try
            {
                if (lightControl.IsOpen == true)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        lightControl.Open();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetLightValue(string lightStr, int camIdx)
        {
            // 조명 컨트롤러의 설정값을 변경한다.
            lightControl = new System.IO.Ports.SerialPort(mLightControllerPort);

            lightControl.BaudRate = 9600;
            lightControl.Parity = Parity.None;
            lightControl.StopBits = StopBits.One;
            lightControl.DataBits = 8;
            lightControl.Handshake = Handshake.None;

            if (lightControl.IsOpen == false)
            {
                try
                {
                    lightControl.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            string strMessage;
            try
            {
                if (mLightControllerMaker == "MEGA")
                {
                    strMessage = (camIdx + 1).ToString() + lightStr.PadLeft(3, '0');
                }
                else if (mLightControllerMaker == "LFINE")
                {
                    strMessage = camIdx.ToString() + 'w' + lightStr.PadLeft(4, '0');
                }
                else
                {
                    strMessage = camIdx.ToString() + 'w' + lightStr.PadLeft(4, '0');
                }

                byte[] stx = new byte[] { 0x02 };
                byte[] message = Encoding.ASCII.GetBytes(strMessage);
                byte[] etx = new byte[] { 0x03 };
                byte[] data = stx.Concat(message).Concat(etx).ToArray();

                lightControl.Write(data, 0, data.Length);
                lightControl.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); ;
            }
        }
    }
}