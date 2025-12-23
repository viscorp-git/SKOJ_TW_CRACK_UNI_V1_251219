using Class;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using HslCommunication;
using IDMAX_FrameWork;
using MelsecProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SKON_TabWelldingInspection.Class;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViDi2;
using static Class.cls_Util;
using Button = System.Windows.Forms.Button;
using File = System.IO.File;
using TextBox = System.Windows.Forms.TextBox;

namespace SKON_TabWelldingInspection
{
    public partial class frm_Main : BaseConfigForm
    {
        #region Class

        private static string iniFilename = Path.Combine(System.Windows.Forms.Application.StartupPath, "Main_Information.ini");
        public cls_Ini ini = new cls_Ini(iniFilename);
        private cls_SentechSDK mCamera = null;
        private cls_DeepLearning mViDi = new cls_DeepLearning();
        private cls_Display mDisplay = new cls_Display();
        private cls_LightControl lightControl;
        private cls_Log mLog = new cls_Log();
        private cls_detectAnomal mDetect = new cls_detectAnomal();
        private cls_Image mImage = new cls_Image();
        public cls_EdgeStatus edgeStatus = new cls_EdgeStatus();  // 상태값 저장

        private Thread EdgeStatus_Thread; // 상태값 저장 Thread
        private Thread ModelChange_Thread; // 모델 체인지 Thread
        private Thread log_Thread;  // 로그 기록 Thread (Queue 내용 기록용)

        bool isEdgeStatusThRunning = true;

        private CogToolBlock CathodeToolBlock = null;
        private CogToolBlock AnodeToolBlock = null;

        public cls_Log Log => mLog;
        #endregion Class

        #region Variable_Camera

        private const int CAM_CATHODE = 0;
        private const int CAM_ANODE = 1;

        public ICogImage Cathode_Image = null;
        public ICogImage Anode_Image = null;

        private Stopwatch m_Cathode_StopWatch = new Stopwatch();
        private Stopwatch m_Anode_StopWatch = new Stopwatch();

        public bool mbCathode_Connect = false;
        public bool mbAndoe_Connect = false;
        private bool mbCathode_Live = false;
        private bool mbAnode_Live = false;
        private bool mbHeartbeat = false;

        public bool mbCathode_Process = false;
        public bool mbAndoe_Process = false;

        private ConcurrentQueue<Tuple<Bitmap, int>> m_Cam_Queue = new ConcurrentQueue<Tuple<Bitmap, int>>();

        #endregion Variable_Camera

        #region Variable_InI

        //Maintenance
        private string mProcess;
        private string mLineNo;
        public string mPosition;
        private string mImageSaveDir;
        private int mImageSaveRatio;
        private string mVissSaveDir;
        private string mVissSaveDirFormat;
        public string mImageSaveErrFormat;
        public string mWorkSpacePath;
        private string mLocateLocalSaveDir;
        private string mLocateVISSSaveDir;
        private string mHeatmapLocalSaveDir;
        private string mHeatmapVISSSaveDir;
        private string mVPROLocalSaveDir;
        private string mJsonSaveDir;
        private bool m_VPDL_Bypass;
        private bool m_VPDL_Locate;
        private bool m_VPRO_Bypass;
        private bool m_VPRO_Use;
        private int m_IO_Num = 0;
        private int mModelChangeTimout;  // 모델체인지 타임아웃 설정, 초(s) 2024-07-29

        //Camera IP
        private string mAnode_Vision_IP;
        private string mCathode_Vision_IP;

        //Camera Setup
        private double mCathode_Exposure;
        private double mCathode_Gain;
        private double mAnode_Exposure;
        private double mAnode_Gain;
        private bool mCathode_Reverse;
        private bool mAnode_Reverse;
        private string mCathode_White_R;
        private string mCathode_White_G;
        private string mCathode_White_B;
        private string mAnode_White_R;
        private string mAnode_White_G;
        private string mAnode_White_B;

        private string mLightControllerPort;
        private string mLightControllerMaker;
        private string mCathode_Light;
        private string mAnode_Light;

        // Camera Guide Line
        private int camGuideLine_line1_x = 0;
        private int camGuideLine_line2_x = 0;
        private int camGuideLine_line3_y = 0;
        private int brightPositionCA_x;
        private int brightPositionCA_y;
        private int brightPositionAN_x;
        private int brightPositionAN_y;

        // 카메라 트리거 개수 카운트 2024-06-18
        private bool isTriggerAction_CA = false;
        private bool isTriggerAction_AN = false;

        // PLC
        public string m_PLC_IP;
        public int m_PLC_Port;
        public bool m_PLC_Bypass;
        public string mPlc_Read_CELLID_CA;
        public string mPlc_Read_CELLID_AN;
        public string mPlc_Read_Carrier_ID;
        public string mPlc_Read_Stack_ID;
        public string mPlc_Read_Model_No;
        public string mPlc_Read_Change_Model_PLC;
        public string mPlc_Read_VisionIO_Reset;
        public string mPlc_Read_Image_Save_Err_Reset;
        public string mPlc_Read_Image_Detect_Err_Reset;
        public string mPlc_Write_HeartBeat;
        public string mPlc_Write_Cathode_Result;
        public string mPlc_Write_Anode_Result;
        public string mPlc_Write_Cathode_Result_Code;
        public string mPlc_Write_Anode_Result_Code;
        public string mPlc_Write_Change_Model_PC;
        public string mPlc_Write_Complete_VisionIO_Reset;
        public string mPlc_Write_Image_Save_Err;
        public string mPlc_Write_Cathode_Image_Detect_Err;
        public string mPlc_Write_Anode_Image_Detect_Err;
        public string mPlc_Write_Cathode_Cell_Id;
        public string mPlc_Write_Anode_Cell_Id;
        public string mPlc_Write_Cathode_Vpro_Result;
        public string mPlc_Write_Anode_Vpro_Result;

        // Threshold
        public double mThreshold_CA_R1; // ROI1 CA
        public double mThreshold_CA_R2; // ROI2 CA
        public double mThreshold_CA_R3; // ROI3 CA
        public double mThreshold_AN_R1; // ROI1 AN
        public double mThreshold_AN_R2; // ROI2 AN
        public double mThreshold_AN_R3; // ROI3 AN

        // Detect Anomal
        public bool mBright_Det_YN;
        public bool mSharp_Det_YN;
        public bool mLocation_Det_YN;
        public bool mNG_Det_YN;
        public bool mNG_ITV_Det_YN;
        public bool mLog_Det_YN;

        public int mBright_Det_CNT;
        public int mSharp_Det_CNT;
        public int mLocation_Det_CNT;
        public int mNG_Det_CNT;
        public int mNG_ITV_Det_CNT;
        public int mNG_ITV_Det_MAX_CNT;

        public int mBright_Rank_Qty;

        public double mBright_CA_STD_VAL;
        public double mBright_CA_LOW_VAL;
        public double mBright_CA_UPP_VAL;
        public double mSharp_CA_STD_VAL;
        public double mSharp_CA_LOW_VAL;

        public double mBright_AN_STD_VAL;
        public double mBright_AN_LOW_VAL;
        public double mBright_AN_UPP_VAL;
        public double mSharp_AN_STD_VAL;
        public double mSharp_AN_LOW_VAL;

        public string curTabpage;

        #endregion Variable_InI

        #region Variable_IO

        private Thread IO_Thread;
        bool isIoThRunning = true;

        private IconLamp[] lamp_in32_0;
        private IconLamp[] lamp_in32_1;
        private IconLamp[] lamp_Out32_0;
        private IconLamp[] lamp_Out32_1;

        public bool[] mINPUT_IO_0 = new bool[8];
        public bool[] mINPUT_IO_1 = new bool[8];
        public bool[] mOUTPUT_IO_0 = new bool[8];
        public bool[] mOUTPUT_IO_1 = new bool[8];

        private int mIO_IN_PLC_ONLINE = -1;
        private int mIO_IN_CA_TRIG = -1;
        private int mIO_IN_AN_TRIG = -1;
        private int mIO_IN_COMM_ERR_RESET = -1;

        private int mIO_OUT_HEARTBEAT = -1;
        private int mIO_OUT_CA_READY = -1;
        private int mIO_OUT_CA_RES_NG = -1;
        private int mIO_OUT_CA_RES_OK = -1;
        private int mIO_OUT_AN_READY = -1;
        private int mIO_OUT_AN_RES_NG = -1;
        private int mIO_OUT_AN_RES_OK = -1;
        private int mIO_OUT_COMM_ERROR = -1;

        #endregion Variable_IO

        #region Variable_PLC

        private MCProtocol mPLC = null;

        public int _imageSave_ErrorCount = 0; // 이미지 Save Error Count

        public int _imageBGT_CA_ErrorCount = 0; // 이미지 Brightness 이상 Error Count (CA)
        public int _imageSHP_CA_ErrorCount = 0; // 이미지 Sharpness 이상 Error Count (CA)
        public int _imageLOC_CA_ErrorCount = 0; // 이미지 Location 이상 Error Count (CA)
        public int _imageNG_CA_ErrorCount = 0; // 이미지 불량(NG) Error Count (CA)
        public static Queue<bool> _imageNG_INTV_CA_ErrorCount = new Queue<bool>(); // Max Count내 이미지 불량(NG) Error Count (CA)

        public int _imageBGT_AN_ErrorCount = 0; // 이미지 Brightness 이상 Error Count (AN)
        public int _imageSHP_AN_ErrorCount = 0; // 이미지 Sharpness 이상 Error Count (AN)
        public int _imageLOC_AN_ErrorCount = 0; // 이미지 Location 이상 Error Count (AN)
        public int _imageNG_AN_ErrorCount = 0; // 이미지 불량(NG) Error Count (AN)
        public static Queue<bool> _imageNG_INTV_AN_ErrorCount = new Queue<bool>(); // Max Count내 이미지 불량(NG) Error Count (AN)

        public static CustomBool _isRisingErrBGT_CA = new CustomBool(false, nameof(_isRisingErrBGT_CA));
        public static CustomBool _isRisingErrSHP_CA = new CustomBool(false, nameof(_isRisingErrSHP_CA));
        public static CustomBool _isRisingErrLOC_CA = new CustomBool(false, nameof(_isRisingErrLOC_CA));
        public static CustomBool _isRisingErrNG_CA = new CustomBool(false, nameof(_isRisingErrNG_CA));
        public static CustomBool _isRisingErrNGINTV_CA = new CustomBool(false, nameof(_isRisingErrNGINTV_CA));

        public static CustomBool _isRisingErrBGT_AN = new CustomBool(false, nameof(_isRisingErrBGT_AN));
        public static CustomBool _isRisingErrSHP_AN = new CustomBool(false, nameof(_isRisingErrSHP_AN));
        public static CustomBool _isRisingErrLOC_AN = new CustomBool(false, nameof(_isRisingErrLOC_AN));
        public static CustomBool _isRisingErrNG_AN = new CustomBool(false, nameof(_isRisingErrNG_AN));
        public static CustomBool _isRisingErrNGINTV_AN = new CustomBool(false, nameof(_isRisingErrNGINTV_AN));

        private Thread m_Thread_PLC;

        public bool isPlcThRunning = false;

        private OperateResult<String> op_Cell_ID_CA;
        private OperateResult<String> op_Cell_ID_AN;

        private string mCell_ID_CA;
        private string mCell_ID_AN;
        private string mCarrier_ID;
        private string mStack_ID;

        private int mCommunicationErr = 0;
        private int mEdgestatusInterval;
        private int _imageSaveCnt;
        private const int PLC_RESULT_NULL = 0;  // PLC 결과값 초기값
        private const int PLC_RESULT_OK = 1;
        private const int PLC_RESULT_NG = 2;

        private const short PLC_IN_0 = 0;
        private const short PLC_IN_1 = 1;
        private const short PLC_OUT_0 = 0;
        private const short PLC_OUT_1 = 1;
        private const short PLC_OUT_2 = 2;

        //private bool mbl_Status;
        //private string mtr_Style;

        public enum PLC_ERROR_TYPE
        {
            OK,
            Data_Error,
            Connect_Error,
        }

        #endregion Variable_PLC

        #region Initialize

        public frm_Main()
        {
            InitializeComponent();
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            try
            {
                lightControl = new cls_LightControl(iniFilename);

                //250624 매개변수에 Log 추가
                mCamera = new cls_SentechSDK(ref mLog);


                //250624 Logding, initMain 위치 변경 (장치 연결 전에 initMain 작동시 thread에서 에러 발생)
                Loading();
                initMain(); // Thread 실행, 이벤트 선언 등.
                initIOLamp();

                // VPRO를 사용하지 않을 경우에 화면에 표시되지 않도록 수정함.
                if (m_VPRO_Use == false)
                {
                    if (materialTabControl1.TabPages.Contains(tabVPRO))
                        materialTabControl1.TabPages.Remove(tabVPRO);
                }

                // 최초 VPDL 로딩 해줘야만 처음 로딩시, 로딩 이슈를 해결할 수 있음.
                VPDL_initailize();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Form Loading Error: {ex.Message}");
            }
        }

        private void initMain()
        {
            try
            {
                // Log Thread Start
                mLog.OnLogMsg += AddLogList;
                mLog.keepThread = true;
                log_Thread = new Thread(new ThreadStart(mLog.WriteLogQueue));
                log_Thread.IsBackground = true;
                log_Thread.Start();

                // Status Check and Json Make Start
                EdgeStatus_Thread = new Thread(new ThreadStart(EdgeStatus_Th));
                EdgeStatus_Thread.IsBackground = true;
                EdgeStatus_Thread.Start();

                // timer start
                timer_drive_Tick(null, null);
                timer_drive.Start();

                // Detect Anomal Manual Button Event Map
                // Cathode
                _isRisingErrBGT_CA.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrSHP_CA.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrLOC_CA.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNG_CA.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNGINTV_CA.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;

                // Anode
                _isRisingErrBGT_AN.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrSHP_AN.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrLOC_AN.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNG_AN.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNGINTV_AN.OnValueChanged += _isRisingErrImageDetect_OnValueChanged;
            }
            catch { throw; }
        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                mLog.WriteLogDirect("FormClosing", "Form Closing Started...");

                _isRisingErrBGT_CA.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrSHP_CA.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrLOC_CA.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNG_CA.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNGINTV_CA.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;

                // Anode
                _isRisingErrBGT_AN.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrSHP_AN.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrLOC_AN.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNG_AN.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;
                _isRisingErrNGINTV_AN.OnValueChanged -= _isRisingErrImageDetect_OnValueChanged;

                // 종료 플래그 설정
                isEdgeStatusThRunning = false;
                isIoThRunning = false;
                isPlcThRunning = false;
                mLog.keepThread = false;

                // ViDi 자원 해제
                if (mViDi != null)
                {
                    try
                    {
                        mViDi.Dispose();
                        Task.Delay(1500).Wait();

                    }
                    catch (System.Exception ex)
                    {
                        mLog.WriteLogDirect("FormClosing", $"Error disposing mViDi: {ex.Message}");
                    }
                }

                mLog.WriteLogDirect("FormClosing", "Advan IO Initial...");
                IO_Reset_Run();

                mLog.WriteLogDirect("FormClosing", "Advan IO Thread Stop...");
                AdvanDevice.StopOutputThread();

                mLog.WriteLogDirect("FormClosing", "Stopping timers...");
                timer_IO?.Stop();
                timer_Heartbeat?.Stop();
                timer_drive?.Stop();

                mLog.WriteLogDirect("FormClosing", "Disconnecting cameras...");
                if (btnCathodeLive.Text.Contains("On")) LivePolarity(CAM_CATHODE, btnCathodeLive, mbCathode_Live);
                if (btnAnodeLive.Text.Contains("On")) LivePolarity(CAM_ANODE, btnAnodeLive, mbAnode_Live);

                mCamera?.Disconnect(CAM_CATHODE);
                mCamera?.Disconnect(CAM_ANODE);

                mLog.WriteLogDirect("FormClosing", "Waiting for threads to terminate...");

                // 스레드 종료 대기
                WaitForThreadTermination(EdgeStatus_Thread, "EdgeStatus Thread");
                WaitForThreadTermination(ModelChange_Thread, "Model Change Thread");
                WaitForThreadTermination(IO_Thread, "I/O Thread");
                WaitForThreadTermination(m_Thread_PLC, "PLC Thread");
                WaitForThreadTermination(log_Thread, "Log Thread");

                try { mPLC.Release(); } catch (System.Exception ex) { mLog.WriteLogDirect("FormClosing", "MC Protocol release..."); }
                try { AdvanDevice.Device_Di.Dispose(); AdvanDevice.Device_Do.Dispose(); } catch (System.Exception ex) { mLog.WriteLogDirect("FormClosing", "AdvanDevice Dispose..."); }

                mLog.WriteLogDirect("FormClosing", "Form Closing Completed...");

                mCamera = null;
                mDisplay = null;
                mLog = null;
                mDetect = null;
                mImage = null;
                edgeStatus = null;
                mPLC = null;
                lightControl = null;
                ini = null;
                EdgeStatus_Thread = null;
                ModelChange_Thread = null;
                IO_Thread = null;
                m_Thread_PLC = null;
                log_Thread = null;
                mViDi = null;

                Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Form Closing Error: {ex.Message}");
            }
        }

