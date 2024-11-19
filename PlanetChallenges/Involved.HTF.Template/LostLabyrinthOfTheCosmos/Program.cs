using Involved.HTF.Common;

public class Program
{
    public static async Task Main(string[] args)
    {
        HackTheFutureClient hackTheFutureClient = new HackTheFutureClient();

        // Log in with provided credentials
        await hackTheFutureClient.Login("MechaCode", "f0aefee1-e656-4077-91b9-bbc2d25d276d");

        // Get the maze
        var mazeData = await hackTheFutureClient.GetMaze();

        // Parse the maze data
        char[,] maze = ParseMazeData(mazeData);

        // Find the shortest path
        int shortestPathLength = FindShortestPath(maze);

        // Post the result
        await hackTheFutureClient.PostMazeResult(shortestPathLength);

        Console.WriteLine("Result successfully sent!");
        Console.WriteLine(shortestPathLength == -1 ? "Geen pad gevonden" : $"Kortste pad lengte: {shortestPathLength}");
        
    }

    public static int FindShortestPath(char[,] maze)
    {
        int rows = maze.GetLength(0);
        int cols = maze.GetLength(1);
        (int row, int col) start = (-1, -1);
        (int row, int col) end = (-1, -1);

        // Zoek de start- en eindpositie in het doolhof
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (maze[i, j] == 'S') start = (i, j);
                if (maze[i, j] == 'E') end = (i, j);
            }
        }

        // Richtingen voor bewegingen (boven, onder, links, rechts)
        int[] dRow = { -1, 1, 0, 0 };
        int[] dCol = { 0, 0, -1, 1 };

        Queue<(int row, int col, int steps)> queue = new Queue<(int, int, int)>();
        bool[,] visited = new bool[rows, cols];

        // Voeg startpositie toe aan de queue
        queue.Enqueue((start.row, start.col, 0));
        visited[start.row, start.col] = true;

        while (queue.Count > 0)
        {
            var (currentRow, currentCol, steps) = queue.Dequeue();

            // Controleer of we het einde hebben bereikt
            if (currentRow == end.row && currentCol == end.col) return steps;

            // Verken de vier richtingen
            for (int i = 0; i < 4; i++)
            {
                int newRow = currentRow + dRow[i];
                int newCol = currentCol + dCol[i];

                if (IsValidMove(newRow, newCol, rows, cols, maze, visited))
                {
                    queue.Enqueue((newRow, newCol, steps + 1));
                    visited[newRow, newCol] = true;
                }
            }
        }

        // Als we hier komen, is er geen pad naar het einde
        return -1;
    }

    private static bool IsValidMove(int row, int col, int rows, int cols, char[,] maze, bool[,] visited)
    {
        // Controleer of de beweging binnen de grenzen valt, niet bezocht is, en geen obstakel is
        return row >= 0 && row < rows &&
               col >= 0 && col < cols &&
               !visited[row, col] &&
               maze[row, col] != '#' &&
               maze[row, col] != 'B';
    }

    private static char[,] ParseMazeData(string mazeData)
    {
        var lines = mazeData.Split('\n');
        int rows = lines.Length;
        int cols = lines[0].Length;

        char[,] maze = new char[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                maze[i, j] = lines[i][j];
            }
        }
        return maze;
    }
}

