using DebugForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySQLite;

namespace AGV_ANALYZE_SQLITE
{
    public partial class MainForm : Form
    {
        AnalyseData analyseData = new AnalyseData();
        struct StructDataToThread
        {
            public string _date;
            public string _dept;
            public string _block;
            public string _filePath;
        }
        public MainForm()
        {
            InitializeComponent();
        }

        SQLiteConn sqliteConn = new SQLiteConn("Data Source=AGV_Data.db;Version=3;datetimeformat=CurrentCulture");

        private void MainForm_Load(object sender, EventArgs e)
        {
            RecordDebug.SetLoggerForm(ucLogger);
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            DialogResult resutl = open.ShowDialog();
            DateTime date;
            string fileName;
            string dept, block;

            if (resutl == DialogResult.OK)
            {
                fileName = open.SafeFileName;
                string[] nameArray = fileName.Split('_');
                dept = nameArray[0];
                block = nameArray[1];
                date = DateTime.ParseExact(nameArray[2].Remove(8), "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            else
            {
                return;
            }

            Thread calculateThread = new Thread((obj) =>
            {
                StructDataToThread st = (StructDataToThread)obj;
                OpenAndCalculate(st);
            });
            calculateThread.Name = date.ToString("yyyyMMdd") + " " + dept + " " + block;
            calculateThread.Start(new StructDataToThread() { _date = date.ToString("yyyyMMdd"), _dept = dept, _block = block, _filePath = open.FileName });
        }

        private void CheckDb(string date)
        {
            //check if table allready exist
            //sqliteConn.Open();
            var dtKtra = sqliteConn.DataTable_Sql("SELECT name FROM sqlite_master WHERE type='table' AND name='AgvData_" + date + "'");
            if (dtKtra.Rows.Count > 0)
            {
                RecordDebug.Print("Table [AgvData_" + date + "] already exist");
                RecordDebug.Print("Upload data to table: [AgvData_" + date + "]");
            }
            else
            {
                RecordDebug.Print("Create table [AgvData_" + date + "]");
                sqliteConn.Execute_NonSQL("CREATE TABLE [AgvData_" + date + "]( " +
                    "[Id] BIGINT PRIMARY KEY," +
                    "[Dept]   VARCHAR(50)      NULL," +
                    "[Block]   VARCHAR(50) NULL," +
                    "[DateTime]   DATETIME      NULL," +
                    "[Item]   VARCHAR(50) NULL," +
                    "[Para1]  VARCHAR(50) NULL," +
                    "[Value1] VARCHAR(50) NULL," +
                    "[Para2]  VARCHAR(50) NULL," +
                    "[Value2] VARCHAR(50) NULL," +
                    "[Para3]  VARCHAR(50) NULL," +
                    "[Value3] VARCHAR(50) NULL)"
                    );
            }
            //sqliteConn.Close();
        }

        private void OpenAndCalculate(object obje)
        {
            string date = ((StructDataToThread)obje)._date;
            string dept = ((StructDataToThread)obje)._dept;
            string block = ((StructDataToThread)obje)._block;
            string path = ((StructDataToThread)obje)._filePath;

            List<string> s = new List<string>();
            s = File.ReadLines(path).ToList();
            setMaximum(s.Count);
            CheckDb(date);
            RecordDebug.Print("Start copy at " + DateTime.Now.ToString("HH:mm:ss"));
            int startTime = Environment.TickCount;
            FileStream fStream = new FileStream(path, FileMode.Open);
            StreamReader sReader = new StreamReader(fStream);
            int count = 0;

            //sqliteConn.Open();
            sqliteConn.StartInsert();
            while (!sReader.EndOfStream)
            {
                string line = sReader.ReadLine();
                string[] values = line.Split(',');
                sqliteConn.Execute_NonSQL("insert into [AgvData_" + date + "](Dept,Block,DateTime,Item,Para1,Value1,Para2,Value2,Para3,Value3)" +
                    "values('" + dept + "','" + block + "','" + values[0] + "','" + values[1] + "','" + values[2] + "','" + values[3] + "','" + values[4] + "','" +
                    values[5] + "','" + values[6] + "','" + values[7] + "')");
                count++;
                //showProgresss(count);
            }
            fStream.Close();
            sReader.Close();

            //sqliteConn.EndInsert();
            //sqliteConn.Close();

            int stopTime = Environment.TickCount;
            float copyTime = (stopTime - startTime) / 1000;
            RecordDebug.Print("Copy time: " + copyTime.ToString());
            RecordDebug.Print("Start calculate at " + DateTime.Now.ToString("HH:mm:ss"));

            string getDept = "SELECT DISTINCT Dept FROM [AGVData_" + date + "]";
            var listDept = sqliteConn.DataTable_Sql(getDept);
            if (listDept.Rows.Count > 0)
            {
                for (int deptNum = 0; deptNum < listDept.Rows.Count; deptNum++)
                {
                    dept = listDept.Rows[deptNum]["Dept"].ToString();
                    string getBlock = "SELECT DISTINCT Block FROM [AGVData_" + date + "] " +
                                      "where Dept= '" + listDept.Rows[deptNum]["Dept"].ToString() + "'";
                    var listBlock = sqliteConn.DataTable_Sql(getBlock);
                    if (listBlock.Rows.Count > 0)
                    {
                        for (int blockNum = 0; blockNum < listBlock.Rows.Count; blockNum++)
                        {
                            block = listBlock.Rows[blockNum]["Block"].ToString();
                            //CalculateThread(new StructDataToThread() { _date = date, _dept = dept, _block = block });

                            Thread calculateThread = new Thread((obj) =>
                            {
                                StructDataToThread st = (StructDataToThread)obj;
                                CalculateThread(st);
                            });
                            calculateThread.Name = date + " " + dept + " " + block;
                            calculateThread.Start(new StructDataToThread() { _date = date, _dept = dept, _block = block });

                            block = "";
                        }
                    }
                    dept = "";
                }
            }
        }

        private void CalculateThread(object obj)
        {
            StructDataToThread d = (StructDataToThread)obj;
            string date = d._date;
            string dept = d._dept;
            string block = d._block;

            string _StartShipTime = "08:00:00";
            string _StartFirstBreakTime = "09:50:00";
            string _EndFirstBreakTime = "10:00:00";
            string _StartEatTime = "11:40:00";
            string _EndEatTime = "12:35:00";
            string _StartSecondBreakTime = "14:50:00";
            string _EndSecondBreakTime = "14:00:00";
            string _EndShipTime = "14:00:00";

            DateTime StartShipTime = DateTime.ParseExact(date + " " + _StartShipTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime StartFirstBreakTime = DateTime.ParseExact(date + " " + _StartFirstBreakTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime EndFirstBreakTime = DateTime.ParseExact(date + " " + _EndFirstBreakTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime StartEatTime = DateTime.ParseExact(date + " " + _StartEatTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime EndEatTime = DateTime.ParseExact(date + " " + _EndEatTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime StartSecondBreakTime = DateTime.ParseExact(date + " " + _StartSecondBreakTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime EndSecondBreakTime = DateTime.ParseExact(date + " " + _EndSecondBreakTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime EndShipTime = DateTime.ParseExact(date + " " + _EndShipTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);

            string[] normalStt = { "NORMAL", "STOP_BY_CARD", "SAFETY", "NO_CART", "BATTERY_EMPTY", "OUT_OF_LINE", "EMERGENCY", "POLE_ERROR", "FREE" };
            //string agv;
            string time2 = "((cast(datetime as time)>='" + StartShipTime + "' and cast(datetime as time)<='" + StartFirstBreakTime + "') or " +
                        "(cast(datetime as time) >= '" + EndFirstBreakTime + "' and cast(datetime as time) <= '" + StartEatTime + "') or " +
                        "(cast(datetime as time) >= '" + EndEatTime + "' and cast(datetime as time) <= '" + StartSecondBreakTime + "') or " +
                        "(cast(datetime as time) >= '" + EndSecondBreakTime + "' and cast(datetime as time) <= '" + EndShipTime + "')) " +
                        "order by DateTime";
            string time = "(cast(datetime as time)>='" + StartShipTime + "' and cast(datetime as time) <= '" + EndShipTime + "') " +
                        "order by DateTime";

            string getAgv = "SELECT DISTINCT Item, Dept, Block FROM [AGVData_" + date + "] " +
                "where Item like 'AGV%' and dept= '" + dept + "' and block = '" + block + "'";

            var listAgvs = sqliteConn.DataTable_Sql(getAgv);

            time = "and(cast(datetime as time) >= '08:00:00 AM' and cast(datetime as time) <= '17:00:00 PM') " + "order by DateTime";
            if (listAgvs.Rows.Count > 0)
            {
                for (int i = 0; i < listAgvs.Rows.Count; i++)
                {
                    string agv = listAgvs.Rows[i]["item"].ToString();
                    analyseData.CalculateSupplying(date, dept, block, agv, time);
                }
            }



            RecordDebug.Print(DateTime.Now.ToString("HH:mm:ss") + " finish calculate " + dept + " " + block);
            //RecordDebug.stopTime = Environment.TickCount;
            //RecordDebug.Print("Total time to calculate: " + ((RecordDebug.stopTime - RecordDebug.startTime) / 1000).ToString());
        }

        public delegate void showww(float valuesss);
        public delegate void setMaximunDelegate(int value);
        public void showProgresss(float valuesss)
        {
            if (this.InvokeRequired)
            {
                Invoke(new showww(showProgresss), valuesss);
            }
            else
            {
                progressBar1.Value = (int)valuesss;
                labelProgress.Text = ((int)valuesss).ToString() + "/" + progressBar1.Maximum.ToString() + " lines.";
            }
        }
        private void setMaximum(int value)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new setMaximunDelegate(setMaximum), value);
            }
            else
            {
                this.progressBar1.Maximum = value;
                labelProgress.Text = value.ToString();
            }
        }

    }
}
