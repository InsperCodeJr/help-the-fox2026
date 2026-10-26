using System;
using System.Collections.Generic;

// Pure board rules: no Unity lifecycle, input or physics dependency.
public sealed class TableBoardState
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public readonly int X;
        public readonly int Y;
        public Cell(int x, int y) { X = x; Y = y; }
        public bool Equals(Cell other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Cell other && Equals(other);
        public override int GetHashCode() => unchecked(X * 397 ^ Y);
        public static Cell operator +(Cell a, Cell b) => new Cell(a.X + b.X, a.Y + b.Y);
        public static bool operator ==(Cell a, Cell b) => a.Equals(b);
        public static bool operator !=(Cell a, Cell b) => !a.Equals(b);
        public override string ToString() => $"({X}, {Y})";
        public bool IsCardinal => (X == 0 && (Y == 1 || Y == -1)) ||
                                  (Y == 0 && (X == 1 || X == -1));
    }

    private readonly int width;
    private readonly int height;
    private readonly HashSet<Cell> walls;
    private readonly HashSet<Cell> targetSet;
    private readonly Cell[] targets;
    private readonly Cell[] initialTables;
    private readonly Cell[] tables;
    private readonly Cell initialPlayer;

    public Cell Player { get; private set; }
    public bool IsSolved { get; private set; }
    public int TableCount => tables.Length;
    public int TargetCount => targets.Length;
    public int OccupiedTargetCount
    {
        get
        {
            int count = 0;
            foreach (Cell table in tables) if (targetSet.Contains(table)) count++;
            return count;
        }
    }

    public TableBoardState(int width, int height, IEnumerable<Cell> walls,
        IEnumerable<Cell> tables, IEnumerable<Cell> targets, Cell player)
    {
        if (width < 1 || height < 1) throw new ArgumentException("A grade precisa ter tamanho positivo.");
        this.width = width;
        this.height = height;
        this.walls = new HashSet<Cell>(walls ?? throw new ArgumentNullException(nameof(walls)));
        this.tables = new List<Cell>(tables ?? throw new ArgumentNullException(nameof(tables))).ToArray();
        this.targets = new List<Cell>(targets ?? throw new ArgumentNullException(nameof(targets))).ToArray();
        targetSet = new HashSet<Cell>(this.targets);
        if (this.tables.Length == 0 || this.tables.Length != this.targets.Length)
            throw new ArgumentException("Configure uma mesa para cada alvo, com pelo menos um alvo.");
        if (new HashSet<Cell>(this.tables).Count != this.tables.Length || targetSet.Count != this.targets.Length)
            throw new ArgumentException("Mesas e alvos não podem repetir células.");
        foreach (Cell wall in this.walls)
            if (!Inside(wall)) throw new ArgumentException("Parede fora da grade.");
        foreach (Cell table in this.tables)
            if (BlockedByWall(table)) throw new ArgumentException("Mesa dentro de parede ou fora da grade.");
        foreach (Cell target in this.targets)
            if (BlockedByWall(target)) throw new ArgumentException("Alvo dentro de parede ou fora da grade.");
        if (BlockedByWall(player) || TableAt(player) >= 0)
            throw new ArgumentException("O nascimento do jogador precisa estar livre.");
        initialTables = (Cell[])this.tables.Clone();
        initialPlayer = Player = player;
        IsSolved = OccupiedTargetCount == TargetCount;
    }

    public bool Inside(Cell cell) => cell.X >= 0 && cell.X < width && cell.Y >= 0 && cell.Y < height;
    public bool BlockedByWall(Cell cell) => !Inside(cell) || walls.Contains(cell);
    public Cell TableAtIndex(int index) => tables[index];
    public int TableAt(Cell cell)
    {
        for (int i = 0; i < tables.Length; i++) if (tables[i] == cell) return i;
        return -1;
    }

    public bool TryStep(Cell direction, out int pushedTable)
    {
        pushedTable = -1;
        if (!direction.IsCardinal) return false;
        Cell next = Player + direction;
        if (BlockedByWall(next)) return false;
        int table = TableAt(next);
        if (table >= 0)
        {
            Cell destination = next + direction;
            if (IsSolved || BlockedByWall(destination) || TableAt(destination) >= 0) return false;
            tables[table] = destination;
            pushedTable = table;
        }
        Player = next;
        if (!IsSolved && OccupiedTargetCount == TargetCount) IsSolved = true;
        return true;
    }

    public bool Reset()
    {
        if (IsSolved) return false;
        Array.Copy(initialTables, tables, tables.Length);
        Player = initialPlayer;
        return true;
    }

    // The completion adapter calls this only when this room was solved earlier in this play session.
    public void RestoreSolvedLayout()
    {
        Array.Copy(targets, tables, targets.Length);
        Player = initialPlayer;
        if (TableAt(Player) >= 0)
        {
            bool found = false;
            for (int y = 0; y < height && !found; y++)
                for (int x = 0; x < width && !found; x++)
                {
                    Cell candidate = new Cell(x, y);
                    if (!BlockedByWall(candidate) && TableAt(candidate) < 0)
                    {
                        Player = candidate;
                        found = true;
                    }
                }
            if (!found) throw new InvalidOperationException("Não existe nascimento livre no tabuleiro resolvido.");
        }
        IsSolved = true;
    }
}
