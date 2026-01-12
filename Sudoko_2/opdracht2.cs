/// <summary>
/// A Chronological Backtracking (CBT) solver for Sudoku.
/// </summary>
public class CBT_Solver
{
    protected int[,] sudoko;
    private int[,] baseSudoko;
    public long nodesVisited = 0;

    /// <summary>
    /// Initialize the CBT solver with a copy of the unsolved Sudoku.
    /// </summary>
    /// <param name="sudoko">The unsolved Sudoku grid.</param>
    public CBT_Solver(int[,] sudoko)
    {
        this.baseSudoko = (int[,])sudoko.Clone();
        this.sudoko = (int[,])sudoko.Clone();
    }

    /// <summary>
    /// Public entry point to solve the Sudoku using backtracking.
    /// </summary>
    /// <returns>True if a solution is found, false otherwise.</returns>
    public bool Solve()
    {
        nodesVisited = 0;
        return Backtrack();
    }

    /// <summary>
    /// Core recursive backtracking function that tries values chronologically.
    /// </summary>
    /// <returns>True if the current branch leads to a solution.</returns>
    protected virtual bool Backtrack()
    {
        nodesVisited++;
        for (int row = 0; row < 9; row++)
        {
            for (int collum = 0; collum < 9; collum++)
            {
                // Find the first empty cell
                if (sudoko[row, collum] == 0)
                {
                    for (int value = 1; value <= 9; value++)
                    {
                        if (IsValid(row, collum, value))
                        {
                            sudoko[row, collum] = value;
                            // Continue the search using backtrack
                            if (Backtrack()) return true;
                            // Reset on backtrack
                            sudoko[row, collum] = 0;
                        }
                    }
                    // No valid values for the empty space, backtrack
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Validates if a value can be placed at a specific position according to Sudoku rules.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="collum">Column index.</param>
    /// <param name="value">The value to check (1-9).</param>
    /// <returns>True if the move is legal.</returns>
    protected bool IsValid(int row, int collum, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (sudoko[row, i] == value || sudoko[i, collum] == value) return false;
        }

        // Checking a cell is a bit harder. First get the cell coordintates
        // and then use a nested for loop to check all the values.
        int startRow = (row / 3) * 3;
        int startCollum = (collum / 3) * 3;
        for (int i = 0; i < 3; i++)
        {
            for (int i2 = 0; i2 < 3; i2++)
            {
                if (sudoko[startRow + i, startCollum + i2] == value) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Prints the original and solved Sudoku to the console.
    /// </summary>
    public void PrintResult()
    {
        int[] rowScores = new int[9];
        int[] colScores = new int[9];
        Debug.PrintSudokoFancy(baseSudoko, sudoko, rowScores, colScores);
    }
}

/// <summary>
/// A Sudoku solver that uses Forward Checking (FC) to prune the search space.
/// </summary>
public class FC_Solver : CBT_Solver
{
    protected List<int>[,] domains;

    /// <summary>
    /// Initialize the FC solver and set up the initial domains for each cell.
    /// </summary>
    /// <param name="input">The unsolved Sudoku grid.</param>
    public FC_Solver(int[,] input) : base(input)
    {
        domains = new List<int>[9, 9];
        InitializeDomains();
    }

    /// <summary>
    /// Sets up the 1-9 domains and performs initial node consistency.
    /// </summary>
    private void InitializeDomains()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int collum = 0; collum < 9; collum++)
            {
                domains[row, collum] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
        }
        for (int row = 0; row < 9; row++)
        {
            for (int collum = 0; collum < 9; collum++)
            {
                if (sudoko[row, collum] != 0)
                {
                    UpdateDomains(row, collum, sudoko[row, collum]);
                }
            }
        }
    }

    /// <summary>
    /// Updates the domain of a specific cell to a single value.
    /// </summary>
    protected void UpdateDomains(int row, int collum, int value)
    {
        domains[row, collum] = new List<int> { value };
    }

    /// <summary>
    /// Overridden backtrack that uses Forward Checking 
    /// </summary>
    protected override bool Backtrack()
    {
        nodesVisited++;
        int row = -1;
        int collum = -1;

        for (int i = 0; i < 9; i++)
        {
            for (int i2 = 0; i2 < 9; i2++)
            {
                if (sudoko[i, i2] == 0)
                {
                    row = i;
                    collum = i2;
                    break;
                }
            }
            if (row != -1) break;
        }

        if (row == -1) return true;

        List<int> currentDomain = GetPossibleValues(row, collum);

        // loop through all values in the domain
        foreach (int value in currentDomain)
        {
            sudoko[row, collum] = value;

            // Only proceed if this choice leaves options for future cells
            if (HasFutureOptions(row, collum, value))
            {
                if (Backtrack()) return true;
            }
            sudoko[row, collum] = 0;
        }
        return false;
    }

    /// <summary>
    /// Calculates all currently valid values for a cell based on Sudoku constraints.
    /// </summary>
    protected List<int> GetPossibleValues(int row, int collum)
    {
        List<int> values = new List<int>();
        for (int value = 1; value <= 9; value++)
        {
            if (IsValid(row, collum, value)) values.Add(value);
        }
        return values;
    }

    /// <summary>
    /// Performs a simple look-ahead to see if any empty cell in the 
    /// same row or column would have its domain reduced to zero.
    /// </summary>
    protected bool HasFutureOptions(int row, int collum, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (sudoko[row, i] == 0 && GetPossibleValues(row, i).Count == 0) return false;
            if (sudoko[i, collum] == 0 && GetPossibleValues(i, collum).Count == 0) return false;
        }
        return true;
    }
}

/// <summary>
/// A solver that combines Forward Checking with the Most Constrained Variable (MCV) heuristic.
/// </summary>
public class MCV_FC_Solver : FC_Solver
{
    public MCV_FC_Solver(int[,] input) : base(input) { }

    /// <summary>
    /// Overridden backtrack that implements the MCV heuristic: 
    /// it selects the empty cell with the smallest domain (fewest legal moves) first.
    /// </summary>
    protected override bool Backtrack()
    {
        nodesVisited++;
        int bestRow = -1;
        int bestCollum = -1;
        int minOptions = 10;

        // MCV Heuristic: Search for the cell with the fewest remaining legal options
        for (int row = 0; row < 9; row++)
        {
            for (int collum = 0; collum < 9; collum++)
            {
                if (sudoko[row, collum] == 0)
                {
                    int optionsCount = GetPossibleValues(row, collum).Count;
                    if (optionsCount < minOptions)
                    {
                        minOptions = optionsCount;
                        bestRow = row;
                        bestCollum = collum;
                    }
                }
            }
        }

        if (bestRow == -1) return true; // Puzzle solved
        if (minOptions == 0) return false; // Dead end

        foreach (int val in GetPossibleValues(bestRow, bestCollum))
        {
            sudoko[bestRow, bestCollum] = val;
            if (HasFutureOptions(bestRow, bestCollum, val))
            {
                if (Backtrack()) return true;
            }
            sudoko[bestRow, bestCollum] = 0;
        }
        return false;
    }
}