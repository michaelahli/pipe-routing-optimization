using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace Routing_Optimized_with_GA
{
    class Pipe
    {
        int id, diameter;
        string name;
        int start_x, start_y, goal_x, goal_y;

        //read pipe informations
        public Pipe(IXLWorksheet sheet, Node[,] allnode, int row)
        {
            id = sheet.Cell(row, 1).GetValue<Int32>();
            name = sheet.Cell(row, 2).GetValue<string>();
            diameter = sheet.Cell(row, 3).GetValue<Int32>();

            int _x, _y;

            _x = sheet.Cell(row, 4).GetValue<Int32>();
            _y = sheet.Cell(row, 5).GetValue<Int32>();
            start_x = _x; start_y = _y;
            
            _x = sheet.Cell(row, 6).GetValue<Int32>();
            _y = sheet.Cell(row, 7).GetValue<Int32>();
            goal_x = _x; goal_y = _y;
        }

        int[] penalty = new int[3];

        //penalty[0] is length penalty and penalty[2] is bending penalty
        public void GetPenalty(IXLWorksheet sheet)
        {
            int row = 2;
            while (sheet.Cell(row, 1).GetValue<Int32>() != diameter)
            {
                row++;
            }
            penalty[0] = Convert.ToInt32(sheet.Cell(row, 2).GetValue<Int32>());
            penalty[1] = Convert.ToInt32(penalty[0]);
            penalty[2] = Convert.ToInt32(sheet.Cell(row, 3).GetValue<Int32>());
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public int Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }
        public int Start_X
        {
            get { return start_x; }
            set { start_x = value; }
        }
        public int Goal_X
        {
            get { return goal_x; }
            set { goal_x = value; }
        }
        public int Start_Y
        {
            get { return start_y; }
            set { start_y = value; }
        }
        public int Goal_Y
        {
            get { return goal_y; }
            set { goal_y = value; }
        }
        public int[] Penalty
        {
            get { return penalty; }
            set { penalty = value; }
        }
    }
}
