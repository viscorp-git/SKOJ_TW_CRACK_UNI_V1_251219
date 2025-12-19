using Class;
using HslCommunication;
using IDMAX_FrameWork;
using MelsecProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Class.cls_Util;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_PLC
    {
        public static void processHeartBeat(frm_Main main, ref MCProtocol mPLC, ref int heartBeatCount, ref bool heartBeatFlag, ref int notConnectCount)
        {
            if (!string.IsNullOrEmpty(main.mPlc_Write_HeartBeat))
            {
                heartBeatCount++;

                if (heartBeatCount % 10 == 0)
                {
                    heartBeatFlag = !heartBeatFlag;

                    if (mPLC.WriteShort(main.mPlc_Write_HeartBeat, (short)(heartBeatFlag ? 0 : 1)) == -9999)
                        notConnectCount++;
                    else
                        notConnectCount = 0;

                    heartBeatCount = 0; // heartbeat 초기화
                }
            }
        }

        public static void HandleImagesaveErr(frm_Main main, ref MCProtocol mPLC, ref cls_Log mLog)
        {
            if (!string.IsNullOrEmpty(main.mPlc_Write_Image_Save_Err))   // Save Err Write가 있을때만 한다.
            {
                if (!string.IsNullOrEmpty(main.mPlc_Read_Image_Save_Err_Reset))
                {
                    // 이미지 저장 오류 발생시 PLC에 오류 전달
                    if (main._imageSave_ErrorCount > 0)
                    {
                        mPLC.WriteShort(main.mPlc_Write_Image_Save_Err, (short)(main.mImageSaveErrFormat.Equals("01") ? 1 : 2));
                        mLog.WriteLog("PLC", $"Image Save Error : {main._imageSave_ErrorCount}");
                    }

                    // PLC에서 이미지 저장 오류 리셋시 초기화
                    if (mPLC.ReadInt16(main.mPlc_Read_Image_Save_Err_Reset) == 1)
                    {
                        mPLC.WriteShort(main.mPlc_Write_Image_Save_Err, (short)(main.mImageSaveErrFormat.Equals("01") ? 0 : 1));
                        main._imageSave_ErrorCount = 0;
                        mLog.WriteLog("PLC", $"Image Save Error Reset : {main._imageSave_ErrorCount}");
                    }
                }
                else  // 이미지 저장 오류 리셋을 PLC가 보내주지 않을 경우
                {
                    // 이미지 저장 오류 발생시 PLC에 오류 전달
                    if (main._imageSave_ErrorCount > 0)
                    {
                        mPLC.WriteShort(main.mPlc_Write_Image_Save_Err, (short)(main.mImageSaveErrFormat.Equals("01") ? 1 : 2));
                        mLog.WriteLog("PLC", $"Image Save Error : {main._imageSave_ErrorCount}");

                        Task.Delay(1000).Wait();

                        main._imageSave_ErrorCount = 0;
                        mPLC.WriteShort(main.mPlc_Write_Image_Save_Err, (short)(main.mImageSaveErrFormat.Equals("01") ? 0 : 1));
                    }
                }
            }
        }

        public static void HandleImageDetectError(ref MCProtocol mPLC, ref cls_Log mLog, string plcWriteError, string plcReadErrorReset, ref CustomBool isRisingErrBGT, ref CustomBool isRisingErrSHP, ref CustomBool isRisingErrLOC, ref CustomBool isRisingErrNG, ref CustomBool isRisingErrNGINTV, string errorType)
        {
            // WriteError 번지가 없다면 수행할 필요 없음.
            if (string.IsNullOrEmpty(plcWriteError))
                return;

            // PLC에서 이미지 이상감지 리셋시 초기화
            if (!string.IsNullOrEmpty(plcReadErrorReset))
            {
                // 이미지 이상감지 발생시 PLC에 오류 전달
                if (isRisingErrBGT.Value || isRisingErrSHP.Value || isRisingErrLOC.Value || isRisingErrNG.Value || isRisingErrNGINTV.Value)
                {
                    int plcValue = isRisingErrBGT.Value ? 1 : isRisingErrSHP.Value ? 2 : isRisingErrLOC.Value ? 3 : isRisingErrNG.Value ? 4 : isRisingErrNGINTV.Value ? 5 : 0;

                    mPLC.WriteShort(plcWriteError, (short)plcValue);
                    mLog.WriteLog("PLC", $"{errorType} Image Detect Error : " +
                        $"{(isRisingErrBGT.Value ? "Bright" : isRisingErrSHP.Value ? "Sharp" : isRisingErrLOC.Value ? "Location" : isRisingErrNG.Value ? "NG" : isRisingErrNGINTV.Value ? "Interval NG" : "None")} Error");
                }

                if (mPLC.ReadInt16(plcReadErrorReset) == 1)
                {
                    mPLC.WriteShort(plcWriteError, 0);

                    // Reset errors
                    isRisingErrBGT.Value = isRisingErrSHP.Value = isRisingErrLOC.Value = isRisingErrNG.Value = isRisingErrNGINTV.Value = false;

                    mLog.WriteLog("PLC", $"{errorType} Image Detect Error Reset");
                }
            }
            else  // 이미지 이상감지 리셋을 PLC가 보내주지 않을 경우
            {
                // 이미지 이상감지 발생시 PLC에 오류 전달
                if (isRisingErrBGT.Value || isRisingErrSHP.Value || isRisingErrLOC.Value || isRisingErrNG.Value || isRisingErrNGINTV.Value)
                {
                    int plcValue = isRisingErrBGT.Value ? 1 : isRisingErrSHP.Value ? 2 : isRisingErrLOC.Value ? 3 : isRisingErrNG.Value ? 4 : isRisingErrNGINTV.Value ? 5 : 0;

                    mPLC.WriteShort(plcWriteError, (short)plcValue);
                    mLog.WriteLog("PLC", $"{errorType} Image Detect Error : " +
                        $"{(isRisingErrBGT.Value ? "Bright" : isRisingErrSHP.Value ? "Sharp" : isRisingErrLOC.Value ? "Location" : isRisingErrNG.Value ? "NG" : isRisingErrNGINTV.Value ? "Interval NG" : "None")} Error");

                    Task.Delay(1000).Wait();

                    // Reset errors
                    isRisingErrBGT.Value = isRisingErrSHP.Value = isRisingErrLOC.Value = isRisingErrNG.Value = isRisingErrNGINTV.Value = false;
                    mPLC.WriteShort(plcWriteError, 0);
                    mLog.WriteLog("PLC", $"{errorType} Image Detect Error Reset [Self]");
                }
            }
        }

        public static void HandlePLCData(frm_Main main, ref MCProtocol mPLC, ref cls_Log mLog, string _curTabpage)
        {
            try
            {
                // PLC 정보 읽어서 창에 표시하는 부분
                if (main.btn_PLC_Connect.Text == "Connect")
                {
                    string curTabpage = _curTabpage;

                    if (curTabpage == "tabPLC")
                    {
                        if (!string.IsNullOrEmpty(main.mPlc_Read_CELLID_CA)) PLC_OpResult_String(main, ref mPLC, ref mLog, main.mPlc_Read_CELLID_CA, main.tbx_Value_Cell_id_CA);
                        if (!string.IsNullOrEmpty(main.mPlc_Read_CELLID_AN)) PLC_OpResult_String(main, ref mPLC, ref mLog, main.mPlc_Read_CELLID_AN, main.tbx_Value_Cell_id_AN);

                        if (!string.IsNullOrEmpty(main.mPlc_Read_Carrier_ID)) PLC_OpResult_String(main, ref mPLC, ref mLog, main.mPlc_Read_Carrier_ID, main.tbx_Value_Carrier_id);
                        if (!string.IsNullOrEmpty(main.mPlc_Read_Stack_ID)) PLC_OpResult_String(main, ref mPLC, ref mLog, main.mPlc_Read_Stack_ID, main.tbx_Value_Stack_id);

                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Read_Model_No, main.tbx_Value_Model_No);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Read_Change_Model_PLC, main.tbx_Value_Change_Model_PLC);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Read_VisionIO_Reset, main.tbx_Value_Vision_IO_Reset);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Read_Image_Save_Err_Reset, main.tbx_Value_Image_Save_Error_Reset);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Read_Image_Detect_Err_Reset, main.tbx_Value_Image_Detect_Error_Reset);

                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Cathode_Result, main.tbx_Value_Cathode_Result);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Anode_Result, main.tbx_Value_Anode_Result);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Cathode_Result_Code, main.tbx_Value_Cathode_Result_Code);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Anode_Result_Code, main.tbx_Value_Anode_Result_Code);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Change_Model_PC, main.tbx_Value_Change_Model_PC);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Complete_VisionIO_Reset, main.tbx_Value_Complete_Vision_IO_Reset);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Image_Save_Err, main.tbx_Value_Image_Save_Error);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Cathode_Image_Detect_Err, main.tbx_Cathode_Value_Image_Detect_Error);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Anode_Image_Detect_Err, main.tbx_Anode_Value_Image_Detect_Error);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Cathode_Vpro_Result, main.tbx_Value_Cathode_VPRO_Result);
                        ReadIntPlc(main, ref mPLC, ref mLog, main.mPlc_Write_Anode_Vpro_Result, main.tbx_Value_Anode_VPRO_Result);

                        if (!string.IsNullOrEmpty(main.mPlc_Write_Cathode_Cell_Id)) PLC_OpResult_String(main, ref mPLC, ref mLog, main.mPlc_Write_Cathode_Cell_Id, main.tbx_Value_Cathode_Write_Cell_id);
                        if (!string.IsNullOrEmpty(main.mPlc_Write_Anode_Cell_Id)) PLC_OpResult_String(main, ref mPLC, ref mLog, main.mPlc_Write_Anode_Cell_Id, main.tbx_Value_Anode_Write_Cell_id);
                    
                    }
                }
            }
            catch (System.Exception ex)
            {
                mLog.WriteLog("System", $"error : {ex.Message}");
            }
        }

        public static void HandleErrPLCData(frm_Main main, ref cls_Log mLog, ref int notConnectCount)
        {
            notConnectCount++;

            mLog.WriteLog("PLC", $"PLC HeartBeat Not Read As Connection Error : Count {notConnectCount}");

            if (!string.IsNullOrEmpty(main.mPlc_Read_CELLID_CA)) ErrorPlc(main, main.tbx_Value_Cell_id_CA);
            if (!string.IsNullOrEmpty(main.mPlc_Read_CELLID_AN)) ErrorPlc(main, main.tbx_Value_Cell_id_AN);
            if (!string.IsNullOrEmpty(main.mPlc_Read_Carrier_ID)) ErrorPlc(main, main.tbx_Value_Carrier_id);
            if (!string.IsNullOrEmpty(main.mPlc_Read_Stack_ID)) ErrorPlc(main, main.tbx_Value_Stack_id);

            if (!string.IsNullOrEmpty(main.mPlc_Read_Model_No)) ErrorPlc(main, main.tbx_Value_Model_No);
            if (!string.IsNullOrEmpty(main.mPlc_Read_Change_Model_PLC)) ErrorPlc(main, main.tbx_Value_Change_Model_PLC);
            if (!string.IsNullOrEmpty(main.mPlc_Read_VisionIO_Reset)) ErrorPlc(main, main.tbx_Value_Vision_IO_Reset);
            if (!string.IsNullOrEmpty(main.mPlc_Read_Image_Save_Err_Reset)) ErrorPlc(main, main.tbx_Value_Image_Save_Error_Reset);
            if (!string.IsNullOrEmpty(main.mPlc_Read_Image_Detect_Err_Reset)) ErrorPlc(main, main.tbx_Value_Image_Detect_Error_Reset);

            if (!string.IsNullOrEmpty(main.mPlc_Write_Cathode_Result)) ErrorPlc(main, main.tbx_Value_Cathode_Result);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Anode_Result)) ErrorPlc(main, main.tbx_Value_Anode_Result);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Cathode_Result_Code)) ErrorPlc(main, main.tbx_Value_Cathode_Result_Code);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Anode_Result_Code)) ErrorPlc(main, main.tbx_Value_Anode_Result_Code);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Change_Model_PC)) ErrorPlc(main, main.tbx_Value_Change_Model_PC);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Complete_VisionIO_Reset)) ErrorPlc(main, main.tbx_Value_Complete_Vision_IO_Reset);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Image_Save_Err)) ErrorPlc(main, main.tbx_Value_Image_Save_Error);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Cathode_Image_Detect_Err)) ErrorPlc(main, main.tbx_Cathode_Value_Image_Detect_Error);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Anode_Image_Detect_Err)) ErrorPlc(main, main.tbx_Anode_Value_Image_Detect_Error);

            if (!string.IsNullOrEmpty(main.mPlc_Write_Cathode_Cell_Id)) ErrorPlc(main, main.tbx_Value_Cathode_Write_Cell_id);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Anode_Cell_Id)) ErrorPlc(main, main.tbx_Value_Anode_Write_Cell_id);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Cathode_Vpro_Result)) ErrorPlc(main, main.tbx_Value_Cathode_VPRO_Result);
            if (!string.IsNullOrEmpty(main.mPlc_Write_Anode_Vpro_Result)) ErrorPlc(main, main.tbx_Value_Anode_VPRO_Result);

        }


        private static void PLC_OpResult_String(frm_Main main, ref MCProtocol mPLC, ref cls_Log mLog, string mPlcString, TextBox tbxName)
        {
            if (string.IsNullOrEmpty(mPlcString))
                return;

            OperateResult<String> opResult = (OperateResult<string>)mPLC.ReadString_Oper(mPlcString, 10);
            if (opResult.IsSuccess)
            {
                main.Invoke(new Action(delegate () {
                    tbxName.Text = opResult.Content.ToString().Replace("\0", "");
                }));

            }
        }

        private static void ReadIntPlc(frm_Main main, ref MCProtocol mPLC, ref cls_Log mLog, string mPlc_Address, TextBox tbx_Value)
        {
            if (string.IsNullOrEmpty(mPlc_Address))
                return;

            string value = mPLC.ReadInt16(mPlc_Address).ToString();
            if (!string.IsNullOrEmpty(mPlc_Address))
            {
                main.Invoke(new Action(delegate () {
                    tbx_Value.Text = value;
                }));
            }
        }

        private static void ErrorPlc(frm_Main main, TextBox tbx_Value)
        {
            main.Invoke(new Action(delegate () {
                tbx_Value.Text = "-9990";
            }));
        }
    }
}
