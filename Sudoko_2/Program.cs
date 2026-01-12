using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



public class MainProgram
{
    /// <summary>
    /// Main function of the program
    /// It allows to read in sudokus in 3 different ways:
    /// 
    /// 1. Give it as an argument in the console, linked to a .txt
    /// 2. Put it in the project folder and name it 'puzzles.txt'
    /// 3. Harde code it all the way at the top of this code (copy paste it)
    /// 
    /// This function then solves all the sudokus and shows the solutions, 
    /// including how many iterations it took to get to these solutions.
    /// 
    /// Optionally, you can uncomment some code to run a single sudoku 100.000X,
    /// in order to collect data points, because the algorithm is stochastic
    /// 
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        InputReader inputReader = new InputReader();
        int[][,] sudokos = inputReader.Text2Sudokos(SUDOKOS.SUDOKUS);


        if (args.Length > 0)
        {
            Console.WriteLine($"Bezig met inlezen van: {args[0]}");
            sudokos = inputReader.ReadFile(args[0]);
        }
        else if (File.Exists("puzzles.txt"))
        {
            Console.WriteLine("Geen argumenten. Leest standaard 'puzzles.txt'...");
            sudokos = inputReader.ReadFile("puzzles.txt");
        }
        else
        {
            Console.WriteLine("Geen bestand gevonden. Gebruik interne test-data...");
            //sudokos = inputReader.Text2Sudokos(SUDOKOS.SUDOKUS);
            sudokos = inputReader.Text2Sudokos(SUDOKOS.HARD_SUDOKUS);
        }


        /* -- first exercise --
        //Uncomment this code to collect data points:

        //DataHoarder hoarder = new DataHoarder();
        //hoarder.GetData(sudokos[0], 1, 5);

        // This solves all of the sudokus and shows the solved sudokus
        foreach (int[,] sudoko in sudokos)
        {
            Solver solver = new Solver(sudoko);
            solver.solve(true, 3, 5);
        }
        */


        foreach (int[,] puzzle in sudokos)
        {
            Console.WriteLine("--------------------------------------------------");

            // 1. Chronological Backtracking
            CBT_Solver cbt = new CBT_Solver(puzzle);
            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            cbt.Solve();
            sw1.Stop();
            Console.WriteLine($"CBT     - Nodes: {cbt.nodesVisited,10} | Tijd: {sw1.Elapsed.TotalMilliseconds,8:F2}ms");

            // 2. Forward Checking
            FC_Solver fc = new FC_Solver(puzzle);
            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            fc.Solve();
            sw2.Stop();
            Console.WriteLine($"FC      - Nodes: {fc.nodesVisited,10} | Tijd: {sw2.Elapsed.TotalMilliseconds,8:F2}ms");

            // 3. Forward Checking + MCV
            MCV_FC_Solver mcv = new MCV_FC_Solver(puzzle);
            var sw3 = System.Diagnostics.Stopwatch.StartNew();
            mcv.Solve();
            sw3.Stop();
            Console.WriteLine($"FC+MCV  - Nodes: {mcv.nodesVisited,10} | Tijd: {sw3.Elapsed.TotalMilliseconds,8:F2}ms");

            
            cbt.PrintResult();
            Console.WriteLine("");
            fc.PrintResult();
            Console.WriteLine("");
            mcv.PrintResult();
            Console.WriteLine("");
            

        }


    }




}


/// <summary>
/// A class used for experiments/research
/// </summary>
public class DataHoarder
{
    /// <summary>
    /// A function that solves the same sudoku a 100.000 times
    /// with the given S and minimaThreshold values. 
    /// The amount of iterations sorted in ascending order are saved into a .txt file, seperated by a ','

    /// </summary>
    /// <param name="sudoko"> The to be solved sudoku </param>
    /// <param name="S"> S: number of random switches when in a local minimum </param>
    /// <param name="minimaThreshold"> Variable that decides when S should be employed  </param>
    public void GetData(int[,] sudoko, int S, int minimaThreshold)
    {

        int dataPoints = 100000;
        int[] iterationArray = new int[dataPoints];
        int index = 0;

        // Use multi-threading, because otherwise it takes infinitely
        Parallel.For(0, dataPoints, i =>
        {
            Console.WriteLine(index);
            index++;

            Solver solver = new Solver((int[,])sudoko.Clone());
            int n = solver.solve(false, S, minimaThreshold);

            iterationArray[i] = n;
        });


        Array.Sort(iterationArray);


        //  Write the data into a text file, seperated by a ',' 
        string docPath = "C:\\Users\\Alek\\Desktop\\data";

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, $"S_{S}_T_{minimaThreshold}.txt")))
        {
            foreach (int value in iterationArray)
                outputFile.Write($"{value}, ");
        }

    }
}

/// <summary>
/// A class that can read in sudokus: converts strings to sudoku(s)
/// </summary>
public class InputReader
{

    /// <summary>
    /// Read in a file with the sudokus, convert these to the 
    /// sudoku data structure ([,]) and return these.

    /// </summary>
    /// <param name="filepath"> The location of the  .txt file  </param>
    /// <returns> a list of sudokus in [,] format  </returns>
    public int[][,] ReadFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            Console.WriteLine($"Fout: Bestand '{filepath}' niet gevonden.");
            return new int[0][,];
        }

        string fileContent = File.ReadAllText(filepath);
        return Text2Sudokos(fileContent);
    }

    /// <summary>
    /// Convert a string in the format '0 0 .... 0 0' to a single sudoku in the [,] data format
    /// </summary>
    /// <param name="text"> A sudoku in text format:  ' 0 0 .... 0 0' </param>
    /// <returns> sudoku in format [,] </returns>
    private int[,] Text2Sudoko(string text)
    {
        int[,] sudoko = new int[9, 9];
        int index = 1;

        string[] sudokuStrSplit = text.Split();

        for (int i = 0; i < 9; i++)
        {
            for (int i2 = 0; i2 < 9; i2++)
            {
                sudoko[i2, i] = int.Parse(sudokuStrSplit[index]);
                index++;
            }
        }
        return sudoko;
    }

    /// <summary>
    /// Convert one or more sudukus to [,] sudokus.
    /// Uses the format that was given in the example .txt of the assignment

    /// </summary>
    /// <param name="text"> One or more sudukus in the example format </param>
    /// <returns> a list of sudokus in [,] format  </returns>
    public int[][,] Text2Sudokos(string text)
    {


        string[] textSplitArray = text.Split("\\n");
        List<string> textSplit = textSplitArray.OfType<string>().ToList();

        textSplit.RemoveAt(0);
        for (int i = textSplit.Count - 1; i >= 1; i -= 2)
        {
            textSplit.RemoveAt(i);
            textSplit[i - 1] = textSplit[i - 1].Replace("\\r", "");
        }

        int[][,] sudokus = new int[textSplit.Count][,];

        for (int i = 0; i < sudokus.Length; i++)
        {
            sudokus[i] = Text2Sudoko(textSplit[i]);
        }

        return sudokus;
    }

}







