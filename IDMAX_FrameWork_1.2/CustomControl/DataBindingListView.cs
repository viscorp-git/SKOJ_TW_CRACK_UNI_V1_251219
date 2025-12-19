using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace IDMAX_FrameWork
{
    public class DataBindingListView : ListView
    {
        public DataBindingListView()
        {
            this.DoubleBuffered = true;
        }

        private bool _PageChanging = false;

        public delegate void EventHandler();
        public event EventHandler DataSource_Changed;

        private int _CurrentPage = 0;
        public int CurrentPage
        {
            get { return this._CurrentPage; }
            set
            {
                try
                {
                    //if (value <= 0) return;
                    if (value > this._TotalPageCount) return;

                    this._CurrentPage = value;

                    if (this._DataSource == null)
                    {
                        this.Clear();
                        return;
                    }

                    this.BeginUpdate();
                    this._PageChanging = true;

                    int dintRowCount = this._DataSource.Rows.Count;
                    DataTable ddtTemp = this._DataSource.Clone();

                    if (dintRowCount > this._PageRowCount)
                    {
                        for (int dintCount = (this._CurrentPage * this._PageRowCount) - this._PageRowCount;
                            dintCount < (this._CurrentPage * this._PageRowCount); dintCount++)
                        {
                            if (dintCount <= dintRowCount - 1)
                            {
                                ddtTemp.Rows.Add(this._DataSource.Rows[dintCount].ItemArray);
                            }
                        }
                    }
                    else
                    {
                        ddtTemp = this._DataSource;
                    }

                    this.Clear();

                    for (int dintCount = 0; dintCount < ddtTemp.Columns.Count; dintCount++)
                    {
                        ColumnHeader tempHeader = new ColumnHeader();
                        tempHeader.Text = ddtTemp.Columns[dintCount].ColumnName;
                        tempHeader.Name = ddtTemp.Columns[dintCount].ColumnName;

                        this.Columns.Add(tempHeader);
                    }

                    foreach (DictionaryEntry tempItem in this._ColumnHeaderTextTable)
                    {
                        this.Columns[tempItem.Key.ToString()].Text = tempItem.Value.ToString();
                    }

                    foreach (DictionaryEntry tempItem in this._ColumnHeaderWidthTable)
                    {
                        int dintWidth = 0;
                        int.TryParse(tempItem.Value.ToString(), out dintWidth);

                        this.Columns[tempItem.Key.ToString()].Width = dintWidth;
                    }

                    for (int dintCount = 0; dintCount < ddtTemp.Rows.Count; dintCount++)
                    {
                        string[] tempArray = new string[ddtTemp.Columns.Count];

                        for (int dintColumnCount = 0; dintColumnCount < ddtTemp.Columns.Count; dintColumnCount++)
                        {
                            tempArray[dintColumnCount] = ddtTemp.Rows[dintCount][dintColumnCount].ToString();
                        }

                        ListViewItem tempItem = new ListViewItem(tempArray);

                        this.Items.Add(tempItem);
                    }

                    this.EndUpdate();
                    this._PageChanging = false;
                }
                catch
                {
                    this._PageChanging = false;

                    throw;
                }
            }
        }

        private int _TotalPageCount = 0;
        public int TotalPageCount
        {
            get { return this._TotalPageCount; }
        }

        private int _PageRowCount = 20;
        public int PageRowCount
        {
            get { return this._PageRowCount; }
            set { this._PageRowCount = value; }
        }

        private DataTable _DataSource = null;
        public DataTable DataSource
        {
            get { return this._DataSource; }
            set
            {
                try
                {
                    this._DataSource = value;

                    if (this.DataSource_Changed != null) this.DataSource_Changed();

                    if (this._DataSource == null)
                    {
                        this.CurrentPage = 0;

                        this.Clear();

                        return;
                    }

                    int dintRowCount = value.Rows.Count;

                    if (dintRowCount % this._PageRowCount != 0)
                    {
                        this._TotalPageCount = (dintRowCount / this._PageRowCount) + 1;
                    }
                    else
                    {
                        this._TotalPageCount = (dintRowCount / this._PageRowCount);
                    }

                    this._ColumnHeaderTextTable = new Hashtable();
                    this._ColumnHeaderWidthTable = new Hashtable();

                    this.CurrentPage = (dintRowCount == 0) ? 0 : 1;
                }
                catch
                {
                    throw;
                }
            }
        }

        public void FirstPage()
        {
            try
            {
                this.CurrentPage = 1;
            }
            catch
            {
                throw;
            }
        }

        public void LastPage()
        {
            try
            {
                this.CurrentPage = this.TotalPageCount;
            }
            catch
            {
                throw;
            }
        }

        public void PrevPage()
        {
            try
            {
                --this.CurrentPage;
            }
            catch
            {
                throw;
            }
        }

        public void NextPage()
        {
            try
            {
                ++this.CurrentPage;
            }
            catch
            {
                throw;
            }
        }

        private Hashtable _ColumnHeaderTextTable = new Hashtable();
        private Hashtable _ColumnHeaderWidthTable = new Hashtable();

        public void SetColumnHeaderText(string ColumnName, string HeaderText)
        {
            try
            {
                if (!this._ColumnHeaderTextTable.ContainsKey(ColumnName))
                {
                    this._ColumnHeaderTextTable.Add(ColumnName, HeaderText);
                }
                else
                {
                    this._ColumnHeaderTextTable[ColumnName] = HeaderText;
                }

                this.Columns[ColumnName].Text = HeaderText;
            }
            catch
            {
                throw;
            }
        }

        public void SetColumnHeaderWidth(string ColumnName, int Width)
        {
            try
            {
                if (!this._ColumnHeaderWidthTable.ContainsKey(ColumnName))
                {
                    this._ColumnHeaderWidthTable.Add(ColumnName, Width);
                }
                else
                {
                    this._ColumnHeaderWidthTable[ColumnName] = Width;
                }

                this.Columns[ColumnName].Width = Width;
            }
            catch
            {
                throw;
            }
        }

        protected override void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
        {
            try
            {
                base.OnColumnWidthChanged(e);

                if (this._PageChanging) return;

                if (!this._ColumnHeaderWidthTable.ContainsKey(this.Columns[e.ColumnIndex].Name))
                {
                    this._ColumnHeaderWidthTable.Add(this.Columns[e.ColumnIndex].Name, this.Columns[e.ColumnIndex].Width);
                }
                else
                {
                    this._ColumnHeaderWidthTable[this.Columns[e.ColumnIndex].Name] = this.Columns[e.ColumnIndex].Width;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
