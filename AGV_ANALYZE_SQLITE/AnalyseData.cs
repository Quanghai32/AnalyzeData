using DebugForm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySQLite;

namespace AGV_ANALYZE_SQLITE
{
    class AnalyseData
    {
        private List<SupplyDetail> listSupplying = new List<SupplyDetail>();
        SQLiteConn sqliteConn = new SQLiteConn("Data Source=AGV_Data.db;Version=3;datetimeformat=CurrentCulture");
        public void CalculateSupplying(string date, string dept, string block, string agv, string time)
        {
            RecordDebug.Print("Calcualate supplying detail for: " + agv + " ------- Dept: " + dept + " Block" + block);
            bool calAbNormal;

            DateTime startTime, stopTime, startAbnormal, stopAbnormal;
            Int32 count;
            string part;
            int route;

            string query = "select Dept, Block, DateTime, Item, para1, value1, Value2,Value3 FROM [AgvData_" + date + "] " +
                "where Dept = '" + dept + "' and Block = '" + block + "' and Item = '" + agv + "'  and Para1 = 'Working status' ";// +
                //time;

            var dtKtra = sqliteConn.DataTable_Sql(query);
            if (dtKtra.Rows.Count > 0)
            {
                calAbNormal = false;
                count = 0;
                while (count < dtKtra.Rows.Count - 3)
                {

                    if ((dtKtra.Rows[count]["Value1"].ToString() == "SUPPLYING") &&
                        (dtKtra.Rows[count + 1]["Value1"].ToString() == "RETURNING") &&
                        (dtKtra.Rows[count + 2]["Value1"].ToString() == "FREE"))
                    {
                        if (calAbNormal)
                        {
                            stopAbnormal = DateTime.Parse(dtKtra.Rows[count]["DateTime"].ToString());
                            calAbNormal = false;
                        }

                        startTime = DateTime.Parse(dtKtra.Rows[count]["DateTime"].ToString());
                        stopTime = DateTime.Parse(dtKtra.Rows[count + 2]["DateTime"].ToString());
                        TimeSpan supplyTime = stopTime - startTime;

                        part = dtKtra.Rows[count]["Value3"].ToString();
                        route = Int16.Parse(dtKtra.Rows[count]["Value2"].ToString());
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
                            startAbnormal = DateTime.Parse(dtKtra.Rows[count]["DateTime"].ToString());
                            calAbNormal = true;
                        }
                        count++;
                    }
                }
            }
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

            var dtKtra = sqliteConn.DataTable_Sql(query);

            if (dtKtra.Rows.Count > 0)
            {
                for (int j = 0; j < dtKtra.Rows.Count; j++)
                {
                    if (dtKtra.Rows[j]["Para1"].ToString() == "Position")
                    {
                        position = Convert.ToInt16(dtKtra.Rows[j]["Value1"].ToString());
                    }

                    if (dtKtra.Rows[j]["Para1"].ToString() == "Status")
                    {
                        sttChange = dtKtra.Rows[j]["Value1"].ToString();
                        stopTime = DateTime.Parse(dtKtra.Rows[j]["DateTime"].ToString());
                        TimeSpan duration = stopTime - startTime;

                        if (stt != sttChange)
                        {
                            _supply.SetStatus(stt, position, duration.TotalSeconds);
                        }
                        startTime = DateTime.Parse(dtKtra.Rows[j]["DateTime"].ToString());
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
