using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public class CSVHelper
    {
        public static void SaveDataTableToCSVFile(string absolutePathAndFileName, DataTable dataTable, params string[] options)
        {
            string separator = string.Empty;

            if (options.Length > 0)
            {
                separator = options[0];
            }
            else
            {
                separator = ",";
            }

            string quote = "\"";

            //StreamWriter sw = new StreamWriter(absolutePathAndFileName, false, Encoding.Default);

            FileStream fs = new FileStream(absolutePathAndFileName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            int colCount = dataTable.Columns.Count;

            sw.Write(sw.NewLine);
            for (int i = 0; i < colCount; i++)
            {
                sw.Write(dataTable.Columns[i]);

                if (i < colCount - 1)
                {
                    sw.Write(separator);
                }
            }
            sw.Write(sw.NewLine);

            foreach (DataRow dr in dataTable.Rows)
            {
                for (int i = 0; i < colCount; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string data = dr[i].ToString();
                        data = data.Replace("\"", "\\\"");
                        sw.Write(quote + data + quote);
                    }
                    if (i < colCount - 1)
                    {
                        sw.Write(separator);
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        

        public static void SaveDataGridViewToCSVFile(string absolutePathAndFileName, DataGridView dataGridView, bool visibleColumn, params string[] options)
        {
            string separator = string.Empty;

            if (options.Length > 0)
            {
                separator = options[0];
            }
            else
            {
                separator = ",";
            }

            string quote = "\"";

            using (StreamWriter sw = new StreamWriter(absolutePathAndFileName, false, Encoding.Default))
            {
                if (visibleColumn)
                {
                    for (int i = 0; i < dataGridView.Columns.Count; i++)
                    {
                        if (dataGridView.Columns[i].Visible)
                        {
                            sw.Write(dataGridView.Columns[i].HeaderText);

                            if (i < dataGridView.Columns.Count - 1)
                            {
                                sw.Write(separator);
                            }
                        }
                    }
                    sw.Write(sw.NewLine);

                    for (int i = 0; i < dataGridView.RowCount - 1; i++)
                    {
                        for (int j = 0; j < dataGridView.Rows[i].Cells.Count; j++)
                        {
                            if (dataGridView.Columns[j].Visible)
                            {
                                string data = dataGridView.Rows[i].Cells[j].Value.ToString();
                                data = data.Replace("\"", "\\\"");
                                sw.Write(quote + data + quote);

                                if (j < dataGridView.Rows[i].Cells.Count - 1)
                                {
                                    sw.Write(separator);
                                }
                            }
                        }
                        sw.Write(sw.NewLine);
                    }
                }
                else
                {
                    for (int i = 0; i < dataGridView.Columns.Count; i++)
                    {
                        if (dataGridView.Columns[i].Visible)
                        {
                            sw.Write(dataGridView.Columns[i].HeaderText);

                            if (i < dataGridView.Columns.Count - 1)
                            {
                                sw.Write(separator);
                            }
                        }
                    }
                    sw.Write(sw.NewLine);

                    for (int i = 0; i < dataGridView.RowCount - 1; i++)
                    {
                        for (int j = 0; j < dataGridView.Rows[i].Cells.Count; j++)
                        {
                            if (dataGridView.Columns[j].Visible)
                            {
                                string data = dataGridView.Rows[i].Cells[j].Value.ToString();
                                data = data.Replace("\"", "\\\"");
                                sw.Write(quote + data + quote);

                                if (j < dataGridView.Rows[i].Cells.Count - 1)
                                {
                                    sw.Write(separator);
                                }
                            }
                        }
                        sw.Write(sw.NewLine);
                    }
                }
            }
        }

        public static DataTable LoadCSVtoDataTable(string absolutePathAndFileName, bool isFirstRowColumnName)
        {
            if (!File.Exists(absolutePathAndFileName)) return null;

            DataTable dataTable = new DataTable();

            string[] tableData = File.ReadAllLines(absolutePathAndFileName, Encoding.UTF8);

            if (isFirstRowColumnName)
            {
                var columns = from column in tableData[0].Trim().Split(",".ToCharArray())
                              select new DataColumn(column);

                dataTable.Columns.AddRange(columns.ToArray());
            }

            (from contents in tableData.Skip((isFirstRowColumnName) ? 1 : 0)
             select dataTable.Rows.Add(contents.Split(",".ToCharArray()))).ToList();

            return dataTable;
        }
    }
}