using System;
using System.Collections.Generic;
using System.Text;

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