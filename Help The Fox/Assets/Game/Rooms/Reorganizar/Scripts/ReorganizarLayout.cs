using System.Collections.Generic;
using Cell = TableBoardState.Cell;

public static class ReorganizarLayout
{
    public const int Width = 14;
    public const int Height = 10;
    public static Cell Spawn => new Cell(1, 4);
    public static Cell Exit => new Cell(1, 4);

    public static Cell[] CreateTargets() => new[]
    {
        new Cell(4, 2), new Cell(4, 3), new Cell(4, 4), new Cell(4, 5), new Cell(4, 6),
        new Cell(5, 2), new Cell(6, 2), new Cell(5, 4), new Cell(6, 4),
        new Cell(5, 6), new Cell(6, 6)
    };

    public static Cell[] CreateTables() => new[]
    {
        new Cell(2, 2), new Cell(2, 3), new Cell(2, 4), new Cell(5, 5), new Cell(3, 6),
        new Cell(4, 4), new Cell(7, 3), new Cell(5, 4), new Cell(9, 4),
        new Cell(6, 7), new Cell(5, 7)
    };

    public static List<Cell> CreateWalls()
    {
        var walls = new List<Cell>();
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    walls.Add(new Cell(x, y));
        return walls;
    }

    // A legal solution, obtained by reversing pulls from the finished E; not shown during play.
    public const string VerifiedSolution =
        "URRRRLDURURRRRRDDLUULLLLLDLUDDDRUULLLDDRLUURRRRURRDDDRRULUUUULLLDLDDDDDRRUULULLLLDDDRULUURRRRDDUULLDRULLLDRUUUURDRRURRDDDLRUULRRDDDDDLRUULDLLLUUURLDLLDDRLDRULUURRUUURDLLLDRLDDRUUURRRRDDLLRRUULLLLLDDD";

    public static TableBoardState CreateBoard() => new TableBoardState(
        Width, Height, CreateWalls(), CreateTables(), CreateTargets(), Spawn);

    public static Cell Direction(char command)
    {
        switch (command)
        {
            case 'U': return new Cell(0, 1);
            case 'D': return new Cell(0, -1);
            case 'L': return new Cell(-1, 0);
            case 'R': return new Cell(1, 0);
            default: return new Cell(0, 0);
        }
    }
}
