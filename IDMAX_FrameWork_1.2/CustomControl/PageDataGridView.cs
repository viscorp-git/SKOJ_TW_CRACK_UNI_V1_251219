using System.Data;
using Helper;
namespace IDMAX_FrameWork
{
    public class PageDataGridView : System.Windows.Forms.DataGridView
    {
        public delegate void PageDelegate(int currentPage, int totalPage);
        public event PageDelegate Page_Changed = null;

        public void SaveDataTableToCSVFile(string absolutePathAndFileName, bool colflag, params string[] options)
        {
            CSVHelper.SaveDataTableToCSVFile(absolutePathAndFileName, this._dataSource, options);
        }

        public void SaveDataGridViewToCSVFile(string absolutePathAndFileName, bool visibleColumn, params string[] options)
        {
            CSVHelper.SaveDataGridViewToCSVFile(absolutePathAndFileName, this, visibleColumn, options);
        }

        private int _currentPageNum = 1;
        public int CurrentPageNum
        {
            get { return this._currentPageNum; }
            set
            {
                if (value <= 0) return;
                if (value > this._totalPageCount) return;

                this._currentPageNum = value;

                if (this._dataSource == null) return;

                int rowCount = this._dataSource.Rows.Count;

                DataTable tempTable = this._dataSource.Clone();

                if (rowCount > this._pageRowCount)
                {
                    for (int dintCount = (this._currentPageNum * this._pageRowCount) - this._pageRowCount;
                        dintCount < (this._currentPageNum * this._pageRowCount); dintCount++)
                    {
                        if (dintCount <= rowCount - 1)
                        {
                            tempTable.Rows.Add(this._dataSource.Rows[dintCount].ItemArray);
                        }
                    }
                }
                else
                {
                    tempTable = this._dataSource;
                }

                base.DataSource = tempTable;

                if (this.Page_Changed != null) this.Page_Changed(this._currentPageNum, this._totalPageCount);
            }
        }

        private int _totalPageCount = 1;
        public int TotalPageCount
        {
            get { return this._totalPageCount; }
            //set { this._totalPageCount = value; }
        }

        private int _pageRowCount = 20;
        public int PageRowCount
        {
            get { return this._pageRowCount; }
            set { this._pageRowCount = value; }
        }

        private DataTable _dataSource = null;
        public new object DataSource
        {
            get { return this._dataSource; }
            set
            {
                this._dataSource = value as DataTable;
                base.DataSource = this._dataSource;

                if (this._dataSource == null || this._dataSource.Rows.Count == 0)
                {
                    return;
                }

                int dintRowCount = ((DataTable)value).Rows.Count;

                if (dintRowCount % this._pageRowCount != 0)
                {
                    this._totalPageCount = (dintRowCount / this._pageRowCount) + 1;
                }
                else
                {
                    this._totalPageCount = (dintRowCount / this._pageRowCount);
                }

                this.CurrentPageNum = 1;
            }
        }

        public void FirstPage()
        {
            this.CurrentPageNum = 1;
        }

        public void LastPage()
        {
            this.CurrentPageNum = this.TotalPageCount;
        }

        public void PrevPage()
        {
            this.CurrentPageNum--;
        }

        public void NextPage()
        {
            this.CurrentPageNum++;
        }
    }
}
