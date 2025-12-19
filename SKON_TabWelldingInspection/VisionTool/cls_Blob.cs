using Cognex.VisionPro;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VisionTool
{
    [Serializable]
    public class cls_Blob
    {
        public CogBlobTool blob_tool = null;
        public CogBlob blob = null;

        public bool Run_flag = false;

        public bool Run_Result_flag = false;

        public ICogImage Run_Result_Image = null;
        public string Run_Result = "";
        public string Run_Stat_Result = "";
        public CogBlobSegmentationPolarityConstants polarity;
        public int blob_size = 10;
        public int Threshold = 0;

        public bool none_flag = false;

        public cls_Blob()
        {
            blob_tool = new CogBlobTool();
        }

        public bool Run(Cognex.VisionPro.Display.CogDisplay cogDisplay, string cell_id, string position)
        {
            int blobResultCount = 0;
            List<Tuple<string, string, string, int, double, double, double>> csvData = new List<Tuple<string, string, string, int, double, double, double>>();

            try
            {
                try
                {
                    cogDisplay.InteractiveGraphics.Clear();
                    blob_tool.InputImage = cogDisplay.Image;
                    blob_tool.Run();
                    Run_Result_Image = blob_tool.Results.CreateBlobImage();
                }
                catch (Exception)
                {
                    return false;
                }

                ICogRecord resultRecord = blob_tool.CreateLastRunRecord();
                AddGraphicsContent(cogDisplay, resultRecord);

                DateTime Date = DateTime.Now;
                string hhmmss = Date.ToString("HH:mm:ss");
                string yyyymmdd = Date.ToString("yyyyMMdd");

                if (!none_flag)
                {
                    if (blob_tool.Results.GetBlobs().Count > 0)
                    {
                        Run_Result_flag = true;

                        blobResultCount = blob_tool.Results.GetBlobs().Count;

                        try
                        {
                            for (int i = 0; i < blobResultCount; i++)
                            {
                                csvData.Add(new Tuple<string, string, string, int, double, double, double>(hhmmss, cell_id, position
                                    , i
                                    , blob_tool.Results.GetBlobByID(i).Area
                                    , blob_tool.Results.GetBlobs()[i].CenterOfMassX
                                    , blob_tool.Results.GetBlobs()[i].CenterOfMassY));

                                // getBlobByID 로 찾는 방법과 GetBlobs()로 찾는 방법 2가지가 있는것 같다.
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex}");
                        }

                        DirectoryInfo di_Local = new DirectoryInfo(@"D:\CSV");
                        if (di_Local.Exists == false)
                        {
                            di_Local.Create();
                        }
                        SaveDataToCSV(csvData, @"D:\CSV\VisionProData_" + yyyymmdd + ".csv");

                        Run_Result = blobResultCount.ToString();
                    }
                    else
                    {
                        Run_Result_flag = false;
                        Run_Result = "0";
                    }
                }
                else
                {
                    if (blob_tool.Results.GetBlobs().Count > 0)
                    {
                        Run_Result_flag = false;

                        Run_Result = blobResultCount.ToString();
                    }
                    else
                    {
                        Run_Result_flag = true;
                        Run_Result = "0";
                    }
                }

                Run_Stat_Result = Run_Result;
                Run_flag = true;
            }
            catch (Exception)
            {
                Run_flag = false;
            }
            return Run_flag;
        }

        private void SaveDataToCSV(List<Tuple<string, string, string, int, double, double, double>> csvData, string filePath)
        {
            StringBuilder sb = new StringBuilder();
            if (File.Exists(filePath) == false)
            {
                File.WriteAllText(filePath, sb.ToString());

                sb.AppendLine("Time,Cell_ID,Position,ID,Area,CenterMassX,CenterMassY");
            }

            foreach (var item in csvData)
            {
                sb.AppendLine($" {item.Item1}, {item.Item2}, {item.Item3}, {item.Item4}, {item.Item5}, {item.Item6}, {item.Item7}");
            }

            File.AppendAllText(filePath, sb.ToString());
        }

        public bool Run(ICogImage img)
        {
            try
            {
                CogImage8Grey aImg8 = CogImageConvert.GetIntensityImage(img, 0, 0, img.Width, img.Height);

                blob_tool.InputImage = aImg8;
                blob_tool.Run();
                Run_Result_Image = blob_tool.Results.CreateBlobImage();

                if (!none_flag)
                {
                    if (blob_tool.Results.GetBlobs().Count > 0)
                    {
                        Run_Result_flag = true;
                        Run_Result = blob_tool.Results.GetBlobs().Count.ToString();
                    }
                    else
                    {
                        Run_Result_flag = false;
                        Run_Result = "0";
                    }
                    Run_Stat_Result = Run_Result;
                }
                else
                {
                    if (blob_tool.Results.GetBlobs().Count > 0)
                    {
                        Run_Result_flag = false;
                        Run_Result = blob_tool.Results.GetBlobs().Count.ToString();
                    }
                    else
                    {
                        Run_Result_flag = true;
                        Run_Result = "0";
                    }
                    Run_Stat_Result = Run_Result;
                }

                Run_flag = true;
            }
            catch (Exception)
            {
                Run_flag = false;
            }
            return Run_flag;
        }

        public static void AddGraphicsContent(Cognex.VisionPro.Display.CogDisplay display, ICogRecord parent)
        {
            foreach (CogRecord record in parent.SubRecords)
            {
                if (typeof(ICogGraphic).IsAssignableFrom(record.ContentType))
                {
                    if (record.Content != null)
                        display.InteractiveGraphics.Add(record.Content as ICogGraphicInteractive, "Result", false);
                }
                else if (typeof(CogGraphicCollection).IsAssignableFrom(record.ContentType))
                {
                    if (record.Content != null)
                    {
                        CogGraphicCollection graphics = record.Content as CogGraphicCollection;
                        foreach (ICogGraphic graphic in graphics)
                        {
                            display.InteractiveGraphics.Add(graphic as ICogGraphicInteractive, "Result", false);
                        }
                    }
                }
                else if (typeof(CogGraphicInteractiveCollection).IsAssignableFrom(record.ContentType))
                {
                    if (record.Content != null)
                    {
                        display.InteractiveGraphics.AddList(record.Content as CogGraphicInteractiveCollection, "Result", false);
                    }
                }

                AddGraphicsContent(display, record);
            }
        }
    }
}