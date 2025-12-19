using Cognex.VisionPro;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ColorExtractor;
using Cognex.VisionPro.Implementation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace VisionTool
{
    [Serializable]
    public class cls_ColorBlob
    {
        public CogColorExtractorTool ColorBlob_tool = null;
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

        public cls_ColorBlob() 
        {
            ColorBlob_tool = new CogColorExtractorTool();
            blob_tool = new CogBlobTool();
        }

        public bool Run(Cognex.VisionPro.Display.CogDisplay cogDisplay)
        {
            try
            {
                try
                {
                    cogDisplay.InteractiveGraphics.Clear();
                    ColorBlob_tool.InputImage = (CogImage24PlanarColor)cogDisplay.Image;
                    ColorBlob_tool.Run();
                }
                catch
                {
                    return false;
                }
                
                CogImage8Grey aImg8 = CogImageConvert.GetIntensityImage(ColorBlob_tool.Results.OverallResult.GreyscaleImage, 0, 0, ColorBlob_tool.Results.OverallResult.GreyscaleImage.Width, ColorBlob_tool.Results.OverallResult.GreyscaleImage.Height);
                CogBlobLastRunRecordConstants a = blob_tool.LastRunRecordEnable;

                try
                {
                    blob_tool.InputImage = aImg8;
                    blob_tool.Run();
                }
                catch
                {
                    return false;
                }


                ICogRecord resultRecord = blob_tool.CreateLastRunRecord();
                AddGraphicsContent(cogDisplay, resultRecord);

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

        static public void AddGraphicsContent(Cognex.VisionPro.Display.CogDisplay display, ICogRecord parent)
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
