using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ClosedXML.Excel;

namespace Routing_Optimized_with_GA
{
    class Individual
    {
        Dijkstra dij; List<int> genome = new List<int>(); Random r; int genome_size;
        public Individual(string map_address, string pipe_adress, string penalty_address, Random r)
        {
            dij = new Dijkstra(map_address, pipe_adress, penalty_address);
            foreach(var item in dij.Allpipe)
            {
                genome.Add(item.ID); //extract genome
            }
            this.r = r;
            genome_size = genome.Count;
        }

        int cost, bend, step; List<string> name = new List<string>();
        public void CrossOver(bool diagonalallowance) //mate individual to produce child
        {
            Mutate(genome);
            dij.GetCost(diagonalallowance, genome); 
            cost = dij.Cost;
            bend = dij.Bend;
            step = dij.Step;
            name = dij.Name;
        }

        List<int> child;
        public void CrossOver(Individual otherparent, double mutation_rate) //mate parent with otherparent
        {
            int i = 0; double dice;
            child = new List<int>();
            while (i < genome_size)
            {
                dice = r.NextDouble();
                if(dice < mutation_rate/2)
                {
                    if(child.Contains(genome[i]))
                    {
                        Mutation(child);
                    }
                }
                else if(dice < mutation_rate)
                {
                    if (!child.Contains(otherparent.Genome[i]))
                    {
                        child.Add(otherparent.Genome[i]);
                        i++;
                    }
                }
                else
                {
                    Mutation(child);
                }
            }
            genome.Clear();
            foreach(var item in child)
            {
                genome.Add(item);
            }
        }

        public int Mutation(List<int> list) //performing gen mutation
        {
            List<int> copy = new List<int>();
            for(int i = 1; i <= genome_size; i++)
            {
                copy.Add(i);
            }
            foreach(var item in list)
            {
                copy.Remove(item); //remove item that already exist on child genome
            }
            return copy[r.Next(0, copy.Count - 1)]; //return random available genome
        }

        public void Mutate(List<int> list) //shuffle the child genome
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void GetExcel(IXLWorksheet sheet)
        {
            sheet.Cell(1, 1).Value = "Name";
            sheet.Cell(1, 2).Value = "ID";
            sheet.Cell(1, 3).Value = "Cost";
            sheet.Cell(2, 3).Value = cost;
            sheet.Cell(1, 4).Value = "Bend";
            sheet.Cell(2, 4).Value = bend;
            sheet.Cell(1, 5).Value = "Distance";
            sheet.Cell(2, 5).Value = step;
            for (int i = 0; i < genome_size; i++)
            {
                sheet.Cell(i + 2, 2).Value = genome[i];
            }
            int j = 0;
            foreach(var item in name)
            {
                j++;
                sheet.Cell(j + 1, 1).Value = item;
            }
        }

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        public List<int> Genome
        {
            get { return genome; }
            set { genome = value; }
        }

        public List<int> Child
        {
            get { return child; }
            set { child = value; }
        }

        public int Genome_size
        {
            get { return genome_size; }
            set { genome_size = value; }
        }

        public Dijkstra Dij
        {
            get { return dij; }
            set { dij = value; }
        }
    }
}
