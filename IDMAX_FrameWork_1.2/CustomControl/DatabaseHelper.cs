using System;
using System.Data.OleDb;
using System.Windows.Forms;


namespace Helper
{
    public class DatabaseHelper
    {
        public static OleDbConnection GetConnection(string connectionString)
        {
            try
            {
                OleDbConnection connection = new OleDbConnection(connectionString);
                
                connection.Open();

                return connection;
            }
            catch (Exception ex)
            {
                //Log.SetLog(Log.Type.APP_ERROR, ex.ToString());

                System.Windows.Forms.MessageBox.Show("데이터베이스 연결에 실패하였습니다.", "오류", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
        }

        public static bool Disconnection(OleDbConnection connection)
        {
            try
            {
                if (connection != null)
                {
                    connection.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
