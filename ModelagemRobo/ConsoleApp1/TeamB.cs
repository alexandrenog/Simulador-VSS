using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class TeamB : Team
    {
        public class Ball
        {
            public Vector2D pos, lastPos, nextPos;
            public Ball(Vector2D pos = null)
            {
                this.pos = this.lastPos = this.nextPos = (pos != null) ? (pos) : (Vector2D.Zero());
            }
            public void newPos(Vector2D pos)
            {
                lastPos = this.pos;
                this.pos = pos;
                nextPos = pos.Add(pos.Sub(lastPos));
            }
        }
        public Ball ball;
        public TeamB() : base()
        {
            ball = new Ball();
        }
        public void printTest()
        {
            Console.WriteLine("Teste");
        }
        public Vector2D GoalieIntersection(Robot r)
        {
            Vector2D goal = Vector2D.Create((lado == Lado.Esquerdo) ? (0.1f) : (1.7f - 0.1f), (ball.pos.y < 1.3f / 2) ? (0.45f) : (1.7f - 0.45f));
            Vector2D goalie = r.pos;
            Vector2D where = Vector2D.Zero();
            where.x = (lado == Lado.Esquerdo) ? (0.15f) : (1.7f - 0.15f);
            float a = 999;
            if (goal.x != ball.pos.x)
            {
                a = (ball.pos.y - goal.y) / (ball.pos.x - goal.x);
            }
            float b = goal.y - a * goal.x;
            where.y = a * where.x + b;
            where.y = Math.Max(0.3f, Math.Min(1.3f - 0.3f, where.y));
            if (ball.pos.Distance(goalie) < 0.18)
                return ball.pos;
            return where;
        }
        public Vector2D DefenderIntersection(Robot r)
        {
            Vector2D defender = r.pos;
            Vector2D where = Vector2D.Zero();
            where.x = (lado == Lado.Esquerdo) ? (0.475f) : (1.7f - 0.475f);
            where.y = ball.nextPos.y;
            if (ball.pos.Distance(defender) < 0.18)
                return ball.pos;
            return where;
        }
    }
}
