using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ImageProcessing;
using SKON_TabWelldingInspection.Class;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Class
{
    public class cls_Util
    {
        private cls_Ini ini;
        private string mPosition;
        private const int CAM_CATHODE = 0;
        private const int CAM_ANODE = 1;

        public cls_Util(string iniFilename)
        {
            ini = new cls_Ini(iniFilename);

            // INI 파일에서 포트 설정을 로드
            LoadIni();
        }

        public void LoadIni()
        {
            mPosition = ini.ReadIniString("Maintenance", "Position", "TOP");
        }

  

        public static byte Encode_BooltoByte(bool[] _arr)
        {
            byte val = 0;

            foreach (bool b in _arr)
            {
                val <<= 1;
                if (b) val |= 1;
            }

            return val;
        }

        public static class Rotate
        {
            public static CogIPOneImageTool cogOneImageTool = new CogIPOneImageTool();

            public static ICogImage rotateImage = null;

            public static CogImage8Grey Rotation(CogImage8Grey image, CogIPOneImageFlipRotateOperationConstants rotate)
            {
                CogIPOneImageTool cogOneImageTool = new CogIPOneImageTool();
                CogIPOneImageFlipRotate flip = new CogIPOneImageFlipRotate();
                cogOneImageTool.InputImage = image;
                cogOneImageTool.Operators.Add(flip);
                flip.OperationInPixelSpace = rotate;
                cogOneImageTool.Run();
                CogTransform2DLinear linear = new CogTransform2DLinear();
                linear.TranslationX = 0;
                linear.TranslationY = 0;
                cogOneImageTool.OutputImage.PixelFromRootTransform = linear;

                ICogImage rotateImage = cogOneImageTool.OutputImage.CopyBase(CogImageCopyModeConstants.CopyPixels);

                CogImage8Grey temp = CogImageConvert.GetIntensityImage(rotateImage, 0, 0, rotateImage.Width, rotateImage.Height);
                GC.Collect();
                return temp;
            }
        }

        public static ICogImage OpenImage()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = "jpg";
            openFile.Filter = "Graphics interchange Format (*.jpg)|*.jpg|All files(*.*)|*.*";
            openFile.ShowDialog();

            if (openFile.FileName.Length > 0)
            {
                Bitmap mBitmap = new Bitmap(openFile.FileName);
                CogImageFile cImageFile = new CogImageFile();
                // cImageFile.Open(openFile.FileName, CogImageFileModeConstants.Read);
                ICogImage cimage = new CogImage24PlanarColor(mBitmap);
                cImageFile.Close();
                //CogImage8Grey aImg8;
                //PatternRun(cimage);

                //aImg8 = CogImageConvert.GetIntensityImage(cimage, 0, 0, cimage.Width, cimage.Height);

                GC.Collect();
                return cimage;
            }
            else
            {
                return null;
            }
        }

        public static float GetAverage(float _summary, int _valueCount)
        {
            if (_valueCount == 0f)
            {
                return 0f;
            }

            float average = _summary / _valueCount;

            return average;
        }

        public static string GetPath(string _masterPath, string _name, int _number)
        {
            return _masterPath + "\\" + _number.ToString().PadLeft(2, '0') + "_" + _name + ".mpp";
        }

        public static bool FileDelete(string _path)
        {
            //삭제 - 먼저 삭제할 파일을 FileInfo로 연다.
            FileInfo fileDel = new FileInfo(_path);

            if (fileDel.Exists) //삭제할 파일이 있는지
            {
                try
                {
                    fileDel.Delete(); //없어도 에러안남
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                foreach (var srcPath in Directory.GetFiles(sourceFolder))
                {
                    //Copy the mFile from sourcepath and place into mentioned target path,
                    //Overwrite the mFile if same mFile is exist in target path
                    File.Copy(srcPath, srcPath.Replace(sourceFolder, destFolder), true);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool FileCopy(string old_Path, string newPath)
        {
            try
            {
                File.Copy(old_Path, newPath);   // 파일 복사
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool FileRename(string _oldPath, string _newPath)
        {
            FileInfo file = new FileInfo(_oldPath);

            if (file.Exists)//파일이 있는지
            {
                try
                {
                    file.MoveTo(_newPath); //이미있으면 에러
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public class CellGenerator
        {
            public static string CellGenerateCode(string LineNo, DateTime timestamp, string forceText = "")
            {
                // 코드의 고정된 부분
                string code = string.IsNullOrEmpty(LineNo) ? "T1000" : $"T{LineNo}000";

                int year = Convert.ToInt32(timestamp.ToString("yyyy"));
                int month = Convert.ToInt32(timestamp.ToString("MM"));
                int day = Convert.ToInt32(timestamp.ToString("dd"));

                // 6번째부터 8번째 문자: 위치에 따른 룩업 값
                string lookupValue6 = GetLookupValue(year - 2010);
                string lookupValue7 = GetLookupValue(month);
                string lookupValue8 = GetLookupValue(day);

                // 9번째부터 16번째 문자: 시간 형식
                string timePart = string.Empty;
                
                if(string.IsNullOrEmpty(forceText))
                    timePart = timestamp.ToString("HHmmssff").Substring(0, 8);
                else
                {
                    if (forceText.Length != 8)
                        timePart = timestamp.ToString("HHmmssff").Substring(0, 8);
                    else
                        timePart = forceText;
                }

                // 마지막 두 문자 "01" 고정
                string suffix = "01";

                return code + lookupValue6 + lookupValue7 + lookupValue8 + timePart + suffix;
            }

            private static string GetLookupValue(int position)
            {
                // 위치가 1에서 9 사이일 경우 숫자를 그대로 문자열로 사용
                if (position >= 1 && position <= 9)
                {
                    return position.ToString("D1");
                }

                // 이미지 규칙에 따른 룩업 테이블
                string[,] table = new string[,]
                {
                    { "A", "B", "C", "D", "E", "F", "G", "H", "J" },
                    { "K", "L", "M", "N", "P", "R", "S", "T", "V" },
                    { "W", "X", "Y", "0", "", "", "", "", "" }
                };

                int row = (position - 10) / 9;
                int col = (position - 10) % 9;

                // 테이블 범위 내에서 값을 가져옴
                if (row >= 0 && row < table.GetLength(0) && col >= 0 && col < table.GetLength(1))
                {
                    return table[row, col];
                }

                // 위치가 테이블 범위를 벗어나면 빈 문자열 반환
                return "";
            }
        }

        public static (double, double) CheckDrive(string driveLetter)
        {
            try
            {
                string drivePath = driveLetter + @":\";
                DriveInfo driveInfo = new DriveInfo(drivePath);

                if (driveInfo.IsReady)
                {
                    double totalSpaceGB = driveInfo.TotalSize / (1024 * 1024 * 1024); // 총 용량 (GB 단위)
                    double freeSpaceGB = driveInfo.AvailableFreeSpace / (1024 * 1024 * 1024); // 남은 공간 (GB 단위)

                    return (totalSpaceGB, freeSpaceGB);
                }
                else
                {
                    return (0, 0);
                }
            }
            catch (DriveNotFoundException)
            {
                return (0, 0);
            }
            catch (Exception ex)
            {
                return (0, 0);
            }
        }

        public class CustomBool
        {
            private bool _value;
            private string _name;
            // 값 변경 시 발생하는 이벤트
            public event Action<string, bool> OnValueChanged;

            // 속성으로 값 관리
            public bool Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        var handler = OnValueChanged; // 로컬 복사본 생성
                        handler?.Invoke(_name, _value); // 이벤트 트리거
                    }
                }
            }

            // 생성자
            public CustomBool(bool initialValue = false, string name = "")
            {
                _value = initialValue;
                _name = name;
            }

        }
    }
}