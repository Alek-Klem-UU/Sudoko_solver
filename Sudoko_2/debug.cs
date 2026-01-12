using System;
using System.Collections.Generic;
using System.Text;

public static class SUDOKOS
{
    public static string SUDOKUS = "\"Grid  01\\r\\n 0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0\\r\\nGrid  02\\r\\n 2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3\\r\\nGrid  03\\r\\n 0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0\\r\\nGrid  04\\r\\n 0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0\\r\\nGrid  05\\r\\n 0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0\\r\\n\";\r\n\r\n";
    public static string HARD_SUDOKUS =
            "\"Grid  01\\r\\n 1 0 0 0 0 7 0 9 0 0 3 0 0 2 0 0 0 8 0 0 9 6 0 0 5 0 0 0 0 5 3 0 0 9 0 0 0 1 0 0 8 0 0 0 2 6 0 0 0 0 4 0 0 0 3 0 0 0 0 0 0 1 0 0 4 0 0 0 0 0 0 7 0 0 7 0 0 0 3 0 0\\r\\nGrid  02\\r\\n 8 0 0 0 0 0 0 0 0 0 0 3 6 0 0 0 0 0 0 7 0 0 9 0 2 0 0 0 5 0 0 0 7 0 0 0 0 0 0 0 4 5 7 0 0 0 0 0 1 0 0 0 3 0 0 0 1 0 0 0 0 6 8 0 0 8 5 0 0 0 1 0 0 9 0 0 0 0 4 0 0\\r\\n\";\r\n\r\n";
}
public static class Debug
{
    /// <summary>
    /// This is a function that displays the sudoku in a visually pleasing way.
    /// De fixed values are colored green, and the unfixed ones red.
    /// The row and column scores are displayed, as well as the sum of these two right underneath

    /// </summary>
    /// <param name="sudokoBase"> The unfilled sudoku </param>
    /// <param name="sudokoMaking"> A sudoku that is filled in, is not necessarily solved  </param>
    /// <param name="rowScores"> The scores of all rows  </param>
    /// <param name="columScores"> The scores of all columns </param>

    public static void PrintSudokoFancy(int[,] sudokoBase, int[,] sudokoMaking, int[] rowScores, int[] columScores)
    {
        // Use StringBuilder for speed
        StringBuilder printing = new StringBuilder();
        for (int i = 0; i < 9; i++)
        {
            for (int i2 = 0; i2 < 9; i2++)
            {

                int base_value = sudokoBase[i2, i];
                int making_value = sudokoMaking[i2, i];

                // Check whether we should display green, red, or a dot
                if (making_value == 0) { printing.Append(". "); }
                else if (base_value != 0) { printing.Append($"\x1b[92m{base_value} \x1b[39m"); }
                else { printing.Append($"\x1b[95m{making_value} \u001b[39m"); }

                // Add borders 
                if (i2 % 3 == 2 && i2 != 8)
                {
                    printing.Append("\x1b[1m| \x1b[22m");
                }
            }

            // Add row scores
            printing.Append($" {rowScores[i]}\n");

            // Add borders
            if (i % 3 == 2 && i != 8)
            {
                printing.Append("\x1b[1m------+-------+------\x1b[22m\n");
            }

        }
        printing.Append($"\n");
        // Add column scores
        for (int i = 0; i < 9; i++)
        {
            printing.Append($"{columScores[i]} ");
            if (i % 3 == 2 && i != 8)
            {
                printing.Append("  ");
            }
        }
        printing.Append($" {columScores.Sum() + rowScores.Sum()}");

        Console.WriteLine(printing.ToString());
    }
}