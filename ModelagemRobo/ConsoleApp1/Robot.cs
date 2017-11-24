using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleApp1
{
    class Robot
    {
        private static double tick_size = 30; //[millisecond]
        private Timer timer;
        private double r, l, k, wr, wl;
        public readonly double max_motor_vel, max_angular_vel;
        public double orientation, wr_ref, wl_ref;
        public Vector2D pos;
        public RobotSavedInfo pastInfo;
        public Func<Vector2D,Robot,bool,bool> move_to_pos;
        public Robot(double r, double l, double k, double max_motor_vel, double max_angular_vel)
        {
            this.r = r;
            this.l = l;
            this.k = k;
            this.max_motor_vel = max_motor_vel; // maximum angular velocity of the motors
            this.max_angular_vel = max_angular_vel; // maximum angular velocity of the robot
            wr = wl = 0.0;
            pos = Vector2D.Zero();
            orientation = 0.0;
            pastInfo = new RobotSavedInfo(50);
            timer = new Timer(tick_size);
            timer.Elapsed += HandleTimer;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        private void HandleTimer(Object source, ElapsedEventArgs e) {
            double d_wr = (1 / k) * (wr_ref - wr); // [radians/second^2]
            double d_wl = (1 / k) * (wl_ref - wl); // [radians/second^2]
            wr = Math.Max(-max_motor_vel, Math.Min(max_motor_vel, wr + d_wr * tick_size / 1000.0) );
            wl = Math.Max(-max_motor_vel, Math.Min(max_motor_vel, wl + d_wl * tick_size / 1000.0) );
            pastInfo.save(wr,wl,linear_velocity(),angular_velocity());
            //Console.WriteLine(pastInfo.all().Count);
            //pastInfo.last().print();
        }
        public double radius() {
            return r;
        }
        public double length()
        {
            return l;
        }
        public float linear_velocity() {
            return (float)((wr + wl) * (r / 2.0));
        }
        public float angular_velocity()
        {
            return (float)Math.Max(-max_angular_vel, Math.Min(max_angular_vel,(wr - wl) * (1.0 / l)));
            //return (wr - wl) * (1.0 / l);
        }
    }
}
