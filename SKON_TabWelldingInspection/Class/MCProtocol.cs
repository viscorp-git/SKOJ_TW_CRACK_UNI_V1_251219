using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Melsec;

namespace MelsecProtocol
{
    public class MCProtocol
    {
        private MelsecMcNet _plc;
        private AlienSession alienSession;

        public MCProtocol(string ipAddress, int port)
        {
            try
            {
                _plc = new MelsecMcNet();
                _plc.ConnectTimeOut = 1000;
                _plc.ReceiveTimeOut = 1000;

                alienSession = new AlienSession();
                alienSession.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                alienSession.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                alienSession.Socket.LingerState = new LingerOption(true, 0); // LingerTime을 0으로 설정하여 즉시 종료
                alienSession.Socket.Bind(new IPEndPoint(IPAddress.Any, port)); // 로컬 포트 고정
                alienSession.Socket.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), port));
                alienSession.DTU = _plc.ConnectionId;

                //_plc.IpAddress = ipAddress;
                //_plc.Port = port;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool Connect()
        {
            try
            {
                OperateResult connectResult = _plc.ConnectServer(alienSession);

                if (connectResult.IsSuccess == true)
                    return true;
                else
                    return false;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }     
        }

        public void Disconnect()
        {
            try
            {
                if (_plc != null)
                    _plc.ConnectClose();
            }
            catch
            {

            }
        }

        public void Release()
        {
            try
            {
                if (_plc != null)
                {
                    _plc.ConnectClose();
                    _plc.Dispose();
                    _plc = null;
                }
            }
            catch
            {

            }
        }

        public int ReadInt16(string memoryAddress)
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    OperateResult<Int16> getResult = _plc.ReadInt16(memoryAddress);

                    if (getResult.IsSuccess == true)
                    {
                        int resultValue = getResult.Content;
                        getResult = null;
                        return resultValue;
                    }
                }

                return -9999;
            }
            catch
            {
                return -9999;
            }
        }

        public int ReadInt32(string memoryAddress)
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    OperateResult<Int32> getResult = _plc.ReadInt32(memoryAddress);

                    if (getResult.IsSuccess == true)
                    {
                        int resultValue = getResult.Content;
                        getResult = null;
                        return resultValue;
                    }
                }

                return -9999;
            }
            catch
            {
                return -9999;
            }
        }

        public string ReadString(string memoryAddress, ushort length)
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    OperateResult<string> getResult = _plc.ReadString(memoryAddress, length);

                    if (getResult.IsSuccess == true)
                    {
                        string resultValue = getResult.Content.ToString().Replace("\r\n", "").Replace("\0", "");

                        getResult = null;
                        return resultValue;
                    }
                }

                return "-9999";
            }
            catch (Exception)
            {
                return "-9999";

            }
        }

        public int WriteString(string memoryAddress, string value)
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    OperateResult setResult = _plc.Write(memoryAddress, value);

                    if (setResult.IsSuccess == true)
                    {
                        setResult = null;
                        return 1;
                    }
                }
                return -9999;
            }
            catch
            {
                return -9999;
            }
        }

        public OperateResult ReadString_Oper(string memoryAddress, ushort length)
        {
            OperateResult<string> getResult = null;
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    getResult = _plc.ReadString(memoryAddress, length);

                    if (getResult.IsSuccess == true)
                    {
                        string resultValue = getResult.Content.ToString().Replace("\r\n", "").Replace("\0", "");

                        //getResult = null;
                    }
                }

                return getResult;
            }
            catch (Exception)
            {
                getResult.IsSuccess = false;

                return getResult;
            }
        }


        public int WriteShort(string memoryAddress, short value)
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    OperateResult setResult = _plc.Write(memoryAddress, value);

                    if (setResult.IsSuccess == true)
                    {
                        setResult = null;
                        return 1;
                    }
                }

                return -9999;
            }
            catch
            {
                return -9999;
            }
        }
    }
}
