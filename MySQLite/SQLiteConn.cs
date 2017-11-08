using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLite
{
    public class SQLiteConn
    {
        private SQLiteConnection sqliteConnection;

        public SQLiteConn(string connection)
        {
            sqliteConnection = new SQLiteConnection(connection);//"Data Source=AGV_Data.db;Version=3;datetimeformat=CurrentCulture"
        }

        public void Open()
        {
            sqliteConnection.Open();
        }

        public void Close()
        {
            sqliteConnection.Close();
        }

        public DataTable DataTable_Sql(string sql)
        {
            SQLiteDataAdapter MyAdapter;
            DataSet MyDataSet = new DataSet();
            DataTable MyDataTable = new DataTable();
            MyAdapter = new SQLiteDataAdapter(sql, sqliteConnection);
            MyDataSet.Reset();
            MyAdapter.Fill(MyDataSet);
            MyDataTable = MyDataSet.Tables[0];
            return MyDataTable;
        }

        public int Execute_NonSQL(string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
            int ret = command.ExecuteNonQuery();
            return ret;
        }

        public void StartInsert()
        {
            SQLiteCommand sqlComm;
            sqlComm = new SQLiteCommand("begin", sqliteConnection);
            sqlComm.ExecuteNonQuery();
        }
        public void EndInsert()
        {
            SQLiteCommand sqlComm;
            sqlComm = new SQLiteCommand("end", sqliteConnection);
            sqlComm.ExecuteNonQuery();
        }
    }
}
