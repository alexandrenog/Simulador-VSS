using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace ConsoleApp1
{
    class RobotInfo
    {
        public double wr, wl, v, w;
        public RobotInfo(double wr, double wl, double v, double w)
        {
            this.wr = wr; this.wl = wl; this.v = v; this.w = w;
        }
        public void print()
        {
            Console.Write( String.Format("WR[rot/s]: {0:F3}, WL[rot/s]: {1:F3}, V[cm/s]: {2:F3}, W[rot/s]: {3:F3}\n", (wr / (2 * Math.PI)), (wl / (2 * Math.PI)), (v*1e2), (w/(2*Math.PI))) );
        }
    }
    class RobotSavedInfo
    {
        public ArrayList _robotInfo;
        public int max_saved_states;
        public RobotSavedInfo(int mss=0)
        {
            _robotInfo = new ArrayList();
            max_saved_states = Math.Max(mss,0);
        }
        public void save(double wr, double wl, double v, double w)
        {
            _robotInfo.Add(new RobotInfo(wr,wl,v,w));
            if (max_saved_states != 0 && _robotInfo.Count > max_saved_states) {
                _robotInfo.RemoveAt(0);
            }
        }
        public RobotInfo last()
        {
            if (_robotInfo.Count>0)
                return (RobotInfo)_robotInfo[_robotInfo.Count-1];
            return null;
        }
        public ArrayList all()
        {
            return _robotInfo;
        }
        public RobotInfo at(int index)
        {
            return (RobotInfo)((index>=0 && index<_robotInfo.Count) ? (_robotInfo[index]) : (null));
        }
    }
}
