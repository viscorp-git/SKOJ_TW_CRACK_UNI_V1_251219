using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cognex.VisionPro;
using Cognex.VisionPro.PMRedLine;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.CalibFix;
using Class;

namespace VisionTool
{
    [Serializable]
    public class cls_Pattern
    {
        public CogPMRedLineTool PatternTool = null;
        public CogFixtureTool fixture_tool = null;
        public CogTransform2DLinear getpose = null;
        public bool Run_Result_Flag = false;
        public bool Run_Fixture_Flag = false;
        public double Point_X = 0;
        public double Point_Y = 0;

        public bool Train_Flag
        {
            get
            {
                if (PatternTool != null)
                {
                    return PatternTool.Pattern.Trained;
                }

                return false;
            }
        }

        public cls_Pattern()
        {
            PatternTool = new CogPMRedLineTool();
            fixture_tool = new CogFixtureTool();
        }

        public bool Run(ICogImage _img, List<ICogRecord> _record, bool _draw_Flag = true)
        {
            try
            {
                if (_img != null)
                {
                    PatternTool.InputImage = _img;

                    if (PatternTool.Pattern.Trained)
                    {
                        PatternTool.Run();

                        if (PatternTool.Results.Count > 0)
                        {
                            getpose = PatternTool.Results[0].GetPose();
                            Point_X = PatternTool.Results[0].GetPose().TranslationX;
                            Point_Y = PatternTool.Results[0].GetPose().TranslationY;

                            if (_draw_Flag)
                            {
                                _record.Add(PatternTool.CreateLastRunRecord());
                            }

                            Run_Result_Flag = true;
                        }
                        else
                        {
                            Run_Result_Flag = false;
                        }
                    }
                    else
                    {
                        Run_Result_Flag = false;
                    }
                }
                else
                {
                    Run_Result_Flag = false;
                }
            }
            catch (Exception)
            {
                Run_Result_Flag = false;
            }

            return Run_Result_Flag;
        }
    }
}