        private void WaitForThreadTermination(Thread thread, string threadName)
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Join(1500); // 1.5초 동안 스레드 종료 대기
                if (thread.IsAlive)
                {
                    thread.Abort();
                    mLog.WriteLogDirect("FormClosing", $"{threadName} did not terminate in time.");
                }
            }
        }

        private void initIOLamp()
        {
            lamp_in32_0 = new IconLamp[8] { ic_In32_0, ic_In32_1, ic_In32_2, ic_In32_3, ic_In32_4, ic_In32_5, ic_In32_6, ic_In32_7 };
            lamp_in32_1 = new IconLamp[8] { ic_In32_8, ic_In32_9, ic_In32_10, ic_In32_11, ic_In32_12, ic_In32_13, ic_In32_14, ic_In32_15 };
            lamp_Out32_0 = new IconLamp[8] { ic_Out32_0, ic_Out32_1, ic_Out32_2, ic_Out32_3, ic_Out32_4, ic_Out32_5, ic_Out32_6, ic_Out32_7 };
            lamp_Out32_1 = new IconLamp[8] { ic_Out32_8, ic_Out32_9, ic_Out32_10, ic_Out32_11, ic_Out32_12, ic_Out32_13, ic_Out32_14, ic_Out32_15 };

            // 사용하지 않는 램프는 모두 안보이게 꺼둔다.
            for (int i = 0; i < 8; i++)
            {
                lamp_in32_0[i].Visible = false;
                lamp_in32_1[i].Visible = false;
                lamp_Out32_0[i].Visible = false;
                lamp_Out32_1[i].Visible = false;
            }

            // 사용하지 않는 버튼도 꺼둔다.
            for (int i = 1; i <= 16; i++)
            {
                System.Windows.Forms.Control[] buttons = this.Controls.Find("button" + i, true);
                if (buttons.Length > 0 && buttons[0] is Button)
                {
                    ((Button)buttons[0]).Visible = false;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (mIO_IN_PLC_ONLINE == i)
                {
                    lamp_in32_0[i].TitleText = i.ToString() + " - " + "PLC Online";
                    lamp_in32_0[i].Visible = true;
                }
                else if (mIO_IN_CA_TRIG == i)
                {
                    lamp_in32_0[i].TitleText = i.ToString() + " - " + "Cathode Trigger";
                    lamp_in32_0[i].Visible = true;
                }
                else if (mIO_IN_AN_TRIG == i)
                {
                    lamp_in32_0[i].TitleText = i.ToString() + " - " + "Anode Trigger";
                    lamp_in32_0[i].Visible = true;
                }
                else if (mIO_IN_COMM_ERR_RESET == i)
                {
                    lamp_in32_0[i].TitleText = i.ToString() + " - " + "Communication Error Reset";
                    lamp_in32_0[i].Visible = true;
                }

                System.Windows.Forms.Control[] buttons = this.Controls.Find($"button{i + 1}", true);
                if (mIO_OUT_HEARTBEAT == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Heartbeat";
                    lamp_Out32_0[i].Visible = true;
                }
                else if (mIO_OUT_CA_READY == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Cathode Ready";
                    lamp_Out32_0[i].Visible = true;
                    if (buttons.Length > 0 && buttons[0] is Button) ((Button)buttons[0]).Visible = true;
                }
                else if (mIO_OUT_CA_RES_NG == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Cathode Result NG";
                    lamp_Out32_0[i].Visible = true;
                    if (buttons.Length > 0 && buttons[0] is Button) ((Button)buttons[0]).Visible = true;
                }
                else if (mIO_OUT_CA_RES_OK == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Cathode Result OK";
                    lamp_Out32_0[i].Visible = true;
                    if (buttons.Length > 0 && buttons[0] is Button) ((Button)buttons[0]).Visible = true;
                }
                else if (mIO_OUT_AN_READY == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Anode Ready";
                    lamp_Out32_0[i].Visible = true;
                    if (buttons.Length > 0 && buttons[0] is Button) ((Button)buttons[0]).Visible = true;
                }
                else if (mIO_OUT_AN_RES_NG == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Anode Result NG";
                    lamp_Out32_0[i].Visible = true;
                    if (buttons.Length > 0 && buttons[0] is Button) ((Button)buttons[0]).Visible = true;
                }
                else if (mIO_OUT_AN_RES_OK == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Anode Result OK";
                    lamp_Out32_0[i].Visible = true;
                    if (buttons.Length > 0 && buttons[0] is Button) ((Button)buttons[0]).Visible = true;
                }
                else if (mIO_OUT_COMM_ERROR == i)
                {
                    lamp_Out32_0[i].TitleText = i.ToString() + " - " + "Communication Error";
                    lamp_Out32_0[i].Visible = true;
                    if (buttons.Length > 0 && buttons[0] is Button) ((Button)buttons[0]).Visible = true;
                }
            }
        }

        private void btn_Maintenance_Click(object sender, EventArgs e)
        {
            // 저장 여부 확인
            DialogResult result = MessageBox.Show(
                "Would you like to save the changes?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                // 메모리 저장
                mProcess = tbxSetupProcess.Text;
                mLineNo = tbxLineNo.Text;
                mPosition = rdoSetupLineT.Checked ? "TOP" : "BOTTOM";
                mImageSaveDir = lbl_LocalImageSavePath.Text;
                mImageSaveRatio = int.Parse(tbxImageSaveRatio.Text);
                mVissSaveDir = lbl_NasSavePath.Text;
                if (!string.IsNullOrEmpty(cmbSavePathFormatVISS.Text)) mVissSaveDirFormat = cmbSavePathFormatVISS.Text.Substring(0, 2);
                if (!string.IsNullOrEmpty(cmbIMGSaveErrFormat.Text)) mImageSaveErrFormat = cmbIMGSaveErrFormat.Text.Substring(0, 2);

                mWorkSpacePath = lbl_WorkspacePath.Text;
                m_VPDL_Bypass = rdoVpdlBypassTrue.Checked ? true : false;
                m_VPDL_Locate = rdoVpdlLocateTrue.Checked ? true : false;

                mLocateLocalSaveDir = lbl_LocateLocalSavePath.Text;
                mLocateVISSSaveDir = lbl_LocateVISSSavePath.Text;
                mHeatmapLocalSaveDir = lbl_HeatmapLocalSavePath.Text;
                mHeatmapVISSSaveDir = lbl_HeatmapVISSSavePath.Text;
                mJsonSaveDir = lbl_JsonSavePath.Text;
                mVPROLocalSaveDir = lbl_VproSavePath.Text;

                mCathode_Vision_IP = txt_CathodeVision_IP.Text.Trim();
                mAnode_Vision_IP = txt_AnodeVision_IP.Text.Trim();
                mCathode_Reverse = rdoCamRevCaTrue.Checked ? true : false;
                mAnode_Reverse = rdoCamRevAnTrue.Checked ? true : false;
                mLightControllerPort = cmbLightControllerPort.Text;
                mLightControllerMaker = cmbLightControllerMaker.Text;
                m_IO_Num = (int)num_IO_Index.Value;

                m_PLC_Bypass = rdoPlcBypassTrue.Checked ? true : false;
                mEdgestatusInterval = int.Parse(tbxEdgeStatusInterval.Text);
                mModelChangeTimout = int.Parse(tbxModelChangeTimeout.Text);
                m_VPRO_Bypass = rdoVproBypassTrue.Checked ? true : false;
                m_VPRO_Use = rdoVproUseTrue.Checked ? true : false;
                cls_GlobalValue.ModelPath = lbl_VproModelPath.Text;

                // ini 저장
                ini.WriteIniValue("Maintenance", "Process", mProcess);
                ini.WriteIniValue("Maintenance", "LineNo", mLineNo);
                ini.WriteIniValue("Maintenance", "Position", mPosition);
                ini.WriteIniValue("Maintenance", "SavePath", mImageSaveDir);
                ini.WriteIniValue("Maintenance", "ImageSaveRatio", mImageSaveRatio.ToString());
                // 로컬저장용량제한 옵션
                ini.WriteIniValue("Maintenance", "NasSavePath", mVissSaveDir);
                ini.WriteIniValue("Maintenance", "VissSaveDirFormat", mVissSaveDirFormat);

                ini.WriteIniValue("Maintenance", "ImageSaveErrFormat", mImageSaveErrFormat);

                ini.WriteIniValue("Maintenance", "WorkspaceSavePath", mWorkSpacePath);
                ini.WriteIniValue("Maintenance", "VPDL_Bypass", m_VPDL_Bypass.ToString());
                ini.WriteIniValue("Maintenance", "VPDL_Locate", m_VPDL_Locate.ToString());

                ini.WriteIniValue("Maintenance", "LocateLocalSavePath", mLocateLocalSaveDir);
                ini.WriteIniValue("Maintenance", "LocateVISSSavePath", mLocateVISSSaveDir);
                ini.WriteIniValue("Maintenance", "HeatmapLocalSavePath", mHeatmapLocalSaveDir);
                ini.WriteIniValue("Maintenance", "HeatmapVISSSavePath", mHeatmapVISSSaveDir);

                ini.WriteIniValue("Maintenance", "JsonSavePath", mJsonSaveDir);
                ini.WriteIniValue("Maintenance", "VproLocalSavePath", mVPROLocalSaveDir);

                ini.WriteIniValue("Camera", "Cathode_Vision_IP", mCathode_Vision_IP);
                ini.WriteIniValue("Camera", "Anode_Vision_IP", mAnode_Vision_IP);
                ini.WriteIniValue("CameraSetup", "Cathode_Reverse", mCathode_Reverse.ToString());
                ini.WriteIniValue("CameraSetup", "Anode_Reverse", mAnode_Reverse.ToString());
                ini.WriteIniValue("Camera", "LightControllerPort", mLightControllerPort.ToString());
                ini.WriteIniValue("Camera", "LightControllerMaker", mLightControllerMaker.ToString());

                ini.WriteIniValue("Maintenance", "IOIndex", Convert.ToString((int)m_IO_Num));

                ini.WriteIniValue("PLC", "PLC_Bypass", m_PLC_Bypass.ToString());

                ini.WriteIniValue("Maintenance", "EdgeStatusInterval", mEdgestatusInterval.ToString());
                ini.WriteIniValue("Maintenance", "ModelChangeTimout", mModelChangeTimout.ToString());

                ini.WriteIniValue("VPRO", "VPRO_Bypass", m_VPRO_Bypass.ToString());
                ini.WriteIniValue("VPRO", "VPRO_Use", m_VPRO_Use.ToString());
                ini.WriteIniValue("VPRO", "ModelSavePath", cls_GlobalValue.ModelPath);

                ResetDirty();
                MessageBox.Show("Settings have been saved.", "Save");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"저장 중 오류가 발생했습니다: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Initialize

        #region Loading

        private void Loading()
        {
            cls_LoadingProvider loading = new cls_LoadingProvider();
            int loadingCount = 0;

            List<LoadingStep> loadingSteps = new List<LoadingStep>
            {
                new LoadingStep("Config-File Loading", LoadingIni),
                new LoadingStep("Deep Learning", LoadingVPDL),
                new LoadingStep("Cathode_Vision Connecting", LoadingCameraCA),
                new LoadingStep("Anode_Vision Connecting", LoadingCameraAN),
                new LoadingStep("I/O Connecting", LoadingIO),
                new LoadingStep("PLC Connecting", LoadingPLC),
                new LoadingStep("VisionPro Loading", LoadingVPRO),
                new LoadingStep("Light Controller", LoadingLight)
            };

            loading.Open(loadingSteps.Count);

            foreach (LoadingStep step in loadingSteps)
            {
                PerformLoadingStep(loading, step, ref loadingCount);
            }

            loading.Close();

            DisplayViewSetup();

            loading.Join();
        }

        private void DisplayViewSetup()
        {
            InitSetting();

            try
            {
                BtnCameraSetupView.BackColor = Color.Lime;
                ViewCameraSetup();
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("system", $"Display View Setup Error : {ex.Message}");
            }

            cogDisplayStatusBarV21.Display = displayCathode.Display;
            cogDisplayToolbarV21.Display = displayCathode.Display;
            displayCathode.Display.AutoFit = true;

            cogDisplayStatusBarV22.Display = displayAnode.Display;
            cogDisplayToolbarV22.Display = displayAnode.Display;
            displayAnode.Display.AutoFit = true;
        }

        private void PerformLoadingStep(cls_LoadingProvider loading, LoadingStep step, ref int loadingCount)
        {
            bool isSuccess = false;
            try
            {
                loading.UpdateProgress($"{step.Name}........", ++loadingCount);
                isSuccess = step.Action();
            }
            catch (System.Exception)
            {
                isSuccess = false;
            }

            if (isSuccess)
            {
                loading.UpdateProgress($"{step.Name}...OK", loadingCount);
                mLog.WriteLog("Loading", $"{step.Name}...OK");
            }
            else
            {
                loading.UpdateProgress($"{step.Name}...Fail", loadingCount);
                mLog.WriteLog("Loading", $"{step.Name}...Fail");
            }
        }

        private class LoadingStep
        {
            public string Name { get; }
            public Func<bool> Action { get; }

            public LoadingStep(string name, Func<bool> action)
            {
                Name = name;
                Action = action;
            }
        }

        public bool LoadingIni()
        {
            try
            {
                //Maintenance
                mProcess = ini.ReadIniString("Maintenance", "Process", "CRACK");
                mLineNo = ini.ReadIniString("Maintenance", "LineNo", "LINE00");
                mPosition = ini.ReadIniString("Maintenance", "Position", "TOP");
                mImageSaveDir = ini.ReadIniString("Maintenance", "SavePath", @"D:\IMAGE");
                mImageSaveRatio = ini.ReadIniInt("Maintenance", "ImageSaveRatio", "80");
                mVissSaveDir = ini.ReadIniString("Maintenance", "NasSavePath", @"D:\NAS");
                mVissSaveDirFormat = ini.ReadIniString("Maintenance", "VissSaveDirFormat", @"");
                mImageSaveErrFormat = ini.ReadIniString("Maintenance", "ImageSaveErrFormat", @"");
                mWorkSpacePath = ini.ReadIniString("Maintenance", "WorkspaceSavePath", @"D:\MODEL\TAB_WELDING_MODEL.vrws");
                m_VPDL_Bypass = ini.ReadIniBool("Maintenance", "VPDL_Bypass", "true");
                m_VPDL_Locate = ini.ReadIniBool("Maintenance", "VPDL_Locate", "true");
                mLocateLocalSaveDir = ini.ReadIniString("Maintenance", "LocateLocalSavePath", @"D:\LOCATE_LOCAL"); ;
                mLocateVISSSaveDir = ini.ReadIniString("Maintenance", "LocateVISSSavePath", @"D:\LOCATE_VISS"); ;
                mHeatmapLocalSaveDir = ini.ReadIniString("Maintenance", "HeatmapLocalSavePath", @"D:\HEATMAP_LOCAL");
                mHeatmapVISSSaveDir = ini.ReadIniString("Maintenance", "HeatmapVISSSavePath", @"D:\HEATMAP_VISS");
                mJsonSaveDir = ini.ReadIniString("Maintenance", "JsonSavePath", @"D:\JSON");
                mVPROLocalSaveDir = ini.ReadIniString("Maintenance", "VproLocalSavePath", @"D:\VPRO_LOCAL");
                m_IO_Num = ini.ReadIniInt("Maintenance", "IOIndex", "0");
                mEdgestatusInterval = ini.ReadIniInt("Maintenance", "EdgeStatusInterval", "10");
                mModelChangeTimout = ini.ReadIniInt("Maintenance", "ModelChangeTimout", "600");

                //Camera
                mCathode_Vision_IP = ini.ReadIniString("Camera", "Cathode_Vision_IP", "192.168.0.110");
                mAnode_Vision_IP = ini.ReadIniString("Camera", "Anode_Vision_IP", "192.168.0.108");
                mLightControllerPort = ini.ReadIniString("Camera", "LightControllerPort", "COM1");
                mLightControllerMaker = ini.ReadIniString("Camera", "LightControllerMaker", "LFINE");

                //Camera Setup
                mCathode_Reverse = ini.ReadIniBool("CameraSetup", "Cathode_Reverse", "true");
                mAnode_Reverse = ini.ReadIniBool("CameraSetup", "Anode_Reverse", "true");

                mCathode_Exposure = ini.ReadIniDouble("CameraSetup", "Cathode_Exposure", "500");
                mCathode_Gain = ini.ReadIniDouble("CameraSetup", "Cathode_Gain", "1");
                mAnode_Exposure = ini.ReadIniDouble("CameraSetup", "Anode_Exposure", "500");
                mAnode_Gain = ini.ReadIniDouble("CameraSetup", "Anode_Gain", "1");

                mCathode_Light = ini.ReadIniString("CameraSetup", "Cathode_Light", "400");
                mAnode_Light = ini.ReadIniString("CameraSetup", "Anode_Light", "400");

                mCathode_White_R = ini.ReadIniString("CameraSetup", "Cathode_White_R", "45");
                mCathode_White_G = ini.ReadIniString("CameraSetup", "Cathode_White_G", "5");
                mCathode_White_B = ini.ReadIniString("CameraSetup", "Cathode_White_B", "80");
                mAnode_White_R = ini.ReadIniString("CameraSetup", "Anode_White_R", "45");
                mAnode_White_G = ini.ReadIniString("CameraSetup", "Anode_White_G", "5");
                mAnode_White_B = ini.ReadIniString("CameraSetup", "Anode_White_B", "80");

                // Camera Guide Line
                camGuideLine_line1_x = ini.ReadIniInt("CameraGuideLine", "Crack_Line1_x", "354");
                camGuideLine_line2_x = ini.ReadIniInt("CameraGuideLine", "Crack_Line2_x", "2094");
                camGuideLine_line3_y = ini.ReadIniInt("CameraGuideLine", "Crack_Line3_y", "1270");
                brightPositionCA_x = ini.ReadIniInt("CameraGuideLine", "brightPositionCA_x", "1174");
                brightPositionCA_y = ini.ReadIniInt("CameraGuideLine", "brightPositionCA_y", "974");
                brightPositionAN_x = ini.ReadIniInt("CameraGuideLine", "brightPositionAN_x", "1174");
                brightPositionAN_y = ini.ReadIniInt("CameraGuideLine", "brightPositionAN_y", "974");

                // IO pin
                mIO_IN_PLC_ONLINE = ini.ReadIniInt("IO", "IO_IN_PLC_ONLINE", "");
                mIO_IN_CA_TRIG = ini.ReadIniInt("IO", "IO_IN_CA_TRIG", "");
                mIO_IN_AN_TRIG = ini.ReadIniInt("IO", "IO_IN_AN_TRIG", "");
                mIO_IN_COMM_ERR_RESET = ini.ReadIniInt("IO", "IO_IN_COMM_ERR_RESET", "");

                mIO_OUT_HEARTBEAT = ini.ReadIniInt("IO", "IO_OUT_HEARTBEAT", "");
                mIO_OUT_CA_READY = ini.ReadIniInt("IO", "IO_OUT_CA_READY", "");
                mIO_OUT_CA_RES_NG = ini.ReadIniInt("IO", "IO_OUT_CA_RES_NG", "");
                mIO_OUT_CA_RES_OK = ini.ReadIniInt("IO", "IO_OUT_CA_RES_OK", "");
                mIO_OUT_AN_READY = ini.ReadIniInt("IO", "IO_OUT_AN_READY", "");
                mIO_OUT_AN_RES_NG = ini.ReadIniInt("IO", "IO_OUT_AN_RES_NG", "");
                mIO_OUT_AN_RES_OK = ini.ReadIniInt("IO", "IO_OUT_AN_RES_OK", "");
                mIO_OUT_COMM_ERROR = ini.ReadIniInt("IO", "IO_OUT_COMM_ERROR", "");

                //PLC IP
                m_PLC_IP = ini.ReadIniString("PLC", "PLC_IP", "192.168.251.110");
                m_PLC_Port = ini.ReadIniInt("PLC", "PLC_Port", "4000");
                m_PLC_Bypass = ini.ReadIniBool("PLC", "PLC_Bypass", "true");

                //PLC Address
                mPlc_Read_CELLID_CA = ini.ReadIniString("PLC", "CELLID_CA", "");
                mPlc_Read_CELLID_AN = ini.ReadIniString("PLC", "CELLID_AN", "");
                mPlc_Read_Carrier_ID = ini.ReadIniString("PLC", "Carrier_id", "");
                mPlc_Read_Stack_ID = ini.ReadIniString("PLC", "Stack_id", "");
                mPlc_Read_Model_No = ini.ReadIniString("PLC", "Model_No", "");
                mPlc_Read_Change_Model_PLC = ini.ReadIniString("PLC", "Change_Model_PLC", "");
                mPlc_Read_VisionIO_Reset = ini.ReadIniString("PLC", "Vision_IO_Reset", "");
                mPlc_Read_Image_Save_Err_Reset = ini.ReadIniString("PLC", "Image_Save_Error_Reset", "");
                mPlc_Read_Image_Detect_Err_Reset = ini.ReadIniString("PLC", "Image_Detect_Error_Reset", "");

                mPlc_Write_HeartBeat = ini.ReadIniString("PLC", "HeartBeat", "");
                mPlc_Write_Cathode_Result = ini.ReadIniString("PLC", "Cathode_Result", "");
                mPlc_Write_Anode_Result = ini.ReadIniString("PLC", "Anode_Result", "");
                mPlc_Write_Cathode_Result_Code = ini.ReadIniString("PLC", "Cathode_Result_Code", "");
                mPlc_Write_Anode_Result_Code = ini.ReadIniString("PLC", "Anode_Result_Code", "");
                mPlc_Write_Change_Model_PC = ini.ReadIniString("PLC", "Change_Model_PC", "");
                mPlc_Write_Complete_VisionIO_Reset = ini.ReadIniString("PLC", "Complete_Vision_IO_Reset", "");
                mPlc_Write_Image_Save_Err = ini.ReadIniString("PLC", "Image_Save_Error", "");
                mPlc_Write_Cathode_Image_Detect_Err = ini.ReadIniString("PLC", "Cathode_Image_Detect_Error", "");
                mPlc_Write_Anode_Image_Detect_Err = ini.ReadIniString("PLC", "Anode_Image_Detect_Error", "");
                mPlc_Write_Cathode_Cell_Id = ini.ReadIniString("PLC", "Write_Cathode_Cell_Id", "");
                mPlc_Write_Anode_Cell_Id = ini.ReadIniString("PLC", "Write_Anode_Cell_Id", "");
                mPlc_Write_Cathode_Vpro_Result = ini.ReadIniString("PLC", "Cathode_Vpro_Result", "");
                mPlc_Write_Anode_Vpro_Result = ini.ReadIniString("PLC", "Anode_Vpro_Result", "");

                // VPRO
                m_VPRO_Use = ini.ReadIniBool("VPRO", "VPRO_Use", "false");
                m_VPRO_Bypass = ini.ReadIniBool("VPRO", "VPRO_Bypass", "true");
                cls_GlobalValue.ModelPath = ini.ReadIniString("VPRO", "ModelSavePath", @"D:\MODEL");
                cls_GlobalValue.LastModelPath = ini.ReadIniString("VPRO", "LastModelSavePath", "");


                // Threshold
                mThreshold_CA_R1 = ini.ReadIniDouble("Threshold", "Threshold_CA_R1", @"0.1");
                mThreshold_CA_R2 = ini.ReadIniDouble("Threshold", "Threshold_CA_R2", @"0.1");
                mThreshold_CA_R3 = ini.ReadIniDouble("Threshold", "Threshold_CA_R3", @"0.1");
                mThreshold_AN_R1 = ini.ReadIniDouble("Threshold", "Threshold_AN_R1", @"0.1");
                mThreshold_AN_R2 = ini.ReadIniDouble("Threshold", "Threshold_AN_R2", @"0.1");
                mThreshold_AN_R3 = ini.ReadIniDouble("Threshold", "Threshold_AN_R3", @"0.1");

                // detectAnomal
                mBright_Det_YN = ini.ReadIniBool("Detect", "Bright_Det_YN", "false");
                mSharp_Det_YN = ini.ReadIniBool("Detect", "Sharp_Det_YN", "false");
                mLocation_Det_YN = ini.ReadIniBool("Detect", "Location_Det_YN", "false");
                mNG_Det_YN = ini.ReadIniBool("Detect", "NG_Det_YN", "false");
                mNG_ITV_Det_YN = ini.ReadIniBool("Detect", "NG_ITV_Det_YN", "false");
                mLog_Det_YN = ini.ReadIniBool("Detect", "Log_Det_YN", "false");

                mBright_Det_CNT = ini.ReadIniInt("Detect", "Bright_Det_CNT", "50");
                mSharp_Det_CNT = ini.ReadIniInt("Detect", "Sharp_Det_CNT", "50");
                mLocation_Det_CNT = ini.ReadIniInt("Detect", "Location_Det_CNT", "50");
                mNG_Det_CNT = ini.ReadIniInt("Detect", "NG_Det_CNT", "50");
                mNG_ITV_Det_CNT = ini.ReadIniInt("Detect", "NG_ITV_Det_CNT", "50");
                mNG_ITV_Det_MAX_CNT = ini.ReadIniInt("Detect", "NG_ITV_Det_MAX_CNT", "60");

                mBright_Rank_Qty = ini.ReadIniInt("Detect", "Bright_Rank_Qty", "10");

                mBright_CA_STD_VAL = ini.ReadIniDouble("Detect", "Bright_CA_STD_VAL", "10");
                mBright_CA_LOW_VAL = ini.ReadIniDouble("Detect", "Bright_CA_LOW_VAL", "10");
                mBright_CA_UPP_VAL = ini.ReadIniDouble("Detect", "Bright_CA_UPP_VAL", "10");
                mSharp_CA_STD_VAL = ini.ReadIniDouble("Detect", "Sharp_CA_STD_VAL", "10");
                mSharp_CA_LOW_VAL = ini.ReadIniDouble("Detect", "Sharp_CA_LOW_VAL", "10");

                mBright_AN_STD_VAL = ini.ReadIniDouble("Detect", "Bright_AN_STD_VAL", "10");
                mBright_AN_LOW_VAL = ini.ReadIniDouble("Detect", "Bright_AN_LOW_VAL", "10");
                mBright_AN_UPP_VAL = ini.ReadIniDouble("Detect", "Bright_AN_UPP_VAL", "10");
                mSharp_AN_STD_VAL = ini.ReadIniDouble("Detect", "Sharp_AN_STD_VAL", "10");
                mSharp_AN_LOW_VAL = ini.ReadIniDouble("Detect", "Sharp_AN_LOW_VAL", "10");

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private bool LoadingVPDL()
        {
            if (m_VPDL_Bypass)
            {
                mLog.WriteLog("Loading", $"DeepLearning is Bypass");
                return false;
            }
            else
            {
                try
                {
                    mViDi.Init(mWorkSpacePath);
                    if (mViDi.Workspace == null) return false;
                    else return true;
                }
                catch (ViDi2.Exception ex)
                {
                    mViDi.Workspace = null;
                    MessageBox.Show($"VPDL LOAD FAIL : {ex.Message}");
                    return false;
                }
            }
        }

        private bool LoadingCameraCA()
        {
            try
            {
                if (Cathode_Vision_Init(mCathode_Vision_IP))
                {
                    AdvanDevice.OutputStart(mIO_OUT_CA_READY, true, 0, 0, 0);
                    mbCathode_Connect = true;

                    mCamera.ExposureValue(CAM_CATHODE, mCathode_Exposure.ToString());
                    mCamera.GainValue(CAM_CATHODE, mCathode_Gain.ToString());

                    try
                    {
                        mCamera.WhiteBalance_SetVal(CAM_CATHODE, "Red", mCathode_White_R);
                        mCamera.WhiteBalance_SetVal(CAM_CATHODE, "Green", mCathode_White_G);
                        mCamera.WhiteBalance_SetVal(CAM_CATHODE, "Blue", mCathode_White_B);

                        tbxCaWbR.Text = mCathode_White_R;
                        tbxCaWbG.Text = mCathode_White_G;
                        tbxCaWbB.Text = mCathode_White_B;
                    }
                    catch (System.Exception ex)
                    {
                        mLog.WriteLog("Loading", $"Cathode WhiteBalance Set Fail : {ex.Message}");
                    }

                    return true;
                }
                else
                {
                    mbCathode_Connect = false;
                    AdvanDevice.OutputStart(mIO_OUT_CA_READY, false, 0, 0, 0);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("Loading", $"Cathode Vision Error : {ex.Message}");
                mbCathode_Connect = false;
                AdvanDevice.OutputStart(mIO_OUT_CA_READY, false, 0, 0, 0);
                return false;
            }
        }

        private bool LoadingCameraAN()
        {
            try
            {
                if (Anode_Vision_Init(mAnode_Vision_IP))
                {
                    AdvanDevice.OutputStart(mIO_OUT_AN_READY, true, 0, 0, 0);
                    mbAndoe_Connect = true;
                    mCamera.ExposureValue(CAM_ANODE, mAnode_Exposure.ToString());
                    mCamera.GainValue(CAM_ANODE, mAnode_Gain.ToString());
                    try
                    {
                        mCamera.WhiteBalance_SetVal(CAM_ANODE, "Red", mAnode_White_R);
                        mCamera.WhiteBalance_SetVal(CAM_ANODE, "Green", mAnode_White_G);
                        mCamera.WhiteBalance_SetVal(CAM_ANODE, "Blue", mAnode_White_B);

                        tbxAnWbR.Text = mAnode_White_R;
                        tbxAnWbG.Text = mAnode_White_G;
                        tbxAnWbB.Text = mAnode_White_B;
                    }
                    catch (System.Exception ex)
                    {
                        mLog.WriteLog("Loading", $"Anode WhiteBalance Set Fail : {ex.Message}");
                    }


                    return true;
                }
                else
                {
                    mbAndoe_Connect = false;
                    AdvanDevice.OutputStart(mIO_OUT_AN_READY, false, 0, 0, 0);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("Loading", $"Anode Vision Error : {ex.Message}");
                mbAndoe_Connect = false;
                AdvanDevice.OutputStart(mIO_OUT_AN_READY, false, 0, 0, 0);
                return false;
            }
        }

        private bool LoadingIO()
        {
            try
            {
                if (OpenIOCard(m_IO_Num))
                {
                    IO_Thread = new Thread(new ThreadStart(IO_Read_Th));
                    IO_Thread.IsBackground = true;
                    IO_Thread.Start();
                    timer_IO.Start();
                    timer_Heartbeat.Start();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private bool LoadingPLC()
        {
            try
            {
                if (PLC_Initialize())
                {
                    if (btn_PLC_Connect.Text == "Connect")
                    {
                        //PLC 메모리맵 초기화 (Write만 초기화함)
                        if (!string.IsNullOrEmpty(mPlc_Write_Complete_VisionIO_Reset)) mPLC.WriteShort(mPlc_Write_Complete_VisionIO_Reset, PLC_OUT_0);
                        if (!string.IsNullOrEmpty(mPlc_Write_Image_Save_Err)) mPLC.WriteShort(mPlc_Write_Image_Save_Err, mImageSaveErrFormat.Equals("01") ? PLC_OUT_0 : PLC_OUT_1);
                        if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Image_Detect_Err)) mPLC.WriteShort(mPlc_Write_Cathode_Image_Detect_Err, PLC_OUT_0);
                        if (!string.IsNullOrEmpty(mPlc_Write_Anode_Image_Detect_Err)) mPLC.WriteShort(mPlc_Write_Anode_Image_Detect_Err, PLC_OUT_0);
                        if (!string.IsNullOrEmpty(mPlc_Write_Change_Model_PC)) mPLC.WriteShort(mPlc_Write_Change_Model_PC, PLC_OUT_0);
                        if (!string.IsNullOrEmpty(mPlc_Write_HeartBeat)) mPLC.WriteShort(mPlc_Write_HeartBeat, PLC_OUT_0);
                        if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Result)) mPLC.WriteShort(mPlc_Write_Cathode_Result, PLC_RESULT_NULL);
                        if (!string.IsNullOrEmpty(mPlc_Write_Anode_Result)) mPLC.WriteShort(mPlc_Write_Anode_Result, PLC_RESULT_NULL);
                        if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Result_Code)) mPLC.WriteShort(mPlc_Write_Cathode_Result_Code, PLC_RESULT_NULL);
                        if (!string.IsNullOrEmpty(mPlc_Write_Anode_Result_Code)) mPLC.WriteShort(mPlc_Write_Anode_Result_Code, PLC_RESULT_NULL);
                        if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Cell_Id)) mPLC.WriteString(mPlc_Write_Cathode_Cell_Id, "");
                        if (!string.IsNullOrEmpty(mPlc_Write_Anode_Cell_Id)) mPLC.WriteString(mPlc_Write_Anode_Cell_Id, "");
                        if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Vpro_Result)) mPLC.WriteShort(mPlc_Write_Cathode_Vpro_Result, PLC_OUT_1);
                        if (!string.IsNullOrEmpty(mPlc_Write_Anode_Vpro_Result)) mPLC.WriteShort(mPlc_Write_Anode_Vpro_Result, PLC_OUT_1);

                        if (string.IsNullOrEmpty(mPlc_Read_CELLID_CA)) lbl_Cell_id_CA.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_CELLID_AN)) lbl_Cell_id_AN.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_Carrier_ID)) lbl_Carrier_id.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_Stack_ID)) lbl_Stack_id.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_Model_No)) lbl_Model_No.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_Change_Model_PLC)) lbl_Change_Model_PLC.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_VisionIO_Reset)) lbl_Vision_IO_Reset.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_Image_Save_Err_Reset)) lbl_Image_Save_Error_Reset.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Read_Image_Detect_Err_Reset)) lbl_Image_Detect_Error_Reset.Text = "";

                        if (string.IsNullOrEmpty(mPlc_Write_Complete_VisionIO_Reset)) lbl_Complete_Vision_IO_Reset.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Image_Save_Err)) lbl_Image_Save_Error.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Cathode_Image_Detect_Err)) lbl_Cathode_Image_Detect_Error.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Anode_Image_Detect_Err)) lbl_Anode_Image_Detect_Error.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Change_Model_PC)) lbl_Change_Model_PC.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_HeartBeat)) lbl_HeartBeat.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Cathode_Result)) lbl_Cathode_Result.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Anode_Result)) lbl_Anode_Result.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Cathode_Result_Code)) lbl_Cathode_Result_Code.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Anode_Result_Code)) lbl_Anode_Result_Code.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Cathode_Cell_Id)) lbl_Cathode_Write_Cell_id.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Anode_Cell_Id)) lbl_Anode_Write_Cell_id.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Cathode_Vpro_Result)) lbl_Cathode_VPRO_Result.Text = "";
                        if (string.IsNullOrEmpty(mPlc_Write_Anode_Vpro_Result)) lbl_Anode_VPRO_Result.Text = "";
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
            finally
            {
                StartPlcThread();
            }
        }

        private bool LoadingVPRO()
        {
            if (m_VPRO_Use)
            {
                try
                {
                    cls_GlobalValue.Model = new cls_Model();

                    if (cls_GlobalValue.Model.Model_Load(cls_GlobalValue.LastModelPath))
                    {
                        CathodeToolBlock = CogSerializer.DeepCopyObject(cls_GlobalValue.Model.CathodeToolBlock) as CogToolBlock;
                        AnodeToolBlock = CogSerializer.DeepCopyObject(cls_GlobalValue.Model.AnodeToolBlock) as CogToolBlock;
                        //displayCathode.Tool = CathodeToolBlock;
                        //displayAnode.Tool = AnodeToolBlock;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (System.Exception)
                {
                    return false;
                }

            }
            return false;
        }

        private bool LoadingLight()
        {
            try
            {
                if (mLightControllerPort == "")
                {
                    return false;
                }
                else
                    return true;
                // 조명 로딩 초기화 방법 확인 필요
                //if (lightControl.IsConnected())
                //{
                //return true;
                //}
                //else { return false; }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private void InitSetting()
        {
            // Maintenence Tab
            tbxSetupProcess.Text = mProcess;
            tbxLineNo.Text = mLineNo;
            rdoSetupLineT.Checked = mPosition == "TOP" ? true : false;
            rdoSetupLineB.Checked = mPosition == "BOTTOM" ? true : false;
            lbl_LocalImageSavePath.Text = mImageSaveDir.Trim();
            lbl_NasSavePath.Text = mVissSaveDir.Trim();
            lbl_WorkspacePath.Text = mWorkSpacePath.Trim();
            lbl_VproModelPath.Text = cls_GlobalValue.ModelPath;
            lbl_LocateLocalSavePath.Text = mLocateLocalSaveDir.Trim();
            lbl_LocateVISSSavePath.Text = mLocateVISSSaveDir.Trim();
            lbl_HeatmapLocalSavePath.Text = mHeatmapLocalSaveDir.Trim();
            lbl_HeatmapVISSSavePath.Text = mHeatmapVISSSaveDir.Trim();
            lbl_JsonSavePath.Text = mJsonSaveDir.Trim();
            lbl_VproSavePath.Text = mVPROLocalSaveDir.Trim();

            if (mVissSaveDirFormat == "01")
                cmbSavePathFormatVISS.Text = @"01 - Root\yyyy\MMdd\";
            else if (mVissSaveDirFormat == "02")
                cmbSavePathFormatVISS.Text = @"02 - Root\Line#\yyyy\MMdd\HH\";   // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15
            else
                cmbSavePathFormatVISS.Text = "";


            if (mImageSaveErrFormat == "01")
                cmbIMGSaveErrFormat.Text = @"01 - 0 or 1";   // 이미지 저장오류 알림에서 oh1동은 0이 Default, 1이 Error 임.
            else if (mImageSaveErrFormat == "12")
                cmbIMGSaveErrFormat.Text = @"12 - 1 or 2";   // 이미지 저장오류 알림에서 oh2동은 1이 Default, 2가 Error 임.
            else
                cmbIMGSaveErrFormat.Text = "";

            rdoPlcBypassTrue.Checked = m_PLC_Bypass ? true : false;
            rdoPlcBypassFalse.Checked = m_PLC_Bypass ? false : true;
            rdoVpdlBypassTrue.Checked = m_VPDL_Bypass ? true : false;
            rdoVpdlBypassFalse.Checked = m_VPDL_Bypass ? false : true;
            rdoVpdlLocateTrue.Checked = m_VPDL_Locate ? true : false;
            rdoVpdlLocateFalse.Checked = m_VPDL_Locate ? false : true;
            rdoVproBypassTrue.Checked = m_VPRO_Bypass ? true : false;
            rdoVproBypassFalse.Checked = m_VPRO_Bypass ? false : true;
            rdoVproUseTrue.Checked = m_VPRO_Use ? true : false;
            rdoVproUseFalse.Checked = m_VPRO_Use ? false : true;
            txt_CathodeVision_IP.Text = mCathode_Vision_IP.Trim();
            txt_AnodeVision_IP.Text = mAnode_Vision_IP.Trim();
            rdoCamRevCaTrue.Checked = mCathode_Reverse ? true : false;
            rdoCamRevCaFalse.Checked = mCathode_Reverse ? false : true;
            rdoCamRevAnTrue.Checked = mAnode_Reverse ? true : false;
            rdoCamRevAnFalse.Checked = mAnode_Reverse ? false : true;
            cmbLightControllerPort.Text = mLightControllerPort;
            cmbLightControllerMaker.Text = mLightControllerMaker;
            num_IO_Index.Value = Convert.ToDecimal(m_IO_Num);

            // Model Tab
            lbl_Model.Text = cls_GlobalValue.LastModelFileName;
            //lbl_Model.Text = m_nLastModelNo.ToString() == "0" ? "" : m_nLastModelNo.ToString().PadLeft(2, '0') + "_" + m_strLastModel.Trim();
            // 패턴툴

            // Inspection Tab
            // bright , contrast
            tbxLightCA.Text = mCathode_Light;
            tbxLightAN.Text = mAnode_Light;
            tbxExpCa.Text = mCathode_Exposure.ToString();
            tbxExpAn.Text = mAnode_Exposure.ToString();
            tbxGainCa.Text = mCathode_Gain.ToString();
            tbxGainAn.Text = mAnode_Gain.ToString();
            // white balance r g b

            // PLC Tab
            tbx_PLC_IP.Text = string.IsNullOrEmpty(m_PLC_IP) ? "" : m_PLC_IP.Trim();
            tbx_PLC_Port.Text = m_PLC_Port.ToString() == "0" ? "" : m_PLC_Port.ToString();

            tbx_Cell_id_CA.Text = string.IsNullOrEmpty(mPlc_Read_CELLID_CA) ? "" : mPlc_Read_CELLID_CA.Trim();
            tbx_Cell_id_AN.Text = string.IsNullOrEmpty(mPlc_Read_CELLID_AN) ? "" : mPlc_Read_CELLID_AN.Trim();
            tbx_Carrier_id.Text = string.IsNullOrEmpty(mPlc_Read_Carrier_ID) ? "" : mPlc_Read_Carrier_ID.Trim();
            tbx_Stack_id.Text = string.IsNullOrEmpty(mPlc_Read_Stack_ID) ? "" : mPlc_Read_Stack_ID.Trim();
            tbx_Model_No.Text = string.IsNullOrEmpty(mPlc_Read_Model_No) ? "" : mPlc_Read_Model_No.Trim();
            tbx_Change_Model_PLC.Text = string.IsNullOrEmpty(mPlc_Read_Change_Model_PLC) ? "" : mPlc_Read_Change_Model_PLC.Trim();
            tbx_Vision_IO_Reset.Text = string.IsNullOrEmpty(mPlc_Read_VisionIO_Reset) ? "" : mPlc_Read_VisionIO_Reset.Trim();
            tbx_Image_Save_Error_Reset.Text = string.IsNullOrEmpty(mPlc_Read_Image_Save_Err_Reset) ? "" : mPlc_Read_Image_Save_Err_Reset.Trim();
            tbx_Image_Detect_Error_Reset.Text = string.IsNullOrEmpty(mPlc_Read_Image_Detect_Err_Reset) ? "" : mPlc_Read_Image_Detect_Err_Reset.Trim();

            tbx_HeartBeat.Text = string.IsNullOrEmpty(mPlc_Write_HeartBeat) ? "" : mPlc_Write_HeartBeat.Trim();
            tbx_Cathode_Result.Text = string.IsNullOrEmpty(mPlc_Write_Cathode_Result) ? "" : mPlc_Write_Cathode_Result.Trim();
            tbx_Anode_Result.Text = string.IsNullOrEmpty(mPlc_Write_Anode_Result) ? "" : mPlc_Write_Anode_Result.Trim();
            tbx_Cathode_Result_Code.Text = string.IsNullOrEmpty(mPlc_Write_Cathode_Result_Code) ? "" : mPlc_Write_Cathode_Result_Code.Trim();
            tbx_Anode_Result_Code.Text = string.IsNullOrEmpty(mPlc_Write_Anode_Result_Code) ? "" : mPlc_Write_Anode_Result_Code.Trim();
            tbx_Change_Model_PC.Text = string.IsNullOrEmpty(mPlc_Write_Change_Model_PC) ? "" : mPlc_Write_Change_Model_PC.Trim();
            tbx_Complete_Vision_IO_Reset.Text = string.IsNullOrEmpty(mPlc_Write_Complete_VisionIO_Reset) ? "" : mPlc_Write_Complete_VisionIO_Reset.Trim();
            tbx_Image_Save_Error.Text = string.IsNullOrEmpty(mPlc_Write_Image_Save_Err) ? "" : mPlc_Write_Image_Save_Err.Trim();
            tbx_Cathode_Image_Detect_Error.Text = string.IsNullOrEmpty(mPlc_Write_Cathode_Image_Detect_Err) ? "" : mPlc_Write_Cathode_Image_Detect_Err.Trim();
            tbx_Anode_Image_Detect_Error.Text = string.IsNullOrEmpty(mPlc_Write_Anode_Image_Detect_Err) ? "" : mPlc_Write_Anode_Image_Detect_Err.Trim();
            tbx_Cathode_Write_Cell_id.Text = string.IsNullOrEmpty(mPlc_Write_Cathode_Cell_Id) ? "" : mPlc_Write_Cathode_Cell_Id.Trim();
            tbx_Anode_Write_Cell_id.Text = string.IsNullOrEmpty(mPlc_Write_Anode_Cell_Id) ? "" : mPlc_Write_Anode_Cell_Id.Trim();
            tbx_Cathode_VPRO_Result.Text = string.IsNullOrEmpty(mPlc_Write_Cathode_Vpro_Result) ? "" : mPlc_Write_Cathode_Vpro_Result.Trim();
            tbx_Anode_VPRO_Result.Text = string.IsNullOrEmpty(mPlc_Write_Anode_Vpro_Result) ? "" : mPlc_Write_Anode_Vpro_Result.Trim();
            
            tbxThreshold_CR1.Text = Convert.ToString(mThreshold_CA_R1);
            tbxThreshold_CR2.Text = Convert.ToString(mThreshold_CA_R2);
            tbxThreshold_CR3.Text = Convert.ToString(mThreshold_CA_R3);
            tbxThreshold_AR1.Text = Convert.ToString(mThreshold_AN_R1);
            tbxThreshold_AR2.Text = Convert.ToString(mThreshold_AN_R2);
            tbxThreshold_AR3.Text = Convert.ToString(mThreshold_AN_R3);
        }

        #endregion Loading

        #region PLC

        private bool PLC_Initialize()
        {
            string l_plcStat_Text = "Initialize"; // Log 기록용 Text

            if (mPLC != null)
            {
                mPLC.Disconnect();
                l_plcStat_Text = "Reconnect";
            }

            mPLC = new MCProtocol(m_PLC_IP, m_PLC_Port);

            if (mPLC.Connect() == true)
            {
                this.Invoke(new Action(delegate ()
                {
                    btn_PLC_Connect.Text = "Connect";
                    btn_PLC_Connect.BackColor = Color.Lime;
                }));

                mLog.WriteLog("PLC", $"PLC {l_plcStat_Text}...OK");
                return true;
            }
            else
            {
                this.Invoke(new Action(delegate ()
                {
                    btn_PLC_Connect.Text = "DisConnect";
                    btn_PLC_Connect.BackColor = Color.LightGray;
                }));

                mLog.WriteLog("PLC", $"PLC {l_plcStat_Text}...Fail");
                return false;
            }
        }

        private void PLCDisConnect()
        {
            this.Invoke(new Action(delegate ()
            {
                if (mPLC != null)
                {
                    mPLC.Disconnect();
                    btn_PLC_Connect.Text = "DisConnect";
                    btn_PLC_Connect.BackColor = Color.LightGray;
                    mLog.WriteLog("PLC", "PLC Disconnect");
                }
            }));
        }

        private void btn_PLCInfo_Save_Click(object sender, EventArgs e)
        {
            // 저장 여부 확인
            DialogResult result = MessageBox.Show(
                "저장하시겠습니까?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // No 선택 시 아무 작업도 하지 않음
            if (result != DialogResult.Yes)
                return;

            try
            {
                m_PLC_IP = tbx_PLC_IP.Text.Trim();
                m_PLC_Port = Convert.ToInt32(tbx_PLC_Port.Text.Trim());

                mPlc_Read_CELLID_CA = tbx_Cell_id_CA.Text.Trim();
                mPlc_Read_CELLID_AN = tbx_Cell_id_AN.Text.Trim();
                mPlc_Read_Carrier_ID = tbx_Carrier_id.Text.Trim();
                mPlc_Read_Stack_ID = tbx_Stack_id.Text.Trim();
                mPlc_Read_Model_No = tbx_Model_No.Text.Trim();
                mPlc_Read_Change_Model_PLC = tbx_Change_Model_PLC.Text.Trim();
                mPlc_Read_VisionIO_Reset = tbx_Vision_IO_Reset.Text.Trim();
                mPlc_Read_Image_Save_Err_Reset = tbx_Image_Save_Error_Reset.Text.Trim();
                mPlc_Read_Image_Detect_Err_Reset = tbx_Image_Detect_Error_Reset.Text.Trim();

                mPlc_Write_HeartBeat = tbx_HeartBeat.Text.Trim();
                mPlc_Write_Cathode_Result = tbx_Cathode_Result.Text.Trim();
                mPlc_Write_Anode_Result = tbx_Anode_Result.Text.Trim();
                mPlc_Write_Cathode_Result_Code = tbx_Cathode_Result_Code.Text.Trim();
                mPlc_Write_Anode_Result_Code = tbx_Anode_Result_Code.Text.Trim();
                mPlc_Write_Change_Model_PC = tbx_Change_Model_PC.Text.Trim();
                mPlc_Write_Complete_VisionIO_Reset = tbx_Complete_Vision_IO_Reset.Text.Trim();
                mPlc_Write_Image_Save_Err = tbx_Image_Save_Error.Text.Trim();
                mPlc_Write_Cathode_Image_Detect_Err = tbx_Cathode_Image_Detect_Error.Text.Trim();
                mPlc_Write_Anode_Image_Detect_Err = tbx_Anode_Image_Detect_Error.Text.Trim();
                mPlc_Write_Cathode_Cell_Id = tbx_Cathode_Write_Cell_id.Text.Trim();
                mPlc_Write_Anode_Cell_Id = tbx_Anode_Write_Cell_id.Text.Trim();
                mPlc_Write_Cathode_Vpro_Result = tbx_Cathode_VPRO_Result.Text.Trim();
                mPlc_Write_Anode_Vpro_Result = tbx_Anode_VPRO_Result.Text.Trim();

                ini.WriteIniValue("PLC", "PLC_IP", m_PLC_IP);
                ini.WriteIniValue("PLC", "PLC_Port", Convert.ToString((int)m_PLC_Port));

                ini.WriteIniValue("PLC", "CELLID_CA", mPlc_Read_CELLID_CA);
                ini.WriteIniValue("PLC", "CELLID_AN", mPlc_Read_CELLID_AN);
                ini.WriteIniValue("PLC", "Carrier_id", mPlc_Read_Carrier_ID);
                ini.WriteIniValue("PLC", "Stack_id", mPlc_Read_Stack_ID);
                ini.WriteIniValue("PLC", "Model_No", mPlc_Read_Model_No);
                ini.WriteIniValue("PLC", "Change_Model_PLC", mPlc_Read_Change_Model_PLC);
                ini.WriteIniValue("PLC", "Vision_IO_Reset", mPlc_Read_VisionIO_Reset);
                ini.WriteIniValue("PLC", "Image_Save_Error_Reset", mPlc_Read_Image_Save_Err_Reset);
                ini.WriteIniValue("PLC", "Image_Detect_Error_Reset", mPlc_Read_Image_Detect_Err_Reset);

                ini.WriteIniValue("PLC", "HeartBeat", mPlc_Write_HeartBeat);
                ini.WriteIniValue("PLC", "Anode_Result", mPlc_Write_Anode_Result);
                ini.WriteIniValue("PLC", "Cathode_Result", mPlc_Write_Cathode_Result);
                ini.WriteIniValue("PLC", "Anode_Result_Code", mPlc_Write_Anode_Result_Code);
                ini.WriteIniValue("PLC", "Cathode_Result_Code", mPlc_Write_Cathode_Result_Code);
                ini.WriteIniValue("PLC", "Change_Model_PC", mPlc_Write_Change_Model_PC);
                ini.WriteIniValue("PLC", "Complete_Vision_IO_Reset", mPlc_Write_Complete_VisionIO_Reset);
                ini.WriteIniValue("PLC", "Image_Save_Error", mPlc_Write_Image_Save_Err);
                ini.WriteIniValue("PLC", "Cathode_Image_Detect_Error", mPlc_Write_Cathode_Image_Detect_Err);
                ini.WriteIniValue("PLC", "Anode_Image_Detect_Error", mPlc_Write_Anode_Image_Detect_Err);
                ini.WriteIniValue("PLC", "Write_Cathode_Cell_Id", mPlc_Write_Cathode_Cell_Id);
                ini.WriteIniValue("PLC", "Write_Anode_Cell_Id", mPlc_Write_Anode_Cell_Id);
                ini.WriteIniValue("PLC", "Cathode_Vpro_Result", mPlc_Write_Cathode_Vpro_Result);
                ini.WriteIniValue("PLC", "Anode_Vpro_Result", mPlc_Write_Anode_Vpro_Result);

                ResetDirty();
                // 저장 완료 메시지
                MessageBox.Show("저장되었습니다.", "Save");
            }
            catch(System.Exception ex)
            {
                mLog.WriteLog("PLC", $"PLC Info Save Error : {ex.Message}");
                MessageBox.Show("저장 중 오류가 발생했습니다.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btn_PLC_Connect_Click(object sender, EventArgs e)
        {
            if (btn_PLC_Connect.Text == "DisConnect")
            {
                PLC_Initialize();
                StartPlcThread();
            }
            else if (btn_PLC_Connect.Text == "Connect")
            {
                StopPLC();
            }
        }

        private void StartPlcThread()
        {
            try
            {
                isPlcThRunning = true;
                m_Thread_PLC = new Thread(Thread_PLC);
                m_Thread_PLC.IsBackground = true;
                m_Thread_PLC.Start();
                btn_PLC_Connect.Text = "Connect";
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("PLC", $"PLC Thread Error : {ex.Message}");
            }
        }

        private void StopPLC()
        {
            isPlcThRunning = false;

            if (m_Thread_PLC != null)
                m_Thread_PLC.Abort();
            btn_PLC_Connect.Text = "DisConnect";

            PLCDisConnect();
        }

        private void Thread_PLC()
        {
            int l_ntGCCount = 0;
            int heartBeatCount = 0;
            bool heartBeatFlag = false;
            int notConnectCount = 0;
            bool reconnectFlag = false;
            int reconnectCount = 0;
            string hb_value = string.Empty;

            while (isPlcThRunning == true)
            {
                if (reconnectFlag == false)
                {
                    try
                    {
                        l_ntGCCount++;

                        // Heartbeat로 Status를 체크한다. -9999라면 통신 불량이라서 하위 내용들을 수행해도 의미 없음.
                        if (!string.IsNullOrEmpty(mPlc_Write_HeartBeat))
                        {
                            hb_value = mPLC.ReadInt16(mPlc_Write_HeartBeat).ToString();

                            if (curTabpage == "tabPLC")
                            {
                                this.Invoke(new Action(delegate ()
                                {
                                    tbx_Value_HeartBeat.Text = hb_value;
                                }));
                            }
                        }

                        if (!hb_value.Equals("-9999"))
                        {
                            // PLC 하트 비트 사용 할 때만 체크
                            cls_PLC.processHeartBeat(this, ref mPLC, ref heartBeatCount, ref heartBeatFlag, ref notConnectCount);

                            // Image Save error Alarm Part
                            cls_PLC.HandleImagesaveErr(this, ref mPLC, ref mLog);

                            // Cathode Image Detect error Alarm Part
                            cls_PLC.HandleImageDetectError(ref mPLC, ref mLog, mPlc_Write_Cathode_Image_Detect_Err, mPlc_Read_Image_Detect_Err_Reset
                                , ref _isRisingErrBGT_CA, ref _isRisingErrSHP_CA, ref _isRisingErrLOC_CA, ref _isRisingErrNG_CA, ref _isRisingErrNGINTV_CA
                                , "Cathode");

                            // Anode Image Detect error Alarm Part
                            cls_PLC.HandleImageDetectError(ref mPLC, ref mLog, mPlc_Write_Anode_Image_Detect_Err, mPlc_Read_Image_Detect_Err_Reset
                                , ref _isRisingErrBGT_AN, ref _isRisingErrSHP_AN, ref _isRisingErrLOC_AN, ref _isRisingErrNG_AN, ref _isRisingErrNGINTV_AN
                                , "Anode");

                            // PLC 필요정보 모두 읽어오기 (PLC tab View에서만 읽도록 함)
                            cls_PLC.HandlePLCData(this, ref mPLC, ref mLog, curTabpage);
                        }
                        else
                        {
                            // PLC 읽어 오기 실패시, -~ 로 시작하게끔 초기화 시켜준다.
                            cls_PLC.HandleErrPLCData(this, ref mLog, ref notConnectCount);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        notConnectCount++;
                        mLog.WriteLog("PLC", $"PLC Thread Exception : {ex.Message.ToString()}");
                    }

                    if (notConnectCount >= 3)
                    {
                        reconnectFlag = true;
                        //PLC 연결 실패로 인하여 communicationERR 추가
                        mLog.WriteLog("PLC", $"CommunicationError PLC Connection : {mCommunicationErr}");
                        mCommunicationErr++;
                    }
                }
                else
                {
                    //m_blPLCFlag = false;
                    mLog.WriteLog("PLC", "PLC Disconnect, Try to Reconnection");

                    if (mPLC != null)
                    {
                        mPLC.Disconnect();

                        for (int i = 0; i < 3; i++)
                        {
                            Task.Delay(500).Wait(); // 3초 주기로 리커넥션 한다.
                            mPLC = new MCProtocol(m_PLC_IP, m_PLC_Port);
                            if (mPLC.Connect())
                            {
                                mLog.WriteLog("PLC", "PLC ReConnection Success");
                                reconnectFlag = false;
                                reconnectCount = 0;
                                notConnectCount = 0;
                                break;
                            }
                        }

                        if (reconnectFlag)
                        {
                            if (reconnectCount >= 2)
                            {
                                mCommunicationErr++;
                                reconnectCount = 0;
                                mLog.WriteLog("PLC", "PLC ReConnection Failed with Alarm");
                            }
                            else
                            {
                                reconnectCount++;
                                mLog.WriteLog("PLC", "PLC ReConnection Failed");
                            }
                        }
                    }
                    else
                    {
                        PLC_Initialize();
                    }
                }

                if (l_ntGCCount > 10)
                {
                    l_ntGCCount = 0;
                    GC.Collect();
                }

                //Task.Delay(100).Wait();
                Thread.Sleep(100);
            }
        }

        #endregion PLC

        #region IO

        private bool OpenIOCard(int DeviceNumber)
        {
            if (AdvanDevice.Init(DeviceNumber))
            {
                mLog.WriteLog("IO", "I/O Connect...OK");
                return true;
            }
            else
            {
                mLog.WriteLog("IO", "I/O Connect...Fail");
                return false;
            }
        }

        private void timer_IO_Tick(object sender, EventArgs e)
        {
            try
            {
                if (curTabpage == "tabIO")
                {
                    UpdateLampState(lamp_in32_0, mINPUT_IO_0);
                    UpdateLampState(lamp_in32_1, mINPUT_IO_1);
                    UpdateLampState(lamp_Out32_0, mOUTPUT_IO_0);
                    UpdateLampState(lamp_Out32_1, mOUTPUT_IO_1);
                }

                if (panInspection.RowStyles[2].Height != 0)
                {
                    UpdateIconLamp(ic_CathodeVision, mbCathode_Connect);
                    UpdateIconLamp(ic_AnodeVision, mbAndoe_Connect);

                    if (mIO_IN_CA_TRIG != -1) UpdateIconLamp(ic_CathodeVision_Trigger, mINPUT_IO_0[mIO_IN_CA_TRIG]);
                    if (mIO_OUT_CA_READY != -1) UpdateIconLamp(ic_CathodeVision_Ready, mOUTPUT_IO_0[mIO_OUT_CA_READY]);
                    if (mIO_IN_AN_TRIG != -1) UpdateIconLamp(ic_AnodeVision_Trigger, mINPUT_IO_0[mIO_IN_AN_TRIG]);
                    if (mIO_OUT_AN_READY != -1) UpdateIconLamp(ic_AnodeVision_Ready, mOUTPUT_IO_0[mIO_OUT_AN_READY]);

                    // OK NG 결과 I/O 로 처리하지 않고 ETH로 받음.
                    if (mIO_OUT_CA_RES_OK != -1) UpdateIconLamp(ic_CathodeVision_OK, mOUTPUT_IO_0[mIO_OUT_CA_RES_OK]);
                    if (mIO_OUT_CA_RES_NG != -1) UpdateIconLamp(ic_CathodeVision_NG, mOUTPUT_IO_0[mIO_OUT_CA_RES_NG]);
                    if (mIO_OUT_AN_RES_OK != -1) UpdateIconLamp(ic_AnodeVision_OK, mOUTPUT_IO_0[mIO_OUT_AN_RES_OK]);
                    if (mIO_OUT_AN_RES_NG != -1) UpdateIconLamp(ic_AnodeVision_NG, mOUTPUT_IO_0[mIO_OUT_AN_RES_NG]);
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("IO", $"I/O Timer : {ex.ToString()}");
            }
        }

        private void UpdateIconLamp(IconLamp lamp, bool state)
        {
            if (state)
                lamp.On();
            else
                lamp.Off();
        }

        private void UpdateLampState(IconLamp[] lamps, bool[] states)
        {
            for (int i = 0; i < lamps.Length; i++)
            {
                if (states[i])
                    lamps[i].On();
                else
                    lamps[i].Off();
            }
        }

        private void IO_Read_Th()
        {
            while (isIoThRunning)
            {
                bool[] btemps_Input_0 = new bool[8];
                bool[] btemps_Input_1 = new bool[8];
                bool[] btemps_Output_0 = new bool[8];
                bool[] btemps_Output_1 = new bool[8];

                try
                {
                    AdvanDevice.GET32_IO(out btemps_Input_0, out btemps_Input_1, out btemps_Output_0, out btemps_Output_1);

                    // io Read
                    for (int i = 0; i < 8; i++)
                    {
                        AdvanDeviceInputIO_0(i, btemps_Input_0);
                    }

                    // io Write
                    for (int i = 0; i < 8; i++)
                    {
                        AdvanDeviceOutputIO_0(i, btemps_Output_0);
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("IO_Thread Error");
                    mLog.WriteLog("IO", $"I/O Thread Error : {ex.Message}");
                }

                // Communication Error 처리
                try
                {
                    if (mCommunicationErr > 0)
                    {
                        if (mCommunicationErr >= 5)
                            mCommunicationErr = 1;

                        // Communication Error Reset 배선이 되어 있는 경우  skoh2
                        if (mIO_IN_COMM_ERR_RESET > 0)  // skoh2
                        {
                            AdvanDevice.OutputStart(mIO_OUT_COMM_ERROR, true, 0, 0, 0);
                            mLog.WriteLog("IO", $"PLC CommunicationError ({mCommunicationErr++})");
                        }
                        else  // Comunication Error Reset 배선이 되어 있지 않는 경우  skoh
                        {
                            AdvanDevice.OutputStart(mIO_OUT_COMM_ERROR, true, 0, 0, 0);
                            mLog.WriteLog("IO", $"PLC CommunicationError ({mCommunicationErr})");
                            Task.Delay(100).Wait();
                            AdvanDevice.OutputStart(mIO_OUT_COMM_ERROR, false, 0, 0, 0);
                            mCommunicationErr = 0;
                            // 이렇게 오류를 바로 꺼주더라도 커뮤니케이션 오류가 해결되지 않으면 100ms 간격으로 계속 알람을 주게 되어 있다.
                            // 컴퓨터를 끄던지 오류를 해결하던지 해야함. 리셋 배선을 추가적으로 하는게 제일 좋은 방법
                            // skoh는 배선이 안되어 있어서 일단 이렇게 하기로 했다. 커뮤니케이션 오류가 잦게 발생하는 것은 아니기 때문
                            // 커뮤니케이션 오류 체크에 관련된 옵션도 환경설정에서 주는게 좋을것 같다.   2024-06-18
                        }
                    }
                }
                catch (System.Exception)
                {
                    mLog.WriteLog("IO", $"Except PLC CommunicationError ({mCommunicationErr})");
                }

                Task.Delay(10).Wait();
            }
        }

        private void btn_IO_Reset_Click(object sender, EventArgs e)
        {
            IO_Reset_Run();
        }

        private void timer_Heartbeat_Tick(object sender, EventArgs e)
        {
            if (mbHeartbeat)
            {
                AdvanDevice.OutputStart(mIO_OUT_HEARTBEAT, true, 0, 0, 0);
                mbHeartbeat = false;
            }
            else
            {
                AdvanDevice.OutputStart(mIO_OUT_HEARTBEAT, false, 0, 0, 0);
                mbHeartbeat = true;
            }
        }

        private void EdgeStatus_Th()
        {
            try
            {
                while (isEdgeStatusThRunning)
                {
                    try
                    {
                        //250624 카메라는 연결되어 있지만 프로그램이 연결이 안되었을 경우에도 true값 반환되어 수정
                        edgeStatus.CAMERA_CA_LINK_CD = mCamera.CheckConnectionCamera(CAM_CATHODE) ? "LINK_ST_CD_ON" : "LINK_ST_CD_OFF";

                        ////250624 Cathode 카메라 연결 해제시 Communication Err 발생
                        if (edgeStatus.CAMERA_CA_LINK_CD != "LINK_ST_CD_ON")
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"Cathode Camera Connection Check Error: Cathode Camera is DeviceLost!");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        mLog.WriteLog("CAM", $"Cathode Camera Connection Check Error: {ex.Message}");
                    }

                    try
                    {
                        ////250624 카메라는 연결되어 있지만 프로그램이 연결이 안되었을 경우에도 true값 반환되어 수정
                        edgeStatus.CAMERA_AN_LINK_CD = mCamera.CheckConnectionCamera(CAM_ANODE) ? "LINK_ST_CD_ON" : "LINK_ST_CD_OFF";

                        ////250624 Anode 카메라 연결 해제시 Communication Err 발생
                        if (edgeStatus.CAMERA_AN_LINK_CD != "LINK_ST_CD_ON")
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"Anode Camera Connection Check Error: Anode Camera is DeviceLost!");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        mLog.WriteLog("CAM", $"Anode Camera Connection Check Error: {ex.Message}");
                    }

                    try
                    {
                        // plc online 체크를 i/o로 하지 않을 경우 ip체크로 확인한다.
                        if (mIO_IN_PLC_ONLINE == -1)
                        {
                            edgeStatus.PLC_LINK_CD = mCamera.CheckConnectionIP(m_PLC_IP) ? "LINK_ST_CD_ON" : "LINK_ST_CD_OFF";
                        }
                    }
                    catch (System.Exception ex)
                    {
                        mLog.WriteLog("PLC", $"PLC Connection Check Error: {ex.Message}");
                    }


                    try
                    {
                        SaveStatusJson(edgeStatus); // 상태값을 Json에 저장
                    }
                    catch (System.Exception ex)
                    {

                        mLog.WriteLog("SYS", $"Save Status Json Error: {ex.Message}");
                    }


                    // 장시간 Sleep를 주는것 때문에 프로그램 종료에 문제가 생겼다.
                    // 짧게 100 Sleep를 주면서 Thread 종료 여부를 확인해서 갱신주기를 맞춘다.
                    for (int i = 0; i < mEdgestatusInterval * 10; i++)
                    {
                        if (!isEdgeStatusThRunning)
                        {
                            break;
                        }

                        Task.Delay(100).Wait();
                    }
                }
            }
            //catch (ThreadAbortException)
            //{
            //    // 스레드가 강제로 종료될 때 발생하는 예외 처리
            //    Thread.ResetAbort(); // 스레드 종료 요청 취소
            //}
            catch (System.Exception ex)
            {
                // 기타 예외 처리
                mLog.WriteLog("SYS", $"Edge Status Thread Error: {ex.Message}");
            }
        }

        private void SaveStatusJson(cls_EdgeStatus edgeStatus)
        {
            if (string.IsNullOrEmpty(mLineNo))
                return;

            edgeStatus.LAST_UPD_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            edgeStatus.RGST_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string regLineNo = Regex.Replace(mLineNo, "[^0-9]", "");  // 숫자만 추출하는 정규식
            edgeStatus.LINE_NUM = Int32.Parse(regLineNo);
            edgeStatus.PC_CL_CD = "PC_" + mPosition;
            edgeStatus.UPD_CYCL = mEdgestatusInterval;

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CustomContractResolver()
            };

            string json = JsonConvert.SerializeObject(edgeStatus, settings);
            if (!Directory.Exists(mJsonSaveDir))  // JSON 디렉토리 없으면 생성
            {
                Directory.CreateDirectory(mJsonSaveDir);
            }

            string jsonSavePath = Path.Combine(mJsonSaveDir, "STS_" + edgeStatus.LINE_NUM + "_" + edgeStatus.PC_CL_CD + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json");
            try
            {
                File.WriteAllText(jsonSavePath, json);
            }
            catch (System.Exception)
            {
                //
            }
        }

        private void IO_Reset_Run()
        {
            // io output 초기화
            for (int i = 0; i < 8; i++)
            {
                AdvanDevice.OutputStart(i, false, 0, 0, 0);
            }

            Thread.Sleep(200);
            // 카메라 READY 신호는 살려준다.
            AdvanDevice.OutputStart(mIO_OUT_CA_READY, true, 0, 0, 0);
            AdvanDevice.OutputStart(mIO_OUT_AN_READY, true, 0, 0, 0);
        }

        private void AdvanDeviceInputIO_0(int num, bool[] btemps_Input_0)
        {
            // Current Memory Value 와 Input Value 비교
            // (다들 경우 신호가 들어온것으로 판단하고 프로세스 진행)
            // (같을 경우, Return)
            if (btemps_Input_0[num] != mINPUT_IO_0[num])
                mINPUT_IO_0[num] = btemps_Input_0[num];
            else
                return;

            // Cathode Trigger
            if (num == mIO_IN_CA_TRIG)
            {
                try
                {
                    if (mINPUT_IO_0[num])  // 프로세스가 완료 되지 않으면 트리거를 안받도록
                    {
                        // Cathode Trigger On
                        AdvanDevice.OutputStart(mIO_OUT_CA_RES_OK, false, 0, 0, 0);
                        AdvanDevice.OutputStart(mIO_OUT_CA_READY, false, 0, 0, 0);

                        m_Cathode_StopWatch.Reset();
                        m_Cathode_StopWatch.Start();

                        mCamera.SoftwareTrigger(CAM_CATHODE);
                        mLog.WriteLog("CAM", "Cathode Trigger ON", CamName(CAM_CATHODE));
                    }
                    else
                    {
                    }
                }
                catch (System.Exception ex)
                {
                    mLog.WriteLog("CAM", $"Cathode Trigger Error : {ex.Message}", CamName(CAM_CATHODE));
                }
            }
            // Anode Trigger
            else if (num == mIO_IN_AN_TRIG)
            {
                try
                {
                    if (mINPUT_IO_0[num])
                    {
                        // Anode Trigger On
                        AdvanDevice.OutputStart(mIO_OUT_AN_RES_OK, false, 0, 0, 0);
                        AdvanDevice.OutputStart(mIO_OUT_AN_READY, false, 0, 0, 0);

                        m_Anode_StopWatch.Reset();
                        m_Anode_StopWatch.Start();

                        mCamera.SoftwareTrigger(CAM_ANODE);

                        mLog.WriteLog("CAM", "Anode Trigger ON", CamName(CAM_ANODE));
                    }
                    else
                    {
                    }
                }
                catch (System.Exception ex)
                {
                    mLog.WriteLog("CAM", $"Anode Trigger Error : {ex.Message}", CamName(CAM_ANODE));
                }
            }
            // PLC Online 상태값
            else if (num == mIO_IN_PLC_ONLINE)
            {
                if (mINPUT_IO_0[num])
                {
                    edgeStatus.PLC_LINK_CD = "LINK_ST_CD_ON";
                    // 온라인일 경우 모델 체인지 등 작업 못하도록 할것
                }
                else
                {
                    edgeStatus.PLC_LINK_CD = "LINK_ST_CD_OFF";
                }
            }
            // Communication Error Reset
            // SKOH2는 Reset 스위치를 누르기 전까지는 계속 Communication Error을 전송하고, Reset 신호가 들어오면 그때 끈다.
            // SKOH 는 Communication Error 을 한번만 보내주면 알아서 처리함.
            else if (num == mIO_IN_COMM_ERR_RESET)
            {
                try
                {
                    if (mINPUT_IO_0[num])
                    {
                        AdvanDevice.OutputStart(mIO_OUT_COMM_ERROR, false, 0, 0, 0);
                        mCommunicationErr = 0;
                        mLog.WriteLog("PLC", $"PLC Communication Error Reset ({mCommunicationErr})");
                    }
                    else
                    {
                    }
                }
                catch (System.Exception ex)
                {
                    mLog.WriteLog("PLC", $"PLC Communication Error : {ex.Message}");
                }
                // 커뮤니케이션 오류 리셋
            }
        }

        private void AdvanDeviceOutputIO_0(int num, bool[] btemps_Output_0)
        {
            if (btemps_Output_0[num] != mOUTPUT_IO_0[num])
            {
                mOUTPUT_IO_0[num] = btemps_Output_0[num];
                if (num != 0) mLog.WriteLog("IO", $"AdvanDeviceOutputIO_0 ({num.ToString()}) Ready : {mOUTPUT_IO_0[num].ToString()}");
            }
        }

        private void AdvanDeviceOutputIO_1(int num, bool[] btemps_Output_1)
        {
            if (btemps_Output_1[num] != mOUTPUT_IO_1[num])
            {
                mOUTPUT_IO_1[num] = btemps_Output_1[num];
                if (num != 0) mLog.WriteLog("IO", $"AdvanDeviceOutputIO_1 ({num.ToString()}) Ready : {mOUTPUT_IO_0[num].ToString()}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(5);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(6);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_0(7);
        }

        private void AdvanDeviceOutputStartBitArrOut_0(int num)
        {
            AdvanDevice.OutputStart(num, !AdvanDevice.bitArrOut_0[num], 0, 0, 0);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(0);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(1);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(2);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(3);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(4);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(5);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(6);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            AdvanDeviceOutputStartBitArrOut_1(7);
        }

        private void AdvanDeviceOutputStartBitArrOut_1(int num)
        {
            AdvanDevice.OutputStart(num, !AdvanDevice.bitArrOut_1[num], 0, 0, 1);
        }

        #endregion IO

        #region Camera

        public bool Cathode_Vision_Init(string strIP)
        {
            return Vision_Init(CAM_CATHODE, strIP);
        }

        public bool Anode_Vision_Init(string strIP)
        {
            return Vision_Init(CAM_ANODE, strIP);
        }

        public bool Vision_Init(int camIdx, string strIP)
        {
            Button btnPolarityConnect = null;
            Button btnPolarityLive = null;
            bool isCamReverse = false;

            if (camIdx == CAM_CATHODE)
            {
                btnPolarityConnect = btn_Cathode_Connect;
                btnPolarityLive = btnCathodeLive;
                isCamReverse = mCathode_Reverse;
            }
            else if (camIdx == CAM_ANODE)
            {
                btnPolarityConnect = btn_Anode_Connect;
                btnPolarityLive = btnAnodeLive;
                isCamReverse = mAnode_Reverse;
            }

            try
            {
                if (camIdx == CAM_CATHODE)
                {
                    mCamera.CathodeGrabimage += Cathode_GrabEvent;
                    mCamera.CathodeCaptureimage += CaptureImage;
                }
                else if (camIdx == CAM_ANODE)
                {
                    mCamera.AnodeGrabimage += Anode_GrabEvent;
                    mCamera.AnodeCaptureimage += CaptureImage;
                }

                if (mCamera.Connect(camIdx, strIP))
                {
                    btnPolarityConnect.Text = "Connect";
                    btnPolarityConnect.BackColor = Color.Lime;
                    btnPolarityLive.Enabled = true;

                    mCamera.CamReverse(camIdx, isCamReverse);
                    mCamera.StartCapture(camIdx);
                    mCamera.TriggerModeOn(camIdx);

                    mLog.WriteLog("CAM", $"{CamName(camIdx)} Camera Connect...OK", CamName(camIdx));
                    return true;
                }
                else
                {
                    btnPolarityLive.Enabled = false;
                    mLog.WriteLog("CAM", $"{CamName(camIdx)} Camera Connect...Fail", CamName(camIdx));
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                btnPolarityLive.Enabled = false;
                mLog.WriteLog("CAM", $"{CamName(camIdx)} Camera Connect...Fail {ex.Message}", CamName(camIdx));
                return false;
            }
        }

        private void CaptureImage(Bitmap bimage, int CamNum)
        {
            try
            {
                if (!mbCathode_Live && !mbAnode_Live)
                {
                    m_Cam_Queue.Enqueue(new Tuple<Bitmap, int>(bimage, CamNum));
                    //mbl_Status = false;
                    mCamera.StartCapture(CamNum);
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Capture Error : {ex.Message}", CamName(CamNum));
            }
        }

        private void Cathode_GrabEvent(Bitmap bimage)
        {
            PolarityGrabEvent(bimage, CAM_CATHODE);
        }

        private void VPDL_initailize()
        {
            cls_RunResultJson runResultJson = new cls_RunResultJson();

            // CLASS 기본 DATA 생성
            SaveJsonBasicData(CAM_CATHODE, runResultJson);

            // PLC Read : Cell_id, Carrier_id
            ReadDataFromPLC(CAM_CATHODE, runResultJson);

            // VPDL 검사 진행
            InspectVPDL(new Bitmap(2448, 2048), CAM_CATHODE, runResultJson, true);
        }

        private void PolarityGrabEvent(Bitmap bimage, int camIdx)
        {
            Button btnLive;
            cls_RunResultJson runResultJson;  // db 업로드용 추가 240608
            CogDisplay display;
            string plcResult;
            string plcResult_Code;
            string plcVproResult;
            Label lblBrightValue;
            Label lblContrastValue;
            int ioResult;
            Stopwatch stopWatch;
            Label polaResult;
            int brightPosition_x;
            int brightPosition_y;

            if (camIdx == CAM_CATHODE)
            {
                btnLive = btnCathodeLive;
                display = displayCathode.Display;
                plcResult = mPlc_Write_Cathode_Result;
                plcResult_Code = mPlc_Write_Cathode_Result_Code;
                plcVproResult = mPlc_Write_Cathode_Vpro_Result;
                lblBrightValue = lblBrightValCa;
                lblContrastValue = lblContrastValCa;
                ioResult = mIO_OUT_CA_RES_OK;
                stopWatch = m_Cathode_StopWatch;
                polaResult = lb_CathodeResult;
                brightPosition_x = brightPositionCA_x;
                brightPosition_y = brightPositionCA_y;
                isTriggerAction_CA = true;
            }
            else
            {
                btnLive = btnAnodeLive;
                display = displayAnode.Display;
                plcResult = mPlc_Write_Anode_Result;
                plcResult_Code = mPlc_Write_Anode_Result_Code;
                plcVproResult = mPlc_Write_Anode_Vpro_Result;
                lblBrightValue = lblBrightValAn;
                lblContrastValue = lblContrastValAn;
                ioResult = mIO_OUT_AN_RES_OK;
                stopWatch = m_Anode_StopWatch;
                polaResult = lb_AnodeResult;
                brightPosition_x = brightPositionAN_x;
                brightPosition_y = brightPositionAN_y;
                isTriggerAction_AN = true;
            }

            try
            {
                // 디스플레이에 영상 표시
                display.Image = new Cognex.VisionPro.CogImage24PlanarColor((Bitmap)bimage.Clone());

                // 라이브가 꺼져 있을 때만 검사를 진행한다.
                if (btnLive.Text.Contains("Off"))
                {
                    runResultJson = new cls_RunResultJson();  // db 업로드용 추가 2024-06-08

                    // CLASS 기본 DATA 생성
                    SaveJsonBasicData(camIdx, runResultJson);

                    // PLC Read : Cell_id, Carrier_id
                    ReadDataFromPLC(camIdx, runResultJson);

                    // VPDL 검사 진행
                    InspectVPDL(bimage, camIdx, runResultJson);

                    // VPRO 검사 진행
                    if (m_VPRO_Use == true) InspectVPRO(bimage, camIdx, runResultJson);

                    // 이미지 이상 감지
                    DetectImageAnomalies(bimage, camIdx, runResultJson);

                    // PLC 결과 전송
                    SendResultToPLC(plcResult, plcResult_Code, plcVproResult, runResultJson, camIdx);
                    polaResult.Text = runResultJson.CORNER_JUDGE_STR;

                    // IO 결과 전송
                    SendResultToIO(ioResult, camIdx);

                    // Image 저장
                    SaveImage(bimage, camIdx, runResultJson);

                    // Json 저장
                    SaveToJsonFile(runResultJson);

                    stopWatch.Stop();

                    mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} Final Result : {runResultJson.CORNER_JUDGE_STR} ({stopWatch.ElapsedMilliseconds.ToString()})", CamName(camIdx));

                    // 오래된 이미지 삭제
                    DeleteOldImage();

                    // 모델 체인지를 하기전 다른 작업들이 진행중인지 확인하기 위한 flag 2024-07-29
                    if (camIdx == CAM_CATHODE)
                        isTriggerAction_CA = false;
                    else
                        isTriggerAction_AN = false;
                }

                // 조명값, 대조값 화면 표시 (텍타임 때문에 가이드가 on일때만 화면에 표시하도록 수정)
                if (btnViewGuideRect.Text.Contains("On"))
                {
                    lblBrightValue.Text = mDisplay.ViewBrightValue(display, brightPosition_x, brightPosition_y).ToString();
                    lblContrastValue.Text = mDisplay.ViewContrastValue(display, brightPosition_x, brightPosition_y).ToString();
                }
            }
            catch (System.Exception)
            {
                if (camIdx == CAM_CATHODE)
                    isTriggerAction_CA = false;
                else
                    isTriggerAction_AN = false;
            }

        }

        private void DetectImageAnomalies(Bitmap bImage, int camIdx, cls_RunResultJson runResultJson)
        {
            string Polarity = string.Empty;

            try
            {
                Polarity = CamName(camIdx);

                using (Mat mat = BitmapConverter.ToMat(bImage))
                {
                    // 주어진 영역을 추출 (ROI)
                    using (Mat roiMat = new Mat(mat, new OpenCvSharp.Rect(189, 760, 2070, 701)))
                    {
                        // 카메라별 다른 변수 참조.
                        ref int _imageBGT_ErrorCount = ref (camIdx == CAM_CATHODE ? ref _imageBGT_CA_ErrorCount : ref _imageBGT_AN_ErrorCount);
                        ref int _imageSHP_ErrorCount = ref (camIdx == CAM_CATHODE ? ref _imageSHP_CA_ErrorCount : ref _imageSHP_AN_ErrorCount);
                        ref int _imageLOC_ErrorCount = ref (camIdx == CAM_CATHODE ? ref _imageLOC_CA_ErrorCount : ref _imageLOC_AN_ErrorCount);
                        ref int _imageNG_ErrorCount = ref (camIdx == CAM_CATHODE ? ref _imageNG_CA_ErrorCount : ref _imageNG_AN_ErrorCount);
                        ref Queue<bool> _imageNG_INTV_ErrorCount = ref (camIdx == CAM_CATHODE ? ref _imageNG_INTV_CA_ErrorCount : ref _imageNG_INTV_AN_ErrorCount);

                        int[] histogram;
                        double averageIndex = 0.0;
                        double sharpness = 0.0;

                        double bright_STD_VAL = camIdx == CAM_CATHODE ? mBright_CA_STD_VAL : mBright_AN_STD_VAL;
                        double bright_LOW_VAL = camIdx == CAM_CATHODE ? mBright_CA_LOW_VAL : mBright_AN_LOW_VAL;
                        double bright_UPP_VAL = camIdx == CAM_CATHODE ? mBright_CA_UPP_VAL : mBright_AN_UPP_VAL;
                        double sharp_STD_VAL = camIdx == CAM_CATHODE ? mSharp_CA_STD_VAL : mSharp_AN_STD_VAL;
                        double sharp_LOW_VAL = camIdx == CAM_CATHODE ? mSharp_CA_LOW_VAL : mSharp_AN_LOW_VAL;

                        //==============================================
                        // Brightness Part
                        //==============================================
                        if (!mBright_Det_YN)
                        {
                            // 밝기 히스토그램 계산
                            histogram = mDetect.CalculateHistogram(roiMat);
                            // 밝기 Pixel 상위 N개의 값 추출
                            averageIndex = mDetect.CalculateAverage(histogram, mBright_Rank_Qty);
                            // 밝기 측정값 판정결과 (True 일 경우, 알람 울림)
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrBGT_CA.Value = mDetect.brightCheckResult("B", averageIndex, bright_STD_VAL, bright_LOW_VAL, bright_UPP_VAL, ref _imageBGT_ErrorCount, mBright_Det_CNT);
                            else
                                _isRisingErrBGT_AN.Value = mDetect.brightCheckResult("B", averageIndex, bright_STD_VAL, bright_LOW_VAL, bright_UPP_VAL, ref _imageBGT_ErrorCount, mBright_Det_CNT);
                        }
                        else
                        {
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrBGT_CA.Value = false;
                            else
                                _isRisingErrBGT_AN.Value = false;
                        }


                        //==============================================
                        // Sharpness Part
                        //==============================================
                        if (!mSharp_Det_YN)
                        {
                            // 선명도 계산
                            sharpness = mDetect.CalculateSharpness(roiMat);
                            // 선명도 측정값 판정결과 (True 일 경우, 알람 울림)
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrSHP_CA.Value = mDetect.sharpnessCheckResult("S", sharpness, sharp_STD_VAL, sharp_LOW_VAL, 0, ref _imageSHP_ErrorCount, mSharp_Det_CNT);
                            else
                                _isRisingErrSHP_AN.Value = mDetect.sharpnessCheckResult("S", sharpness, sharp_STD_VAL, sharp_LOW_VAL, 0, ref _imageSHP_ErrorCount, mSharp_Det_CNT);
                        }
                        else
                        {
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrSHP_CA.Value = false;
                            else
                                _isRisingErrSHP_AN.Value = false;
                        }


                        //==============================================
                        // Location Part
                        //==============================================
                        if (!mLocation_Det_YN)
                        {
                            // 위치보정 여부 결과 (True 일 경우, 알람 울림)
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrLOC_CA.Value = mDetect.locationCheckResult(m_VPDL_Locate, runResultJson.BLUTOL_STRD1_X, runResultJson.BLUTOL_STRD1_Y, runResultJson.BLUTOL_STRD2_X, runResultJson.BLUTOL_STRD2_Y, runResultJson.BLUTOL_ROTAT_ANGL, ref _imageLOC_ErrorCount, mLocation_Det_CNT);
                            else
                                _isRisingErrLOC_AN.Value = mDetect.locationCheckResult(m_VPDL_Locate, runResultJson.BLUTOL_STRD1_X, runResultJson.BLUTOL_STRD1_Y, runResultJson.BLUTOL_STRD2_X, runResultJson.BLUTOL_STRD2_Y, runResultJson.BLUTOL_ROTAT_ANGL, ref _imageLOC_ErrorCount, mLocation_Det_CNT);
                        }
                        else
                        {
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrLOC_CA.Value = false;
                            else
                                _isRisingErrLOC_AN.Value = false;
                        }


                        //==============================================
                        // NG Part
                        //==============================================
                        if (!mNG_Det_YN)
                        {
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrNG_CA.Value = mDetect.NGCellCheckResult("A", runResultJson.CORNER_JUDGE_STR, ref _imageNG_ErrorCount, mNG_Det_CNT);
                            else
                                _isRisingErrNG_AN.Value = mDetect.NGCellCheckResult("A", runResultJson.CORNER_JUDGE_STR, ref _imageNG_ErrorCount, mNG_Det_CNT);
                        }

                        else
                        {
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrNG_CA.Value = false;
                            else
                                _isRisingErrNG_AN.Value = false;
                        }

                        //==============================================
                        // NG Interval Part
                        //==============================================
                        if (!mNG_ITV_Det_YN)
                        {
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrNGINTV_CA.Value = mDetect.NGCellIntervalCheckResult("B", runResultJson.CORNER_JUDGE_STR, ref _imageNG_INTV_ErrorCount, mNG_ITV_Det_MAX_CNT, mNG_ITV_Det_CNT);
                            else
                                _isRisingErrNGINTV_AN.Value = mDetect.NGCellIntervalCheckResult("B", runResultJson.CORNER_JUDGE_STR, ref _imageNG_INTV_ErrorCount, mNG_ITV_Det_MAX_CNT, mNG_ITV_Det_CNT);
                        }
                        else
                        {
                            if (camIdx == CAM_CATHODE)
                                _isRisingErrNGINTV_CA.Value = false;
                            else
                                _isRisingErrNGINTV_AN.Value = false;
                        }

                        // 로그 기록 허용시, 운영중에는 사용하지 말것. (상,하한 기준 수치 정할때 필요한 정보)
                        if (mLog_Det_YN)
                            mLog.WriteLog("ANM", $"[{Polarity}] {runResultJson.CELL_ID}" +
                                $"|| BGT,SHP,LOC,NG,NGITV Using: {!mBright_Det_YN},{!mSharp_Det_YN},{!mLocation_Det_YN},{!mNG_Det_YN},{!mNG_ITV_Det_YN} " +
                                $"|| BGT : {averageIndex} ({bright_LOW_VAL},{bright_STD_VAL},{bright_UPP_VAL}) " +
                                $"|| SHP: {sharpness} ({sharp_LOW_VAL},{sharp_STD_VAL})" +
                                $"|| BT : {runResultJson.BLUTOL_STRD1_X}, {runResultJson.BLUTOL_STRD1_Y}, {runResultJson.BLUTOL_STRD2_X}, {runResultJson.BLUTOL_STRD2_Y}, {runResultJson.BLUTOL_ROTAT_ANGL} " +
                                $"|| {_imageBGT_ErrorCount},{_imageSHP_ErrorCount},{_imageLOC_ErrorCount},{_imageNG_ErrorCount},{_imageNG_INTV_ErrorCount.Count(result => !result)}");

                        // 다섯가지중 하나라도 범주에 속하면서, 카운팅이 초과하면 Error 발생
                        if (camIdx == CAM_CATHODE)
                        {
                            if (_isRisingErrBGT_CA.Value || _isRisingErrSHP_CA.Value || _isRisingErrLOC_CA.Value || _isRisingErrNG_CA.Value || _isRisingErrNGINTV_CA.Value)
                            {
                                // 알람 Writing.    
                                mLog.WriteLog("CAM", $"[{Polarity}][BGT,SHP,LOC,NG,NGITV] [RISING ERR] - {(_isRisingErrBGT_CA.Value ? "NG" : "OK")}, {(_isRisingErrSHP_CA.Value ? "NG" : "OK")}, {(_isRisingErrLOC_CA.Value ? "NG" : "OK")}, {(_isRisingErrNG_CA.Value ? "NG" : "OK")}, {(_isRisingErrNGINTV_CA.Value ? "NG" : "OK")}", CamName(camIdx));
                            }
                        }
                        else
                        {
                            if (_isRisingErrBGT_AN.Value || _isRisingErrSHP_AN.Value || _isRisingErrLOC_AN.Value || _isRisingErrNG_AN.Value || _isRisingErrNGINTV_AN.Value)
                            {
                                // 알람 Writing.    
                                mLog.WriteLog("CAM", $"[{Polarity}][BGT,SHP,LOC,NG,NGITV] [RISING ERR] - {(_isRisingErrBGT_AN.Value ? "NG" : "OK")}, {(_isRisingErrSHP_AN.Value ? "NG" : "OK")}, {(_isRisingErrLOC_AN.Value ? "NG" : "OK")}, {(_isRisingErrNG_AN.Value ? "NG" : "OK")}, {(_isRisingErrNGINTV_AN.Value ? "NG" : "OK")}", CamName(camIdx));
                            }
                        }
                        //else
                        //{
                        //    // 로그 기록 허용시, 운영중에는 사용하지 말것. (상,하한 기준 수치 정할때 필요한 정보)
                        //    if (mLog_Det_YN)
                        //        mLog.WriteLog("ANM", $"[BGT,SHP,LOC,NG,NGITV] [PASS] - {(_isRisingErrBGT.Value ? "NG" : "OK")}, {(_isRisingErrSHP.Value ? "NG" : "OK")}, {(_isRisingErrLOC.Value ? "NG" : "OK")}, {(_isRisingErrNG.Value ? "NG" : "OK")}, {(_isRisingErrNGINTV.Value ? "NG" : "OK")}");
                        //}
                    }
                }
            }
            catch (System.Exception ex) { mLog.WriteLog("ERR", $"Detect Image Anomalies : [{Polarity}][{runResultJson.CELL_ID}] {ex.Message}", CamName(camIdx)); }
            finally
            {
                mLog.WriteLog("CAM", $"{CamName(camIdx)} Image Detect End", CamName(camIdx));
            }
        }

        private void DeleteOldImage()
        {
            // 짝으로 생성되기 때문에, 10000매 생산했을때, 한번씩 수행한다.
            if (_imageSaveCnt > 20000)
            {
                // 이미지 저장 공간 확인후 오래된 순서대로 삭제함.
                // 삭제시 문제 없도록 쓰레드 처리
                // 삭제 폴더 3개 대상 추가, 이미지, 블루, 히트맵 // 2024-07-04
                Thread DeleteOldImage = new Thread(() => cls_File.CheckSpaceAndDeleteOldImage(mLog, mImageSaveRatio, new string[] { mImageSaveDir, mLocateLocalSaveDir, mHeatmapLocalSaveDir }));
                DeleteOldImage.IsBackground = true;
                DeleteOldImage.Start();
                _imageSaveCnt = 0;
            }
            else { _imageSaveCnt++; }
        }

        private void SaveJsonBasicData(int camIdx, cls_RunResultJson runResultJson)
        {
            string polaStr = (camIdx == CAM_CATHODE) ? "CA" : "AN";
            string posStr = mPosition == "BOTTOM" ? "BOT" : mPosition;

            runResultJson.CELL_ID_DTM = DateTime.Now;
            string year = runResultJson.CELL_ID_DTM.ToString("yyyy");
            string day = runResultJson.CELL_ID_DTM.ToString("MMdd");
            string Hour = runResultJson.CELL_ID_DTM.ToString("HH");

            runResultJson.POSITION = $"{mProcess} {polaStr}({posStr})";
            runResultJson.BLUTOL_LOC_FILE_DIR = Path.Combine(mLocateLocalSaveDir, mLineNo, year, day, Hour); // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15
            runResultJson.BLUTOL_VIS_FILE_DIR = Path.Combine(mLocateVISSSaveDir, mLineNo, year, day, Hour); // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15
            runResultJson.HTMP_LOC_FILE_DIR = Path.Combine(mHeatmapLocalSaveDir, mLineNo, year, day, Hour); // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15
            runResultJson.HTMP_VIS_FILE_DIR = Path.Combine(mHeatmapVISSSaveDir, mLineNo, year, day, Hour); // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15
            runResultJson.VPRO_LOC_FILE_DIR = Path.Combine(mVPROLocalSaveDir, mLineNo, year, day, Hour); // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15

            runResultJson.CORNER_ID = FindCornerId(camIdx);
            runResultJson.VPDL_MDL_FILE_NM = mWorkSpacePath;

            runResultJson.CA_ROI_1_THRHLD = mThreshold_CA_R1;
            runResultJson.CA_ROI_2_THRHLD = mThreshold_CA_R2;
            runResultJson.CA_ROI_3_THRHLD = mThreshold_CA_R3;
            runResultJson.AN_ROI_1_THRHLD = mThreshold_AN_R1;
            runResultJson.AN_ROI_2_THRHLD = mThreshold_AN_R2;
            runResultJson.AN_ROI_3_THRHLD = mThreshold_AN_R3;

            runResultJson.CORNER_NAME = CamName(camIdx);
            mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} Image Grab(Trigger On)", CamName(camIdx));
        }

        public void SaveToJsonFile(cls_RunResultJson runResultJson)
        {
            runResultJson.LAST_UPD_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CustomContractResolver()
            };

            string json = JsonConvert.SerializeObject(runResultJson, settings);
            if (!Directory.Exists(mJsonSaveDir))  // 디렉토리 없으면 생성
            {
                Directory.CreateDirectory(mJsonSaveDir);
            }

            string jsonSavePath = Path.Combine(mJsonSaveDir, runResultJson.CELL_ID + "_" + runResultJson.CORNER_ID + ".json");  // cell_id + corner_id로 구분해서 저장

            File.WriteAllText(jsonSavePath, json);
        }

        // 저장에서 제외할 JSON 필드 정의
        private class CustomContractResolver : DefaultContractResolver
        {
            // 제외할 속성 리스트
            private readonly HashSet<string> _excludedProperties = new HashSet<string>
            {
                "CORNER_NAME",
                "CORNER_JUDGE_STR",
                "CORNER_JUDGE_CD",
                "CORNER_JUDGE_PLC_CD",
                "IMG_FILE_DIR",
                "ORGL_IMG_VIS_FILE_DIR",
                "ROI1_INFRCE2_RST_CD",
                "ROI1_INFRCE2_RST_STR",
                "ROI2_INFRCE2_RST_CD",
                "ROI2_INFRCE2_RST_STR",
                "ROI3_INFRCE2_RST_CD",
                "ROI3_INFRCE2_RST_STR",
                "CELL_ID_DTM",
                "BLUTOL_LOC_FILE_DIR",
                "BLUTOL_VIS_FILE_DIR",
                "HTMP_LOC_FILE_DIR",
                "HTMP_VIS_FILE_DIR",
                "POSITION",
                "ROI1_HEATMAP_IMAGE",
                "ROI2_HEATMAP_IMAGE",
                "ROI3_HEATMAP_IMAGE",
                "COMB_HEATMAP_IMAGE",
                //"CA_AVG_BRIGHTNESS",
                //"CA_SHARPNESS",
                //"AN_AVG_BRIGHTNESS",
                //"AN_SHARPNESS"
        };

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                // 제외할 속성을 필터링
                properties = properties.Where(p => !_excludedProperties.Contains(p.PropertyName)).ToList();
                return properties;
            }
        }

        private void ReadDataFromPLC(int camIdx, cls_RunResultJson runResultJson)
        {
            DateTime Date = DateTime.Now;

            try
            {
                if (camIdx == CAM_CATHODE)
                {
                    if (!string.IsNullOrEmpty(mPlc_Read_CELLID_CA))
                    {
                        mCell_ID_CA = mPLC.ReadString(mPlc_Read_CELLID_CA, 9);
                        mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} Cell ID : {mCell_ID_CA}", CamName(camIdx));

                        if (mCell_ID_CA == "-9999")
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"CommunicationError Cell_ID Read : {mCommunicationErr}", CamName(camIdx));

                            return;
                        }

                        // Temp용 셀아이디 가져오기
                        if (!string.IsNullOrEmpty(temp_cell_CA_nm))
                        {
                            mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} Cell ID : {temp_cell_CA_nm}", CamName(camIdx));
                            runResultJson.CELL_ID = temp_cell_CA_nm;

                            if (lb_CathodeID.InvokeRequired)
                            {
                                lb_CathodeID.Invoke(new Action(() =>
                                {
                                    lb_CathodeID.Text = $"Cell ID : {temp_cell_CA_nm}";
                                }));
                            }
                            else
                            {
                                lb_CathodeID.Text = $"Cell ID : {temp_cell_CA_nm}";
                            }


                            //lb_CathodeID.Text = $"Cell ID : {temp_cell_CA_nm}";
                            temp_cell_CA_nm = string.Empty;
                        }
                        else
                        {
                            runResultJson.CELL_ID = mCell_ID_CA;

                            if (lb_CathodeID.InvokeRequired)
                            {
                                lb_CathodeID.Invoke(new Action(() =>
                                {
                                    lb_CathodeID.Text = $"Cell ID : {mCell_ID_CA}";
                                }));
                            }
                            else
                            {
                                lb_CathodeID.Text = $"Cell ID : {mCell_ID_CA}";
                            }
                            //lb_CathodeID.Text = $"Cell ID : {mCell_ID_CA}";
                        }
                    }
                }
                else // 음극 셀 아이디 별도 로딩
                {
                    // 음극 셀 아이디가 별도로 있으면
                    if (!string.IsNullOrEmpty(mPlc_Read_CELLID_AN))
                    {
                        mCell_ID_AN = mPLC.ReadString(mPlc_Read_CELLID_AN, 9);
                        mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} Cell ID : {mCell_ID_AN}", CamName(camIdx));

                        if (mCell_ID_AN == "-9999")
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"CommunicationError Cell_ID Read : {mCommunicationErr}", CamName(camIdx));

                            return;
                        }

                        // Temp용 셀아이디 가져오기
                        if (!string.IsNullOrEmpty(temp_cell_AN_nm))
                        {
                            mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} Cell ID : {temp_cell_AN_nm}", CamName(camIdx));
                            runResultJson.CELL_ID = temp_cell_AN_nm;

                            if (lb_CathodeID.InvokeRequired)
                            {
                                lb_CathodeID.Invoke(new Action(() =>
                                {
                                    lb_AnodeID.Text = $"Cell ID : {temp_cell_AN_nm}";
                                }));
                            }
                            else
                            {
                                lb_AnodeID.Text = $"Cell ID : {temp_cell_AN_nm}";
                            }

                            temp_cell_AN_nm = string.Empty;
                        }
                        else
                        {
                            runResultJson.CELL_ID = mCell_ID_AN;


                            if (lb_CathodeID.InvokeRequired)
                            {
                                lb_CathodeID.Invoke(new Action(() =>
                                {
                                    lb_AnodeID.Text = $"Cell ID : {mCell_ID_AN}";
                                }));
                            }
                            else
                            {
                                lb_AnodeID.Text = $"Cell ID : {mCell_ID_AN}";
                            }

                            //lb_AnodeID.Text = $"Cell ID : {mCell_ID_AN}";
                        }
                    }
                }



                try
                {
                    // PLC 결과 데이터 초기화 Cathode
                    if (camIdx == CAM_CATHODE)
                    {
                        if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Result)) mPLC.WriteShort(mPlc_Write_Cathode_Result, PLC_OUT_0);
                        if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Result_Code)) mPLC.WriteShort(mPlc_Write_Cathode_Result_Code, PLC_OUT_0);
                    }
                    // PLC 결과 데이터 초기화 Anode
                    else
                    {
                        if (!string.IsNullOrEmpty(mPlc_Write_Anode_Result)) mPLC.WriteShort(mPlc_Write_Anode_Result, PLC_OUT_0);
                        if (!string.IsNullOrEmpty(mPlc_Write_Anode_Result_Code)) mPLC.WriteShort(mPlc_Write_Anode_Result_Code, PLC_OUT_0);
                    }
                }
                catch (System.Exception)
                {
                    //
                }

                // 사용 안함  2024-06-18

                if (mCell_ID_CA == mCell_ID_AN)
                {
                    lb_AnodeID.Text = btnDisplayDual.BackColor == Color.Lime ? $"Cell ID : {runResultJson.CELL_ID}" : $"{Date.ToString("yyyy-MM-dd HH:mm:ss")}";
                }
                else
                {
                    lb_AnodeID.Text = $"Cell ID : {mCell_ID_AN}";
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"ReadDataFromPLC Error : {ex.Message}", CamName(camIdx));
            }
        }

        private void InspectVPDL(Bitmap bimage, int camIdx, cls_RunResultJson runResultJson, bool initialize = false)
        {
            // VPDL 검사
            if (m_VPDL_Bypass == true)
            {
                runResultJson.CORNER_JUDGE_STR = "OK";
                runResultJson.CORNER_JUDGE_CD = "00";
                runResultJson.CORNER_JUDGE_PLC_CD = 1;
                mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} VPDL Result : OK (Bypass)", CamName(camIdx));
            }
            else
            {
                try
                {
                    if (mViDi.Workspace != null)
                    {
                        if (m_VPDL_Locate == true)
                        {
                            // bluetool 검사
                            // 이미지 변환 후, BlueTool을 거치고 다시 이미지 변환을 진행한다.
                            // 이유는, BlueTool 이미지를 Storage상 문제 때문에 활용할 수 없다. 나중에 동일한 Quality로 BlueTool 된 이미지를 확보하기 위해서는 
                            // 압축한 이미지를 BlueTool 시켜야만 한다.

                            // "0" Stream이 없다면, 블루툴을 수행할 수 없으므로, 원본이미지를 바로 리턴한다.
                            byte[] jpgBytes;

                            if (!mViDi.Workspace.Streams.Names.Any(t => t == "0"))
                            {
                                jpgBytes = mImage.ConvertJpeg(bimage);
                            }
                            else
                            {
                                jpgBytes = mImage.ConvertJpeg(mViDi.RunLocate(mImage.ConvertJpeg(bimage).getBitmap(), runResultJson));
                                mLog.WriteLog("CAM", $"{CamName(camIdx)} VPDL ROI Detect End", CamName(camIdx));
                            }

                            Bitmap jpgImg = jpgBytes.getBitmap();


                            // 카메라 촬영이 처음일 경우에는 화면에 이미지만 띄우고, 블루툴이미지 저장은 하지 않는다.   2024-07-18
                            // bluetool Image 저장
                            if (camIdx == CAM_CATHODE)
                            {
                                if (!initialize) RunThreadSaveBlueTool(jpgBytes, runResultJson);
                            }
                            else
                            {
                                if (!initialize) RunThreadSaveBlueTool(jpgBytes, runResultJson);
                            }

                            // 딥러닝 검사
                            using (IImage image = new FormsImage((Bitmap)jpgImg.Clone()))
                            {
                                mViDi.RunVpdl(image, runResultJson);
                            }

                            if (string.IsNullOrEmpty(runResultJson.CORNER_JUDGE_STR))
                            {
                                mCommunicationErr++;
                                mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} VPDL Processing Failed");
                            }

                            mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} VPDL Result : {runResultJson.CORNER_JUDGE_STR}", CamName(camIdx));
                        }
                        else
                        {
                            // 이미지 변환
                            Bitmap jpgImg = mImage.ConvertJpeg(bimage).getBitmap();

                            // 딥러닝 검사
                            using (IImage image = new FormsImage((Bitmap)jpgImg.Clone()))
                            {
                                mViDi.RunVpdl(image, runResultJson);
                            }

                            if (string.IsNullOrEmpty(runResultJson.CORNER_JUDGE_STR))
                            {
                                mCommunicationErr++;
                                mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} VPDL Processing Failed");
                            }

                            mLog.WriteLog("CAM", $"{runResultJson.CORNER_NAME} VPDL Result : {runResultJson.CORNER_JUDGE_STR}", CamName(camIdx));
                        }
                    }
                    else
                    {
                        //250624 -> vpdl bypass가 true가 아닌 경우, 모델이 로드 안되어 있으면 communicationErr 발생
                        mCommunicationErr++;
                        mLog.WriteLog("CAM", $"CommunicationError VPDL MODEL : Model is null", CamName(camIdx));
                    }
                }
                catch (System.Exception ex)
                {
                    mLog.WriteLog("CAM", $"InspectVPDL : {ex.Message}", CamName(camIdx));
                }
            }
        }

        private void RunThreadSaveBlueTool(byte[] jpgBytes, cls_RunResultJson runResultJson)
        {
            try
            {
                if (Directory.Exists(mLocateLocalSaveDir))
                    runResultJson.BLUTOL_LOC_FILE_NM = "TAB_" + runResultJson.CELL_ID + "_" + runResultJson.CELL_ID_DTM.ToString("yyyyMMddHHmmss") + "_" + runResultJson.POSITION + "_BLUTOL.jpg";

                if (Directory.Exists(mLocateVISSSaveDir))
                    runResultJson.BLUTOL_VIS_FILE_NM = "TAB_" + runResultJson.CELL_ID + "_" + runResultJson.CELL_ID_DTM.ToString("yyyyMMddHHmmss") + "_" + runResultJson.POSITION + "_BLUTOL.jpg";

                Thread thSaveBlueToolImage = new Thread(() => SaveBlueToolImage(jpgBytes, runResultJson));
                thSaveBlueToolImage.IsBackground = true;
                thSaveBlueToolImage.Start();
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("SYS", $"Save BlueTool Image Error : {ex.Message}");
            }
        }

        private void SaveBlueToolImage(byte[] jpgBytes, cls_RunResultJson runResultJson)
        {
            try
            {
                if (Directory.Exists(mLocateLocalSaveDir))
                    SaveJpgBytesImage(jpgBytes, runResultJson.BLUTOL_LOC_FILE_DIR, runResultJson.BLUTOL_LOC_FILE_NM);

                if (Directory.Exists(mLocateVISSSaveDir))
                    SaveJpgBytesImage(jpgBytes, runResultJson.BLUTOL_VIS_FILE_DIR, runResultJson.BLUTOL_VIS_FILE_NM);
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("SYS", $"Save BlueTool Image Error : {ex.Message}");
            }
        }

        private void SaveJpgBytesImage(byte[] jpgBytes, string saveFileDir, string saveFileName)
        {
            try
            {
                if (!Directory.Exists(saveFileDir)) Directory.CreateDirectory(saveFileDir);
                using (FileStream fileStream = new FileStream(Path.Combine(saveFileDir, saveFileName), FileMode.Create))
                {
                    fileStream.Write(jpgBytes, 0, jpgBytes.Length);
                    fileStream.Close();
                }

            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("SYS", $"Save BlueTool Image Error : {ex.Message}");
            }
        }

        private void InspectVPRO(Bitmap bimage, int camIdx, cls_RunResultJson runResultJson)
        {
            // ToolBlock과 display 객체 선언
            CogToolBlock toolBlock = null;
            CogToolDisplay cogDisplay = null;
            string toolType = string.Empty;

            if (m_VPRO_Bypass == true)
            {
                mLog.WriteLog("VPRO", $"{runResultJson.CORNER_NAME} VPRO Result : OK (Bypass)");
            }
            else
            {
                try
                {
                    // ToolBlock과 display 설정
                    if (camIdx == CAM_CATHODE)
                    {
                        toolType = "Cathode";
                        cogDisplay = displayCathode;


                        if (cls_GlobalValue.ChangeToolblock_Cathode)
                        {
                            CathodeToolBlock = CogSerializer.DeepCopyObject(cls_GlobalValue.Model.CathodeToolBlock) as CogToolBlock;
                            //displayCathode.Tool = CathodeToolBlock;
                            toolBlock = CathodeToolBlock;
                            cls_GlobalValue.ChangeToolblock_Cathode = false;
                        }
                        else
                            toolBlock = CathodeToolBlock;
                    }
                    else if (camIdx == CAM_ANODE)
                    {
                        toolType = "Anode";
                        cogDisplay = displayAnode;
                        if (cls_GlobalValue.ChangeToolblock_Anode)
                        {
                            AnodeToolBlock = CogSerializer.DeepCopyObject(cls_GlobalValue.Model.AnodeToolBlock) as CogToolBlock;
                            //displayAnode.Tool = AnodeToolBlock;
                            toolBlock = AnodeToolBlock;
                            cls_GlobalValue.ChangeToolblock_Anode = false;
                        }
                        else
                            toolBlock = AnodeToolBlock;
                    }

                    if (string.IsNullOrEmpty(toolType))
                    {
                        mLog.WriteLog("VPRO", $"{runResultJson.CORNER_NAME} {runResultJson.CELL_ID} VPRO Error: Tool type not Defined.");
                        return;
                    }

                    // Display에서 이전 그래픽 클리어
                    cogDisplay.Display.InteractiveGraphics.Clear();

                    // ToolBlock 입력값 설정
                    toolBlock.Inputs[$"{toolType}Image"].Value = new CogImage24PlanarColor(bimage);

                    // ToolBlock 실행
                    toolBlock.Run();
                    cogDisplay.UserRecord = toolBlock.CreateLastRunRecord();

                    // Display 새로 고침
                    //cogDisplay.Display.Refresh();

                    // 결과값 넣기
                    string vproResult = toolBlock.Outputs["Result"].Value.ToString();
                    string vproError = toolBlock.Outputs["ErrorMessage"].Value.ToString();

                    runResultJson.VPRO_JUDGE_PLC_STR = vproResult;

                    // 결과 로그 남기기
                    if (vproResult != "OK" && vproResult != "NG")
                        mLog.WriteLog("VPRO", $"{runResultJson.CORNER_NAME} {runResultJson.CELL_ID} VPRO Reulst : {runResultJson.VPRO_JUDGE_PLC_STR} || ERROR : {vproError}");
                    else
                        mLog.WriteLog("VPRO", $"{runResultJson.CORNER_NAME} {runResultJson.CELL_ID} VPRO Reulst : {runResultJson.VPRO_JUDGE_PLC_STR}");

                    // 이미지 저장 (OK가 아닐 경우를 제외하고 저장)
                    if (vproResult != "OK")
                    {
                        Bitmap bmp = new Bitmap(cogDisplay.Display.CreateContentBitmap(CogDisplayContentBitmapConstants.Custom, null, 0));
                        RunThreadSaveVPRO(bmp, runResultJson);
                    }
                }
                catch (System.Exception ex)
                {
                    mLog.WriteLog("VPRO", $"{runResultJson.CORNER_NAME} {runResultJson.CELL_ID} VPRO Error: {ex.Message}");
                    return;
                }
            }
        }
        private void RunThreadSaveVPRO(Bitmap bmp, cls_RunResultJson runResultJson)
        {
            try
            {
                Thread thSaveVproImage = new Thread(() => SaveVPROImage(bmp, runResultJson));
                thSaveVproImage.IsBackground = true;
                thSaveVproImage.Start();
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("VPRO", $"Save VPRO Image Error : {ex.Message}");
            }
        }

        private void SaveVPROImage(Bitmap bmp, cls_RunResultJson runResultJson)
        {
            try
            {
                if (Directory.Exists(mVPROLocalSaveDir))
                {
                    runResultJson.VPRO_LOC_FILE_NM = "TAB_" + runResultJson.CELL_ID + "_" + runResultJson.CELL_ID_DTM.ToString("yyyyMMddHHmmss") + "_" + runResultJson.POSITION + "_VPRO.jpg";

                    cls_File.CreateFolder(runResultJson.VPRO_LOC_FILE_DIR);
                    string saveLocal = Path.Combine(runResultJson.VPRO_LOC_FILE_DIR, runResultJson.VPRO_LOC_FILE_NM);
                    mImage.FileStreamSave(bmp, saveLocal);
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("VPRO", $"Save VPRO Image Error : {ex.Message}");
            }
        }

        private void SendResultToPLC(string plcResult, string plcResult_Code, string plcVproResult, cls_RunResultJson runResultJson, int camIdx)
        {
            try
            {
                if (camIdx == CAM_CATHODE)
                {
                    if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Cell_Id))
                    {
                        if (mPLC.WriteString(mPlc_Write_Cathode_Cell_Id, runResultJson.CELL_ID) == -9999)
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"CommunicationError Write Cell ID: {mCommunicationErr}", CamName(camIdx));
                            return;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(mPlc_Write_Anode_Cell_Id))
                    {
                        if (mPLC.WriteString(mPlc_Write_Anode_Cell_Id, runResultJson.CELL_ID) == -9999)
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"CommunicationError Write Cell ID: {mCommunicationErr}", CamName(camIdx));
                            return;
                        }
                    }
                }

                // VPRO by Pass 일 경우는 무조건 1로 송신
                // 엠플러스 협의에 의하여 1:OK / 2:NG 임.
                if (m_VPRO_Bypass == true)
                {
                    if (!string.IsNullOrEmpty(plcVproResult))
                    {
                        if (mPLC.WriteShort(plcVproResult, (short)1) == -9999)
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("VPRO", $"CommunicationError VPRO OK result: {mCommunicationErr}");
                            return;
                        }
                    }
                }
                else
                {
                    if (runResultJson.VPRO_JUDGE_PLC_STR == "NG")
                    {
                        if (!string.IsNullOrEmpty(plcVproResult))
                        {
                            if (mPLC.WriteShort(plcVproResult, (short)2) == -9999)
                            {
                                mCommunicationErr++;
                                mLog.WriteLog("VPRO", $"CommunicationError VPRO NG result: {mCommunicationErr}");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(plcVproResult))
                        {
                            if (mPLC.WriteShort(plcVproResult, (short)1) == -9999)
                            {
                                mCommunicationErr++;
                                mLog.WriteLog("VPRO", $"CommunicationError VPRO OK result: {mCommunicationErr}");
                                return;
                            }
                        }
                    }
                }

                // by Pass로 PLC에는 무조건 1로만 송신한다.
                if (m_PLC_Bypass == true)
                {
                    if (!string.IsNullOrEmpty(plcResult))
                    {
                        if (mPLC.WriteShort(plcResult, PLC_RESULT_OK) == -9999)
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"CommunicationError OK result: {mCommunicationErr}", CamName(camIdx));
                            return;
                        }
                    }
                    if (!string.IsNullOrEmpty(plcResult_Code))
                    {
                        if (mPLC.WriteShort(plcResult_Code, PLC_RESULT_OK) == -9999)
                        {
                            mCommunicationErr++;
                            mLog.WriteLog("CAM", $"CommunicationError OK result: {mCommunicationErr}", CamName(camIdx));
                            return;
                        }
                    }
                }
                else
                {
                    if (btn_PLC_Connect.Text == "Connect")
                    {
                        if (runResultJson.CORNER_JUDGE_STR == "OK")
                        {
                            if (!string.IsNullOrEmpty(plcResult))
                            {
                                if (mPLC.WriteShort(plcResult, PLC_RESULT_OK) == -9999)
                                {
                                    mCommunicationErr++;
                                    mLog.WriteLog("CAM", $"CommunicationError OK result: {mCommunicationErr}", CamName(camIdx));
                                    return;
                                }
                            }
                            if (!string.IsNullOrEmpty(plcResult_Code))
                            {
                                if (mPLC.WriteShort(plcResult_Code, PLC_RESULT_OK) == -9999)
                                {
                                    mCommunicationErr++;
                                    mLog.WriteLog("CAM", $"CommunicationError OK result: {mCommunicationErr}", CamName(camIdx));
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(plcResult))
                            {
                                if (mPLC.WriteShort(plcResult, PLC_RESULT_NG) == -9999)
                                {
                                    mCommunicationErr++;
                                    mLog.WriteLog("CAM", $"CommunicationError NG result: {mCommunicationErr}", CamName(camIdx));
                                    return;
                                }
                            }
                            if (!string.IsNullOrEmpty(plcResult_Code))
                            {
                                if (mPLC.WriteShort(plcResult_Code, runResultJson.CORNER_JUDGE_PLC_CD) == -9999)
                                {
                                    mCommunicationErr++;
                                    mLog.WriteLog("CAM", $"CommunicationError NG result: {mCommunicationErr}", CamName(camIdx));
                                    return;
                                }
                            }
                        }
                    }
                }
                mLog.WriteLog("CAM", $"{CamName(camIdx)} PLC Result Send OK", CamName(camIdx));
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"SendResultToPLC Error : {ex.Message}", CamName(camIdx));
            }
        }

        private void SendResultToIO(int ioResult, int camIdx)
        {
            try
            {
                AdvanDevice.OutputStart(ioResult, true, 0, 0, 0);

                // skbm에서는 PLC에 결과가 들어오기전에 IO Ready가 먼저 살아나는 오류가 있다고 해서, PLC 전송 이후에 IO 전송을 하도록 이부분을 SendResultToIO로 위치 이동했다.
                // 혹시 비슷한 문제가 발생한다면, 동일한 조건으로 이동시킬 필요가 있다. 2024-07-18
                if (camIdx == CAM_CATHODE)
                    AdvanDevice.OutputStart(mIO_OUT_CA_READY, true, 0, 0, 0);
                else
                    AdvanDevice.OutputStart(mIO_OUT_AN_READY, true, 0, 0, 0);

                mLog.WriteLog("CAM", $"{CamName(camIdx)} I/O Result Send OK", CamName(camIdx));
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"I/O SendResult Error : {ex.Message}", CamName(camIdx));
            }
        }

        private void Anode_GrabEvent(Bitmap bimage)
        {
            PolarityGrabEvent(bimage, CAM_ANODE);
        }

        private void btn_Anode_Connect_Click(object sender, EventArgs e)
        {
            ConnectPolarity(CAM_ANODE);
        }

        private void btn_Cathode_Connect_Click(object sender, EventArgs e)
        {
            ConnectPolarity(CAM_CATHODE);
        }

        private void ConnectPolarity(int camIndex)
        {
            if (camIndex == CAM_CATHODE)
            {
                if (btn_Cathode_Connect.Text == "Connect")
                {
                    if (Cathode_Vision_Init(txt_CathodeVision_IP.Text))
                    {
                        mbCathode_Connect = true;
                        AdvanDevice.OutputStart(mIO_OUT_CA_READY, true, 0, 0, 0);
                        MessageBox.Show("Camera Connect Success");
                    }
                    else
                    {
                        mbCathode_Connect = false;
                        AdvanDevice.OutputStart(mIO_OUT_CA_READY, false, 0, 0, 0);
                        MessageBox.Show("Camera Connect Fail");
                    }
                }
                else
                {
                    mCamera.Disconnect(CAM_CATHODE);
                    mbCathode_Connect = false;
                    btn_Cathode_Connect.Text = "Disconnect";
                    btn_Cathode_Connect.BackColor = Color.LightGray;

                    btnCathodeLive.Text = "Cathode Live Off";
                    btnCathodeLive.BackColor = Color.LightGray;
                    mbCathode_Live = false;

                    btnCathodeLive.Enabled = false;
                }
            }
            else if (camIndex == CAM_ANODE)
            {
                if (btn_Anode_Connect.Text == "Connect")
                {
                    if (Anode_Vision_Init(txt_AnodeVision_IP.Text))
                    {
                        mbAndoe_Connect = true;
                        AdvanDevice.OutputStart(mIO_OUT_AN_READY, true, 0, 0, 0);
                        MessageBox.Show("Camera Connect Success");
                    }
                    else
                    {
                        mbAndoe_Connect = false;
                        AdvanDevice.OutputStart(mIO_OUT_AN_READY, false, 0, 0, 0);
                        MessageBox.Show("Camera Connect Fail");
                    }
                }
                else
                {
                    mCamera.Disconnect(CAM_ANODE);
                    mbAndoe_Connect = false;
                    btn_Anode_Connect.Text = "Disconnect";
                    btn_Anode_Connect.BackColor = Color.LightGray;

                    btnAnodeLive.Text = "Anode Live Off";
                    btnAnodeLive.BackColor = Color.LightGray;
                    mbAnode_Live = false;

                    btnAnodeLive.Enabled = false;
                }
            }
        }

        private void SoftwareTriggerCamera(int camIndex)
        {
            try
            {
                mCamera.SoftwareTrigger(camIndex);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAnodeLive_Click(object sender, EventArgs e)
        {
            LivePolarity(CAM_ANODE, btnAnodeLive, mbAnode_Live);
        }

        private void btnCathodeLive_Click(object sender, EventArgs e)
        {
            LivePolarity(CAM_CATHODE, btnCathodeLive, mbCathode_Live);
        }

        private void LivePolarity(int camIndex, Button btnLivePolarity, bool mbLivePolarity)
        {
            try
            {
                if (btnLivePolarity.Text.Contains("Off"))
                {
                    mCamera.TriggerModeOff(camIndex);
                    mLog.WriteLog("CAM", $"{CamName(camIndex)} Live On");
                    btnLivePolarity.Text = "Live On";
                    btnLivePolarity.BackColor = Color.Lime;
                    mbLivePolarity = true;
                }
                else
                {
                    mCamera.TriggerModeOn(camIndex);
                    mLog.WriteLog("CAM", $"{CamName(camIndex)} Live Off");
                    btnLivePolarity.Text = "Live Off";
                    btnLivePolarity.BackColor = Color.LightGray;
                    mbLivePolarity = false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error : " + ex.ToString());
            }
        }

        #endregion Camera

        #region Utility

        private void btn_FilePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_LocalImageSavePath);
        }

        private void btn_LocalImageSavePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_LocalImageSavePath);
        }

        private void btn_NasFilePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_NasSavePath);
        }

        private void btn_VproModelPath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_VproModelPath);
        }

        private void btn_LocateLocalSavePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_LocateLocalSavePath);
        }

        private void btn_LocateVISSSavePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_LocateVISSSavePath);
        }

        private void btn_HeatmapLocalSavePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_HeatmapLocalSavePath);
        }

        private void btn_HeatmapVISSSavePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_HeatmapVISSSavePath);
        }

        private void btn_JsonSavePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_JsonSavePath);
        }

        private void btn_WorkspacePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFD_WorkspaceOpen = new OpenFileDialog();
            oFD_WorkspaceOpen.Filter = "ViDi Runtime Workspaces (*.vrws)|*.vrws";

            if (oFD_WorkspaceOpen.ShowDialog() == DialogResult.OK)
                lbl_WorkspacePath.Text = oFD_WorkspaceOpen.FileName;
        }

        private void SaveImage(Bitmap bimage, int camIdx, cls_RunResultJson runResultJson)
        {
            string polaStr = (camIdx == CAM_CATHODE) ? "CA" : "AN";
            string posStr = mPosition == "BOTTOM" ? "BOT" : mPosition;

            DateTime Date = runResultJson.CELL_ID_DTM;      //촬상 시간 기준으로 VISS 저장
            //DateTime lotDate = Date.AddHours(-6);   // Lot Day 기준 변경점 (240603-현재는 사용하지 않음. 기준이 결정된다면 LotDay를 사용할것.)
            string year = Date.ToString("yyyy");
            string day = Date.ToString("MMdd");
            //string lotDay = lotDate.ToString("MMdd");
            string Hour = Date.ToString("HH");
            string datefullstr = Date.ToString("yyyyMMddHHmmss");

            // 표준파일명형식 : TAB_CELLID_TIME_CRACK AN(TOP)_OK/NG  (대문자 사용)

            runResultJson.IMG_CLCT_DTM = Date.ToString("yyyy-MM-dd HH:mm:ss.fff");  // IMG_TAKEN_DTM 에서 IMG_CLCT_DTM으로 변경 2024-11-25 Ntels요청 /촬상시간 기준으로 VISS 저장 수정 2025_10_17
            string imageFileDate = runResultJson.CELL_ID_DTM.ToString("yyyyMMddHHmmss");
            runResultJson.IMG_FILE_DIR = Path.Combine(mImageSaveDir, mLineNo, year, day, Hour);  // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15
            runResultJson.IMG_FILE_NM = $"TAB_{runResultJson.CELL_ID}_{imageFileDate}_{mProcess} {polaStr}({posStr})_{runResultJson.CORNER_JUDGE_STR}.jpg";

            if (mVissSaveDirFormat == "01")
            {
                runResultJson.ORGL_IMG_VIS_FILE_DIR = Path.Combine(mVissSaveDir, year, day);              // OH1 저장 방식
            }
            else
            if (mVissSaveDirFormat == "02")
            {
                // Root\tab\Line 에서 Root\Line 바로 붙도록 변경 2024-07-15
                runResultJson.ORGL_IMG_VIS_FILE_DIR = Path.Combine(mVissSaveDir, mLineNo, year, day, Hour); // OH2 저장방식 
            }

            runResultJson.ORGL_IMG_VIS_FILE_NM = $"TAB_{runResultJson.CELL_ID}_{imageFileDate}_{mProcess} {polaStr}({posStr})_{runResultJson.CORNER_JUDGE_STR}.jpg";
            runResultJson.IMG_ST_CD = "CLCT_ST_CD_03";

            // Image 저장
            try
            {
                Thread thSaveImage = new Thread(() => SaveImageProcess(bimage, runResultJson));
                thSaveImage.IsBackground = true;
                thSaveImage.Start();
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Save Image Error : {ex.Message}", CamName(camIdx));
            }

            // 히트맵 이미지 저장 
            try
            {
                //if (runResultJson.CORNER_JUDGE_STR == "NG")
                //{

                if (Directory.Exists(mHeatmapLocalSaveDir))
                {
                    if (runResultJson.COMB_HEATMAP_IMAGE != null)
                        runResultJson.HTMP_LOC_FILE_NM = $"TAB_{runResultJson.CELL_ID}_{runResultJson.CELL_ID_DTM.ToString("yyyyMMddHHmmss")}_{runResultJson.POSITION}_HTMT.jpg";
                }

                if (Directory.Exists(mHeatmapVISSSaveDir))
                {
                    if (runResultJson.COMB_HEATMAP_IMAGE != null)
                        runResultJson.HTMP_VIS_FILE_NM = $"TAB_{runResultJson.CELL_ID}_{runResultJson.CELL_ID_DTM.ToString("yyyyMMddHHmmss")}_{runResultJson.POSITION}_HTMT.jpg";
                }

                Thread thSaveHeatmap = new Thread(() => SaveHeatmapImageProcess(runResultJson));
                thSaveHeatmap.IsBackground = true;
                thSaveHeatmap.Start();
                //}
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Heatmap SaveImage Error : {ex.Message}");
            }
        }

        private void SaveHeatmapImageProcess(cls_RunResultJson runResultJson)
        {
            try
            {
                if (Directory.Exists(mHeatmapLocalSaveDir))
                {
                    if (runResultJson.COMB_HEATMAP_IMAGE != null)
                    {
                        if (!Directory.Exists(runResultJson.HTMP_LOC_FILE_DIR)) Directory.CreateDirectory(runResultJson.HTMP_LOC_FILE_DIR);
                        mImage.FileStreamSave(runResultJson.COMB_HEATMAP_IMAGE, Path.Combine(runResultJson.HTMP_LOC_FILE_DIR, runResultJson.HTMP_LOC_FILE_NM));
                    }
                }

                if (Directory.Exists(mHeatmapVISSSaveDir))
                {
                    if (runResultJson.COMB_HEATMAP_IMAGE != null)
                    {
                        if (!Directory.Exists(runResultJson.HTMP_VIS_FILE_DIR)) Directory.CreateDirectory(runResultJson.HTMP_VIS_FILE_DIR);
                        mImage.FileStreamSave(runResultJson.COMB_HEATMAP_IMAGE, Path.Combine(runResultJson.HTMP_VIS_FILE_DIR, runResultJson.HTMP_VIS_FILE_NM));
                    }
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Heatmap SaveImage Error : {ex.Message}");
            }
        }

        private void SaveImageProcess(Bitmap b, cls_RunResultJson runResultJson)
        {
            try
            {
                using (Bitmap image = new Bitmap(b))
                {
                    try
                    {
                        cls_File.CreateFolder(runResultJson.IMG_FILE_DIR);
                        string saveLocal = Path.Combine(runResultJson.IMG_FILE_DIR, runResultJson.IMG_FILE_NM);
                        mImage.FileStreamSave(image, saveLocal);

                        cls_File.CreateFolder(runResultJson.ORGL_IMG_VIS_FILE_DIR);
                        string saveViss = Path.Combine(runResultJson.ORGL_IMG_VIS_FILE_DIR, runResultJson.ORGL_IMG_VIS_FILE_NM);
                        mImage.FileStreamSave(image, saveViss);
                    }
                    catch (System.Exception ex)
                    {
                        _imageSave_ErrorCount++;
                        mLog.WriteLog("IO", $"{runResultJson.CORNER_NAME} Image Save Error ({_imageSave_ErrorCount}) : {ex.ToString()}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                _imageSave_ErrorCount++;
                mLog.WriteLog("IO", $"{runResultJson.CORNER_NAME} Image Save Error ({_imageSave_ErrorCount}) : {ex.ToString()}");
            }
        }

        private void AddLogList(string strlog)
        {
            this.Invoke(new Action(delegate ()
            {
                if (lv_LogList.Items.Count > 400)
                    lv_LogList.Items.RemoveAt(0);

                ListViewItem itm = new ListViewItem(strlog);
                lv_LogList.Items.Add(itm);

                lv_LogList.EnsureVisible(lv_LogList.Items.Count - 1);
            }));
        }

        private string CamName(int camIdx)
        {
            if (camIdx == CAM_CATHODE)
            {
                return "Cathode";
            }
            else
            {
                return "Anode";
            }
        }

        private string FindCornerId(int camIdx)
        {
            string cornerId = "";
            if (mPosition == "TOP" && camIdx == CAM_CATHODE)
            {
                cornerId = "1";
            }
            else if (mPosition == "TOP" && camIdx == CAM_ANODE)
            {
                cornerId = "2";
            }
            else if (mPosition == "BOTTOM" && camIdx == CAM_CATHODE)
            {
                cornerId = "3";
            }
            else if (mPosition == "BOTTOM" && camIdx == CAM_ANODE)
            {
                cornerId = "4";
            }

            return cornerId;
        }

        #endregion Utility

        #region Inpection_VPRO

        public static Cognex.VisionPro.ICogImage ImageOpen(string _Inspection)
        {
            OpenFileDialog oFD_IamgeOpen = new OpenFileDialog();
            string l_strFilepath = "";
            Bitmap l_BmpTempImage;

            if (oFD_IamgeOpen.ShowDialog() == DialogResult.OK)
            {
                l_strFilepath = oFD_IamgeOpen.FileName;

                l_BmpTempImage = new Bitmap(l_strFilepath);

                if (_Inspection != "Color")
                {
                    Cognex.VisionPro.CogImage8Grey m_CogsettingImage = new Cognex.VisionPro.CogImage8Grey(l_BmpTempImage);
                    return m_CogsettingImage;
                }
                else
                {
                    Cognex.VisionPro.CogImage24PlanarColor m_CogsettingImage = new Cognex.VisionPro.CogImage24PlanarColor(l_BmpTempImage);
                    return m_CogsettingImage;
                }
            }
            else
            {
                IDMAX_FrameWork.MsgBox.Show("Image Load Error", "Image Load", IDMAX_FrameWork.MsgBox.Buttons.OK, IDMAX_FrameWork.MsgBox.Icon.Error);
                return null;
            }
        }

        private void materialTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            curTabpage = materialTabControl1.SelectedTab.Name;
        }

        #endregion Inpection_VPRO

        #region MainViewSetup

        private void btnViewGuideLine_Click(object sender, EventArgs e)
        {
            // 화면에 가이드라인을 표시하거나 숨긴다.
            try
            {
                if (btnViewGuideLine.Text == "Guide Line On")
                {
                    btnViewGuideLine.Text = "Guide Line Off";
                    btnViewGuideLine.BackColor = Color.LightGray;
                    mLog.WriteLog("CAM", "Guide Line Off");

                    displayCathode.Display.StaticGraphics.Clear();
                    displayAnode.Display.StaticGraphics.Clear();
                }
                else
                {
                    btnViewGuideLine.Text = "Guide Line On";
                    btnViewGuideLine.BackColor = Color.Lime;
                    mLog.WriteLog("CAM", "Guide Line On");

                    mDisplay.ViewGuideLineX(displayCathode.Display, camGuideLine_line1_x);
                    mDisplay.ViewGuideLineX(displayAnode.Display, camGuideLine_line1_x);

                    mDisplay.ViewGuideLineX(displayCathode.Display, camGuideLine_line2_x);
                    mDisplay.ViewGuideLineX(displayAnode.Display, camGuideLine_line2_x);

                    mDisplay.ViewGuideLineY(displayCathode.Display, camGuideLine_line3_y);
                    mDisplay.ViewGuideLineY(displayAnode.Display, camGuideLine_line3_y);
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("System", $"Guide Line Error : {ex.Message}");
            }
        }

        private void btnViewGuideRect_Click(object sender, EventArgs e)
        {
            // 화면에 가이드라인을 표시하거나 숨긴다.
            try
            {
                if (btnViewGuideRect.Text == "Guide Rectangle On")
                {
                    btnViewGuideRect.Text = "Guide Rectangle Off";
                    btnViewGuideRect.BackColor = Color.LightGray;
                    mLog.WriteLog("CAM", "Guide Rectangle Off");

                    displayCathode.Display.InteractiveGraphics.Clear();
                    displayAnode.Display.InteractiveGraphics.Clear();
                }
                else // btnViewGuideRect.Text = "Guide Rectangle Off"
                {
                    btnViewGuideRect.Text = "Guide Rectangle On";
                    // btnViewGuideRect.Text = "Guide Rectangle On";
                    btnViewGuideRect.BackColor = Color.Lime;
                    mLog.WriteLog("CAM", "Guide Rectangle On");

                    mDisplay.DrawRect(displayCathode.Display, brightPositionCA_x, brightPositionCA_y);
                    mDisplay.DrawRect(displayAnode.Display, brightPositionAN_x, brightPositionAN_y);
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("System", $"Guide Rectangle Error : {ex.Message}");
            }
        }

        private void btnLightSave_Click(object sender, EventArgs e)
        {
            int lightMaxValue = mLightControllerMaker == "MEGA" ? 255 : 1024;

            // 조명 밝기값 저장. ini에도 저장
            try
            {
                // 251219 이전 저장 값 읽기
                int oldLightCA = int.Parse(ini.ReadIniValue("CameraSetup", "Cathode_Light", "100"));
                int oldLightAN = int.Parse(ini.ReadIniValue("CameraSetup", "Anode_Light", "100"));

                if (int.Parse(tbxLightCA.Text) < 1) tbxLightCA.Text = "1";
                if (int.Parse(tbxLightAN.Text) < 1) tbxLightAN.Text = "1";
                if (int.Parse(tbxLightCA.Text) > lightMaxValue) tbxLightCA.Text = lightMaxValue.ToString();
                if (int.Parse(tbxLightAN.Text) > lightMaxValue) tbxLightAN.Text = lightMaxValue.ToString();

                int newLightCA = int.Parse(tbxLightCA.Text);
                int newLightAN = int.Parse(tbxLightAN.Text);

                // 251219 변경 로그
                if (oldLightCA != newLightCA)
                {
                    mLog.WriteLog("TEST", $"Cathode Light Changed : {oldLightCA} -> {newLightCA}");
                }
                if (oldLightAN != newLightAN)
                {
                    mLog.WriteLog("TEST", $"Anode Light Changed : {oldLightAN} -> {newLightAN}");
                }

                lightControl.SetLightValue(tbxLightCA.Text, CAM_CATHODE);
                lightControl.SetLightValue(tbxLightAN.Text, CAM_ANODE);

                lightControl.SaveIniValue("Cathode_Light", tbxLightCA.Text);
                lightControl.SaveIniValue("Anode_Light", tbxLightAN.Text);

                ResetDirty();
                MessageBox.Show("Light Controller Saved.", "Save");
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("System", $"Light Error : {ex.Message}");
            }
        }

        private void btnCaWbSave_Click(object sender, EventArgs e)
        {
            try
            {
                int saveR = int.Parse(tbxCaWbB.Text);
                int saveG = int.Parse(tbxCaWbG.Text);
                int saveB = int.Parse(tbxCaWbB.Text);

                SetCameraWB(CAM_CATHODE, tbxCaWbR.Text, tbxCaWbG.Text, tbxCaWbB.Text);

                mLog.WriteLog("TEST", $"White Balance(Ca) saved : R={saveR}, G={saveG}, B={saveB}");

                ResetDirty();
                MessageBox.Show("White Balance(Ca) Saved.", "Save");
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Camera WB Setup Error : {ex.Message}");
            }
        }

        private void btnAnWbSave_Click(object sender, EventArgs e)
        {
            try
            {
                int saveR = int.Parse(tbxAnWbB.Text);
                int saveG = int.Parse(tbxAnWbG.Text);
                int saveB = int.Parse(tbxAnWbB.Text);

                SetCameraWB(CAM_ANODE, tbxAnWbR.Text, tbxAnWbG.Text, tbxAnWbB.Text);

                mLog.WriteLog("TEST", $"White Balance(An) saved : R={saveR}, G={saveG}, B={saveB}");

                ResetDirty();
                MessageBox.Show("White Balance(An) Saved.", "Save");
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Camera WB Setup Error : {ex.Message}");
            }
        }

        private void SetCameraWB(int camIdx, string valR, string valG, string valB)
        {
            // 화이트밸런스를 설정하고 ini에도 저장한다.

            string strP = "";
            if (camIdx == CAM_CATHODE)
            {
                strP = "Cathode";
            }
            else if (camIdx == CAM_ANODE)
            {
                strP = "Anode";
            }

            try
            {
                if (int.Parse(valR) < 1) valR = "1";
                if (int.Parse(valG) < 1) valG = "1";
                if (int.Parse(valB) < 1) valB = "1";
                if (int.Parse(valR) > 900) valR = "900";
                if (int.Parse(valG) > 900) valG = "900";
                if (int.Parse(valB) > 900) valB = "900";

                mCamera.WhiteBalance_SetVal(camIdx, "Red", valR);
                mCamera.WhiteBalance_SetVal(camIdx, "Green", valG);
                mCamera.WhiteBalance_SetVal(camIdx, "Blue", valB);

                ini.WriteIniValue("CameraSetup", strP + "_White_R", valR);
                ini.WriteIniValue("CameraSetup", strP + "_White_G", valG);
                ini.WriteIniValue("CameraSetup", strP + "_White_B", valB);
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Camera WB Setup Error : {ex.Message}");
            }
        }

        private void btnCaWbAuto_Click(object sender, EventArgs e)
        {
            GetCameraWb(CAM_CATHODE);
        }

        private void btnAnWbAuto_Click(object sender, EventArgs e)
        {
            GetCameraWb(CAM_ANODE);
        }

        private void GetCameraWb(int camIdx)
        {
            // 화이트밸런스를 자동으로 가져온다

            mCamera.WhiteBalance_Auto(camIdx);
            string valR = mCamera.WhiteBalance_GetVal(camIdx, "Red");
            string valG = mCamera.WhiteBalance_GetVal(camIdx, "Green");
            string valB = mCamera.WhiteBalance_GetVal(camIdx, "Blue");

            string strP = "";
            if (camIdx == CAM_CATHODE)
            {
                strP = "Cathode";
                tbxCaWbR.Text = valR;
                tbxCaWbG.Text = valG;
                tbxCaWbB.Text = valB;
            }
            else if (camIdx == CAM_ANODE)
            {
                strP = "Anode";
                tbxAnWbR.Text = valR;
                tbxAnWbG.Text = valG;
                tbxAnWbB.Text = valB;
            }

            //mLog.WriteLog("TEST", $"White Balance(An) saved : R={valR}, G={valG}, B={valB}");
            mLog.WriteLog("CAM", $"{strP} White Balance Auto R : {valR}");
            mLog.WriteLog("CAM", $"{strP} White Balance Auto G : {valG}");
            mLog.WriteLog("CAM", $"{strP} White Balance Auto B : {valB}");
        }

        private void btnTrigger_Click(object sender, EventArgs e)
        {
            try
            {
                //// 라인정보 가져오기
                string regLineNo = Regex.Replace(mLineNo, "[^0-9]", "");

                // Test Sample Cell 채번
                string tempCell = CellGenerator.CellGenerateCode(regLineNo, DateTime.Now, txtTempCell.Text);
                temp_cell_CA_nm = tempCell;
                temp_cell_AN_nm = tempCell;

                mCamera.SoftwareTrigger(CAM_CATHODE);
                mCamera.SoftwareTrigger(CAM_ANODE);
                //PolarityGrabEvent(new Bitmap(@"C:\Users\uisyh\OneDrive\Desktop\SKOJ1\5LINE\TAB_Y5080F89E830020714_20250809140402_CRACK CA(TOP)_OK.jpg"), CAM_CATHODE);
                //PolarityGrabEvent(new Bitmap(@"C:\Users\uisyh\OneDrive\Desktop\SKOJ1\5LINE\TAB_Y5080F89E830025207_20250809140854_CRACK AN(TOP)_OK.jpg"), CAM_ANODE);
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("CAM", $"Camera Trigger Error : {ex.Message}");
            }
        }

        private void BtnSetupView_Click(object sender, EventArgs e)
        {
            ViewCameraSetup();
        }

        private void ViewCameraSetup()
        {
            // 카메라 셋업 메뉴를 보여주거나 숨기는 기능. 색상으로 on/off 구분
            if (BtnCameraSetupView.BackColor != Color.Lime)
            {
                BtnCameraSetupView.BackColor = Color.Lime;
                panInspection.RowStyles[1] = new RowStyle(SizeType.Absolute, 120);
            }
            else
            {
                BtnCameraSetupView.BackColor = Color.LightGray;
                panInspection.RowStyles[1] = new RowStyle(SizeType.Absolute, 0);
            }
        }

        private void btnMainIOView_Click(object sender, EventArgs e)
        {
            // IO Lamp 메뉴를 보여주거나 숨기는 기능. 색상으로 on/off 구분
            if (btnMainIOView.BackColor != Color.Lime)
            {
                btnMainIOView.BackColor = Color.Lime;
                panInspection.RowStyles[2] = new RowStyle(SizeType.Absolute, 70);
            }
            else
            {
                btnMainIOView.BackColor = Color.LightGray;
                panInspection.RowStyles[2] = new RowStyle(SizeType.Absolute, 0);
            }
        }

        private void btnStatusView_Click(object sender, EventArgs e)
        {
            // Status 메뉴를 보여주거나 숨기는 기능. 색상으로 on/off 구분
            if (btnStatusView.BackColor != Color.Lime)
            {
                btnStatusView.BackColor = Color.Lime;
                panInspection.RowStyles[3] = new RowStyle(SizeType.Absolute, 70);
            }
            else
            {
                btnStatusView.BackColor = Color.LightGray;
                panInspection.RowStyles[3] = new RowStyle(SizeType.Absolute, 0);
            }
        }

        private void lblWBCA_Click(object sender, EventArgs e)
        {
            mDisplay.CogDisplayLoadImage(displayCathode.Display);
        }

        private void lblWBAN_Click(object sender, EventArgs e)
        {
            mDisplay.CogDisplayLoadImage(displayAnode.Display);
        }

        private void btnSaveExpGain_Click(object sender, EventArgs e)
        {
            // Exp gain저장. ini에도 저장
            try
            {
                // 251219 이전 저장 값 읽기
                int oldExpCa = int.Parse(ini.ReadIniValue("CameraSetup", "Cathode_Exposure", "1000"));
                int oldGainCa = int.Parse(ini.ReadIniValue("CameraSetup", "Cathode_Gain", "0"));
                int oldExpAn = int.Parse(ini.ReadIniValue("CameraSetup", "Anode_Exposure", "1000"));
                int oldGainAn = int.Parse(ini.ReadIniValue("CameraSetup", "Anode_Gain", "0"));

                // 입력값 보정
                if (int.Parse(tbxExpCa.Text) < 1) tbxExpCa.Text = "1";
                if (int.Parse(tbxGainCa.Text) < 1) tbxGainCa.Text = "0";
                if (int.Parse(tbxExpCa.Text) > 16777215) tbxExpCa.Text = "16777215";
                if (int.Parse(tbxGainCa.Text) > 208) tbxGainCa.Text = "208";
                if (int.Parse(tbxExpAn.Text) < 1) tbxExpAn.Text = "1";
                if (int.Parse(tbxGainAn.Text) < 1) tbxGainAn.Text = "0";
                if (int.Parse(tbxExpAn.Text) > 16777215) tbxExpAn.Text = "16777215";
                if (int.Parse(tbxGainAn.Text) > 208) tbxGainAn.Text = "208";

                int newExpCa = int.Parse(tbxExpCa.Text);
                int newGainCa = int.Parse(tbxGainCa.Text);
                int newExpAn = int.Parse(tbxExpAn.Text);
                int newGainAn = int.Parse(tbxGainAn.Text);

                // 251219 변경 로그
                if (oldExpCa != newExpCa)
                {
                    mLog.WriteLog("TEST", $"Cathode Exposure Changed : {oldExpCa} -> {newExpCa}");
                }
                if (oldGainCa != newGainCa)
                {
                    mLog.WriteLog("TEST", $"Cathode Gain Changed : {oldGainCa} -> {newGainCa}");
                }
                if (oldExpAn != newExpAn)
                {
                    mLog.WriteLog("TEST", $"Anode Exposure Changed : {oldExpAn} -> {newExpAn}");
                }
                if (oldGainAn != newGainAn)
                {
                    mLog.WriteLog("TEST", $"Anode Gain Changed : {oldGainAn} -> {newGainAn}");
                }

                SetCameraExposureGain(CAM_CATHODE, tbxExpCa.Text, tbxGainCa.Text);
                SetCameraExposureGain(CAM_ANODE, tbxExpAn.Text, tbxGainAn.Text);

                // 251219 ini 저장(갱신)
                ini.WriteIniValue("CameraSetup", "Cathode_Exposure", tbxExpCa.Text);
                ini.WriteIniValue("CameraSetup", "Cathode_Gain", tbxGainCa.Text);
                ini.WriteIniValue("CameraSetup", "Anode_Exposure", tbxExpAn.Text);
                ini.WriteIniValue("CameraSetup", "Anode_Gain", tbxGainAn.Text);

                ResetDirty();
                MessageBox.Show("Exposure/Gain Saved.", "Save");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error : " + ex.ToString());
                mLog.WriteLog("System", $"Exposure/Gain Save Error : {ex.Message}");
            }
        }

        private void SetCameraExposureGain(int camIdx, string expValue, string gainValue)
        {
            mCamera.ExposureValue(camIdx, expValue);
            mCamera.GainValue(camIdx, gainValue);

            ini.WriteIniValue("CameraSetup", $"{CamName(camIdx)}_Exposure", expValue);
            ini.WriteIniValue("CameraSetup", $"{CamName(camIdx)}_Gain", gainValue);
        }

        private void lblBrightCa_Click(object sender, EventArgs e)
        {
            lblBrightValCa.Text = mDisplay.ViewBrightValue(displayCathode.Display, brightPositionCA_x, brightPositionCA_y).ToString();
        }

        private void lblBrightAn_Click(object sender, EventArgs e)
        {
            lblBrightValAn.Text = mDisplay.ViewBrightValue(displayAnode.Display, brightPositionAN_x, brightPositionAN_y).ToString();
        }

        private void lblContrastCa_Click(object sender, EventArgs e)
        {
            lblContrastValCa.Text = mDisplay.ViewContrastValue(displayCathode.Display, brightPositionCA_x, brightPositionCA_y).ToString();
        }

        private void lblContrastAn_Click(object sender, EventArgs e)
        {
            lblContrastValAn.Text = mDisplay.ViewContrastValue(displayAnode.Display, brightPositionAN_x, brightPositionAN_y).ToString();
        }

        private void btnSetBrightPosition_Click2(object sender, EventArgs e)
        {
            if (btnViewGuideRect.Text.Contains("On"))
            {
                brightPositionCA_x = mDisplay.GetRectX(displayCathode.Display);
                brightPositionCA_y = mDisplay.GetRectY(displayCathode.Display);
                brightPositionAN_x = mDisplay.GetRectX(displayAnode.Display);
                brightPositionAN_y = mDisplay.GetRectY(displayAnode.Display);
                
                ini.WriteIniValue("CameraGuideLine", "brightPositionCA_x", brightPositionCA_x.ToString());
                ini.WriteIniValue("CameraGuideLine", "brightPositionCA_y", brightPositionCA_y.ToString());
                ini.WriteIniValue("CameraGuideLine", "brightPositionAN_x", brightPositionAN_x.ToString());
                ini.WriteIniValue("CameraGuideLine", "brightPositionAN_y", brightPositionAN_y.ToString());
            }
        }

        //251218 변경 로그 추가
        private void btnSetBrightPosition_Click(object sender, EventArgs e)
        {
            if (!btnViewGuideRect.Text.Contains("On"))
                return;
            try
            {
                // 이전 메모리 값 계산
                double oldBrightCa = mDisplay.ViewBrightValue(displayCathode.Display, brightPositionCA_x, brightPositionCA_y);
                double oldBrightAn = mDisplay.ViewBrightValue(displayAnode.Display, brightPositionAN_x, brightPositionAN_y);
                double oldContrastCa = mDisplay.ViewContrastValue(displayCathode.Display, brightPositionCA_x, brightPositionCA_y);
                double oldContrastAn = mDisplay.ViewContrastValue(displayAnode.Display, brightPositionAN_x, brightPositionAN_y);

                // 새 Guide Position 좌표 저장
                int newCA_x = mDisplay.GetRectX(displayCathode.Display);
                int newCA_y = mDisplay.GetRectY(displayCathode.Display);
                int newAN_x = mDisplay.GetRectX(displayAnode.Display);
                int newAN_y = mDisplay.GetRectY(displayAnode.Display);

                // 새로운 값 계산
                double newBrightCa = mDisplay.ViewBrightValue(displayCathode.Display, newCA_x, newCA_y);
                double newBrightAn = mDisplay.ViewBrightValue(displayAnode.Display, newAN_x, newAN_y);
                double newContrastCa = mDisplay.ViewContrastValue(displayCathode.Display, newCA_x, newCA_y);
                double newContrastAn = mDisplay.ViewContrastValue(displayAnode.Display, newAN_x, newAN_y);

                // 변경 로그
                if (Math.Abs(oldBrightCa - newBrightCa) > 0.0001)
                {
                    mLog.WriteLog("TEST", $"Guide Position Changed. Bright(CA) : {oldBrightCa:F6} → {newBrightCa:F6}");
                }
                if (Math.Abs(oldBrightAn - newBrightAn) > 0.0001)
                {
                    mLog.WriteLog("TEST", $"Guide Position Changed. Bright(AN) : {oldBrightAn:F6} → {newBrightAn:F6}");
                }
                if (Math.Abs(oldContrastCa - newContrastCa) > 0.0001)
                {
                    mLog.WriteLog("TEST", $"Guide Position Changed. Contrast(CA) : {oldContrastCa:F6} → {newContrastCa:F6}");
                }
                if (Math.Abs(oldContrastAn - newContrastAn) > 0.0001)
                {
                    mLog.WriteLog("TEST", $"Guide Position Changed. Contrast(AN) : {oldContrastAn:F6} → {newContrastAn:F6}");
                }

                // 메모리 값 갱신
                brightPositionCA_x = newCA_x;
                brightPositionCA_y = newCA_y;
                brightPositionAN_x = newAN_x;
                brightPositionAN_y = newAN_y;

                // ini 저장
                ini.WriteIniValue("CameraGuideLine", "brightPositionCA_x", newCA_x.ToString());
                ini.WriteIniValue("CameraGuideLine", "brightPositionCA_y", newCA_y.ToString());
                ini.WriteIniValue("CameraGuideLine", "brightPositionAN_x", newAN_x.ToString());
                ini.WriteIniValue("CameraGuideLine", "brightPositionAN_y", newAN_y.ToString());

                ResetDirty();

                MessageBox.Show("Guide Position Saved.", "Save");
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("System", $"Set Guide Position Error : {ex.Message}");
            }
        }

        private void btnDisplayDual_Click(object sender, EventArgs e)
        {
            btnDisplayDual.BackColor = Color.Lime;
            btnDisplayCathode.BackColor = Color.LightGray;
            btnDisplayAnode.BackColor = Color.LightGray;
            panDisplay.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 50);
            panDisplay.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 50);
        }

        private void btnDisplayCathode_Click(object sender, EventArgs e)
        {
            btnDisplayDual.BackColor = Color.LightGray;
            btnDisplayCathode.BackColor = Color.Lime;
            btnDisplayAnode.BackColor = Color.LightGray;
            panDisplay.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
            panDisplay.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, 0);
            displayCathode.Display.Zoom = 0.8;
        }

        private void btnDisplayAnode_Click(object sender, EventArgs e)
        {
            btnDisplayDual.BackColor = Color.LightGray;
            btnDisplayCathode.BackColor = Color.LightGray;
            btnDisplayAnode.BackColor = Color.Lime;
            panDisplay.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 100);
            panDisplay.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 0);
            displayAnode.Display.Zoom = 0.8;
        }

        #endregion MainViewSetup


        // RestAPI에서 Threshold 변경시 메모리에 반영하고, threshold ini도 수정하도록 변경... 2024-07-04
        public void SetThreshold(string key, double threshold)
        {
            switch (key)
            {
                case "CA_ROI1":
                    mThreshold_CA_R1 = threshold;
                    ini.WriteIniValue("Threshold", "Threshold_CA_R1", threshold.ToString());
                    ThresholdInvoke(tbxThreshold_CR1, threshold);
                    break;

                case "CA_ROI2":
                    mThreshold_CA_R2 = threshold;
                    ini.WriteIniValue("Threshold", "Threshold_CA_R2", threshold.ToString());
                    ThresholdInvoke(tbxThreshold_CR2, threshold);
                    break;

                case "CA_ROI3":
                    mThreshold_CA_R3 = threshold;
                    ini.WriteIniValue("Threshold", "Threshold_CA_R3", threshold.ToString());
                    ThresholdInvoke(tbxThreshold_CR3, threshold);
                    break;

                case "AN_ROI1":
                    mThreshold_AN_R1 = threshold;
                    ini.WriteIniValue("Threshold", "Threshold_AN_R1", threshold.ToString());
                    ThresholdInvoke(tbxThreshold_AR1, threshold);
                    break;

                case "AN_ROI2":
                    mThreshold_AN_R2 = threshold;
                    ini.WriteIniValue("Threshold", "Threshold_AN_R2", threshold.ToString());
                    ThresholdInvoke(tbxThreshold_AR2, threshold);
                    break;

                case "AN_ROI3":
                    mThreshold_AN_R3 = threshold;
                    ini.WriteIniValue("Threshold", "Threshold_AN_R3", threshold.ToString());
                    ThresholdInvoke(tbxThreshold_AR3, threshold);
                    break;

                default:
                    break;
            }
        }

        public void ThresholdInvoke(TextBox tbx, double threshold)
        {
            if (tbx.InvokeRequired)
            {
                // Use Invoke to update the control on the UI thread
                tbx.Invoke(new MethodInvoker(() => tbx.Text = Convert.ToString(threshold)));
            }
            else
            {
                // Directly update the control if we're already on the UI thread
                tbx.Text = Convert.ToString(threshold);
            }
        }

        public (bool, string) ModelChangeProcess(string modelFilePath, string modelFileName)
        {
            // ManualResetEvent 를 사용해서 Thread가 종료될때까지 기다리고 결과를 반환할 수 있다.
            ManualResetEvent threadEndEvent = new ManualResetEvent(false);

            var parameters = new ModelChangeParameters
            {
                FilePath = modelFilePath,
                FileName = modelFileName,
                ThreadEndEvent = threadEndEvent
            };

            ModelChange_Thread = new Thread(new ParameterizedThreadStart(ModelChangeProcessThread));
            ModelChange_Thread.IsBackground = true;
            ModelChange_Thread.Start(parameters);

            // 작업이 완료될 때까지 주기적으로 확인
            while (!threadEndEvent.WaitOne(100))
            {
                Application.DoEvents();  // ui가 응답 할 수 있도록 함.
            }

            // 모델 체인지 결과 리턴
            return (parameters.Result, parameters.ResultReason);
        }

        // Thread를 사용할때 Thread에 매개변수를 넘기기 위해서는 클래스를 생성해서 사용하는 방법이 있다.
        // Thread를 호출할때도 ParameterizedThreadStart 델리게이트를 사용해서 객체 형태로 매개변수를 전달한다.
        private class ModelChangeParameters
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public ManualResetEvent ThreadEndEvent { get; set; }
            public bool Result { get; set; }
            public string ResultReason { get; set; }
        }

        public void ModelChangeProcessThread(object parameters)
        {
            var modelParams = (ModelChangeParameters)parameters;
            string newModelPath = Path.Combine(modelParams.FilePath, modelParams.FileName);
            string oldModelPath = mWorkSpacePath;
            try
            {
                // PLC 확인 및 모델 교체 프로세스 시작 로그
                mLog.WriteLog("ModelChange", "Model Change Process - Start");
                try
                {
                    mLog.WriteLog("ModelChange", $"Current Workspace Name: {mViDi.WorkspaceName}");
                }
                catch (System.Exception)
                {
                    //
                }


                // PLC에 교체 요청 
                if (WaitForPLCConfirmation())
                {
                    // 모델 교체 작업 시작
                    PerformModelChange(newModelPath);
                    mLog.WriteLog("ModelChange", "Model Change Process - Completed");
                    modelParams.ResultReason = "Model Change Process - Completed";
                    modelParams.Result = true;
                }
            }
            catch (TimeoutException)
            {
                mLog.WriteLog("ModelChange", $"Model change process timed out waiting for PLC approval");
                mPLC.WriteShort(mPlc_Write_Change_Model_PC, PLC_OUT_0);
                mLog.WriteLog("ModelChange", "Model Change Process - Failed");
                modelParams.ResultReason = "Model change process timed out waiting for PLC approval";
                modelParams.Result = false;
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("ModelChange", $"Model change process encountered an error: {ex.Message}");
                mPLC.WriteShort(mPlc_Write_Change_Model_PC, PLC_OUT_0);

                // 모델이 교체 되면서 문제 터졌으면, 원복 시켜줘야 함.
                // mWorkSpacePath 가 변경되었다면 모델이 변경되었다고 봐야한다.
                if (mWorkSpacePath != oldModelPath)
                {
                    RestorePreviousModel(oldModelPath);
                }

                mLog.WriteLog("ModelChange", "Model Change Process - Failed");
                modelParams.ResultReason = $"Model change process encountered an error: {ex.Message}";
                modelParams.Result = false;
            }
            finally
            {
                modelParams.ThreadEndEvent.Set();
                try
                {
                    mLog.WriteLog("ModelChange", $"Current Workspace Name: {mViDi.WorkspaceName}");
                }
                catch (System.Exception)
                {
                    //
                }

            }

        }

        /// <summary>
        /// 이미지 이상감지 Error 발생 변수가 변경될 시,
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void _isRisingErrImageDetect_OnValueChanged(string key, bool newValue)
        {
            try
            {
                // false로 들어온다면, 해제된것임.
                if (!newValue)
                {
                    mLog.WriteLog("ANM", $"Cathode currernt Err Count Value Changed : {_imageBGT_CA_ErrorCount} | {_imageSHP_CA_ErrorCount} | {_imageLOC_CA_ErrorCount} | {_imageNG_CA_ErrorCount} | {_imageNG_INTV_CA_ErrorCount.Count(result => !result)}");
                    mLog.WriteLog("ANM", $"Anode currernt Err Count Value Changed : {_imageBGT_AN_ErrorCount} | {_imageSHP_AN_ErrorCount} | {_imageLOC_AN_ErrorCount} | {_imageNG_AN_ErrorCount} | {_imageNG_INTV_AN_ErrorCount.Count(result => !result)}");

                    switch (key)
                    {
                        case "_isRisingErrBGT_CA":
                            _imageBGT_CA_ErrorCount = 0;
                            ic_err_bgt.Off();
                            break;
                        case "_isRisingErrSHP_CA":
                            _imageSHP_CA_ErrorCount = 0;
                            ic_err_shp.Off();
                            break;
                        case "_isRisingErrLOC_CA":
                            _imageLOC_CA_ErrorCount = 0;
                            ic_err_loc.Off();
                            break;
                        case "_isRisingErrNG_CA":
                            _imageNG_CA_ErrorCount = 0;
                            ic_err_ng.Off();
                            break;
                        case "_isRisingErrNGINTV_CA":
                            _imageNG_INTV_CA_ErrorCount.Clear();
                            ic_err_ng_intv.Off();
                            break;
                        case "_isRisingErrBGT_AN":
                            _imageBGT_AN_ErrorCount = 0;
                            ic_err_bgt.Off();
                            break;
                        case "_isRisingErrSHP_AN":
                            _imageSHP_AN_ErrorCount = 0;
                            ic_err_shp.Off();
                            break;
                        case "_isRisingErrLOC_AN":
                            _imageLOC_AN_ErrorCount = 0;
                            ic_err_loc.Off();
                            break;
                        case "_isRisingErrNG_AN":
                            _imageNG_AN_ErrorCount = 0;
                            ic_err_ng.Off();
                            break;
                        case "_isRisingErrNGINTV_AN":
                            _imageNG_INTV_AN_ErrorCount.Clear();
                            ic_err_ng_intv.Off();
                            break;
                    }
                    mLog.WriteLog("ANM", $"Cathode currernt Err Count Value Changed : {_imageBGT_CA_ErrorCount} | {_imageSHP_CA_ErrorCount} | {_imageLOC_CA_ErrorCount} | {_imageNG_CA_ErrorCount} | {_imageNG_INTV_CA_ErrorCount.Count(result => !result)}");
                    mLog.WriteLog("ANM", $"Anode currernt Err Count Value Changed : {_imageBGT_AN_ErrorCount} | {_imageSHP_AN_ErrorCount} | {_imageLOC_AN_ErrorCount} | {_imageNG_AN_ErrorCount} | {_imageNG_INTV_AN_ErrorCount.Count(result => !result)}");
                }
                else
                {
                    switch (key)
                    {
                        case "_isRisingErrBGT_CA":
                        case "_isRisingErrBGT_AN":
                            ic_err_bgt.On();
                            break;
                        case "_isRisingErrSHP_CA":
                        case "_isRisingErrSHP_AN":
                            ic_err_shp.On();
                            break;
                        case "_isRisingErrLOC_CA":
                        case "_isRisingErrLOC_AN":
                            ic_err_loc.On();
                            break;
                        case "_isRisingErrNG_CA":
                        case "_isRisingErrNG_AN":
                            ic_err_ng.On();
                            break;
                        case "_isRisingErrNGINTV_CA":
                        case "_isRisingErrNGINTV_AN":
                            ic_err_ng_intv.On();
                            break;
                    }
                }
            }
            catch (System.Exception ex) { mLog.WriteLog("ANM", $"eventError : {ex.Message}"); }
        }

        private bool WaitForPLCConfirmation()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // PLC에 모델 교체 가능 여부 전송
            mLog.WriteLog("ModelChange", "Waiting for PLC approval...");
            mPLC.WriteShort(mPlc_Write_Change_Model_PC, PLC_OUT_1);

            while (true)
            {
                // PLC에 모델 교체 가능 여부 확인
                if (ModelChangeConfirmPLC())
                {
                    sw.Stop();
                    return true;
                }

                Thread.Sleep(100);

                if (sw.Elapsed.TotalSeconds > mModelChangeTimout)
                {
                    sw.Stop();

                    throw new TimeoutException();
                }
            }
        }

        private void PerformModelChange(string modelFileFullPath)
        {
            Stopwatch swModelChange = new Stopwatch();
            swModelChange.Start();

            mLog.WriteLog("ModelChange", "Starting model change process...");

            while (true)
            {
                // Trigger 상태값을 확인하여 기존 작업이 완료되었는지 확인
                if (!isTriggerAction_CA && !isTriggerAction_AN)
                {
                    // Camera Ready Off
                    CamReadyOff();

                    string oldWorkspaceName = Path.GetFileNameWithoutExtension(mWorkSpacePath);
                    // 신규 워크스페이스와 이전 워크스페이스를 비교해야한다.
                    mLog.WriteLog("ModelChange", $"Old Model : {mWorkSpacePath}");
                    mLog.WriteLog("ModelChange", $"New Model : {modelFileFullPath}");

                    // 중복 체크
                    if (mWorkSpacePath == modelFileFullPath)
                    {
                        throw new System.Exception("New Model is same Model, Unable to change.");
                    }

                    try
                    {
                        // 신규 모델을 메모리에 추가
                        if (mViDi.AddLoadModel(modelFileFullPath))
                        {
                            mLog.WriteLog("ModelChange", $"New model loaded successfully.");
                            mWorkSpacePath = modelFileFullPath;
                            ini.WriteIniValue("Maintenance", "WorkspaceSavePath", mWorkSpacePath);
                            try
                            {
                                // 기존 모델을 메모리에서 삭제
                                mLog.WriteLog("ModelChange", $"Old model unloaded from workspace");
                                mViDi.UnloadModel(oldWorkspaceName);

                                this.Invoke(new Action(delegate ()
                                {
                                    lbl_WorkspacePath.Text = mWorkSpacePath.Trim();
                                }));
                                mLog.WriteLog("ModelChange", $"model change has been completed.");
                            }
                            catch (System.Exception ex)
                            {
                                mLog.WriteLog("ModelChange", $"Old Workspace Remove process error : {ex.Message}");
                            }
                        }
                        else
                        {
                            throw new System.Exception("Workspace Load error");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        mLog.WriteLog("ModelChange", $"Workspace Change process error : {ex.Message}");
                    }
                    finally
                    {
                        // Camera Ready On
                        CamReadyOn();

                        // 모델 교체 완료 신호 전송
                        SendCompleteToPlc();
                    }

                    break;

                }

                Thread.Sleep(100);

                if (swModelChange.Elapsed.TotalSeconds > mModelChangeTimout || swModelChange.Elapsed.TotalSeconds > 600)
                {
                    swModelChange.Stop();
                    mLog.WriteLog("ModelChange", "Model change process timed out.");
                    throw new TimeoutException("Model change process timed out");
                }
            }

            swModelChange.Stop();
        }


        private void RestorePreviousModel(string oldModelPath)
        {
            // 원복작업은 모델을 되돌리는 작업인데, 
            // 기존과는 다르게 딥러닝을 완전 초기화 시키는 방법으로 한다.
            mLog.WriteLog("ModelChange", "Deep Learning Workspace Initialize...");

            // mViDi 초기화
            if (mViDi != null)
            {
                mViDi.Dispose();
                mViDi = null;
            }

            // 다시 모델을 로딩한다.
            try
            {
                mLog.WriteLog("ModelChange", "Restoring previous model...");
                mViDi = new cls_DeepLearning();
                mViDi.Init(oldModelPath);
                mLog.WriteLog("ModelChange", "Previous model restored.");
            }
            catch (System.Exception)
            {
                throw new System.Exception("Restore Previous Model Init Error");
            }
            // 이전 모델을 메모리에 다시 로드           
        }

        private bool ModelChangeConfirmPLC()
        {
            try
            {

                // PLC에 모델 교체 가능 여부 확인
                if (mPLC.ReadInt16(mPlc_Read_Change_Model_PLC) == 1)
                {
                    mLog.WriteLog("ModelChange", "PLC approved model change.");
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("ModelChange", $"Model Change - PLC approval Error : {ex.Message}");
                return false;
            }

        }

        private void CamReadyOff()
        {
            // Camera Ready Off 로직
            mLog.WriteLog("ModelChange", "Camera Ready Off");
            AdvanDevice.OutputStart(mIO_OUT_AN_READY, false, 0, 0, 0);
            AdvanDevice.OutputStart(mIO_OUT_CA_READY, false, 0, 0, 0);
        }

        private void CamReadyOn()
        {
            // Camera Ready On 로직
            mLog.WriteLog("ModelChange", "Camera Ready On");
            AdvanDevice.OutputStart(mIO_OUT_AN_READY, true, 0, 0, 0);
            AdvanDevice.OutputStart(mIO_OUT_CA_READY, true, 0, 0, 0);
        }

        private void SendCompleteToPlc()
        {
            try
            {
                // 모델 교체 완료 신호 PLC로 전송
                mPLC.WriteShort(mPlc_Write_Change_Model_PC, PLC_OUT_0);
                mLog.WriteLog("ModelChange", "Model change complete signal sent to PLC.");

            }
            catch (System.Exception)
            {
                //
            }
        }

        string temp_cell_CA_nm = string.Empty;
        string temp_cell_AN_nm = string.Empty;

        private void btnTestInf_Click(object sender, EventArgs e)
        {
            string folderPath = @"D:\IDMAX_RUN_SMP";

            //string folderPath = @"D:\01.WorkSpace_Vision\CodeSet2\SKOH_jslee_240730\SKON_TabWelldingInspection\bin\Debug\extend\ORI_IMAGE";

            // 지정된 폴더의 모든 파일을 가져오기
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                // json 저장용 테스트 카메라가 없기 때문에 임시 저장
                Bitmap bimage = new Bitmap(file);
                //temp_cell_CA_nm = new FileInfo(file).Name.Substring(4, 18);
                PolarityGrabEvent(bimage, CAM_CATHODE);
                PolarityGrabEvent(bimage, CAM_ANODE);
                Thread.Sleep(1000);
            }
        }

        private void btnModelChange_Click(object sender, EventArgs e)
        {
            string msg = $"Old Model: {mWorkSpacePath}\nNew Model: {lbl_WorkspacePath.Text} \n\nWould you like to proceed with the model change process?";
            DialogResult result = MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            // 251219 result 값에 따라 처리
            if (result == DialogResult.Cancel)
                return;

            mLog.WriteLog("TEST", $"Model change requested by user. Old Model: {mWorkSpacePath}, New Model: {lbl_WorkspacePath.Text}");

            ModelChangeProcess(Path.GetDirectoryName(lbl_WorkspacePath.Text), Path.GetFileName(lbl_WorkspacePath.Text));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_detect_Click(object sender, EventArgs e)
        {
            // DetectConfig 폼을 인스턴스화하고 팝업으로 띄움
            detectConfig detectConfigForm = new detectConfig(this);
            detectConfigForm.ShowDialog(); // 모달로 팝업을 띄움
        }

        private void pnlHidden_DoubleClick(object sender, EventArgs e)
        {
            button17.Visible = !button17.Visible;
        }

        private void timer_drive_Tick(object sender, EventArgs e)
        {
            try
            {
                // C Drive 체크
                var cDrive = CheckDrive("C");
                if (cDrive.Item1 == 0 || cDrive.Item2 == 0)
                {
                    progDrive_C.Value = 0;
                    lbl_driveC_sts.Text = "check Error";
                }
                else
                {
                    progDrive_C.Value = Convert.ToInt32((((cDrive.Item1 - cDrive.Item2) / cDrive.Item1) * 100));
                    lbl_driveC_sts.Text = $"{cDrive.Item2} GB Free of {cDrive.Item1} GB";
                }

                // D Drive 체크
                var dDrive = CheckDrive("D");
                if (dDrive.Item1 == 0 || dDrive.Item2 == 0)
                {
                    progDrive_D.Value = 0;
                    lbl_driveD_sts.Text = "check Error";
                }
                else
                {
                    progDrive_D.Value = Convert.ToInt32((((dDrive.Item1 - dDrive.Item2) / dDrive.Item1) * 100));
                    lbl_driveD_sts.Text = $"{dDrive.Item2} GB Free of {dDrive.Item1} GB";
                }
            }
            catch { }
        }

        private void ic_err_Detect_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if ((ModifierKeys & Keys.Control) == Keys.Control)
                {
                    if (!string.IsNullOrEmpty(mPlc_Write_Cathode_Image_Detect_Err))
                    {
                        string target = (sender as IconLamp).Tag.ToString();

                        switch (target)
                        {
                            case "bright":
                                _isRisingErrBGT_CA.Value = !_isRisingErrBGT_CA.Value;
                                break;
                            case "sharp":
                                _isRisingErrSHP_CA.Value = !_isRisingErrSHP_CA.Value;
                                break;
                            case "location":
                                _isRisingErrLOC_CA.Value = !_isRisingErrLOC_CA.Value;
                                break;
                            case "ng":
                                _isRisingErrNG_CA.Value = !_isRisingErrNG_CA.Value;
                                break;
                            case "ng_intv":
                                _isRisingErrNGINTV_CA.Value = !_isRisingErrNGINTV_CA.Value;
                                break;
                        }
                    }
                    else
                        mLog.WriteLog("SYS", $"Address is null : Cathode mPlc_Write_Image_Detect_Err");
                }
                else if ((ModifierKeys & Keys.Alt) == Keys.Alt)
                {
                    if (!string.IsNullOrEmpty(mPlc_Write_Anode_Image_Detect_Err))
                    {
                        string target = (sender as IconLamp).Tag.ToString();

                        switch (target)
                        {
                            case "bright":
                                _isRisingErrBGT_AN.Value = !_isRisingErrBGT_AN.Value;
                                break;
                            case "sharp":
                                _isRisingErrSHP_AN.Value = !_isRisingErrSHP_AN.Value;
                                break;
                            case "location":
                                _isRisingErrLOC_AN.Value = !_isRisingErrLOC_AN.Value;
                                break;
                            case "ng":
                                _isRisingErrNG_AN.Value = !_isRisingErrNG_AN.Value;
                                break;
                            case "ng_intv":
                                _isRisingErrNGINTV_AN.Value = !_isRisingErrNGINTV_AN.Value;
                                break;
                        }
                    }
                    else
                        mLog.WriteLog("SYS", $"Address is null : Anode mPlc_Write_Image_Detect_Err");
                }
                else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    string target = (sender as IconLamp).Tag.ToString();
                    if (target.Equals("ng_intv"))
                    {
                        if (!string.IsNullOrEmpty(mPlc_Write_Image_Save_Err))
                            _imageSave_ErrorCount++;
                        else
                            mLog.WriteLog("SYS", $"Address is null : mPlc_Write_Image_Save_Err");
                    }
                }
            }
            catch (System.Exception ex) { mLog.WriteLog("SYS", $"ic_err_Detect_DoubleClick Error rising Exception : {ex.Message}"); }
        }

        private void btn_ModelSetup_Click(object sender, EventArgs e)
        {
            frm_Model l_ofrmModelSetting = new frm_Model();
            l_ofrmModelSetting.ShowDialog();

            if (cls_GlobalValue.Model.Model_Load(cls_GlobalValue.LastModelPath))
            {
                cls_GlobalValue.ChangeToolblock_Anode = true;
                cls_GlobalValue.ChangeToolblock_Cathode = true;
                lbl_Model.Text = cls_GlobalValue.LastModelFileName;
                //lp_JobFileLoad.On();
            }
            else
            {
                lbl_Model.Text = "";
                //lp_JobFileLoad.Off();
            }
        }

        private void btn_InspectionSetup_Click(object sender, EventArgs e)
        {
            if (cls_GlobalValue.Model == null)
            {
                MsgBox.Show("Model is Empty", "Inspection Load", MsgBox.Buttons.OK, MsgBox.Icon.Warning);
                return;
            }
            else if (mbCathode_Live)
            {
                MsgBox.Show("Cathode Camera is Live", "Inspection Load", MsgBox.Buttons.OK, MsgBox.Icon.Warning);
                return;
            }
            else if (mbAnode_Live)
            {
                MsgBox.Show("Cathode Camera is Live", "Inspection Load", MsgBox.Buttons.OK, MsgBox.Icon.Warning);
                return;
            }
            else
            {
                frm_Inspection frm = new frm_Inspection();
                frm.ShowDialog();
            }
        }

        private void btn_VproSavePath_Click(object sender, EventArgs e)
        {
            cls_File.OpenFolder(lbl_VproSavePath);
        }

    }
}
