using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Sentech.GenApiDotNET;
using Sentech.StApiDotNET;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using SKON_TabWelldingInspection.Class;
using System.Threading;
using System.Windows.Media.Media3D;

namespace SKON_TabWelldingInspection
{
    public delegate void CathodeGrabEventHandler(Bitmap bimage);
    public delegate void AnodeGrabEventHandler(Bitmap bimage);
    public delegate void CathodeCaptureEventHandle(Bitmap bimage, int CamNum);
    public delegate void AnodeCaptureEventHandle(Bitmap bimage, int CamNum);

    class cls_SentechSDK
    {
        public event CathodeGrabEventHandler CathodeGrabimage;
        public event AnodeGrabEventHandler AnodeGrabimage;
        public event CathodeCaptureEventHandle CathodeCaptureimage;
        public event AnodeCaptureEventHandle AnodeCaptureimage;

        public int CamNum = 0;
        private bool Status = false;

        const string GEV_DEVICE_IP_ADDRESS = "GevDeviceIPAddress";                      //Standard
        const string DEVICE_LINK_HEARTBEAT_TIMEOUT = "DeviceLinkHeartbeatTimeout";      //Standard[us]
        const string GEV_HEARTBEAT_TIMEOUT = "GevHeartbeatTimeout";                     //Standard(Deprecated)[ms]

        const string EVENT_SELECTOR = "EventSelector";                  //Standard
        const string EVENT_NOTIFICATION = "EventNotification";          //Standard
        const string EVENT_NOTIFICATION_ON = "On";                      //Standard
        const string TARGET_EVENT_NAME = "DeviceLost";                  //Standard
        const string CALLBACK_NODE_NAME = "EventDeviceLost";            //Standard

        const string TRIGGER_SELECTOR = "TriggerSelector";              //Standard
        const string TRIGGER_SELECTOR_FRAME_START = "FrameStart";       //Standard
        const string TRIGGER_SELECTOR_EXPOSURE_START = "ExposureStart"; //Standard
        const string TRIGGER_MODE = "TriggerMode";                      //Standard
        const string TRIGGER_MODE_ON = "On";                            //Standard
        const string TRIGGER_MODE_OFF = "Off";                            //Standard
        const string TRIGGER_SOURCE = "TriggerSource";                  //Standard
        const string TRIGGER_SOURCE_SOFTWARE = "Software";              //Standard
        const string TRIGGER_SOFTWARE = "TriggerSoftware";              //Standard

        const string EXPOSURE_TIME = "ExposureTime";            //Standard
        const string GAIN = "Gain";                             //Standard
        const string BALANCE_WHITE_AUTO = "BalanceWhiteAuto";	//Standard

        CStApiAutoInit mApi = null;
        CStSystem mSystem = null;
        CStDevice[] mDevice = new CStDevice[2];
        CStDataStream[] mDataStream = new CStDataStream[2];
        private CStDeviceArray _cStDeviceArray;
        private CStDataStreamArray _cStDataStreamArray;

        bool[] isStopped = new bool[2] { true, true };
        CImageData[] convertBitmap = new CImageData[2];

        //250404 비동기 연결
        private string camCathodeIP = "";
        private string camAnodeIP = "";
        private cls_Log mLog = null;
        private Dictionary<int, CancellationTokenSource> _ctsDictionary = new Dictionary<int, CancellationTokenSource>();
        private bool camCathodeReserve = false;
        private bool camAnodeReserve = false;

