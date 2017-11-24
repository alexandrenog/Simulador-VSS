using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleApp1
{
    class Environment
    {
        private static double tick_size = 30; //[millisecond]
        private Timer timer; 
        public Team teamA, teamB;
        public Vector2D ballPos;
        public Environment()
        {
            ballPos = Vector2D.Create(0, 0);
            teamA = new TeamA();
            teamB = new TeamB();
            set_strategiesA();
            set_strategiesB();
            timer = new Timer(tick_size);
            timer.Elapsed += HandleTimer;
            timer.AutoReset = true;
            timer.Enabled = true;
            teamA.lado = Team.Lado.Direito;
            teamB.lado = Team.Lado.Esquerdo;
        }
        private void HandleTimer(Object source, ElapsedEventArgs e)
        {
            teamA.group_strategy?.Invoke(teamA);
            teamB.group_strategy?.Invoke(teamB);
            // Strategy Execution
            for (int i = 0; i < 3; i++)
            {
                teamA.strategies[i]?.Invoke(teamA, teamA.robots[i]);
                teamB.strategies[i]?.Invoke(teamB, teamB.robots[i]);
            }
        }
        public void set_strategiesA()
        {
            foreach (var robot in teamA.robots)
            {
                robot.move_to_pos = Positioning_Function;
            }
            teamA.group_strategy = (Team t) =>
            {
                t.strategies[2]?.Invoke(t, t.robots[2]);
                var ta = (TeamA)t;
                ta.ball.newPos(ballPos);
                if (ta.robots[0].pos.Distance(ballPos) < ta.robots[1].pos.Distance(ballPos) )
                {
                    ta.strategy_index_1 = 0;
                    ta.strategy_index_2 = 1;
                }
                else
                {
                    ta.strategy_index_1 = 1;
                    ta.strategy_index_2 = 0;
                }
                t.strategies[ta.strategy_index_1]?.Invoke(t, t.robots[0]);
                t.strategies[ta.strategy_index_2]?.Invoke(t, t.robots[1]);
                return true;
            };
            teamA.strategies[0] = (Team t, Robot self) =>
            {
                self.move_to_pos(ballPos, self, false);
                return true;
            };
            teamA.strategies[1] = (Team t, Robot self) =>
            {
                var ta = (TeamA)t;
                //if (ta.ball.pos.Distance(self.pos) < 0.20)
                //    self.move_to_pos(ballPos, self, false);
                //else
                    self.move_to_pos(ta.DefenderIntersection(self), self, true);
                return true;
            };
            teamA.strategies[2] = (Team t, Robot self) =>
            {
                var ta = (TeamA)t;
                if (ta.ball.pos.Distance(self.pos) < 0.40)
                    self.move_to_pos(ballPos, self, false);
                else
                    self.move_to_pos(ta.GoalieIntersection(self), self, true);
                return true;
            };
        }
        public void set_strategiesB()
        {
            foreach (var robot in teamB.robots)
            {
                robot.move_to_pos = Positioning_Function;
            }
            teamB.group_strategy = (Team t) =>
            {
                for (int i = 0; i < 3; i++)
                {
                    t.strategies[i]?.Invoke(t, t.robots[i]);
                }
                var tb = (TeamB)t;
                tb.ball.newPos(ballPos);
                if (tb.robots[0].pos.Distance(ballPos) > tb.robots[1].pos.Distance(ballPos))
                {
                    var temp = tb.strategies[1];
                    tb.strategies[1] = tb.strategies[0];
                    tb.strategies[0] = temp;
                }
                return true;
            };
            teamB.strategies[0] = (Team t, Robot self) =>
            {
                self.move_to_pos(ballPos, self,false);
                return true;
            };
            teamB.strategies[1] = (Team t, Robot self) =>
            {
                var tb = (TeamB)t;
                self.move_to_pos(tb.DefenderIntersection(self), self, true);
                return true;
            };
            teamB.strategies[2] = (Team t, Robot self) =>
            {
                var tb = (TeamB)t;
                if (tb.ball.pos.Distance(self.pos) < 0.20)
                    self.move_to_pos(ballPos, self, false);
                else
                    self.move_to_pos(tb.GoalieIntersection(self), self, true);
                return true;
            };
        }

        public static bool Positioning_Function(Vector2D dest, Robot r, bool perpendicular)
        {
            Vector2D delta_pos = dest.Sub(r.pos);
            //delta_pos.print();
            if (delta_pos.Norm() < 0.003) // MIN Distance
            {
                r.wr_ref = 0;
                r.wl_ref = 0;
            }
            else
            {
                double alfa = delta_pos.Angle();
                double teta = alfa - r.orientation;
                double velLinear = r.max_motor_vel * r.radius() * Math.Cos(teta);
                double velAngular = r.max_angular_vel * Math.Sin(teta);
                if (perpendicular)
                {
                    velAngular = r.max_angular_vel * Math.Sin(teta * 2.0);
                }
                velAngular *= (Math.Abs(teta) < Math.PI / 2) ? (1) : (-1);
                r.wr_ref = (velLinear / r.radius()) + (velAngular * r.length() / 2.0);
                r.wl_ref = (velLinear / r.radius()) - (velAngular * r.length() / 2.0);
                //Console.WriteLine(String.Format("Esq:{0:F3} Dir:{1:F3}", r.wl_ref, r.wr_ref));
            }
            return true;
        }
    }
}
