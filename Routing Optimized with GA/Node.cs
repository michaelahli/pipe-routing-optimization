using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Routing_Optimized_with_GA
{
    class Node
    {
        private int x, y, type;
        public Node(int x, int y, int type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }

        int size = 9;
        public void DrawNode(Graphics gra, Brush bru)
        {
            gra.FillRectangle(bru, x * size, y * size, size, size);
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int TYPE
        {
            get { return type; }
            set { type = value; }
        }

    }
}
