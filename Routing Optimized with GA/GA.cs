using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClosedXML.Excel;

namespace Routing_Optimized_with_GA
{
    class GA
    {
        List<Individual> population = new List<Individual>();
        private int population_size, number_of_generation;
        private string map_address, pipe_address, penalty_address;
        
        //store input
        public GA(string map_address, string pipe_adress, string penalty_address, int population_size, int number_of_generation)
        {
            this.population_size = population_size;
            this.number_of_generation = number_of_generation;
            this.map_address = map_address;
            this.pipe_address = pipe_adress;
            this.penalty_address = penalty_address;
        }

        public void GeneticAlgorithm(string map_address, string pipe_adress, string penalty_address, Graphics gra, ListBox lb, Random r)
        {
            Individual ind; int i = 0;
            while(i < population_size)
            {
                ind = new Individual(map_address, pipe_adress, penalty_address, r); //create new individual
                try
                {
                    ind.CrossOver(false);
                    i++;
                    population.Add(ind);
                }
                catch
                {
                    //skip error
                }
            }
            SortPopulation(population);
        }

        public void SortPopulation(List<Individual> list) //sort population from the fittest
        {
            List<Individual> temp = new List<Individual>();
            foreach(var item in list.OrderBy(x=>x.Cost))
            {
                temp.Add(item);
            }
            list.Clear();
            foreach(var item in temp)
            {
                list.Add(item);
            }
        }

        List<Individual> best_individual = new List<Individual>();
        public void Run(Graphics gra, ListBox lb, Random r, ProgressBar pb, Chart chart, string name)
        {
            chart.Series["Fitness"].Points.Clear(); //clear chart
            pb.Minimum = 0; pb.Maximum = number_of_generation; pb.Value = 1; //clear progressbar
            
            
            for(int k = 0; k < number_of_generation; k++)
            {
                GeneticAlgorithm(map_address,pipe_address,penalty_address, gra, lb, r); //perform algorithm
                int kaccent = k;
                best_individual.Add(population.ElementAt(ElementPosition(population_size, kaccent))); //best individual
                SortPopulation(best_individual); //sort best individual
                lb.Items.Add("Fitness Gen " + (k+1) + " : \t" + best_individual.ElementAt(0).Cost); //print information
                for (int i = 0; i < 100 * population.Count; i++)
                {
                    population.RemoveAt(population.Count - 1); //remove overpopulation
                }
                chart.ChartAreas[0].AxisY.Minimum = best_individual.ElementAt(0).Cost - 1000; //minimum y axis in chart area
                chart.Series["Fitness"].Points.AddXY(k + 1, best_individual.ElementAt(0).Cost); //plot
                pb.Value = k + 1; //progress
;           }
            GetExcel(name, best_individual);
        }

        public void GetExcel(string name, List<Individual> best)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("sheet");
            best.ElementAt(0).GetExcel(worksheet);
            workbook.SaveAs(name + "\\" + "Optimized Sequence" + ".xlsx");
        }

        public int ElementPosition(int populationsize, int k)
        {
            int position = 0;
            k = k + 1;
            if (k >= populationsize)
            {
                k = populationsize - 1;
            }
            position = populationsize - k;
            return position;

        }

        public void traditionalGA(List<Individual> new_generation, int mutation_percentage, Random r)
        {
            double mutation_rate = (100 - Convert.ToDouble(mutation_percentage)) / 100;
            for (int j = 0; j < 3; j++)
            {
                new_generation.Add(population[j]);
            }
            for (int j = 3; j < population_size; j++)
            {
                Individual parent1 = population[r.Next(3, population_size)];
                Individual parent2 = population[r.Next(0, 2)];
                parent1.CrossOver(parent2, mutation_rate);
                new_generation.Add(parent1);
            }
            population.Clear();
            foreach (var item in new_generation)
            {
                population.Add(item);
            }
            SortPopulation(population);
            new_generation.Clear();
        }

    }
}