        public cls_SentechSDK(ref cls_Log log)
        {
            try
            {
                //250404
                mLog = log;

                // Initialize StApi before using.
                mApi = new CStApiAutoInit();
                // Create a system object for device scan and connection.
                mSystem = new CStSystem(eStSystemVendor.Default, eStInterfaceType.GigEVision);
                _cStDeviceArray = new CStDeviceArray();
                _cStDataStreamArray = new CStDataStreamArray();

                for (int i = 0; i < mDataStream.Length; i++)
                {
                    convertBitmap[i] = new CImageData();
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        // Method for handling callback action
        private void OnCathodeNodeCallback(INode node, object[] param)
        {
            if (node.IsAvailable)
            {
                IStDevice device = param[0] as IStDevice;

                //250404 비동기 연결 추가
                int camIndex = (int)param[1];

                // Node event will be triggered when it is invalidated. 
                // Check if DeviceLost occurred.
                if (device.IsDeviceLost)
                {
                    StartCameraConnection(camIndex, camCathodeIP);
                    Console.WriteLine("OnNodeEvent:" + node.DisplayName + " : DeviceLost");
                }
                else
                {
                    Console.WriteLine("OnNodeEvent:" + node.DisplayName + " : Invalidated");
                }
            }
        }


        public bool CheckConnection(string cam_IP)
        {
            bool m_blCheckConnection = false;

            IStInterface iInterface = null;
            try
            {
                for (uint i = 0; i < mSystem.InterfaceCount; i++)
                {
                    if (0 < mSystem.GetIStInterface(i).DeviceCount)
                    {
                        iInterface = mSystem.GetIStInterface(i);
                        INodeMap nodeMap = iInterface.GetIStPort().GetINodeMap();
                        IInteger integerDeviceSelector = nodeMap.GetNode<IInteger>("DeviceSelector");
                        IInteger integerGevDeviceIPAddress = nodeMap.GetNode<IInteger>("GevDeviceIPAddress");

                        for (uint j = 0; j <= integerDeviceSelector.Maximum; ++j)
                        {
                            integerDeviceSelector.Value = j;

                            //내가 선택한 카메라 IP 와 검색된 카메라 IP 비교
                            if (cam_IP == integerGevDeviceIPAddress.ToString())
                            {
                                //string a = iInterface.GetIStDeviceInfo(j).AccessStatus.ToString();
                                if (iInterface.GetIStDeviceInfo(j).AccessStatus == eDeviceAccessStatus.OPEN_READWRITE)
                                {
                                    m_blCheckConnection = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            return m_blCheckConnection;
        }

        //250404 카메라 연결 체크
        public bool CheckConnectionCamera(int index)
        {
            if (mDevice[index] == null)
                return false;

            IStDevice device = mDevice[index] as IStDevice;
            try
            {
                if (device.IsDeviceLost)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckConnectionIP(string ipAddress)
        {
            // Ping 객체 생성
            using (Ping pingSender = new Ping())
            {
                // 타임아웃 설정 (밀리초)
                int timeout = 50;

                try
                {
                    // Ping을 보내고 응답을 기다림
                    PingReply reply = pingSender.Send(ipAddress, timeout);

                    // 응답 결과를 확인하고 연결 상태 반환
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    // 예외가 발생하면 연결되지 않은 것으로 간주
                    return false;
                }
            }
        }

        private void OnAnodeNodeCallback(INode node, object[] param)
        {
            if (node.IsAvailable)
            {
                IStDevice device = param[0] as IStDevice;
                //250404 비동기 연결 추가
                int camIndex = (int)param[1];


                // Node event will be triggered when it is invalidated. 
                // Check if DeviceLost occurred.
                if (device.IsDeviceLost)
                {
                    StartCameraConnection(camIndex, camAnodeIP);
                    Console.WriteLine("OnNodeEvent:" + node.DisplayName + " : DeviceLost");
                }
                else
                {
                    Console.WriteLine("OnNodeEvent:" + node.DisplayName + " : Invalidated");
                }
            }
        }

        private void UpdateHeartbeatTimeout(INodeMap nodeMap)
        {
            string strUnit = string.Empty;
            bool bExit = false;

            while (true)
            {
                // Displays the current HeartbeatTimeout setting
                IValue deviceLinkHeartbeatTimeout = nodeMap.GetNode<IValue>(DEVICE_LINK_HEARTBEAT_TIMEOUT);

                if (deviceLinkHeartbeatTimeout != null)
                {
                    strUnit = "[us]";
                }
                else
                {
                    deviceLinkHeartbeatTimeout = nodeMap.GetNode<IValue>(GEV_HEARTBEAT_TIMEOUT);
                    if (deviceLinkHeartbeatTimeout != null)
                    {
                        strUnit = "[ms]";
                    }
                    else
                    {
                        Console.WriteLine("Unable to get the current heartbeat value");
                        bExit = true;
                    }
                }

                if (bExit)
                {
                    break;
                }

                // Waiting to enter a new HeartbeatTimeout setting.
                Console.WriteLine("Warning: the heartbeat sending interval is fixed when the device is initialized (opened).");
                Console.WriteLine("Thus, changing the heartbeat timeout smaller than the current value may cause timeout.");
                Console.WriteLine("In practical situation, please either set environment variable STGENTL_GIGE_HEARTBEAT before opening the device");
                Console.WriteLine("or re-open the device after changing the heartbeat value without setting the environment variable and debugger.");

                Console.WriteLine("Current Heartbeat Timeout" + strUnit + "=" + deviceLinkHeartbeatTimeout.ToString());
                Console.Write("Input new Heartbeat Timeout" + strUnit + " : ");
                string strInput = Console.ReadLine();

                // Update the camera HeartbeatTimeout settings.
                deviceLinkHeartbeatTimeout.FromString(strInput.Trim());
                bExit = true;
            }
        }

        // Create device by IP address.
        private CStDevice CreateStDeviceByIPAddress(IStInterface iInterface, long deviceIPAddress)
        {
            CStDevice device = null;
            iInterface.UpdateDeviceList();

            INodeMap nodeMap = iInterface.GetIStPort().GetINodeMap();
            IInteger integerDeviceSelector = nodeMap.GetNode<IInteger>("DeviceSelector");
            IInteger integerGevDeviceIPAddress = nodeMap.GetNode<IInteger>("GevDeviceIPAddress");

            for (uint i = 0; i <= integerDeviceSelector.Maximum; ++i)
            {
                integerDeviceSelector.Value = i;

                if (integerGevDeviceIPAddress.IsAvailable)
                {
                    if (integerGevDeviceIPAddress.Value == deviceIPAddress)
                    {
                        device = iInterface.CreateStDevice(i);
                        return device;
                        //break;

                    }
                }
            }

            return device;
        }



        private void OnCallback(IStCallbackParamBase paramBase, object[] param)
        {
            if (isStopped[0] || Status == false)
            {
                return;
            }

            // Check callback type. Only NewBuffer event is handled in here
            if (paramBase.CallbackType == eStCallbackType.TL_DataStreamNewBuffer)
            {
                // In case of receiving a NewBuffer events:
                // Convert received callback parameter into IStCallbackParamGenTLEventNewBuffer for acquiring additional information.
                IStCallbackParamGenTLEventNewBuffer callbackParam = paramBase as IStCallbackParamGenTLEventNewBuffer;

                if (callbackParam != null)
                {
                    try
                    {
                        // Get the IStDataStream interface object from the received callback parameter.
                        IStDataStream dataStream = callbackParam.GetIStDataStream();

                        // Retrieve the buffer of image data for that callback indicated there is a buffer received.
                        using (CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(0))
                        {
                            // Check if the acquired data contains image data.
                            if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                            {
                                // If yes, we create a IStImage object for further image handling.
                                if (CamNum == 0)
                                {
                                    IStImage stImage = streamBuffer.GetIStImage();
                                    CathodeCaptureimage((Bitmap)convertBitmap[0].CreateBitmap(stImage).Clone(), CamNum);
                                }
                                else if (CamNum == 1)
                                {
                                    IStImage stImage = streamBuffer.GetIStImage();
                                    AnodeCaptureimage((Bitmap)convertBitmap[1].CreateBitmap(stImage).Clone(), CamNum);
                                }
                            }
                            else
                            {
                                // If the acquired data contains no image data.
                                Console.WriteLine("Image data does not exist.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // If any exception occurred, display the description of the error here.
                        Console.Error.WriteLine("An exception occurred. \r\n" + e.Message);
                    }
                }
            }
        }

        private void OnCathodeCallback(IStCallbackParamBase paramBase, object[] param)
        {
            if (isStopped[0] || Status)
            {
                return;
            }

            // Check callback type. Only NewBuffer event is handled in here
            if (paramBase.CallbackType == eStCallbackType.TL_DataStreamNewBuffer)
            {
                // In case of receiving a NewBuffer events:
                // Convert received callback parameter into IStCallbackParamGenTLEventNewBuffer for acquiring additional information.
                IStCallbackParamGenTLEventNewBuffer callbackParam = paramBase as IStCallbackParamGenTLEventNewBuffer;

                if (callbackParam != null)
                {
                    try
                    {
                        // Get the IStDataStream interface object from the received callback parameter.
                        IStDataStream dataStream = callbackParam.GetIStDataStream();

                        // Retrieve the buffer of image data for that callback indicated there is a buffer received.
                        using (CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(0))
                        {
                            // Check if the acquired data contains image data.
                            if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                            {
                                // If yes, we create a IStImage object for further image handling.
                                IStImage stImage = streamBuffer.GetIStImage();
                                CathodeGrabimage((Bitmap)convertBitmap[0].CreateBitmap(stImage).Clone());
                            }
                            else
                            {
                                // If the acquired data contains no image data.
                                Console.WriteLine("Image data does not exist.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // If any exception occurred, display the description of the error here.
                        Console.Error.WriteLine("An exception occurred. \r\n" + e.Message);
                    }
                }
            }
        }

        private void OnAnodeCallback(IStCallbackParamBase paramBase, object[] param)
        {
            if (isStopped[1] || Status)
            {
                return;
            }

            // Check callback type. Only NewBuffer event is handled in here
            if (paramBase.CallbackType == eStCallbackType.TL_DataStreamNewBuffer)
            {
                // In case of receiving a NewBuffer events:
                // Convert received callback parameter into IStCallbackParamGenTLEventNewBuffer for acquiring additional information.
                IStCallbackParamGenTLEventNewBuffer callbackParam = paramBase as IStCallbackParamGenTLEventNewBuffer;

                if (callbackParam != null)
                {
                    try
                    {
                        // Get the IStDataStream interface object from the received callback parameter.
                        IStDataStream dataStream = callbackParam.GetIStDataStream();

                        // Retrieve the buffer of image data for that callback indicated there is a buffer received.
                        using (CStStreamBuffer streamBuffer = dataStream.RetrieveBuffer(0))
                        {
                            // Check if the acquired data contains image data.
                            if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                            {
                                // If yes, we create a IStImage object for further image handling.
                                IStImage stImage = streamBuffer.GetIStImage();
                                AnodeGrabimage((Bitmap)convertBitmap[1].CreateBitmap(stImage).Clone());

                            }
                            else
                            {
                                // If the acquired data contains no image data.
                                Console.WriteLine("Image data does not exist.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // If any exception occurred, display the description of the error here.
                        Console.Error.WriteLine("An exception occurred. \r\n" + e.Message);
                    }
                }
            }
        }

        public void Grab_Image()
        {
            try
            {
                using (CStStreamBuffer streamBuffer = mDataStream[0].RetrieveBuffer(0))
                {
                    // Check if the acquired data contains image data.
                    if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                    {
                        // If yes, we create a IStImage object for further image handling.
                        IStImage stImage = streamBuffer.GetIStImage();
                        AnodeGrabimage((Bitmap)convertBitmap[1].CreateBitmap(stImage).Clone());

                    }
                    else
                    {
                        // If the acquired data contains no image data.
                        Console.WriteLine("Image data does not exist.");
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        // Set Enumeration
        private void SetEnumeration(INodeMap nodeMap, string enumerationName, string valueName)
        {
            // Get the IEnum interface.
            IEnum enumNode = nodeMap.GetNode<IEnum>(enumerationName);

            // Update the settings using the IEnum interface.
            enumNode.StringValue = valueName;
        }

        public void TriggerModeOn(int index)
        {
            // Get the INodeMap interface for the camera settings.
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();

            //// Set the TriggerSelector to FrameStart.
            //try
            //{
            //    SetEnumeration(nodeMapRemote, TRIGGER_SELECTOR, TRIGGER_SELECTOR_FRAME_START);
            //}
            //catch (GenericException ex)
            //{
            //    // If "FrameStart" is not supported, use "ExposureStart".
            //    SetEnumeration(nodeMapRemote, TRIGGER_SELECTOR, TRIGGER_SELECTOR_EXPOSURE_START);
            //}

            // Set the TriggerMode to On.
            SetEnumeration(nodeMapRemote, TRIGGER_MODE, TRIGGER_MODE_ON);

            // Set the TriggerSource to Software.
            SetEnumeration(nodeMapRemote, TRIGGER_SOURCE, TRIGGER_SOURCE_SOFTWARE);
        }

        public void TriggerModeOff(int index)
        {
            // Get the INodeMap interface for the camera settings.
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();

            //// Set the TriggerSelector to FrameStart.
            //try
            //{
            //    SetEnumeration(nodeMapRemote, TRIGGER_SELECTOR, TRIGGER_SELECTOR_FRAME_START);
            //}
            //catch (GenericException)
            //{
            //    // If "FrameStart" is not supported, use "ExposureStart".
            //    SetEnumeration(nodeMapRemote, TRIGGER_SELECTOR, TRIGGER_SELECTOR_EXPOSURE_START);
            //}

            // Set the TriggerMode to On.
            SetEnumeration(nodeMapRemote, TRIGGER_MODE, TRIGGER_MODE_OFF);

        }

        public void ExposureValue(int index, string strvalue)
        {
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();
            NumericF<FloatNode>(nodeMapRemote, EXPOSURE_TIME, strvalue);
        }

        public void GainValue(int index, string strvalue)
        {
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();
            NumericF<FloatNode>(nodeMapRemote, GAIN, strvalue);
        }

        public void CamReverse(int index, bool isValue)
        {
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();
            IBool boolReverY = nodeMapRemote.GetNode<IBool>("ReverseY");
            IBool boolReverX = nodeMapRemote.GetNode<IBool>("ReverseX");
            boolReverY.Value = isValue;
            boolReverX.Value = isValue;
        }


        public void WhiteBalance_SetVal(int index, string RGB, string strvalue)
        {
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();

            SetEnumeration(nodeMapRemote, "BalanceRatioSelector", RGB);
            NumericF<FloatNode>(nodeMapRemote, "BalanceRatio", strvalue);
        }

        public string WhiteBalance_GetVal(int index, string RGB)
        {
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();
            SetEnumeration(nodeMapRemote, "BalanceRatioSelector", RGB);
            IFloat Value = nodeMapRemote.GetNode<IFloat>("BalanceRatio");
            return Value.ToString();
        }

        public void WhiteBalance_Auto(int index)
        {
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();
            SetEnumeration(nodeMapRemote, "BalanceWhiteAuto", "Once"); // Off, Once, Continuous
        }


        private void NumericF<NODE_TYPE>(INodeMap nodeMap, string nodeName, string strValue) where NODE_TYPE : IFloat
        {
            // Get the IFloat interface.
            NODE_TYPE node = (NODE_TYPE)nodeMap[nodeName];

            if (node.IsWritable)
            {
                while (true)
                {
                    // Display the feature name, the range, the current value and the incremental value.
                    Console.Write(nodeName);
                    Console.Write(" Minimum={0:F2}", node.Minimum);
                    Console.Write(" Maximum={0:e5}", node.Maximum);
                    Console.Write(" Current={0:F2}", node.Value);
                    if (node.IncrementMode == eIncrementMode.FixedIncrement)
                    {
                        Console.Write(" Increment=" + node.Increment);
                    }

                    double value;
                    if (double.TryParse(strValue.Trim(), out value))
                    {
                        // Reflect the value entered.
                        if (node.Minimum <= value && value <= node.Maximum)
                        {
                            node.Value = value;
                            break;
                        }
                    }
                }
            }
        }

        public void SoftwareTrigger(int index)
        {
            if (mDevice[index] == null)
            {
                throw new Exception($"Software Trigger - Camera index [{index}] is Not Activate");
            }

            // Get the INodeMap interface for the camera settings.
            INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();
            ICommand commandNode = nodeMapRemote.GetNode<ICommand>("TriggerSoftware");
            commandNode.Execute();
        }

        public bool OnConnect(int index, string strDeviceIP)
        {
            IStInterface iInterface = null;
            bool connectflag = false;
            Console.WriteLine($"내가 선택한 카메라 IP 주소 : {strDeviceIP}");
            for (uint i = 0; i < mSystem.InterfaceCount; i++)
            {
                Console.WriteLine(i + "번째 인터페이스 확인 중...");

                if (0 < mSystem.GetIStInterface(i).DeviceCount)
                {
                    iInterface = mSystem.GetIStInterface(i);
                    Console.WriteLine(i + "번째 인터페이스에" + mSystem.GetIStInterface(i).DeviceCount + "개 디바이스 검색됨");

                    INodeMap nodeMap = iInterface.GetIStPort().GetINodeMap();
                    IInteger integerDeviceSelector = nodeMap.GetNode<IInteger>("DeviceSelector");
                    IInteger integerGevDeviceIPAddress = nodeMap.GetNode<IInteger>("GevDeviceIPAddress");
                    long deviceIPAddress = integerGevDeviceIPAddress.Value;

                    for (uint j = 0; j <= integerDeviceSelector.Maximum; ++j)
                    {
                        integerDeviceSelector.Value = j;
                        Console.WriteLine($"검색 IP 주소 : {integerGevDeviceIPAddress}");

                        //내가 선택한 카메라 IP 와 검색된 카메라 IP 비교
                        if (strDeviceIP == integerGevDeviceIPAddress.ToString())
                        {
                            //mDevice[index] = CreateStDeviceByIPAddress(iInterface, deviceIPAddress);
                            mDevice[index] = iInterface.CreateStDevice(j);
                            //_cStDeviceArray.Register(mDevice[index]);
                            //_cStDataStreamArray.Register(mDevice[index].CreateStDataStream(0));
                            connectflag = true;
                            return connectflag;
                        }
                    }
                }
                else
                {
                    Console.WriteLine(i + "번째 인터페이스에" + mSystem.GetIStInterface(i).DeviceCount + "개 디바이스 검색됨");
                    connectflag = false;
                }
            }
            if (iInterface == null)
            {
                connectflag = false;
                Console.WriteLine($"검색된 인터페이스가 없습니다.");

                //throw new RuntimeException("There is no device.");
            }

            return connectflag;
        }

        //250404비동기 시작
        public async Task<bool> StartCameraConnection(int index, string ip)
        {
            if (_ctsDictionary.ContainsKey(index))
            {
                StopCameraConnection(index);
            }


            _ctsDictionary[index] = new CancellationTokenSource();  // 새로운 CancellationTokenSource 생성

            bool result = await ConnectAsync(index, ip, _ctsDictionary[index].Token);

            if (result)
            {
                //250404 temp 카메라 resrve 기능 안됨 (허브 poe 사용시 추정) => program에 추가시 주석 삭제 필요
                //CamReverse(index, index == 0 ? camCathodeReserve : camAnodeReserve);
                StartCapture(index);
                TriggerModeOn(index);

                //250407 연결 성공시 CancellationTokenSource 초기화
                StopCameraConnection(index);

                mLog.WriteLog("CAM", $"Camera {returnCamType(index)} successfully connected.");
                return true;
            }
            else
            {
                mLog.WriteLog("CAM", $"Camera {returnCamType(index)} connection attempt stopped.");
                return false;
            }
        }

        // 특정 카메라만 중단
        private void StopCameraConnection(int index)
        {
            if (_ctsDictionary.ContainsKey(index))
            {
                _ctsDictionary[index].Cancel(); // 특정 카메라의 연결 시도를 중단
                _ctsDictionary[index].Dispose();
                _ctsDictionary.Remove(index);
            }
        }

        //250404 CamType Return
        private string returnCamType(int index)
        {
            if (index == 0)
                return "Cathode";
            else
                return "Anode";
        }

        //250404 장치 초기화
        private void RefreshDeviceList()
        {
            //250407 기존 인터페이스 캐시를 재사용하는 경우가 있어 반복적으로 연결을 해제했다 연결할 경우, mSystem.InterfaceCount가 0이 나오는 경우가 발생
            //강제로 인터페이스 캐시 초기화 후 해당 이슈 발생 안함
            GC.Collect();

            mSystem.UpdateInterfaceList();  // 인터페이스 목록 갱신
            for (uint i = 0; i < mSystem.InterfaceCount; i++)
            {
                mSystem.GetIStInterface(i).UpdateDeviceList();  // 각 인터페이스의 장치 목록 갱신
            }
        }

        //250404 CamReverse 상태값 저장
        public void CamReserve(int index, bool CamReserve)
        {
            if (index == 0)
                camCathodeReserve = CamReserve;
            else
                camAnodeReserve = CamReserve;
        }

        //250404 비동기 연결 구현
        public async Task<bool> ConnectAsync(int index, string strDeviceIP, CancellationToken cancellationToken)
        {
            const int retryDelay = 5000; // 5초 대기 후 재시도

            int retryCount = 0; // 재시도 횟수 기록

            while (!cancellationToken.IsCancellationRequested) // 취소 요청이 들어오면 종료
            {
                try
                {
                    IStInterface iInterface = null;

                    //250404 각 인터페이스의 장치 목록 갱신
                    RefreshDeviceList();


                    for (uint i = 0; i < mSystem.InterfaceCount; i++)
                    {
                        if (0 < mSystem.GetIStInterface(i).DeviceCount)
                        {
                            iInterface = mSystem.GetIStInterface(i);

                            IInteger nodeGevDeviceIPAddress = iInterface.GetIStPort().GetINodeMap().GetNode<IInteger>(GEV_DEVICE_IP_ADDRESS);
                            long deviceIPAddress = nodeGevDeviceIPAddress.Value;

                            if (strDeviceIP == nodeGevDeviceIPAddress.ToString())
                            {


                                mDevice[index] = CreateStDeviceByIPAddress(iInterface, deviceIPAddress);

                                if (mDevice[index] != null)
                                {
                                    return true; // 연결 성공 시 함수 종료
                                }
                            }
                        }
                    }

                    mLog.WriteLog("CAM", $"{returnCamType(index)} Camera Connection Fail!");
                }
                catch (Exception ex)
                {
                    mLog.WriteLog("CAM", $"{returnCamType(index)} Camera Connection Fail: {ex.Message}");
                }

                retryCount++;
                mLog.WriteLog("CAM", $"[Retry {retryCount}] Connecting {returnCamType(index)} Camera.... ");

                await Task.Delay(retryDelay, cancellationToken); // 대기 중 취소 요청이 들어오면 예외 발생
            }
            return false; // 취소되었으므로 false 반환
        }

        public bool Connect(int index, string strDeviceIP)
        {
            try
            {
                IStInterface iInterface = null;

                //250404 IP
                if (index == 0)
                    camCathodeIP = strDeviceIP;
                if (index == 1)
                    camAnodeIP = strDeviceIP;

                for (uint i = 0; i < mSystem.InterfaceCount; i++)
                {
                    if (0 < mSystem.GetIStInterface(i).DeviceCount)
                    {
                        iInterface = mSystem.GetIStInterface(i);
                        // current IP address of the camera.
                        IInteger nodeGevDeviceIPAddress = iInterface.GetIStPort().GetINodeMap().GetNode<IInteger>(GEV_DEVICE_IP_ADDRESS);
                        long deviceIPAddress = nodeGevDeviceIPAddress.Value;

                        if (strDeviceIP == nodeGevDeviceIPAddress.ToString())
                        {
                            mDevice[index] = CreateStDeviceByIPAddress(iInterface, deviceIPAddress);


                            // 화면 상하 반전 2023.11.09
                            // 이 부분은 설정값 자체를 변경해버린다. 한번 적용하면 계속 유지하게 되더라.
                            // 혹시 다음에 쓰더라도 일단 주석처리함
                            //INodeMap nodeMapRemote = mDevice[index].GetRemoteIStPort().GetINodeMap();
                            //IBool boolReverY = nodeMapRemote.GetNode<IBool>("ReverseY");
                            //IBool boolReverX = nodeMapRemote.GetNode<IBool>("ReverseX");
                            //boolReverY.Value = true;
                            //boolReverY.Value = true;

                            return true;
                            // break;
                        }
                    }
                }
                if (iInterface == null)
                {
                    throw new RuntimeException("There is no device.");
                }

                ////250404 연결 실패해도 true값 리턴 해서 true => fasle 수정
                Task.Run(() => StartCameraConnection(index, strDeviceIP));
                return false;

            }
            catch (Exception)
            {
                Task.Run(() => StartCameraConnection(index, strDeviceIP));
                return false;
            }
        }

        public void StartGrab()
        {
            Status = true;
        }


        public void StopGrab()
        {
            Status = false;
        }

        public void StartCapture(int index)
        {
            try
            {
                // Displays the DisplayName of the device.
                Console.WriteLine("Device=" + mDevice[index].GetIStDeviceInfo().DisplayName);
                ////
                // Get the INodeMap interface for the host side device settings.
                INodeMap nodeMapLocal = mDevice[index].GetLocalIStPort().GetINodeMap();


                // Get the INode interface for the EventDeviceLost node.
                INode nodeCallback = nodeMapLocal.GetNode<INode>(CALLBACK_NODE_NAME);

                // Register a callback method.
                object[] param = { mDevice[index], index };
                if (index == 0)
                    nodeCallback.RegisterCallbackMethod(OnCathodeNodeCallback, param, eCallbackType.PostOutsideLock);
                else if (index == 1)
                    nodeCallback.RegisterCallbackMethod(OnAnodeNodeCallback, param, eCallbackType.PostOutsideLock);

                // Enabling the transmission of the target event.
                IEnum eventSelector = nodeMapLocal.GetNode<IEnum>(EVENT_SELECTOR);
                eventSelector.StringValue = TARGET_EVENT_NAME;

                IEnum eventNotification = nodeMapLocal.GetNode<IEnum>(EVENT_NOTIFICATION);
                eventNotification.StringValue = EVENT_NOTIFICATION_ON;



                // Start the event acquisition thread for listening to event.
                mDevice[index].StartEventAcquisitionThread();

                ////
                isStopped[index] = false;

                mDataStream[index] = mDevice[index].CreateStDataStream(0);

                TriggerModeOn(index);

                if (index == 0)
                {
                    // Register callback method. Note that by different usage, we pass different kinds/numbers of parameters in.
                    mDataStream[index].RegisterCallbackMethod(OnCathodeCallback);
                    mDataStream[index].RegisterCallbackMethod(OnCallback);
                }
                else if (index == 1)
                {
                    // Register callback method. Note that by different usage, we pass different kinds/numbers of parameters in.
                    mDataStream[index].RegisterCallbackMethod(OnAnodeCallback);
                    mDataStream[index].RegisterCallbackMethod(OnCallback);
                }

                // Start the image acquisition of the host (local machine) side.
                mDataStream[index].StartAcquisition();

                // Start the image acquisition of the camera side.
                mDevice[index].AcquisitionStart();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }



        public void StopCapture(int index)
        {
            // Stop the grabbing.
            try
            {
                isStopped[index] = true;

                // Stop the image acquisition of the camera side.
                mDevice[index].AcquisitionStop();

                // Stop the image acquisition of the host side.
                mDataStream[index].StopAcquisition();

                // Stop the event acquisition thread.
                mDevice[index].StopEventAcquisitionThread();

                mDataStream[index].Dispose();
                mDataStream[index] = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Bitmap Grab(uint cameraNumber, uint timeOut)
        {
            IStImage stImage = null;

            _cStDataStreamArray[cameraNumber].StartAcquisition();

            _cStDeviceArray[cameraNumber].AcquisitionStart();

            CStStreamBuffer streamBuffer = null;

            while (_cStDataStreamArray[cameraNumber].IsGrabbing)
            {
                streamBuffer = _cStDataStreamArray[cameraNumber].RetrieveBuffer(timeOut);

                if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent == true)
                {
                    stImage = streamBuffer.GetIStImage();
                    break;
                }
            }

            _cStDeviceArray[cameraNumber].AcquisitionStop();

            _cStDataStreamArray[cameraNumber].StopAcquisition();

            if (stImage != null)
            {
                Bitmap resultBitmap = ConvertStImageToBitmap(stImage);

                ColorPalette palette = resultBitmap.Palette;

                for (int i = 0; i < 256; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);

                resultBitmap.Palette = palette;

                stImage = null;

                if (streamBuffer != null)
                    streamBuffer.Dispose();

                return resultBitmap;
            }
            else
            {

                if (streamBuffer != null)
                    streamBuffer.Dispose();

                return null;
            }
        }

        public Bitmap ConvertStImageToBitmap(IStImage stImage)
        {
            byte[] stImageByteArray = stImage.GetByteArray();

            Bitmap resultBitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, PixelFormat.Format8bppIndexed);
            BitmapData resultBitmapData = resultBitmap.LockBits(new Rectangle(0, 0, (int)stImage.ImageWidth, (int)stImage.ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr resultBitmapIntPtr = resultBitmapData.Scan0;
            int resultBitmapLength = Math.Abs(resultBitmapData.Stride) * resultBitmap.Height;
            byte[] resultBitmapBytesValue = new byte[resultBitmapLength];

            Marshal.Copy(stImageByteArray, 0, resultBitmapIntPtr, stImageByteArray.Length);

            resultBitmap.UnlockBits(resultBitmapData);

            return (Bitmap)resultBitmap.Clone();
        }

        public void Disconnect(int index)
        {
            // Destroy the camera object.
            try
            {
                //250404 연결시도 종료
                StopCameraConnection(index);


                if (mDataStream[index] != null)
                {
                    mDataStream[index].Dispose();
                    mDataStream[index] = null;

                }

                //if (_cStDeviceArray[(uint)index] != null)
                //    _cStDeviceArray[(uint)index].Dispose();

                //if (_cStDataStreamArray[(uint)index] != null)
                //    _cStDataStreamArray[(ushort)index].Dispose();

                if (mDevice[index] != null)
                {
                    mDevice[index].Dispose();
                    mDevice[index] = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DestroyCamera()
        {
            // Destroy the camera object.
            try
            {
                if (mSystem != null)
                {
                    mSystem.Dispose();
                    mSystem = null;
                }

                if (mApi != null)
                {
                    mApi.Dispose();
                    mApi = null;
                }

                for (int i = 0; i < mDataStream.Length; i++)
                {
                    if (convertBitmap[i] != null)
                    {
                        convertBitmap[i].Dispose();
                        convertBitmap[i] = null;
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
    class CImageData : IDisposable
    {
        Bitmap m_Bitmap = null;
        CStPixelFormatConverter m_Converter = null;

        public Bitmap CreateBitmap(IStImage stImage)
        {
            if (m_Converter == null)
            {
                m_Converter = new CStPixelFormatConverter();
            }

            bool isColor = CStApiDotNet.GetIStPixelFormatInfo(stImage.ImagePixelFormat).IsColor;

            if (isColor)
            {
                // Convert the image data to BGR8 format.
                m_Converter.DestinationPixelFormat = eStPixelFormatNamingConvention.BGR8;
            }
            else
            {
                // Convert the image data to Mono8 format.
                m_Converter.DestinationPixelFormat = eStPixelFormatNamingConvention.Mono8;
            }

            if (m_Bitmap != null)
            {
                if ((m_Bitmap.Width != (int)stImage.ImageWidth) || (m_Bitmap.Height != (int)stImage.ImageHeight))
                {
                    m_Bitmap.Dispose();
                    m_Bitmap = null;
                }
            }

            if (m_Bitmap == null)
            {
                if (isColor)
                {
                    m_Bitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, PixelFormat.Format24bppRgb);
                }
                else
                {
                    m_Bitmap = new Bitmap((int)stImage.ImageWidth, (int)stImage.ImageHeight, PixelFormat.Format8bppIndexed);
                    ColorPalette palette = m_Bitmap.Palette;
                    for (int i = 0; i < 256; ++i) palette.Entries[i] = Color.FromArgb(i, i, i);
                    m_Bitmap.Palette = palette;
                }
            }

            using (CStImageBuffer imageBuffer = CStApiDotNet.CreateStImageBuffer())
            {
                m_Converter.Convert(stImage, imageBuffer);

                // Lock the bits of the bitmap.
                BitmapData bmpData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.WriteOnly, m_Bitmap.PixelFormat);

                // Place the pointer to the buffer of the bitmap.
                IntPtr ptrBmp = bmpData.Scan0;
                byte[] imageData = imageBuffer.GetIStImage().GetByteArray();
                Marshal.Copy(imageData, 0, ptrBmp, imageData.Length);
                m_Bitmap.UnlockBits(bmpData);
            }

            return m_Bitmap;
        }

        public void Dispose()
        {
            if (m_Bitmap != null)
            {
                m_Bitmap.Dispose();
                m_Bitmap = null;
            }

            if (m_Converter != null)
            {
                m_Converter.Dispose();
                m_Converter = null;
            }
        }

    }
}
