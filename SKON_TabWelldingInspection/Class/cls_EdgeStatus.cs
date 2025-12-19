using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_EdgeStatus
    {
        public int LINE_NUM { get; set; }    // 라인번호
        public string PC_CL_CD { get; set;}   // PC 상부 하부 구분 코드
        public int UPD_CYCL { get; set; }  // 갱신 주기
        public string PLC_LINK_CD { get; set; }  // PLC 연결 상태
        public string CAMERA_CA_LINK_CD { get; set; }  //  카메라 CA 연결 상태
        public string CAMERA_AN_LINK_CD { get; set; }  // 카메라 AN 연결 상태

        public string RGSTR_ID { get; set; }    // 최초 등록자
        public string RGST_DTM { get; set; }    // 최초 등록 일자
        public string LAST_UPDR_ID { get; set; }   // 최종 수정자
        public string LAST_UPD_DTM { get; set; }  // 최종 수정 일자

        public cls_EdgeStatus()
        {
            LINE_NUM = 0;  // 라인번호
            PC_CL_CD = "";  // PC 상부 하부 구분 코드
            UPD_CYCL = 5;  // 갱신 주기
            PLC_LINK_CD = "LINK_ST_CD_OFF";  // PLC 연결 상태
            CAMERA_CA_LINK_CD = "LINK_ST_CD_OFF";    //  카메라 CA 연결 상태
            CAMERA_AN_LINK_CD = "LINK_ST_CD_OFF";    // 카메라 AN 연결 상태
            RGSTR_ID = "IDMAX";    // 최초 등록자
            RGST_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");   // 최초 등록 일자
            LAST_UPDR_ID = "IDMAX";    // 최종 수정자
            LAST_UPD_DTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");  // 최종 수정 일자
        }

    }
}

