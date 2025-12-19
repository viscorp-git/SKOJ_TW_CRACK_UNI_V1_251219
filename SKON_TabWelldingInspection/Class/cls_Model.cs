using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using System.Drawing;
using Class;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.Implementation.Internal;


namespace SKON_TabWelldingInspection
{
    [Serializable]
    class cls_Model_Info : ISerializable
    {
        public CogToolBlock CathodeToolBlock { get; set; }
        public CogToolBlock AnodeToolBlock { get; set; }

        public cls_Model_Info()
        {
            CathodeToolBlock = new CogToolBlock();
            AnodeToolBlock = new CogToolBlock();
        }

        public cls_Model_Info(SerializationInfo _info, StreamingContext _ctxt)
        {
            CathodeToolBlock = (CogToolBlock)_info.GetValue("CathodeToolBlock", typeof(CogToolBlock));
            AnodeToolBlock = (CogToolBlock)_info.GetValue("AnodeToolBlock", typeof(CogToolBlock));
        }

        public void GetObjectData(SerializationInfo _info, StreamingContext _ctext)
        {
            _info.AddValue("CathodeToolBlock", CathodeToolBlock);
            _info.AddValue("AnodeToolBlock", AnodeToolBlock);
        }
    }

    public class cls_Model
    {
        public string ModelName { get; set; }
        public int ModelNumber { get; set; }

        public CogToolBlock CathodeToolBlock { get; set; }
        public CogToolBlock AnodeToolBlock { get; set; }

        public cls_Model()
        {
            ModelName = "None";
            ModelNumber = -1;
            System.Type cogImageType = typeof(Cognex.VisionPro.ICogImage);
            System.Type cogRecord = typeof(Cognex.VisionPro.ICogRecord);
            System.Type stringType = typeof(string);
            System.Type doubleType = typeof(double);
            CathodeToolBlock = new CogToolBlock();
            CathodeToolBlock.Inputs.Add(new CogToolBlockTerminal("CellID", stringType));
            CathodeToolBlock.Inputs.Add(new CogToolBlockTerminal("CathodeImage", cogImageType));
            CathodeToolBlock.Outputs.Add(new CogToolBlockTerminal("CathodeRecord", cogRecord));
            CathodeToolBlock.Outputs.Add(new CogToolBlockTerminal("Result", stringType));
            CathodeToolBlock.Outputs.Add(new CogToolBlockTerminal("ErrorMessage", stringType));
            AnodeToolBlock = new CogToolBlock();
            AnodeToolBlock.Inputs.Add(new CogToolBlockTerminal("CellID", stringType));
            AnodeToolBlock.Inputs.Add(new CogToolBlockTerminal("AnodeImage", cogImageType));
            AnodeToolBlock.Outputs.Add(new CogToolBlockTerminal("AnodeRecord", cogRecord));
            AnodeToolBlock.Outputs.Add(new CogToolBlockTerminal("Result", stringType));
            AnodeToolBlock.Outputs.Add(new CogToolBlockTerminal("ErrorMessage", stringType));
        }

        public cls_Model DeepCopy()
        {
            cls_Model copyModel = new cls_Model();
            copyModel.ModelName = this.ModelName;
            copyModel.ModelNumber = this.ModelNumber;
            copyModel.CathodeToolBlock = CogSerializer.DeepCopyObject(CathodeToolBlock) as CogToolBlock;
            copyModel.AnodeToolBlock = CogSerializer.DeepCopyObject(AnodeToolBlock) as CogToolBlock;
            return copyModel;
        }

        public bool Model_Save(string _path)
        {
            GC.Collect();
            Stream stream = null;

            try
            {
                string sDirPath = "";
                cls_Model_Info modelInfo = new cls_Model_Info();

                modelInfo.CathodeToolBlock = CathodeToolBlock;
                modelInfo.AnodeToolBlock = AnodeToolBlock;

                if (_path == "")
                    sDirPath = Application.StartupPath + "\\Model";
                else
                    sDirPath = _path.Substring(0, _path.LastIndexOf(@"\"));

                DirectoryInfo di = new DirectoryInfo(sDirPath);

                if (!di.Exists)
                {
                    di.Create();
                }

                stream = File.Open(_path, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();

                Console.WriteLine("Writing Information");
                bformatter.Serialize(stream, modelInfo);
                stream.Flush();

                return true;
            }
            catch (SerializationException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        public bool Model_Load(string _path)
        {
            Stream stream = null;

            try
            {
                //Clear mp for further usage.
                cls_Model_Info info;
                string filePath = Path.GetFileName(_path);
                string[] strSplit = filePath.Split('_');

                //Open the file written above and read values from it.
                stream = File.Open(_path, FileMode.Open);
                stream.Position = 0;
                BinaryFormatter bformatter = new BinaryFormatter();

                Console.WriteLine("Reading Information");
                info = (cls_Model_Info)bformatter.Deserialize(stream);

                ModelName = filePath.Substring(strSplit[0].Length + 1).Replace(".mpp", "");
                ModelNumber = Convert.ToInt16(strSplit[0]);
                CathodeToolBlock = info.CathodeToolBlock;
                AnodeToolBlock = info.AnodeToolBlock;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

    }
}
