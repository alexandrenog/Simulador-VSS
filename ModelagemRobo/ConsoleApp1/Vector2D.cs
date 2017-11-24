using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Vector2D
    {
        public float x, y;
        public Vector2D(float x, float y)
        {
            this.x = x; this.y = y;
        }
        public static Vector2D Zero()
        {
            return new Vector2D(0.0f, 0.0f);
        }
        public static Vector2D Create(float x, float y)
        {
            return new Vector2D(x, y);
        }
        public static Vector2D Create(double x, double y)
        {
            return new Vector2D((float)x, (float)y);
        }
        public static Vector2D Create(Microsoft.Xna.Framework.Vector2 other)
        {
            return new Vector2D(other.X, other.Y);
        }
        public Vector2D Add(Vector2D other)
        {
            return new Vector2D(x + other.x, y + other.y);
        }
        public Vector2D Sub(Vector2D other)
        {
            return new Vector2D(x - other.x, y - other.y);
        }
        public Vector2D Mult(float m)
        {
            return new Vector2D(x * m, y * m);
        }
        public float Norm()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
        public float Distance(Vector2D other)
        {
            return Sub(other).Norm();
        }
        public float Angle()
        {
            return (float)Math.Atan2(y, x);
        }
        public void print()
        {
            Console.WriteLine(String.Format("X:{0:F8} - Y:{1:F8} - Norm:{2:F8}", x,y,Norm()));
        }
        public Microsoft.Xna.Framework.Vector2 Vector2()
        {
            return new Microsoft.Xna.Framework.Vector2(x, y);
        }
    }
}
