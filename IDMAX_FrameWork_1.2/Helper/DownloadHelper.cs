using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public class ByteArgs : EventArgs
    {
        private int _downloaded = 0;
        public int Downloaded
        {
            get
            {
                return this._downloaded;
            }
            set
            {
                this._downloaded = value;
            }
        }

        private int _total = 0;
        public int Total
        {
            get
            {
                return this._total;
            }
            set
            {
                this._total = value;
            }
        }
    }

    public class DownloadHelper
    {
        public delegate void BytesDownloadedEventHandler(ByteArgs e);
        public delegate void DownloadResultEventHandler();

        public static event BytesDownloadedEventHandler BytesDownloaded;

        public static bool DownloadFromWeb(string url, string file, string targetFolder)
        {
            try
            {
                //byte[] downloadedData = new byte[0];

                //WebRequest webReq = WebRequest.Create(url + file);

                //webReq.Credentials = new NetworkCredential("User", "@user");

                //WebResponse webResponse = webReq.GetResponse();

                //Stream dataStream = webResponse.GetResponseStream();

                //byte[] dataBuffer = new byte[1024];

                //int dataLength = (int)webResponse.ContentLength;

                //ByteArgs byteArgs = new ByteArgs();

                //byteArgs.Downloaded = 0;
                //byteArgs.Total = dataLength;

                //if (BytesDownloaded != null) BytesDownloaded(byteArgs);

                //MemoryStream memoryStream = new MemoryStream();
                //while (true)
                //{
                //    int bytesFromStream = dataStream.Read(dataBuffer, 0, dataBuffer.Length);

                //    if (bytesFromStream == 0)
                //    {
                //        byteArgs.Downloaded = dataLength;
                //        byteArgs.Total = dataLength;
                //        if (BytesDownloaded != null) BytesDownloaded(byteArgs);

                //        break;
                //    }
                //    else
                //    {
                //        memoryStream.Write(dataBuffer, 0, bytesFromStream);

                //        byteArgs.Downloaded = bytesFromStream;
                //        byteArgs.Total = dataLength;
                //        if (BytesDownloaded != null) BytesDownloaded(byteArgs);
                //    }
                //}

                //downloadedData = memoryStream.ToArray();

                //dataStream.Close();
                //memoryStream.Close();

                //FileStream newFile = new FileStream(targetFolder + file, FileMode.Create);
                //newFile.Write(downloadedData, 0, downloadedData.Length);
                //newFile.Close();




                FtpWebRequest requestFileDownload = (FtpWebRequest)WebRequest.Create(url + file);
                requestFileDownload.Credentials = new NetworkCredential("User", "@user");
                requestFileDownload.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse responseFileDownload = (FtpWebResponse)requestFileDownload.GetResponse();

                Stream responseStream = responseFileDownload.GetResponseStream();
                FileStream writeStream = new FileStream(Application.StartupPath + @"\" + file, FileMode.Create);

                int Length = 2048;
                Byte[] buffer = new Byte[Length];
                int bytesRead = responseStream.Read(buffer, 0, Length);

                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, Length);
                }

                responseStream.Close();
                writeStream.Close();

                requestFileDownload = null;
                responseFileDownload = null;

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
