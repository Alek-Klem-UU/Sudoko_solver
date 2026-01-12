public class CBT_Solver
{
    protected int[,] sudoko;
    private int[,] baseSudoko;
    public long nodesVisited = 0;

    public CBT_Solver(int[,] sudoko)
    {
        this.baseSudoko = (int[,])sudoko.Clone();
        this.sudoko = (int[,])sudoko.Clone();
    }

    public bool Solve()
    {
        nodesVisited = 0;
        return Backtrack();
    }

    protected virtual bool Backtrack()
    {
        nodesVisited++;
        for (int row = 0; row < 9; row++)
        {
            for (int collum = 0; collum < 9; collum++)
            {
                if (sudoko[row, collum] == 0)
                {
                    for (int value = 1; value <= 9; value++)
                    {
                        if (IsValid(row, collum, value))
                        {
                            sudoko[row, collum] = value;
                            if (Backtrack()) return true;
                            sudoko[row, collum] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }

    protected bool IsValid(int row, int collum, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (sudoko[row, i] == value || sudoko[i, collum] == value) return false;
        }
        int startRow = (row / 3) * 3; 
        int startCollum = (collum / 3) * 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (sudoko[startRow + i, startCollum + j] == value) return false;
            }
        }
        return true;
    }

    public void PrintResult()
    {

        int[] rowScores = new int[9];
        int[] colScores = new int[9];

        Debug.PrintSudokoFancy(baseSudoko, sudoko, rowScores, colScores);
    }

}



public class FC_Solver : CBT_Solver
{
    protected List<int>[,] domains;

    public FC_Solver(int[,] input) : base(input)
    {
        domains = new List<int>[9, 9];
        InitializeDomains();
    }

    private void InitializeDomains()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int collum = 0; collum < 9; collum++)
            {
                domains[row, collum] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
        }
        // Maak knoopconsistent met de vaste waarden
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

    protected void UpdateDomains(int row, int collum, int value)
    {
        domains[row, collum] = new List<int> { value };
        // Verwijder uit rij, kolom en blok (simpel forward checking)
        // In een echte implementatie sla je de oude domeinen op voor backtracking
    }

    protected override bool Backtrack()
    {
        nodesVisited++;
        int row = -1;
        int collum = -1;
        // Zoek eerste lege vakje (chronologisch)
        for (int i = 0; i < 9; i++)
        {
            for (int i2 = 0; i2 < 9; i2++)
            {
                if (sudoko[i, i2] == 0) { 
                    row = i; 
                    collum = i2; 
                    break; 
                }
            }
            if (row != -1) break;
        }

        if (row == -1) return true;

        List<int> currentDomain = GetPossibleValues(row, collum);

        foreach (int val in currentDomain)
        {
            sudoko[row, collum] = val;
         
            if (HasFutureOptions(row, collum, val))
            {
                if (Backtrack()) return true;
            }
            sudoko[row, collum] = 0;
        }
        return false;
    }

    protected List<int> GetPossibleValues(int row, int collum)
    {
        List<int> values = new List<int>();
        for (int value = 1; value <= 9; value++)
        {
            if (IsValid(row, collum, value)) values.Add(value);
        }
        return values;
    }

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


public class MCV_FC_Solver : FC_Solver
{
    public MCV_FC_Solver(int[,] input) : base(input) { }

    protected override bool Backtrack()
    {
        nodesVisited++;
        int bestRow = -1; 
        int bestCollum = -1;
        int minOptions = 10;

        // MCV Heuristique: Zoek vakje met minste opties
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

        if (bestRow == -1) return true; // Opgelost
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