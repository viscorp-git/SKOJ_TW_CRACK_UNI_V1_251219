using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace IDMAX_FrameWork
{
    public class UpdateHelper
    {
        public static List<string> GetUpdateInfo(string downloadsURL, string versionFile, string resourceDownloadFolder, string applicationName)
        {
            try
            {
                if (!Directory.Exists(resourceDownloadFolder))
                {
                    Directory.CreateDirectory(resourceDownloadFolder);
                }

                bool updateChecked = DownloadHelper.DownloadFromWeb(downloadsURL, versionFile, resourceDownloadFolder);

                if (updateChecked)
                {
                    return PopulateInfoFromWeb(versionFile, resourceDownloadFolder, applicationName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //Log.SetLog(Log.Type.APP_ERROR, ex.ToString());

                return null;
            }
        }

        private static List<string> PopulateInfoFromWeb(string versionFile, string resourceDownloadFolder, string applicationName)
        {
            try
            {
                List<string> tempList = new List<string>();

                foreach (string strline in File.ReadAllLines(resourceDownloadFolder + versionFile))
                {
                    string[] parts = strline.Split('|');

                    if (parts[0] != applicationName) continue;

                    foreach (string part in parts)
                    {
                        tempList.Add(part);
                    }

                    return tempList;
                }

                return null;
            }
            catch (Exception ex)
            {
                //Log.SetLog(Log.Type.APP_ERROR, ex.ToString());

                return null;
            }
        }

        public static void InstallUpdateRestart(string downloadsURL, string filename, string destinationFolder, string processToEnd, string postProcess, string startupCommand, string updater)
        {
            try
            {
                string arguments = string.Empty;

                arguments += "|다운로드파일|" + filename;
                arguments += "|경로|" + downloadsURL;
                arguments += "|삭제경로|" + destinationFolder;
                arguments += "|프로세스종료|" + processToEnd;
                arguments += "|프로세스시작|" + postProcess;
                arguments += "|명령문|" + @" / " + startupCommand;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = updater;
                startInfo.Arguments = arguments;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
               // Log.SetLog(Log.Type.APP_ERROR, ex.ToString());
            }
        }

        public static void UpdaterUpdate(string updaterPrefix, string containingFolder)
        {

            DirectoryInfo dInfo = new DirectoryInfo(containingFolder);
            FileInfo[] updaterFiles = dInfo.GetFiles(updaterPrefix + "*");
            int fileCount = updaterFiles.Length;

            foreach (FileInfo file in updaterFiles)
            {
                string newFile = containingFolder + file.Name;
                string originalFile = containingFolder + @"\" + file.Name.Substring(updaterPrefix.Length, file.Name.Length - updaterPrefix.Length);

                if (File.Exists(originalFile))
                {
                    File.Delete(originalFile);
                }

                File.Move(newFile, originalFile);
            }
        }

        public static bool CheckUpdateAvailable(string currentVersion, string downloadsURL, string versionFile, string resourceDownloadFolder, string applicationName)
        {
            List<string> updateInformation = GetUpdateInfo(downloadsURL, versionFile, resourceDownloadFolder, applicationName);

            if (updateInformation == null) return false;

            Version _downloadVersion = new Version(updateInformation[1].ToString());
            Version _currentVersion = new Version(currentVersion);

            return CheckUpdateAvailable(_downloadVersion, _currentVersion);
        }

        public static bool CheckUpdateAvailable(Version downloadVersion, Version currentVersion)
        {
            if (downloadVersion > currentVersion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
