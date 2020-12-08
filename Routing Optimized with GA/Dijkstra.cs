using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ClosedXML.Excel;
using System.Windows.Forms;

namespace Routing_Optimized_with_GA
{
    class Dijkstra
    {
        Map network;
        List<Pipe> allpipe = new List<Pipe>();

        //read input
        public Dijkstra(string map_address, string pipe_adress, string penalty_address)
        {
            network = new Map(map_address);
            var wb = new XLWorkbook(pipe_adress);
            var sheet = wb.Worksheet(1);
            int row = 0;
            while (sheet.Cell(row + 2, 2).IsEmpty() == false)
            {
                allpipe.Add(new Pipe(sheet, network.Allnode, row+2));
                row++;
            }
            var wb2 = new XLWorkbook(penalty_address);
            var sheet2 = wb2.Worksheet(1);
            foreach(var item in allpipe)
            {
                item.GetPenalty(sheet2);
            }
        }

        public void Search(Graphics gra, ListBox lb, bool diagonalallowance)
        {
            lb.Items.Add("Name \t Steps \t Bending \t Cost");
            network.DrawNodes(gra); //draw map
            network.Setup();
            foreach (var item in allpipe.OrderByDescending(x => x.Diameter))
            {
                network.Search(item, diagonalallowance); //Searching for Path
                network.DrawNodes(gra); //Drawing path
                ResultItem(lb, item.Name, network.Steps, network.Bending, item.Penalty[0], item.Penalty[2]);
            }
            lb.Items.Add("Total Cost : " + cost_total);
        }

        public void Search(Graphics gra, ListBox lb, bool diagonalallowance, string order)
        {
            var wb = new XLWorkbook(order);
            var sheet = wb.Worksheet(1);
            Pipe p1;
            lb.Items.Add("Name \t Steps \t Bending \t Cost");
            network.DrawNodes(gra); //draw map
            network.Setup();
            int i = 2;
            while(sheet.Cell(i,2).IsEmpty() == false)
            {
                p1 = allpipe.Find(x => x.ID == sheet.Cell(i, 2).GetValue<Int32>());
                network.Search(p1, diagonalallowance); //Searching for Path
                network.DrawNodes(gra); //Drawing path
                ResultItem(lb, p1.Name, network.Steps, network.Bending, p1.Penalty[0], p1.Penalty[2]);
                i++;
            }
            lb.Items.Add("Total Cost : " + cost_total);
        }

        int TotalCost, step, bend; List<string> name = new List<string>();
        public void GetCost(bool diagonalallowance, List<int> genome)
        {
            TotalCost = 0;
            network.Setup();
            for(int i = 0; i < allpipe.Count; i++)
            {
                foreach (var item in allpipe.Where(x => x.ID == genome[i]))
                {
                    network.Search(item, diagonalallowance); //Searching for Path
                    //network.DrawNodes(gra); 
                    cost = network.Steps * item.Penalty[0] + network.Bending * item.Penalty[2];
                    TotalCost += cost;
                    step += network.Steps;
                    bend += network.Bending;
                    name.Add(item.Name);
                }
            }
        }

        int cost; int cost_total;
        public void ResultItem(ListBox lb, string name, int steps, int bending, int step_penalty, int bending_penalty)
        {
            cost = steps * step_penalty + bending * bending_penalty;
            lb.Items.Add(name + "\t" + steps + "\t" + bending + "\t" + cost);
            cost_total += cost;
        }

        //i want to check wheter my program read starts goals point correctly or not
        public void CheckItem(ListBox list)
        {
            List<string> names = new List<string>();
            foreach(var item in allpipe)
            {
                if(names.Find(x => x == item.Name) == null)
                {
                    names.Add(item.Name);
                }
            }
            foreach(var item in names)
            {
                int start_x = 0, start_y = 0, goal_x = 0, goal_y = 0;
                foreach(var items in allpipe.Where(x=>x.Name == item))
                {
                    start_x = items.Start_X;
                    start_y = items.Start_Y;
                    goal_x = items.Goal_X;
                    goal_y = items.Goal_Y;
                    list.Items.Add(item + ":" + "\t sx" + start_x.ToString() + "\t sy" + start_y.ToString() + "\t gx" + goal_x.ToString() + "\t gy" + goal_y.ToString());
                }
            }

            
        }

        public List<Pipe> Allpipe
        {
            get { return allpipe; }
            set { allpipe = value; }
        }

        public int Cost
        {
            get { return TotalCost; }
            set { TotalCost = value; }
        }

        public int Bend
        {
            get { return bend; }
            set { bend = value; }
        }

        public int Step
        {
            get { return step; }
            set { step = value; }
        }

        public List<string> Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
