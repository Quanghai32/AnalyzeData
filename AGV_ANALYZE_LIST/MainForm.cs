using DebugForm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySQLite;

namespace AGV_ANALYZE_LIST
{
    public partial class MainForm : Form
    {
        struct StructDataToThread
        {
            public string _date;
            public string _dept;
            public string _block;
            public string _filePath;
        }

        SQLiteConn sqliteConn = new SQLiteConn("Data Source=AGV_Data.db;Version=3;datetimeformat=CurrentCulture");
        private List<RawData> RawList;
        private List<SupplyDetail> listSupplying = new List<SupplyDetail>();


        public MainForm()
        {
            InitializeComponent();
            RecordDebug.SetLoggerForm(ucLogger);
            sqliteConn.Open();
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

        private void OpenAndCalculate(object obje)
        {
            string date = ((StructDataToThread)obje)._date;
            string dept = ((StructDataToThread)obje)._dept;
            string block = ((StructDataToThread)obje)._block;
            string path = ((StructDataToThread)obje)._filePath;
            RawList = new List<RawData>();

            List<string> s = new List<string>();
            s = File.ReadLines(path).ToList();
            setMaximum(s.Count);
            //CheckDb(date);
            RecordDebug.Print("Start copy at " + DateTime.Now.ToString("HH:mm:ss"));
            int startTime = Environment.TickCount;
            FileStream fStream = new FileStream(path, FileMode.Open);
            StreamReader sReader = new StreamReader(fStream);
            int count = 0;

            while (!sReader.EndOfStream)
            {
                string line = sReader.ReadLine();
                string[] values = line.Split(',');
                RawData r = new RawData
                {
                    Dept = dept,
                    Block = block,
                    DateTime = DateTime.Parse(values[0]),
                    Item = values[1],
                    Para1 = values[2],
                    Value1 = values[3],
                    Para2 = values[4],
                    Value2 = values[5],
                    Para3 = values[6],
                    Value3 = values[7]
                };
                RawList.Add(r);
                count++;
                //showProgresss(count);
            }
            fStream.Close();
            sReader.Close();


            int stopTime = Environment.TickCount;
            float copyTime = (stopTime - startTime) / 1000;
            RecordDebug.Print("Copy time: " + copyTime.ToString());
            RecordDebug.Print("Start calculate at " + DateTime.Now.ToString("HH:mm:ss"));

            Calculate(date, dept, block);
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
            if (this.InvokeRequired)
            {
                this.Invoke(new setMaximunDelegate(setMaximum), value);
            }
            else
            {
                this.progressBar1.Maximum = value;
                labelProgress.Text = value.ToString();
            }
        }

        private void buttonClick_Click(object sender, EventArgs e)
        {
            
        }

        private void Calculate(string date, string dept, string block)
        {
            List<string> listAGV = RawList
                .Select(a => a.Item)
                .Where(a => a.StartsWith("AGV"))
                .GroupBy(a => a)
                .Select(a => a.First())
                .ToList();
            sqliteConn.StartInsert();
            for (int i = 0; i < listAGV.Count; i++)
            {
                RecordDebug.Print("Calculating supplying detail for " + listAGV[i]);
                CalculateSupplyTime(date, dept, block, listAGV[i]);
            }
            sqliteConn.EndInsert();
            RecordDebug.Print("Finish calculate supplying detail at " + DateTime.Now.ToString());

        }

        struct DataAnalyze
        {
            public DateTime DateTime;
            public string Item;
            public string Para1;
            public string Value1;
            public string Value2;
            public string Value3;
        }
        private void CalculateSupplyTime(string date, string dept, string block, string agv)
        {
            List<DataAnalyze> dtKtra = RawList.Where(a => a.Item == agv && a.Para1=="Working status")
                .Select(a => new DataAnalyze()
                {
                    DateTime = a.DateTime,
                    Item = a.Item,
                    Para1 = a.Para1,
                    Value1 = a.Value1,
                    Value2 = a.Value2,
                    Value3 = a.Value3
                })
                .ToList();
           

            if (dtKtra.Count == 0) return;

            bool calAbNormal = false;
            DateTime startTime, stopTime, startAbnormal, stopAbnormal;
            string part;
            int route;
            int count;
            count = 0;
            while (count < dtKtra.Count - 3)
            {

                if ((dtKtra[count].Value1.ToString() == "SUPPLYING") &&
                    (dtKtra[count + 1].Value1.ToString() == "RETURNING") &&
                    (dtKtra[count + 2].Value1.ToString() == "FREE"))
                {
                    if (calAbNormal)
                    {
                        stopAbnormal = DateTime.Parse(dtKtra[count].DateTime.ToString());
                        calAbNormal = false;
                    }

                    startTime = DateTime.Parse(dtKtra[count].DateTime.ToString());
                    stopTime = DateTime.Parse(dtKtra[count + 2].DateTime.ToString());
                    TimeSpan supplyTime = stopTime - startTime;

                    part = dtKtra[count].Value3.ToString();
                    route = Int16.Parse(dtKtra[count].Value2.ToString());
                    SupplyDetail _supply = new SupplyDetail(date, dept, block, agv, part, route, startTime.ToString("HH:mm:ss"),
                        stopTime.ToString("HH:mm:ss"), Math.Round(supplyTime.TotalMinutes, 1));

                    AnalyseDetailByTime(date, dept, block, _supply);

                    listSupplying.Add(_supply);
                    count += 3;

                }
                else
                {
                    if (!calAbNormal)
                    {
                        startAbnormal = DateTime.Parse(dtKtra[count].DateTime.ToString());
                        calAbNormal = true;
                    }
                    count++;
                }
            }

        }
        public struct StructAnalyzeDetail
        {
            public string Dept;
            public string Block;
            public DateTime DateTime;
            public string Item;
            public string Para1;
            public string Value1;
            public string Value3;
        }
        private void AnalyseDetailByTime(string date, string dept, string block, SupplyDetail _supply)
        {
            string[] normalStt = { "NORMAL", "STOP_BY_CARD", "SAFETY", "NO_CART", "BATTERY_EMPTY", "OUT_OF_LINE", "EMERGENCY", "POLE_ERROR" };
            int position = 0;
            string stt = "NORMAL";
            string sttChange;
            string time = "(cast(datetime as time) >= '" + _supply.StartTime + "' and cast(datetime as time)<= '" + _supply.EndTime + "')";

            DateTime startTime = DateTime.ParseExact(date + " " + _supply.StartTime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime stopTime;


            string query = "select Dept, Block, DateTime, Item, para1, value1, Value3 FROM [AgvData_" + date + "] " +
                " where Dept='" + dept + "' and Block ='" + block + "' and  Item='" + _supply.AgvName + "'" +
                "and (Para1='status' or Para1='Position') and Value1 !='CROSS_STOP' and Value1 !='CROSS_RUN' order by DateTime";// and" + time +
                                                                                                                                //"order by DateTime";

            List<StructAnalyzeDetail> dtKtra = RawList
                .Where(d => d.Item == _supply.AgvName && (d.Para1 == "Status" || d.Para1 == "Position") && d.Value1 != "CROSS_STOP" && d.Value1 != "CROSS_RUN")
                .Select(d => new StructAnalyzeDetail()
                {
                    Dept = d.Dept,
                    Block = d.Block,
                    DateTime = d.DateTime,
                    Item = d.Item,
                    Para1 = d.Para1,
                    Value1 = d.Value1,
                    Value3 = d.Value3
                })
                .ToList();
            
            if (dtKtra.Count > 0)
            {
                for (int j = 0; j < dtKtra.Count; j++)
                {
                    if (dtKtra[j].Para1.ToString() == "Position")
                    {
                        position = Convert.ToInt16(dtKtra[j].Value1.ToString());
                    }

                    if (dtKtra[j].Para1.ToString() == "Status")
                    {
                        sttChange = dtKtra[j].Value1.ToString();
                        stopTime = DateTime.Parse(dtKtra[j].DateTime.ToString());
                        TimeSpan duration = stopTime - startTime;

                        if (stt != sttChange)
                        {
                            _supply.SetStatus(stt, position, duration.TotalSeconds);
                        }
                        startTime = DateTime.Parse(dtKtra[j].DateTime.ToString());
                        stt = sttChange;
                    }
                }

                _supply.NORMAL = Math.Round((_supply._normal.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.NORMAL;
                _supply.SAFETY = Math.Round((_supply._safety.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.SAFETY;
                _supply.EMERGENCY = Math.Round((_supply._emergency.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.EMERGENCY;
                _supply.NO_CART = Math.Round((_supply._noCart.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.NO_CART;
                _supply.POLE_ERROR = Math.Round((_supply._poleError.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.POLE_ERROR;
                _supply.STOP_BY_CARD = Math.Round((_supply._stop.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.STOP_BY_CARD;
                _supply.OUT_OF_LINE = Math.Round((_supply._outOfLine.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.OUT_OF_LINE;
                _supply.BATTERY_EMPTY = Math.Round((_supply._batteryLow.totalTime / 60), 1).ToString() + " min. Detail: ," + _supply.BATTERY_EMPTY;
                
                sqliteConn.Execute_NonSQL("insert into [AGV_SupplyDetail](Date, Dept, Block, AgvName, Part, Route, StartTime, EndTime, SupplyTime," +
                    "NORMAL_DETAIL, STOP_BY_CARD_DETAIL, SAFETY_DETAIL, BATTERY_EMPTY_DETAIL, NO_CART_DETAIL, POLE_ERROR_DETAIL, OUT_OF_LINE_DETAIL, EMERGENCY_DETAIL, " +
                    "NORMAL, STOP_BY_CARD, SAFETY, BATTERY_EMPTY, NO_CART, POLE_ERROR, OUT_OF_LINE, EMERGENCY)" +
                      "VALUES('" + date + "','" + dept + "','" + block + "','" + _supply.AgvName + "','" + _supply.Part + "','" +
                      _supply.Route + "','" + _supply.StartTime + "','" + _supply.EndTime + "','" + _supply.SupplyTime + "','" +
                      _supply.NORMAL + "','" + _supply.STOP_BY_CARD + "','" + _supply.SAFETY + "','" + _supply.BATTERY_EMPTY + "','" +
                      _supply.NO_CART + "','" + _supply.POLE_ERROR + "','" + _supply.OUT_OF_LINE + "','" + _supply.EMERGENCY + "','" +
                     Math.Round((_supply._normal.totalTime / 60), 1) + "','" + Math.Round((_supply._stop.totalTime / 60), 1) + "','" + Math.Round((_supply._safety.totalTime / 60), 1) + "','" + Math.Round((_supply._batteryLow.totalTime / 60), 1) + "','" +
                      Math.Round((_supply._noCart.totalTime / 60), 1) + "','" + Math.Round((_supply._poleError.totalTime / 60), 1) + "','" + Math.Round((_supply._outOfLine.totalTime / 60), 1) + "','" + Math.Round((_supply._emergency.totalTime / 60), 1) + "' )");
            }
        }

    }
}

