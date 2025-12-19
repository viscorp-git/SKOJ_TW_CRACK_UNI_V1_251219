using System;
using System.Drawing;

namespace SKON_TabWelldingInspection.Class
{
    public class cls_RunResultJson
    {
        public string IMG_CLCT_DTM { get; set; }   // 이미지 수집 일시(2023-07-13 00:00:00.000)  / NOT NULL  / PRI
        public string CORNER_ID { get; set; }   // CORNER_ID  / NOT NULL  / PRI
        public string CORNER_NAME { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public string CORNER_JUDGE { get; set; }   // CORNER_JUDGE  / NOT NULL  / PRI
        public string CORNER_JUDGE_STR { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public string CORNER_JUDGE_CD { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public short CORNER_JUDGE_PLC_CD { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public string VPRO_JUDGE_PLC_STR { get; set; }    // 프로그램내 관리용으로 사용 JSON에 저장 안함 (VPRO 판정 결과)
        public string IMG_TAKEN_DTM { get; set; }   // 이미지 촬영 일시  / NOT NULL  / PRI
        public DateTime CELL_ID_DTM { get; set; }
        public string CELL_ID { get; set; }   // CELL ID  / NOT NULL  / PRI
        public string POSITION { get; set; }
        public string IMG_FILE_NM { get; set; }   // 이미지 파일 명  / NOT NULL
        public string IMG_FILE_DIR { get; set; }
        public string ORGL_IMG_VIS_FILE_NM { get; set; }   // 원본 IMAGE_파일명(VISS)  / NOT NULL
        public string ORGL_IMG_VIS_FILE_DIR { get; set; }
        public string IMG_ST_CD { get; set; }   // 이미지 상태 코드  / NOT NULL                                     
        public string VPRO_LOC_FILE_NM { get; set; }   // VPRO 파일명(LOCAL)  / NOT NULL
        public string VPRO_LOC_FILE_DIR { get; set; }
        public string BLUTOL_LOC_FILE_NM { get; set; }   // BLUETOOL 파일명(LOCAL)  / NOT NULL
        public string BLUTOL_LOC_FILE_DIR { get; set; }
        public string BLUTOL_VIS_FILE_NM { get; set; }   // BLUETOOL 파일명(VISS)  / NOT NULL
        public string BLUTOL_VIS_FILE_DIR { get; set; }
        public string AI_INSP_DTM { get; set; }   // AI 판정일시
        public string HTMP_LOC_FILE_NM { get; set; }   // HEATMAP 파일명(LOCAL)
        public string HTMP_LOC_FILE_DIR { get; set; }
        public string HTMP_VIS_FILE_NM { get; set; }   // HEATMAP 파일명(VISS)
        public string HTMP_VIS_FILE_DIR { get; set; }
        public double BLUTOL_STRD1_X { get; set; }   // BLUETOOL 기준1_X
        public double BLUTOL_STRD1_Y { get; set; }   // BLUETOOL 기준1_Y
        public double BLUTOL_STRD2_X { get; set; }   // BLUETOOL 기준2_X
        public double BLUTOL_STRD2_Y { get; set; }   // BLUETOOL 기준2_Y
        public double BLUTOL_ROTAT_ANGL { get; set; }   // BLUETOOL 회전각도
        public double ROI_X { get; set; }   // ROI_X  / NOT NULL
        public double ROI_Y { get; set; }   // ROI_Y  / NOT NULL
        public double ROI_W { get; set; }   // ROI_WIDTH  / NOT NULL
        public double ROI_HT { get; set; }   // ROI_HEIGHT  / NOT NULL
        public string ROI1_INFRCE1_RST { get; set; }   // ROI1_추론1_결과
        public double ROI1_INFRCE1_SCOR { get; set; }   // ROI1_추론1_Score
        public string ROI1_INFRCE2_RST { get; set; }   // ROI1_추론2_결과
        public string ROI1_INFRCE2_RST_STR { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public string ROI1_INFRCE2_RST_CD { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public double ROI1_INFRCE2_OK_SCOR { get; set; }   // ROI1_추론2_OK_Score
        public double ROI1_INFRCE2_NG1_SCOR { get; set; }   // ROI1_추론2_NG1_Score
        public double ROI1_INFRCE2_NG2_SCOR { get; set; }   // ROI1_추론2_NG2_Score
        public double ROI1_INFRCE2_NG3_SCOR { get; set; }   // ROI1_추론2_NG3_Score
        public double ROI1_INFRCE2_NG4_SCOR { get; set; }   // ROI1_추론2_NG4_Score

        public string ROI2_INFRCE1_RST { get; set; }   // ROI2_추론1_결과
        public double ROI2_INFRCE1_SCOR { get; set; }   // ROI2_추론1_Score
        public string ROI2_INFRCE2_RST { get; set; }   // ROI2_추론2_결과
        public string ROI2_INFRCE2_RST_STR { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public string ROI2_INFRCE2_RST_CD { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public double ROI2_INFRCE2_OK_SCOR { get; set; }   // ROI2_추론2_OK_Score
        public double ROI2_INFRCE2_NG1_SCOR { get; set; }   // ROI2_추론2_NG1_Score
        public double ROI2_INFRCE2_NG2_SCOR { get; set; }   // ROI2_추론2_NG2_Score
        public double ROI2_INFRCE2_NG3_SCOR { get; set; }   // ROI2_추론2_NG3_Score
        public double ROI2_INFRCE2_NG4_SCOR { get; set; }   // ROI2_추론2_NG4_Score

        public string ROI3_INFRCE1_RST { get; set; }   // ROI3_추론1_결과
        public double ROI3_INFRCE1_SCOR { get; set; }   // ROI3_추론1_Score
        public string ROI3_INFRCE2_RST { get; set; }   // ROI3_추론2_결과
        public string ROI3_INFRCE2_RST_STR { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public string ROI3_INFRCE2_RST_CD { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public double ROI3_INFRCE2_OK_SCOR { get; set; }   // ROI3_추론2_OK_Score
        public double ROI3_INFRCE2_NG1_SCOR { get; set; }   // ROI3_추론2_NG1_Score
        public double ROI3_INFRCE2_NG2_SCOR { get; set; }   // ROI3_추론2_NG2_Score
        public double ROI3_INFRCE2_NG3_SCOR { get; set; }   // ROI3_추론2_NG3_Score
        public double ROI3_INFRCE2_NG4_SCOR { get; set; }   // ROI3_추론2_NG4_Score

        public string AI_INSP_RSLT_CD { get; set; }   // 프로그램내 관리용으로 사용 JSON에 저장 안함
        public double AI_INSP_OK_SCOR { get; set; }   // ROI3_추론2_OK_Score
        public double AI_INSP_NG1_SCOR { get; set; }   // ROI3_추론2_NG1_Score
        public double AI_INSP_NG2_SCOR { get; set; }   // ROI3_추론2_NG2_Score
        public double AI_INSP_NG3_SCOR { get; set; }   // ROI3_추론2_NG3_Score
        public double AI_INSP_NG4_SCOR { get; set; }   // ROI3_추론2_NG4_Score

        public string VPDL_MDL_FILE_NM { get; set; }   // VPDL_MODEL_FILENAME
        public double INFRCE2_OK_THRHLD { get; set; }   // 통합 ROI 임계치  / NOT NULL
        public double CA_ROI_1_THRHLD { get; set; }   // CA ROI1 임계치  / NOT NULL
        public double CA_ROI_2_THRHLD { get; set; }   // CA ROI2 임계치  / NOT NULL
        public double CA_ROI_3_THRHLD { get; set; }   // CA ROI3 임계치  / NOT NULL
        public double AN_ROI_1_THRHLD { get; set; }   // AN ROI1 임계치  / NOT NULL
        public double AN_ROI_2_THRHLD { get; set; }   // AN ROI2 임계치  / NOT NULL
        public double AN_ROI_3_THRHLD { get; set; }   // AN ROI3 임계치  / NOT NULL

        public string RGSTR_ID { get; set; }   // 최초 등록 ID
        public string RGST_DTM { get; set; }   // 최초 등록 일시
        public string LAST_UPDR_ID { get; set; }   // 최종 수정 ID
        public string LAST_UPD_DTM { get; set; }   // 최종 수정 일시
        public Bitmap ROI1_HEATMAP_IMAGE { get; set; }   // ROI1 HEATMAP IMAGE 저장, JSON저장 안함.
        public Bitmap ROI2_HEATMAP_IMAGE { get; set; }   // ROI2 HEATMAP IMAGE 저장, JSON저장 안함.
        public Bitmap ROI3_HEATMAP_IMAGE { get; set; }   // ROI3 HEATMAP IMAGE 저장, JSON저장 안함.
        public Bitmap COMB_HEATMAP_IMAGE { get; set; } // 히트맵 이미지 결합 저장, json 저장 안함.
        public double CA_AVG_BRIGHTNESS { get; set; }  // CA 이미지 이상감지 Brightness 평균값, json 저장 안함.
        public double CA_SHARPNESS { get; set; } // CA 이미지 이상감지 Sharpness, 저장 안함.
        public double AN_AVG_BRIGHTNESS { get; set; }  // AN 이미지 이상감지 Brightness 평균값, json 저장 안함.
        public double AN_SHARPNESS { get; set; } // AN 이미지 이상감지 Sharpness, 저장 안함.

        public cls_RunResultJson()
        {
            DateTime dtNow = DateTime.Now;

            IMG_CLCT_DTM = dtNow.ToString("yyyy-MM-dd HH:mm:ss.fff");   // 이미지 수집 일시(2023-07-13 00:00:00.000)  / NOT NULL  / PRI
            CORNER_ID = "";   // CORNER_ID  / NOT NULL  / PRI
            CORNER_NAME = "";   // CORNER_ID  / NOT NULL  / PRI
            CORNER_JUDGE = "";   // CORNER_JUDGE  / NOT NULL  / PRI
            CORNER_JUDGE_STR = "";
            CORNER_JUDGE_CD = "";
            CORNER_JUDGE_PLC_CD = 0;
            VPRO_JUDGE_PLC_STR = "";
            IMG_TAKEN_DTM = dtNow.ToString("yyyy-MM-dd HH:mm:ss.fff");  // 이미지 촬영 일시  / NOT NULL  / PRI
            CELL_ID = "";   // CELL ID  / NOT NULL  / PRI
            CELL_ID_DTM = dtNow;
            POSITION = "";
            IMG_FILE_NM = "";   // 이미지 파일 명  / NOT NULL
            IMG_FILE_DIR = "";
            ORGL_IMG_VIS_FILE_NM = "";   // 원본 IMAGE_파일명(VISS)  / NOT NULL
            ORGL_IMG_VIS_FILE_DIR = "";   // 원본 IMAGE_파일명(VISS)  / NOT NULL
            IMG_ST_CD = "";   // 이미지 상태 코드  / NOT NULL
            VPRO_LOC_FILE_NM = ""; //VPRO
            VPRO_LOC_FILE_DIR = "";//VPRO
            BLUTOL_LOC_FILE_NM = "";   // BLUETOOL 파일명(LOCAL)  / NOT NULL
            BLUTOL_LOC_FILE_DIR = "";
            BLUTOL_VIS_FILE_NM = "";   // BLUETOOL 파일명(VISS)  / NOT NULL
            BLUTOL_VIS_FILE_DIR = "";
            AI_INSP_DTM = dtNow.ToString("yyyy-MM-dd HH:mm:ss.fff");  // AI 판정일시
            HTMP_LOC_FILE_NM = "";   // HEATMAP 파일명(LOCAL)
            HTMP_LOC_FILE_DIR = "";
            HTMP_VIS_FILE_NM = "";   // HEATMAP 파일명(VISS)
            HTMP_VIS_FILE_DIR = "";
            BLUTOL_STRD1_X = 0;   // BLUETOOL 기준1_X
            BLUTOL_STRD1_Y = 0;   // BLUETOOL 기준1_Y
            BLUTOL_STRD2_X = 0;   // BLUETOOL 기준2_X
            BLUTOL_STRD2_Y = 0;   // BLUETOOL 기준2_Y
            BLUTOL_ROTAT_ANGL = 0;   // BLUETOOL 회전각도
            ROI_X = 0;   // ROI_X  / NOT NULL
            ROI_Y = 0;   // ROI_Y  / NOT NULL
            ROI_W = 0;   // ROI_WIDTH  / NOT NULL
            ROI_HT = 0;   // ROI_HEIGHT  / NOT NULL
            ROI1_INFRCE1_RST = "";   // ROI1_추론1_결과
            ROI1_INFRCE1_SCOR = 0;   // ROI1_추론1_Score
            ROI1_INFRCE2_RST = "";   // ROI1_추론2_결과
            ROI1_INFRCE2_RST_STR = "";   // 프로그램내 관리용으로 사용 JSON에 저장 안함
            ROI1_INFRCE2_RST_CD = "";   // 프로그램내 관리용으로 사용 JSON에 저장 안함
            ROI1_INFRCE2_OK_SCOR = 0;   // ROI1_추론2_OK_Score
            ROI1_INFRCE2_NG1_SCOR = 0;   // ROI1_추론2_NG1_Score
            ROI1_INFRCE2_NG2_SCOR = 0;   // ROI1_추론2_NG2_Score
            ROI1_INFRCE2_NG3_SCOR = 0;   // ROI1_추론2_NG3_Score
            ROI1_INFRCE2_NG4_SCOR = 0;   // ROI1_추론2_NG4_Score
            ROI2_INFRCE1_RST = "";   // ROI2_추론1_결과
            ROI2_INFRCE1_SCOR = 0;   // ROI2_추론1_Score
            ROI2_INFRCE2_RST = "";   // ROI2_추론2_결과
            ROI2_INFRCE2_RST_CD = "";   // 프로그램내 관리용으로 사용 JSON에 저장 안함
            ROI2_INFRCE2_RST_STR = "";   // 프로그램내 관리용으로 사용 JSON에 저장 안함
            ROI2_INFRCE2_OK_SCOR = 0;   // ROI2_추론2_OK_Score
            ROI2_INFRCE2_NG1_SCOR = 0;   // ROI2_추론2_NG1_Score
            ROI2_INFRCE2_NG2_SCOR = 0;   // ROI2_추론2_NG2_Score
            ROI2_INFRCE2_NG3_SCOR = 0;   // ROI2_추론2_NG3_Score
            ROI2_INFRCE2_NG4_SCOR = 0;   // ROI2_추론2_NG4_Score
            ROI3_INFRCE1_RST = "";   // ROI3_추론1_결과
            ROI3_INFRCE1_SCOR = 0;   // ROI3_추론1_Score
            ROI3_INFRCE2_RST = "";   // ROI3_추론2_결과
            ROI3_INFRCE2_RST_CD = "";   // 프로그램내 관리용으로 사용 JSON에 저장 안함
            ROI3_INFRCE2_RST_STR = "";   // 프로그램내 관리용으로 사용 JSON에 저장 안함
            ROI3_INFRCE2_OK_SCOR = 0;   // ROI3_추론2_OK_Score
            ROI3_INFRCE2_NG1_SCOR = 0;   // ROI3_추론2_NG1_Score
            ROI3_INFRCE2_NG2_SCOR = 0;   // ROI3_추론2_NG2_Score
            ROI3_INFRCE2_NG3_SCOR = 0;   // ROI3_추론2_NG3_Score
            ROI3_INFRCE2_NG4_SCOR = 0;   // ROI3_추론2_NG4_Score
            AI_INSP_RSLT_CD = "";   // 프로그램내 관리용으로 사용 JSON에 저장 안함
            AI_INSP_OK_SCOR = 0;    // 통합 ROI_추론2_OK_Score
            AI_INSP_NG1_SCOR = 0;   // 통합 ROI_추론2_NG1_Score
            AI_INSP_NG2_SCOR = 0;   // 통합 ROI_추론2_NG2_Score
            AI_INSP_NG3_SCOR = 0;   // 통합 ROI_추론2_NG3_Score
            AI_INSP_NG4_SCOR = 0;   // 통합 ROI_추론2_NG4_Score
            VPDL_MDL_FILE_NM = "";   // VPDL_MODEL_FILENAME
            INFRCE2_OK_THRHLD = -1; // 통합ROI 임계치
            CA_ROI_1_THRHLD = 0;   // CA_ROI1_임계치  / NOT NULL
            CA_ROI_2_THRHLD = 0;   // CA_ROI2_임계치  / NOT NULL
            CA_ROI_3_THRHLD = 0;   // CA_ROI3_임계치  / NOT NULL
            AN_ROI_1_THRHLD = 0;   // AN_ROI1_임계치  / NOT NULL
            AN_ROI_2_THRHLD = 0;   // AN_ROI2_임계치  / NOT NULL
            AN_ROI_3_THRHLD = 0;   // AN_ROI3_임계치  / NOT NULL
            ROI1_HEATMAP_IMAGE = null; // ROI1 히트맵 이미지 저장, json 저장 안함
            ROI2_HEATMAP_IMAGE = null; // ROI2 히트맵 이미지 저장, json 저장 안함
            ROI3_HEATMAP_IMAGE = null; // ROI3 히트맵 이미지 저장, json 저장 안함
            COMB_HEATMAP_IMAGE = null; // 히트맵 이미지 결합 저장, json 저장 안함
            CA_AVG_BRIGHTNESS = 0;  // CA 이미지 이상감지 Brightness 평균값, json 저장 안함.
            CA_SHARPNESS = 0;   // CA 이미지 이상감지 Sharpness, 저장 안함.
            AN_AVG_BRIGHTNESS = 0;  // AN 이미지 이상감지 Brightness 평균값, json 저장 안함.
            AN_SHARPNESS = 0;   // AN 이미지 이상감지 Sharpness, 저장 안함.
            RGSTR_ID = "IDMAX";   // 최초 등록 ID
            RGST_DTM = dtNow.ToString("yyyy-MM-dd HH:mm:ss.fff");   // 최초 등록 일시
            LAST_UPDR_ID = "IDMAX";   // 최종 수정 ID
            LAST_UPD_DTM = dtNow.ToString("yyyy-MM-dd HH:mm:ss.fff");  // 최종 수정 일시
        }
    }
}