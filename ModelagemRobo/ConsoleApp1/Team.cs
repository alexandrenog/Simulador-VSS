using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Team
    {
        public readonly static double wheel_radius = 2e-2, wheels_distance = 7.5e-2; // 2 [cm] and 7.5 [cm]
        public readonly static double ball_diameter = 4.27e-2; // 4.27 [cm]
        public readonly static double motor_accel_const = 8f/30f; // 0.14 [second] 
        public readonly static double max_motor_angular_velocity = 6 * Math.PI; // 6pi [radians/second] or 3,0 [rot/s]
        public readonly static double max_robot_angular_velocity = 4 * Math.PI; // 4pi [radians/second] or 2,0 [rot/s]
        public Robot[] robots;
        public Func<Team,Robot,bool>[] strategies;
        public Func<Team, bool> group_strategy;
        public enum Lado { Esquerdo, Direito };
        public Lado lado;
        public Team()
        {
            robots = new Robot[3];
            strategies = new Func<Team,Robot,bool>[3];
           
            for (int i = 0; i < 3; i++)
            {
                robots[i]= new Robot(wheel_radius, wheels_distance, motor_accel_const,
                    max_motor_angular_velocity, max_robot_angular_velocity);
            }
        }
    }
}
