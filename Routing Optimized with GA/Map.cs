using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Drawing;

namespace Routing_Optimized_with_GA
{
    class Map
    {
        int rows, columns;
        Node[,] allnode;
        string exportadress;
        public Map(string map_address)
        {
            exportadress = map_address;
            rows = 0; columns = 0;
            //read map excel
            var wb_map = new XLWorkbook(map_address);
            var sheet = wb_map.Worksheet(1);

            //get row size and column size
            while(sheet.Cell(rows+1,1).IsEmpty() == false)
            {
                rows++;
            }
            while (sheet.Cell(1, columns + 1).IsEmpty() == false)
            {
                columns++;
            }

            //define overall node size
            allnode = new Node[rows, columns];

            //read node type from each node in map excel
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    allnode[i, j] = new Node(j, i, sheet.Cell(i + 1, j + 1).GetValue<Int32>());
                }
            }
        }

        int[][,] BendArray = new int[9][,];
        int[,] gridvalue, gridtype, original_gridtype;

        public void Setup()
        {
            original_gridtype = new int[rows + 1, columns + 1];
            gridtype = new int[rows + 1, columns + 1];
            gridvalue = new int[rows + 1, columns + 1];
            for (int i = 1; i < 9; i++)
            {
                BendArray[i] = new int[rows + 1, columns + 1];
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    original_gridtype[i, j] = allnode[i,j].TYPE;
                    gridtype[i, j] = allnode[i,j].TYPE;
                    gridvalue[i, j] = allnode[i, j].TYPE;
                }
            }
        }

        public void Search(Pipe p1, bool diagonalallowance)
        {
            int xstart = p1.Start_X, xfinish = p1.Goal_X, ystart = p1.Start_Y, yfinish = p1.Goal_Y;
            GetValueFromStartingPoint(xstart,ystart,xfinish,yfinish, diagonalallowance);
        }

        bool target;
        int steps = 0, East = 1, West = 2, North = 3, South = 4, 
            North_East = 4, North_West = 5, South_East = 6, South_West = 8,
            iteration, Penalty_bending = 4, Penalty_crossing = 16, cost;
        
        public void GetValueFromStartingPoint(int xstart, int ystart, int xfinish, int yfinish, bool diagonalallowance)
        {
            clear_gridvalue(); //set every grid's value to infinite

            gridvalue[xstart, ystart] = 0; iteration = 0; steps = 0; target = true;
            BendArray[East][xstart, ystart] = 0; BendArray[West][xstart, ystart] = 0;
            BendArray[North][xstart, ystart] = 0; BendArray[South][xstart, ystart] = 0;
            BendArray[North_East][xstart, ystart] = 0; BendArray[North_West][xstart, ystart] = 0;
            BendArray[South_East][xstart, ystart] = 0; BendArray[South_West][xstart, ystart] = 0;
            DijkstraAlgorithm(xfinish, yfinish);
            ChoosePathandCountCost(xfinish,yfinish, diagonalallowance); //trace from target point
        }

        public void DijkstraAlgorithm(int xfinish, int yfinish)
        {
            while (target)
            {
                for (int i = 1; i <= rows; i++)
                {
                    for (int j = 1; j <= columns; j++)
                    {
                        if (gridvalue[i, j] == (iteration - 1))
                        {
                            {
                                Horizontal_Checking(i, j, xfinish, yfinish);
                                Vertical_Checking(i, j, xfinish, yfinish);
                            }
                        }
                    }
                }
                iteration++;
            }
        }

        public void Horizontal_Checking(int i, int j, int xfinish, int yfinish)
        {
            RightChecking_Value(i, j, xfinish, yfinish);
            LeftChecking_Value(i, j, xfinish, yfinish);
        }

        public void Vertical_Checking(int i, int j, int xfinish, int yfinish)
        {
            UpwardChecking_Value(i, j, xfinish, yfinish);
            DownwardChecking_Value(i, j, xfinish, yfinish);
        }

        public void RightChecking_Value(int i, int j, int xfinish, int yfinish)
        {
            steps = BendArray[East][i, j];
            cost = iteration;

            if (columns - j > 0)
            {
                if (steps == 0 || steps == East){}
                else { cost += Penalty_bending; } //if checking by different from previous direction, add bending penalty value
                if (i == xfinish && j + 1 == yfinish){
                    target = false; } //if checking target point, stop iteration
                //only check grid with TYPE 0, 4 and 5
                else if (gridtype[i, j + 1] == 0 || gridtype[i, j + 1] == 5 || gridtype[i, j + 1] == 4 || gridtype[i, j + 1] == 1)
                {
                    //if crossing another pipe (grid with type 5), add crossing penalty value
                    if (gridtype[i, j + 1] == 1 || gridtype[i, j+1] == 5) { cost += Penalty_crossing; }
                    if (gridvalue[i, j + 1] > cost) //if grid havent been checked
                    {
                        gridvalue[i, j + 1] = cost;
                        BendArray[East][i, j + 1] = East;
                        //other nearest grids also have not been checked
                        BendArray[West][i, j + 1] = Int32.MaxValue;
                        BendArray[North][i, j + 1] = Int32.MaxValue;
                        BendArray[South][i, j + 1] = Int32.MaxValue;
                        BendArray[North_East][i, j + 1] = Int32.MaxValue;
                        BendArray[North_West][i, j + 1] = Int32.MaxValue;
                        BendArray[South_East][i, j + 1] = Int32.MaxValue;
                        BendArray[South_West][i, j + 1] = Int32.MaxValue;
                    }
                    else if (gridvalue[i, j + 1] == cost) //if grid have already been checked
                    {
                        gridvalue[i, j + 1] = cost;
                        BendArray[East][i, j + 1] = East;
                        //other nearest grid also have been checked
                    }
                }
            }
        }

        public void LeftChecking_Value(int i, int j, int xfinish, int yfinish)
        {
            steps = BendArray[West][i, j];
            cost = iteration;
            if (j > 1)
            {
                if (steps == 0 || steps == West) { }
                else { cost += Penalty_bending; }
                if (i == xfinish && j - 1 == yfinish){
                    target = false; }
                else if (gridtype[i, j - 1] == 0 || gridtype[i, j - 1] == 5 || gridtype[i, j - 1] == 4|| gridtype[i, j - 1] == 1)
                {
                    if (gridtype[i, j - 1] == 1 || gridtype[i, j - 1] == 5) { cost += Penalty_crossing; }
                    if (gridvalue[i, j - 1] > cost)
                    {
                        gridvalue[i, j - 1] = cost;
                        BendArray[West][i, j - 1] = West;
                        BendArray[East][i, j - 1] = Int32.MaxValue;
                        BendArray[North][i, j - 1] = Int32.MaxValue;
                        BendArray[South][i, j - 1] = Int32.MaxValue;
                        BendArray[North_East][i, j - 1] = Int32.MaxValue;
                        BendArray[North_West][i, j - 1] = Int32.MaxValue;
                        BendArray[South_East][i, j - 1] = Int32.MaxValue;
                        BendArray[South_West][i, j - 1] = Int32.MaxValue;
                    }
                    else if (gridvalue[i, j - 1] == cost)
                    {
                        gridvalue[i, j - 1] = cost;
                        BendArray[West][i, j - 1] = West;
                    }
                }
            }
        }

        public void UpwardChecking_Value(int i, int j, int xfinish, int yfinish)
        {
            steps = BendArray[North][i, j];
            cost = iteration;
            if (i > 1)
            {
                if (steps == 0 || steps == North) { }
                else { cost += Penalty_bending; }
                if (i - 1 == xfinish && j == yfinish){
                    target = false; }
                else if (gridtype[i - 1, j] == 0 || gridtype[i - 1, j] == 5 || gridtype[i - 1, j] == 4|| gridtype[i - 1, j] == 1)
                {
                    if (gridtype[i - 1, j] == 1 || gridtype[i - 1, j] == 5) { cost += Penalty_crossing; }
                    if (gridvalue[i - 1, j] > cost)
                    {
                        gridvalue[i - 1, j] = cost;
                        BendArray[North][i - 1, j] = North;
                        BendArray[East][i - 1, j] = Int32.MaxValue;
                        BendArray[West][i - 1, j] = Int32.MaxValue;
                        BendArray[South][i - 1, j] = Int32.MaxValue;
                        BendArray[North_East][i - 1, j] = Int32.MaxValue;
                        BendArray[North_West][i - 1, j] = Int32.MaxValue;
                        BendArray[South_East][i - 1, j] = Int32.MaxValue;
                        BendArray[South_West][i - 1, j] = Int32.MaxValue;
                    }
                    else if (gridvalue[i - 1, j] == cost)
                    {
                        gridvalue[i - 1, j] = cost;
                        BendArray[North][i - 1, j] = North;
                    }
                }
            }
        }

        public void DownwardChecking_Value(int i, int j, int xfinish, int yfinish)
        {
            steps = BendArray[South][i, j];
            cost = iteration;
            if (rows - i > 0)
            {
                if (steps == 0 || steps == South) { }
                else { cost += Penalty_bending; }
                if (i + 1 == xfinish && j == yfinish){
                    target = false; }
                else if (gridtype[i + 1, j] == 0 || gridtype[i + 1, j] == 5 || gridtype[i + 1, j] == 4 || gridtype[i + 1, j] == 1)
                {
                    if (gridtype[i + 1, j] == 1 || gridtype[i + 1, j] == 5) { cost += Penalty_crossing; }
                    if (gridvalue[i + 1, j] > cost)
                    {
                        gridvalue[i + 1, j] = cost;
                        BendArray[South][i + 1, j] = South;
                        BendArray[East][i + 1, j] = Int32.MaxValue;
                        BendArray[West][i + 1, j] = Int32.MaxValue;
                        BendArray[North][i + 1, j] = Int32.MaxValue;
                        BendArray[North_East][i + 1, j] = Int32.MaxValue;
                        BendArray[North_West][i + 1, j] = Int32.MaxValue;
                        BendArray[South_East][i + 1, j] = Int32.MaxValue;
                        BendArray[South_West][i + 1, j] = Int32.MaxValue;
                    }
                    else if (gridvalue[i + 1, j] == cost)
                    {
                        gridvalue[i + 1, j] = cost;
                        BendArray[South][i + 1, j] = South;
                    }
                }
            }
        }

        int row_position, column_position, check_East, check_West, check_North, check_South,
            check_NorthEast, check_NorthWest, check_SouthEast, check_SouthWest,
            crossing, bending, previous_path, chosen_path;

        public void ChoosePathandCountCost(int xfinish, int yfinish, bool diagonalallowance)
        {
            row_position = xfinish; column_position = yfinish;
            crossing = 0; bending = 0; steps = 1; target = false;
            
            while (target == false)
            {
                //if we reach the starting point, stop iteration
                if (gridvalue[row_position, column_position] == 0){ target = true; }
                else
                {   
                    GetDirectionValue_FromTarget(diagonalallowance);
                    ChoosePath_FromTarget(diagonalallowance);
                    CountCost(diagonalallowance);
                    previous_path = chosen_path;
                    steps++;
                }
            }

        }

        
        public void GetDirectionValue_FromTarget(bool diagonalallowance)
        {
            check_East = -100; check_West = -100; check_North = -100; check_South = -100; //set check value to max
            if (columns - row_position > 0) //Check East
            {
                //if gridvalue of the nearest grid from target point is higher than the target point's gridvalue itself
                check_East = gridvalue[row_position, column_position] - gridvalue[row_position, column_position + 1];
                if (check_East < 0)
                {
                    check_East = -100; //we don't choose that point
                }
                //add penalty bending
                if (BendArray[West][row_position, column_position] != previous_path)
                {
                    check_East -= Penalty_bending;
                }
                //add penalty crossing
                if (gridtype[row_position, column_position + 1] == 5 || gridtype[row_position, column_position + 1] == 1)
                {
                    check_East -= Penalty_crossing;
                }
            }
            if (column_position > 1) //Check West
            {
                check_West = gridvalue[row_position, column_position] - gridvalue[row_position, column_position - 1];
                if (check_West < 0)
                {
                    check_West = -100;
                }
                if (BendArray[East][row_position, column_position] != previous_path)
                {
                    check_West -= Penalty_bending;
                }
                if (gridtype[row_position, column_position - 1] == 5 || gridtype[row_position, column_position - 1] == 1)
                {
                    check_West -= Penalty_crossing;
                }
            }
            if (row_position > 1) //Check North
            {
                check_North = gridvalue[row_position, column_position] - gridvalue[row_position - 1, column_position];
                if (check_North < 0)
                {
                    check_North = -100;
                }
                if (BendArray[South][row_position, column_position] != previous_path)
                {
                    check_North -= Penalty_bending;
                }
                if (gridtype[row_position - 1, column_position] == 5 || gridtype[row_position - 1, column_position] == 1)
                {
                    check_North -= Penalty_crossing;
                }
            }
            if (rows - row_position > 0) //Check South
            {
                check_South = gridvalue[row_position, column_position] - gridvalue[row_position + 1, column_position];
                if (check_South < 0)
                {
                    check_South = -100;
                }
                if (BendArray[North][row_position, column_position] != previous_path)
                {
                    check_South -= Penalty_bending;
                }
                if (gridtype[row_position + 1, column_position] == 5 || gridtype[row_position + 1, column_position] == 1)
                {
                    check_South -= Penalty_crossing;
                }
            }
            if(diagonalallowance == true)
            {
                checkDiagonal();
            }
        }

        public void checkDiagonal()
        {
            if (columns - column_position > 0 && row_position > 1) //Check North East
            {
                check_NorthEast = gridvalue[row_position, column_position] - gridvalue[row_position - 1, column_position + 1];
                if (check_South < 0)
                {
                    check_NorthEast = -100;
                }
                if (BendArray[North_East][row_position, column_position] != previous_path)
                {
                    check_NorthEast -= Penalty_bending;
                }
                if (gridtype[row_position - 1, column_position + 1] == 5 || gridtype[row_position - 1, column_position + 1] == 1)
                {
                    check_NorthEast -= Penalty_crossing;
                }
            }
            if (column_position > 1 && row_position > 1) //Check North West
            {
                check_NorthWest = gridvalue[row_position, column_position] - gridvalue[row_position - 1, column_position - 1];
                if (check_NorthWest < 0)
                {
                    check_NorthWest = -100;
                }
                if (BendArray[North_West][row_position, column_position] != previous_path)
                {
                    check_NorthWest -= Penalty_bending;
                }
                if (gridtype[row_position - 1, column_position - 1] == 5 || gridtype[row_position - 1, column_position - 1] == 1)
                {
                    check_NorthWest -= Penalty_crossing;
                }
            }
            if (columns - column_position > 0 && rows - row_position > 0) //Check South East
            {
                check_SouthEast = gridvalue[row_position, column_position] - gridvalue[row_position + 1, column_position + 1];
                if (check_SouthEast < 0)
                {
                    check_SouthEast = -100;
                }
                if (BendArray[South_East][row_position, column_position] != previous_path)
                {
                    check_SouthEast -= Penalty_bending;
                }
                if (gridtype[row_position + 1, column_position + 1] == 5 || gridtype[row_position + 1, column_position + 1] == 1)
                {
                    check_SouthEast -= Penalty_crossing;
                }
            }
            if (column_position > 1 && rows - row_position > 0) //Check South West
            {
                check_SouthWest = gridvalue[row_position, column_position] - gridvalue[row_position + 1, column_position - 1];
                if (check_SouthWest < 0)
                {
                    check_SouthWest = -100;
                }
                if (BendArray[South_West][row_position, column_position] != previous_path)
                {
                    check_SouthWest -= Penalty_bending;
                }
                if (gridtype[row_position + 1, column_position - 1] == 5 || gridtype[row_position + 1, column_position - 1] == 5)
                {
                    check_SouthWest -= Penalty_crossing;
                }
            }
        }

        int temporaryValue;
        public void ChoosePath_FromTarget(bool diagonalallowance)
        {
            temporaryValue = -1000;
            //minimum temporaryValue is the minimum gridvalue from checkdirection
            ChooseMinimumtemporaryValue(diagonalallowance);
            //choose the direction itself
            if (temporaryValue == check_East)
            {
                chosen_path = East;
            }
            if (temporaryValue == check_West)
            {
                chosen_path = West;
            }
            if (temporaryValue == check_North)
            {
                chosen_path = North;
            }
            if (temporaryValue == check_South)
            {
                chosen_path = South;
            }
            if(diagonalallowance == true)
            {
                chooseDiagonal();
            }
            
        }

        public void chooseDiagonal()
        {
            if (temporaryValue == check_NorthEast)
            {
                chosen_path = North_East;
            }
            if (temporaryValue == check_NorthWest)
            {
                chosen_path = North_West;
            }
            if (temporaryValue == check_SouthEast)
            {
                chosen_path = South_East;
            }
            if (temporaryValue == check_SouthWest)
            {
                chosen_path = South_West;
            }
        }

        //Searching for the minimum value
        public void ChooseMinimumtemporaryValue(bool diagonalallowance)
        {
            if (temporaryValue < check_East)
            {
                temporaryValue = check_East;
            }
            if (temporaryValue < check_West)
            {
                temporaryValue = check_West;
            }
            if (temporaryValue < check_North)
            {
                temporaryValue = check_North;
            }
            if (temporaryValue < check_South)
            {
                temporaryValue = check_South;
            }
            if(diagonalallowance == true)
            {
                FindDiagonalMinValue();
            }
        }

        public void FindDiagonalMinValue()
        {
            if (temporaryValue < check_NorthEast)
            {
                temporaryValue = check_NorthEast;
            }
            if (temporaryValue < check_NorthWest)
            {
                temporaryValue = check_NorthWest;
            }
            if (temporaryValue < check_SouthEast)
            {
                temporaryValue = check_SouthEast;
            }
            if (temporaryValue < check_SouthWest)
            {
                temporaryValue = check_SouthWest;
            }
        }

        public void CountCost(bool diagonalallowance)
        {
            if (chosen_path == East)
            {
                //Count bending and steps
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                //Count crossing and steps
                if (gridtype[row_position, column_position + 1] == 5)
                {
                    crossing++; steps++;
                }
                //Mark the node
                if (gridvalue[row_position, column_position + 1] != 0)
                {
                    gridtype[row_position, column_position + 1] = 5;
                    allnode[row_position, column_position + 1].TYPE = 3;
                }
                column_position++; //move position
            }
            else if (chosen_path == West)
            {
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                if (gridtype[row_position, column_position - 1] == 5)
                {
                    crossing++; steps++;
                }
                if (gridvalue[row_position, column_position - 1] != 0)
                {
                    gridtype[row_position, column_position - 1] = 5;
                    allnode[row_position, column_position - 1].TYPE = 3;
                }
                column_position--;
            }
            else if (chosen_path == North)
            {
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                if (gridtype[row_position - 1, column_position] == 5)
                {
                    crossing++; steps++;
                }
                if (gridvalue[row_position - 1, column_position] != 0)
                {
                    gridtype[row_position - 1, column_position] = 5;
                    allnode[row_position - 1, column_position].TYPE = 3;
                }
                row_position--;
            }
            else if (chosen_path == South)
            {
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                if (gridtype[row_position + 1, column_position] == 5)
                {
                    crossing++; steps++;
                }
                if (gridvalue[row_position + 1, column_position] != 0)
                {
                    gridtype[row_position + 1, column_position] = 5;
                    allnode[row_position + 1, column_position].TYPE = 3;
                }
                row_position++;
            }
            else
            {
                if(diagonalallowance == true)
                {
                    allowDiagonal();
                }
            }
        }

        public void allowDiagonal()
        {
            if (chosen_path == North_East)
            {
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                if (gridtype[row_position - 1, column_position + 1] == 5)
                {
                    crossing++; steps++;
                }
                if (gridvalue[row_position - 1, column_position + 1] != 0)
                {
                    gridtype[row_position - 1, column_position + 1] = 5;
                    allnode[row_position - 1, column_position + 1].TYPE = 3;
                }
                column_position++;
                row_position--;
            }
            else if (chosen_path == North_West)
            {
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                if (gridtype[row_position - 1, column_position - 1] == 5)
                {
                    crossing++; steps++;
                }
                if (gridvalue[row_position - 1, column_position - 1] != 0)
                {
                    gridtype[row_position - 1, column_position - 1] = 5;
                    allnode[row_position - 1, column_position - 1].TYPE = 3;
                }
                column_position--;
                row_position--;
            }
            else if (chosen_path == South_East)
            {
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                if (gridtype[row_position + 1, column_position + 1] == 5)
                {
                    crossing++; steps++;
                }
                if (gridvalue[row_position + 1, column_position + 1] != 0)
                {
                    gridtype[row_position + 1, column_position + 1] = 5;
                    allnode[row_position + 1, column_position + 1].TYPE = 3;
                }
                column_position++;
                row_position++;
            }
            else if (chosen_path == South_West)
            {
                if (previous_path != chosen_path)
                {
                    bending++; steps++;
                }
                if (gridtype[row_position + 1, column_position - 1] == 5)
                {
                    crossing++; steps++;
                }
                if (gridvalue[row_position + 1, column_position - 1] != 0)
                {
                    gridtype[row_position + 1, column_position - 1] = 5;
                    allnode[row_position + 1, column_position - 1].TYPE = 3;
                }
                column_position--;
                row_position++;
            }
        }

        public void clear_gridvalue()
        {
            for (int i = 0; i <= rows; i++)
            {
                for (int j = 0; j <= columns; j++)
                {
                    gridvalue[i, j] = Int32.MaxValue;
                    for (int l = 1; l < 9; l++)
                    {
                        BendArray[l][i, j] = Int32.MaxValue;
                    }
                }
            }
        }

        public void clearPath()
        {
            for (int i = 0; i <= rows; i++)
            {
                for (int j = 0; j <= columns; j++)
                {
                    //set the gridtype to original (earase the marked node for pipe)
                    gridtype[i, j] = original_gridtype[i, j];
                    if(allnode[i,j].TYPE == 3)
                    {
                        allnode[i, j].TYPE = original_gridtype[i,j]; 
                    }
                }
            }
        }

        //fill each node as rectangle with color according to each type
        public void DrawNodes(Graphics gra)
        {
            for(int i = 0; i <rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    switch (allnode[i, j].TYPE)
                    {
                        case 0:
                            allnode[i, j].DrawNode(gra, Brushes.GreenYellow);
                            break;
                        case 1:
                            allnode[i, j].DrawNode(gra, Brushes.LightBlue);
                            break;
                        case 2:
                            allnode[i, j].DrawNode(gra, Brushes.DimGray);
                            break;
                        case 3:
                            allnode[i, j].DrawNode(gra, Brushes.LightPink);
                            break;
                        case 4:
                            allnode[i, j].DrawNode(gra, Brushes.LightGoldenrodYellow);
                            break;
                        default:
                            allnode[i, j].DrawNode(gra, Brushes.Gainsboro);
                            break;
                    }
                }
            }
        }
        
        //i want to check pipe simulation drawing on picturebox1
        public void TestChangeNodeType()
        {
            for(int i = 9; i < 19; i++)
            {
                allnode[i, 19].TYPE = 3;
            }
        }

        //i want to check target nodes
        public void TestDrawGoals(List<Pipe> pipes)
        {
            foreach(var item in pipes)
            {
                allnode[item.Goal_X, item.Goal_Y].TYPE = 3;
            }
            foreach(var item in pipes)
            {
                allnode[item.Start_X, item.Start_Y].TYPE = 2;
            }
        }

        public Node[,] Allnode
        {
            get { return allnode; }
            set { allnode = value; }
        }

        public int Steps
        {
            get { return steps; }
            set { steps = value; }
        }

        public int Bending
        {
            get { return bending; }
            set { bending = value; }
        }

        public int Crossing
        {
            get { return crossing; }
            set { crossing = value; }
        }
    }
}
