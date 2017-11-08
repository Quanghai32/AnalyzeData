using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV_ANALYZE_SQLITE
{
    public struct TimeInPosition
    {
        public int position;
        public double duration;
        public double totalTime;
    }

    class SupplyDetail
    {
        public SupplyDetail(string _date, string _dept, string _block, string _agvName,
            string _part, int _route, string _startTime, string _endTime, double _supplyTime)
        {
            this.Date = _date;
            this.Dept = _dept;
            this.Block = _block;
            this.AgvName = _agvName;
            this.Part = _part;
            this.Route = _route;
            this.StartTime = _startTime;
            this.EndTime = _endTime;
            this.SupplyTime = _supplyTime;

            this.NORMAL = "\r\n";
            this.STOP_BY_CARD = "\r\n";
            this.SAFETY = "\r\n";
            this.BATTERY_EMPTY = "\r\n";
            this.NO_CART = "\r\n";
            this.EMERGENCY = "\r\n";
            this.OUT_OF_LINE = "\r\n";
            this.POLE_ERROR = "\r\n";
        }

        public string Date { get; set; }
        public string Dept { get; set; }
        public string Block { get; set; }
        public string AgvName { get; set; }
        public string Part { get; set; }
        public int Route { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double SupplyTime { get; set; }

        public string NORMAL { get; set; }
        public string STOP_BY_CARD { get; set; }
        public string SAFETY { get; set; }
        public string BATTERY_EMPTY { get; set; }
        public string NO_CART { get; set; }
        public string EMERGENCY { get; set; }
        public string OUT_OF_LINE { get; set; }
        public string POLE_ERROR { get; set; }

        public TimeInPosition _normal;
        public TimeInPosition _stop;
        public TimeInPosition _safety;
        public TimeInPosition _batteryLow;
        public TimeInPosition _noCart;
        public TimeInPosition _emergency;
        public TimeInPosition _outOfLine;
        public TimeInPosition _poleError;


        public void SetStatus(string stt, int position, double duration)
        {

            switch (stt)
            {
                case "NORMAL":
                    SetNormal(position, duration);
                    break;
                case "STOP_BY_CARD":
                    SetStop(position, duration);
                    break;
                case "SAFETY":
                    SetSafety(position, duration);
                    break;
                case "BATTERY_EMPTY":
                    SetBatteryLow(position, duration);
                    break;
                case "NO_CART":
                    SetNoCart(position, duration);
                    break;
                case "EMERGENCY":
                    SetEmergency(position, duration);
                    break;
                case "OUT_OF_LINE":
                    SetOutLine(position, duration);
                    break;
                case "POLE_ERROR":
                    SetPoleErr(position, duration);
                    break;
            }

        }

        private void SetNormal(int position, double duration)
        {
            if (_normal.position != position)//create, add to string
            {
                _normal.position = position;
                _normal.duration = duration;
                this.NORMAL += (_normal.duration.ToString() + " s in position: " + _normal.position.ToString() + ",\r\n");
                _normal.totalTime += duration;
            }
            else
            {
                _normal.duration += duration;
                if (position != 0)
                {
                    this.NORMAL = this.NORMAL.Remove(this.NORMAL.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.NORMAL += ("\r\n" + _normal.duration.ToString() + " s in position: " + _normal.position.ToString() + ",\r\n");
                }
                _normal.totalTime += duration;
            }
        }

        private void SetSafety(int position, double duration)
        {
            if (_safety.position != position)
            {
                _safety.position = position;
                _safety.duration = duration;
                this.SAFETY += (_safety.duration.ToString() + " s in position: " + _safety.position.ToString() + ",\r\n");
                _safety.totalTime += duration;
            }
            else
            {
                _safety.duration += duration;
                if (position != 0)
                {
                    this.SAFETY = this.SAFETY.Remove(this.SAFETY.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.SAFETY += ("\r\n" + _safety.duration.ToString() + " s in position: " + _safety.position.ToString() + ",\r\n");
                }
                _safety.totalTime += duration;
            }
        }

        private void SetStop(int position, double duration)
        {
            if (_stop.position != position)
            {
                _stop.position = position;
                _stop.duration = duration;
                this.STOP_BY_CARD += (_stop.duration.ToString() + " s in position: " + _stop.position.ToString() + ",\r\n");
                _stop.totalTime += duration;
            }
            else
            {
                _stop.duration += duration;
                if (position != 0)
                {
                    this.STOP_BY_CARD = this.STOP_BY_CARD.Remove(this.STOP_BY_CARD.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.STOP_BY_CARD += ("\r\n" + _stop.duration.ToString() + " s in position: " + _stop.position.ToString() + ",\r\n");
                }
                _stop.totalTime += duration;
            }
        }

        private void SetBatteryLow(int position, double duration)
        {
            if (_batteryLow.position != position)
            {
                _batteryLow.position = position;
                _batteryLow.duration = duration;
                this.BATTERY_EMPTY += (_batteryLow.duration.ToString() + " s in position: " + _batteryLow.position.ToString() + ",\r\n");
                _batteryLow.totalTime += duration;
            }
            else
            {
                _batteryLow.duration += duration;
                if (position != 0)
                {
                    this.BATTERY_EMPTY = this.BATTERY_EMPTY.Remove(this.BATTERY_EMPTY.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.BATTERY_EMPTY += ("\r\n" + _batteryLow.duration.ToString() + " s in position: " + _batteryLow.position.ToString() + ",\r\n");
                }
                _batteryLow.totalTime += duration;
            }
        }

        private void SetNoCart(int position, double duration)
        {
            if (_noCart.position != position)
            {
                _noCart.position = position;
                _noCart.duration = duration;
                this.NO_CART += (_noCart.duration.ToString() + " s in position: " + _noCart.position.ToString() + ",\r\n");
                _noCart.totalTime += duration;
            }
            else
            {
                _noCart.duration += duration;
                if (position != 0)
                {
                    this.NO_CART = this.NO_CART.Remove(this.NO_CART.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.NO_CART += ("\r\n" + _noCart.duration.ToString() + " s in position: " + _noCart.position.ToString() + ",\r\n");
                }
                _noCart.totalTime += duration;
            }
        }

        private void SetEmergency(int position, double duration)
        {
            if (_emergency.position != position)
            {
                _emergency.position = position;
                _emergency.duration = duration;
                this.EMERGENCY += (_emergency.duration.ToString() + " s in position: " + _emergency.position.ToString() + ",\r\n");
                _emergency.totalTime += duration;
            }
            else
            {
                _emergency.duration += duration;
                if (position != 0)
                {
                    this.EMERGENCY = this.EMERGENCY.Remove(this.EMERGENCY.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.EMERGENCY += ("\r\n" + _safety.duration.ToString() + " s in position: " + _safety.position.ToString() + ",\r\n");
                }
                _emergency.totalTime += duration;
            }
        }

        private void SetOutLine(int position, double duration)
        {
            if (_outOfLine.position != position)
            {
                _outOfLine.position = position;
                _outOfLine.duration = duration;
                this.OUT_OF_LINE += (_outOfLine.duration.ToString() + " s in position: " + _outOfLine.position.ToString() + ",\r\n");
                _outOfLine.totalTime += duration;
            }
            else
            {
                _outOfLine.duration += duration;
                if (position != 0)
                {
                    this.OUT_OF_LINE = this.OUT_OF_LINE.Remove(this.OUT_OF_LINE.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.OUT_OF_LINE += ("\r\n" + _outOfLine.duration.ToString() + " s in position: " + _outOfLine.position.ToString() + ",\r\n");
                }
                _outOfLine.totalTime += duration;
            }
        }

        private void SetPoleErr(int position, double duration)
        {
            if (_poleError.position != position)
            {
                _poleError.position = position;
                _poleError.duration = duration;
                this.POLE_ERROR += (_poleError.duration.ToString() + " s in position: " + _poleError.position.ToString() + ",\r\n");
                _poleError.totalTime += duration;
            }
            else
            {
                _poleError.duration += duration;
                if (position != 0)
                {
                    this.POLE_ERROR = this.POLE_ERROR.Remove(this.POLE_ERROR.TrimEnd().LastIndexOf(Environment.NewLine));
                    this.POLE_ERROR += ("\r\n" + _poleError.duration.ToString() + " s in position: " + _poleError.position.ToString() + ",\r\n");
                }
                _poleError.totalTime += duration;
            }
        }
    }
}
