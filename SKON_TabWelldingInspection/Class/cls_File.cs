
using HslCommunication.LogNet;
using SKON_TabWelldingInspection.Class;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    [Serializable]
    class cls_File
    {
        private static bool _isRunning = false;
        private static readonly object _lock = new object();

        public static void CreateFolder(string pstr_FolderName)
        {
            DirectoryInfo l_DirectoryInfo = new DirectoryInfo(pstr_FolderName);

            if (l_DirectoryInfo.Exists == false)
            {
                l_DirectoryInfo.Create();
            }
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

        public static void OpenFolder(Label label)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                label.Text = dialog.SelectedPath;
                Environment.SpecialFolder root = dialog.RootFolder;
            }
        }

        public static bool FormScreen_Save(Form p_FormName, string pstr_ImageName, string pstr_Filepath)
        {
            try
            {
                DirectoryInfo l_DirectoryInfo = new DirectoryInfo(pstr_Filepath);

                Bitmap l_BmpTemp = new Bitmap(p_FormName.Width, p_FormName.Height);
                Graphics l_Graphics = Graphics.FromImage(l_BmpTemp);
                l_Graphics.CopyFromScreen(new System.Drawing.Point(p_FormName.Bounds.X, p_FormName.Bounds.Y), new System.Drawing.Point(0, 0), p_FormName.Size);

                if (l_DirectoryInfo.Exists == false)
                {
                    l_DirectoryInfo.Create();
                }

                l_BmpTemp.Save(pstr_Filepath + "\\" + pstr_ImageName + ".bmp");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static String[] GetFromToDays(DateTime FromDate, DateTime EndDate)
        {
            String[] arrDays;
            String strDays = "";

            String strFrom = FromDate.ToString("yyyy_MM_dd 00:00:00");
            String strEnd = EndDate.ToString("yyyy_MM_dd 00:00:00");
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dtFrom = DateTime.ParseExact(strFrom, "yyyy_MM_dd HH:mm:ss", provider);
            DateTime dtTo = DateTime.ParseExact(strEnd, "yyyy_MM_dd HH:mm:ss", provider);

            while (dtFrom <= dtTo)
            {
                strDays += dtFrom.ToString("yyyy_MM_dd") + ",";
                dtFrom = dtFrom.AddDays(1);
            }

            arrDays = strDays.Substring(0, strDays.Length - 1).Split(',');

            return arrDays;
        }

        /// <summary>
        /// 저장 디렉터리 (최하위 디렉토리)를 기준으로 삭제하는 메서드
        /// </summary>
        /// <param name="saveFilePercent">저장 한계치</param>
        /// <param name="deleteTargetDirs">디렉토리 배열</param>
        public static void CheckSpaceAndDeleteOldImage(cls_Log mLog, int saveFilePercent, string[] deleteTargetDirs)
        {
            try
            {
                lock (_lock)
                {
                    if (_isRunning)
                        return;

                    _isRunning = true;
                }

                int maxDeleteCount = 48;   // 한번 수행에 폴더 48개 지운다. Max 기준 이틀 정도 지우도록.

                // TargetDirectory를 기입하지 않았다면, 수행하지 않음.
                if (deleteTargetDirs.Length < 1)
                    return;

                // 첫 번째 디렉토리의 드라이브 정보 사용 (모든 디렉토리는 같은 드라이브에 있다고 가정)
                DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(deleteTargetDirs[0]));
                double usedSpacePercent = (double)(driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / driveInfo.TotalSize * 100;

                // 저장 용량 초과 시 삭제 작업 수행
                if (usedSpacePercent > saveFilePercent)
                {
                    var allDirectories = new List<DirectoryInfo>();

                    // 각 루트 디렉토리 순회
                    foreach (var rootDir in deleteTargetDirs)
                    {
                        try
                        {
                            // 최하위 폴더만 추가
                            var directories = Directory.EnumerateDirectories(rootDir, "*", SearchOption.AllDirectories)
                                                       .Select(dir =>
                                                       {
                                                           try
                                                           {
                                                               return new DirectoryInfo(dir);
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               mLog.WriteLog("SYS", $"Error creating DirectoryInfo for {dir}: {ex.Message}");
                                                               return null;
                                                           }
                                                       })
                                                       .Where(dir => dir != null && dir.EnumerateDirectories().Count() == 0);

                            if (directories != null && directories.Any())
                            {
                                var directoriesList = directories.ToList();
                                allDirectories.AddRange(directoriesList);
                                mLog.WriteLog("SYS", $"Added {directoriesList.Count} directories to allDirectories.");
                            }

                        }
                        catch (Exception ex)
                        {
                            mLog.WriteLog("SYS", $"Error accessing directory {rootDir}: {ex.Message}");
                        }
                    }

                    // 전체 디렉토리를 생성 시간 기준으로 정렬
                    allDirectories = allDirectories.OrderBy(dir => dir.CreationTime).ToList();
                    mLog.WriteLog("SYS", $"Total directories found: {allDirectories.Count}");

                    foreach (var dir in allDirectories)
                    {
                        try
                        {
                            if (dir.Name.Equals("atis") || dir.Name.Equals("tab"))
                                continue;

                            mLog.WriteLog("SYS", $"Deleting folder: {dir.FullName}");
                            
                            maxDeleteCount--;

                            dir.Delete(true); // 폴더와 하위 모든 파일 삭제
                            
                            //try
                            //{
                            //    // 상위 디렉토리(Parent) 확인
                            //    var parentDir = dir.Parent;
                            //    if (parentDir != null && !parentDir.EnumerateFileSystemInfos().Any()) // 상위 디렉토리가 비어 있는지 확인
                            //    {
                            //        mLog.WriteLog("SYS", $"Deleting empty folder: {dir.FullName}");
                            //        parentDir.Delete(true); // 폴더 삭제
                            //    }
                            //    else
                            //    {
                            //        mLog.WriteLog("SYS", $"Skipping folder {dir.FullName}, as it contains files or subdirectories.");
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    mLog.WriteLog("SYS", $"Error deleting folder {dir.FullName}: {ex.Message}");
                            //}
                        }
                        catch (Exception ex)
                        {
                            mLog.WriteLog("SYS", $"Error deleting folder {dir.FullName}: {ex.Message}");
                        }

                        if (maxDeleteCount < 0)
                        {
                            mLog.WriteLog("SYS", "Storage usage is now below the threshold.");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLog.WriteLog("SYS", $"Delete File Error: {ex.Message}");
            }
            finally
            {
                lock(_lock)
                {
                    _isRunning = false;
                }
            }
        }

        // 삭제 폴더 3개 대상 추가, 이미지, 블루, 히트맵 // 2024-07-04
        // 파일이  억단위 이상이 될경우, 문제가 될 소지가 있음.  그래서 사용하지 않고 디렉토리단위로 삭제하는 걸로 새로 코드 생성 (이력을 위해 남겨둠) 2024-12-03
        public static void CheckSpaceAndDeleteOldImage(int saveFilePercent, string deleteTargetDir1, string deleteTargetDir2, string deleteTargetDir3)
        {
            // 삭제조건에 해당할 때, 삭제할 파일 개수
            int deleteFileCnt = 100000;

            try
            {
                // 감시폴더가 포함된 드라이브의 정보
                DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(deleteTargetDir1));
                // 사용 용량 계산
                double usedSpacePercent = (double)(driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / driveInfo.TotalSize * 100;

                // 용량이 부족할 경우 삭제 작업을 진행한다.
                if (usedSpacePercent > saveFilePercent)
                {
                    // 지정된 폴더내 모든 파일을 리스트에 등록
                    List<Tuple<string, DateTime>> allFiles = new List<Tuple<string, DateTime>>();

                    try
                    {
                        DirectoryInfo directoryInfo1 = new DirectoryInfo(deleteTargetDir1);
                        FileInfo[] fi1 = directoryInfo1.GetFiles("*.jpg", SearchOption.AllDirectories);

                        foreach (var file1 in fi1)
                        {
                            allFiles.Add(Tuple.Create(file1.FullName, file1.CreationTime));
                        }

                        DirectoryInfo directoryInfo2 = new DirectoryInfo(deleteTargetDir2);
                        FileInfo[] fi2 = directoryInfo2.GetFiles("*.jpg", SearchOption.AllDirectories);

                        foreach (var file2 in fi2)
                        {
                            allFiles.Add(Tuple.Create(file2.FullName, file2.CreationTime));
                        }

                        DirectoryInfo directoryInfo3 = new DirectoryInfo(deleteTargetDir3);
                        FileInfo[] fi3 = directoryInfo3.GetFiles("*.jpg", SearchOption.AllDirectories);

                        foreach (var file3 in fi3)
                        {
                            allFiles.Add(Tuple.Create(file3.FullName, file3.CreationTime));
                        }

                        // 파일을 생성 시간을 기준으로 정렬
                        allFiles.Sort((x, y) => x.Item2.CompareTo(y.Item2));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"error : {ex.Message}");
                    }

                    // 오래된 순으로 정렬되어 있으므로 allFiles[0]가 가장 오래된 파일이다.
                    // deleteFileCnt 보다 큰 만큼 삭제할예정
                    for (int i = 0; i < deleteFileCnt; i++)
                    {
                        // 파일 요청개수보다 저장된 파일의 수가 작을수가 있다.
                        if (i >= allFiles.Count) break;
                        // 대상 파일을 삭제한다.
                        Console.WriteLine($"{i + 1} {allFiles[i].Item1}  {allFiles[i].Item2}");
                        try
                        {
                            File.Delete(allFiles[i].Item1);
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error : {ex.Message}");
            }
        }
    }
}