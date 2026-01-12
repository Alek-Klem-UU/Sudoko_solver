using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// The class that solves a sudoku
/// </summary>
public class Solver
{
    private Random random = new Random();

    //The old unsolved sudoku
    private int[,] baseSudoko;
    private int[,] sudoko;

    // The scores of the rows and columns
    private int[] rowScores = new int[9];
    private int[] collomScores = new int[9];

    //the array of all the positions that CAN swap
    private List<(int, int)>[] swappable = new List<(int, int)>[9];
    // an array containing all possible pairs of the array above
    private List<(int, int)[]>[] swapPairs = new List<(int, int)[]>[9];

    /// <summary>
    /// Initialize the class
    /// </summary>
    /// <param name="sudoko"> the unsolved sudoku </param>
    public Solver(int[,] sudoko)
    {
        this.baseSudoko = (int[,])sudoko.Clone();
        this.sudoko = sudoko;

        // Initialize all values
        Initalize();
    }

    /// <summary>
    /// A function that you can call a single time to initialize everything
    /// Fills al empty spots, sets all swap pairs, and sets the initial scores
    /// </summary>
    public void Initalize()
    {
        FillAllCells();
        SetAllSwapPairs();
        SetAllScores();
    }

    /// <summary>
    /// You can call this  method outside of the class to solve the sudoku
    /// </summary>
    /// <param name="debug"> Print or not to print the sudokus </param>
    /// <param name="S"> S: number of random switches when in a local minimum </param>
    /// <param name="minimaThreshold"> Variable that decides when S should be employed </param>
    /// <returns> Number of iterations it took to get to a solution  </returns>
    public int solve(bool debug, int S, int minimaThreshold)
    {
        if (debug) Debug.PrintSudokoFancy(baseSudoko, sudoko, rowScores, collomScores);

        int iterations = Solve(minimaThreshold, S);

        if (debug) Console.WriteLine("\n\n");
        if (debug) Console.WriteLine($"Got the following in {iterations} iterations\n");
        if (debug) Debug.PrintSudokoFancy(baseSudoko, sudoko, rowScores, collomScores);
        if (debug) Console.WriteLine("...............................................................................\n\n\n");

        return iterations;
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// FIlls all empty cells with random values 
    /// </summary>
    private void FillAllCells()
    {
        FillCell(0, 0);
        FillCell(1, 0);
        FillCell(2, 0);
        FillCell(0, 1);
        FillCell(1, 1);
        FillCell(2, 1);
        FillCell(0, 2);
        FillCell(1, 2);
        FillCell(2, 2);
    }

    /// <summary>
    /// Fills all empty values in a cell with random values 
    /// that have not occured in that cell. All positions of the new 
    /// values are added to a swappable list in the format:
    ///  
    /// - (int x, int y) <- location WITHIN a cell
    /// 
    /// </summary>
    /// <param name="x"> x position cel - x in (0 1 2) </param>
    /// <param name="y"> y position cel - y in (0 1 2) </param>
    private void FillCell(int x, int y)
    {
        int cellIndex = y * 3 + x;
        swappable[cellIndex] = new List<(int, int)>();

        List<int> possible = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        //Remove all values that are already in the cell
        for (int collom = y * 3; collom < (y + 1) * 3; collom++)
        {
            for (int row = x * 3; row < (x + 1) * 3; row++)
            {
                possible.Remove(sudoko[row, collom]);
            }
        }

        // Add the leftover values in the empty spaces in the cell 
        // and add the position (x, y) to swappable
        for (int collom = y * 3; collom < (y + 1) * 3; collom++)
        {
            for (int row = x * 3; row < (x + 1) * 3; row++)
            {
                if (sudoko[row, collom] == 0)
                {
                    swappable[cellIndex].Add((row, collom));

                    sudoko[row, collom] = possible[0];
                    possible.RemoveAt(0);
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// A function that sets all of the swap pairs for possible cells
    /// </summary>
    private void SetAllSwapPairs()
    {
        for (int i = 0; i < 9; i++)
        {
            SetSwapPairs(i);
        }
    }

    /// <summary>
    /// A function that looks at the possible swap positions within a cell
    /// and makes all possible unique pairs between positions and adds them 
    /// to a global variable swappablePairs
    /// </summary>
    /// <param name="cell"> The selected cell (0 t/m 8) </param>
    private void SetSwapPairs(int cell)
    {

        List<(int, int)[]> swaps = new List<(int, int)[]>();
        int swapCount = swappable[cell].Count;
        // Going through the array in this way
        // ensures unique pairs only

        for (int i = 0; i < swapCount - 1; i++)
        {
            for (int i2 = i + 1; i2 < swapCount; i2++)
            {
                (int, int)[] pair = new (int, int)[2];
                pair[0] = swappable[cell][i];
                pair[1] = swappable[cell][i2];
                swaps.Add(pair);
            }
        }
        swapPairs[cell] = swaps;
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// A function that updates all scores
    /// </summary>
    private void SetAllScores()
    {
        for (int i = 0; i < 9; i++)
        {
            rowScores[i] = GetRowScore(i);
            collomScores[i] = GetCollomScore(i);
        }

    }

    /// <summary>
    /// A function that returns the score of one column. The score is equal
    /// to the number of missing numbers within the range 1 to 9 in the column

    /// </summary>
    /// <param name="collom"> the index of the column </param>
    /// <returns> the score of the given column  </returns>
    private int GetRowScore(int collom)
    {
        bool[] found = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            int value = sudoko[i, collom] - 1;
            if (!found[value])
            {
                found[value] = true;
            }
        }
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            if (!found[i]) sum++;
        }
        return sum;
    }

    /// <summary>
    /// A function that returns the score of one row. The score is equal
    /// to the number of missing numbers within the range 1 to 9 in the row
    /// </summary>
    /// <param name="row"> the index of the rowd </param>
    /// <returns> the score of the given row </returns>
    private int GetCollomScore(int row)
    {
        bool[] found = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            int value = sudoko[row, i] - 1;
            if (!found[value])
            {
                found[value] = true;
            }
        }
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            if (!found[i]) sum++;
        }
        return sum;
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// The function that really solves the sudoky. The function iterates till the
    /// sum of the column and row scores equals to 0, or till it has reached 100.000 iterations.
    /// On average, the number of iterations is under 10.000, so a 10x hard limit is employed
    /// 
    /// Per iteration, the best possible swap is executed once in every cell. The cells are visited
    /// in a random order. It keeps track of whether the score has improved.If this is not the case,
    /// the counter 'minima' is increased. When the threshold of 'minimaTHreshold' is reached, a number of S
    /// random swaps are done. The counter is reset to 0 and the score gets updated (depending on S, it is faster
    /// to update all of the scores once than to update only the rows and columns that have been affected by the swap).
    /// The counter is set back to 0 also if the score does improve but the threshold has 
    /// not yet been reached (escaped from the minimum).
    /// 
    /// For research purposes, the number of iterations till a solution is found, is returned at the end.
    /// </summary>
    /// <param name="S"> S: number of random switches when in a local minimum </param>
    /// <param name="minimaThreshold"> Variable that decides when S should be employed </param>
    /// <returns> number of iterations till a solution is found </returns>
    private int Solve(int minimaThreshold, int S)
    {
        int minima = 0;
        int iterations = 0;
        // stops when the score is 0 OR when stuck on 100000 iteratons
        while (collomScores.Sum() + rowScores.Sum() > 0 && iterations <= 100000)
        {
            // One function call of SwapRoundAllCells executes 9 swaps
            iterations += 9;
            // Keep track of whether the score has improved
            bool foundBetter = SwapRoundAllCells();

            // If the score hasn't improved, increase the counter (number of x in a row)
            if (!foundBetter) minima++;
            else minima = 0;

            // If the counter reaches the threshold, do S random swaps
            if (minima == minimaThreshold)
            {
                minima = 0;
                DoRandomSwaps(S);
                //calculate the new row and column scores
                SetAllScores();
            }

        }
        // Return the iterations for debugging and experiments 
        return iterations;
    }

    /// <summary>
    /// A function that takes a random swappair out of a random cell
    /// and executes a swap with it, without backtracking
    /// </summary>
    /// <param name="S"></param>
    void DoRandomSwaps(int S)
    {
        for (int i = 0; i < S; i++)
        {
            int randomCell = random.Next() % 9;
            int randomPair = random.Next() % swapPairs[randomCell].Count;
            Swap(swapPairs[randomCell][randomPair], false);
        }
    }

    /// <summary>
    /// Do the best possible swap in all cells in random order
    /// </summary>
    /// <returns> 'true' when the score has improved </returns>
    bool SwapRoundAllCells()
    {
        // the old score
        int oldScore = rowScores.Sum() + collomScores.Sum();

        List<int> possible = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 9; i++)
        {
            int randomIndex = random.Next() % (9 - i);
            int randomCell = possible[randomIndex];

            DoBestSwap(randomCell);
        }

        //the new score
        int newScore = rowScores.Sum() + collomScores.Sum();

        // 'false' -> score has NOT improved
        if (newScore == oldScore) { return false; }
        return true;
    }

    /// <summary>
    /// A funciton that takes a swap pair as input, applies the swap 
    /// to  a given pair, calculates the new score and, if necessary,
    /// swaps back. 
    /// </summary>
    /// <param name="pair"> The given swap pair </param>
    /// <param name="backtrack"> swap back </param>
    /// <returns> Difference in score BEFORE and AFTER swap </returns>
    int Swap((int, int)[] pair, bool backtrack)
    {
        (int row_a, int collom_a) = pair[0];
        (int row_b, int collom_b) = pair[1];

        int oldScore = GetRowScore(collom_a) + GetRowScore(collom_b) + GetCollomScore(row_a) + GetCollomScore(row_b);

        int oldValue_a = sudoko[row_a, collom_a];
        int oldValue_b = sudoko[row_b, collom_b];

        sudoko[row_a, collom_a] = oldValue_b;
        sudoko[row_b, collom_b] = oldValue_a;

        int newScore = GetRowScore(collom_a) + GetRowScore(collom_b) + GetCollomScore(row_a) + GetCollomScore(row_b);

        if (backtrack)
        {
            sudoko[row_a, collom_a] = oldValue_a;
            sudoko[row_b, collom_b] = oldValue_b;
        }

        return newScore - oldScore;
    }

    /// <summary>
    /// Loop over all swap pairs in a cell.
    /// Do the swaps with backtracking, and check whether the new score is better
    /// OR EQUAL to the current best. If that is the case, remember this pair.
    /// When this occurs once, set the 'found' to true, so the program knows that
    /// is should actually do a swap at the end.
    /// 
    /// The base score is 0, which means that only better or equally good scores
    /// will be found
    /// 
    /// In the end, use the found pair and swap these without backtracking
    /// and update the scores of the columns and rows that have been affected by this.
    /// 
    /// </summary>
    /// <param name="cell"> The cell in which the swap takes place  </param>
    void DoBestSwap(int cell)
    {
        bool found = false;

        int lowestDif = 0;
        (int, int)[] bestPair = new (int, int)[2];
        foreach ((int, int)[] swapPair in swapPairs[cell])
        {
            int dif = Swap(swapPair, true);
            if (dif <= lowestDif)
            {
                found = true;
                lowestDif = dif;
                bestPair = swapPair;
            }
        }
        //found NOTHING better than or equally as good as the current situation
        if (!found) { return; }


        Swap(bestPair, false);

        // Instead of updating all of the weights again, update only
        // the rows and columns affected by the switch

        (int row_a, int collum_a) = bestPair[0];
        (int row_b, int collum_b) = bestPair[1];

        int NewRowScore_a = GetRowScore(collum_a);
        int NewRowScore_b = GetRowScore(collum_b);

        int newCollomScore_a = GetRowScore(row_a);
        int newCollomScore_b = GetRowScore(row_b);

        collomScores[row_a] = newCollomScore_a;
        collomScores[row_b] = newCollomScore_b;

        rowScores[collum_a] = NewRowScore_a;
        rowScores[collum_b] = NewRowScore_b;



    }

}