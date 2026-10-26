using System;
using NUnit.Framework;
using Cell = TableBoardState.Cell;

public class TableBoardStateTests
{
    private static TableBoardState Board(Cell[] tables = null, Cell[] targets = null,
        Cell[] walls = null, Cell? player = null) => new TableBoardState(7, 7,
            walls ?? Array.Empty<Cell>(), tables ?? new[] { new Cell(2, 2) },
            targets ?? new[] { new Cell(4, 2) }, player ?? new Cell(1, 2));

    [Test]
    public void ValidPushMovesExactlyOneCell()
    {
        var board = Board();
        Assert.That(board.TryStep(new Cell(1, 0), out int moved), Is.True);
        Assert.That(moved, Is.EqualTo(0));
        Assert.That(board.TableAtIndex(0), Is.EqualTo(new Cell(3, 2)));
        Assert.That(board.Player, Is.EqualTo(new Cell(2, 2)));
    }

    [Test]
    public void WallRejectsPushWithoutMovingEitherObject()
    {
        var board = Board(walls: new[] { new Cell(3, 2) });
        Assert.That(board.TryStep(new Cell(1, 0), out _), Is.False);
        Assert.That(board.TableAtIndex(0), Is.EqualTo(new Cell(2, 2)));
        Assert.That(board.Player, Is.EqualTo(new Cell(1, 2)));
    }

    [Test]
    public void CannotPushTwoTablesOrOverlapThem()
    {
        var board = Board(new[] { new Cell(2, 2), new Cell(3, 2) },
            new[] { new Cell(4, 2), new Cell(4, 3) });
        Assert.That(board.TryStep(new Cell(1, 0), out _), Is.False);
        Assert.That(board.TableAtIndex(0), Is.EqualTo(new Cell(2, 2)));
        Assert.That(board.TableAtIndex(1), Is.EqualTo(new Cell(3, 2)));
    }

    [Test]
    public void WalkingAwayDoesNotPullTable()
    {
        var board = Board(player: new Cell(2, 3));
        Assert.That(board.TryStep(new Cell(0, 1), out int moved), Is.True);
        Assert.That(moved, Is.EqualTo(-1));
        Assert.That(board.TableAtIndex(0), Is.EqualTo(new Cell(2, 2)));
    }

    [Test]
    public void DiagonalZeroAndLongMovesAreRejected()
    {
        var board = Board();
        Assert.That(board.TryStep(new Cell(1, 1), out _), Is.False);
        Assert.That(board.TryStep(new Cell(0, 0), out _), Is.False);
        Assert.That(board.TryStep(new Cell(2, 0), out _), Is.False);
        Assert.That(board.TryStep(new Cell(0, -2), out _), Is.False);
    }

    [Test]
    public void BoardBoundaryBlocksPlayerAndTables()
    {
        var board = Board(new[] { new Cell(6, 2) }, new[] { new Cell(4, 2) }, player: new Cell(5, 2));
        Assert.That(board.TryStep(new Cell(1, 0), out _), Is.False);
        var edge = Board(player: new Cell(0, 3));
        Assert.That(edge.TryStep(new Cell(-1, 0), out _), Is.False);
    }

    [Test]
    public void PartialTargetsDoNotComplete()
    {
        var board = Board(new[] { new Cell(4, 2), new Cell(2, 4) },
            new[] { new Cell(4, 2), new Cell(4, 4) });
        Assert.That(board.OccupiedTargetCount, Is.EqualTo(1));
        Assert.That(board.IsSolved, Is.False);
    }

    [Test]
    public void FinalTargetCompletesAndLocksTablesButAllowsWalking()
    {
        var board = Board(targets: new[] { new Cell(3, 2) });
        Assert.That(board.TryStep(new Cell(1, 0), out _), Is.True);
        Assert.That(board.IsSolved, Is.True);
        Assert.That(board.TryStep(new Cell(1, 0), out _), Is.False);
        Assert.That(board.Reset(), Is.False);
        Assert.That(board.TryStep(new Cell(0, 1), out _), Is.True);
        Assert.That(board.IsSolved, Is.True);
    }

    [Test]
    public void ResetRestoresTablesAndSafePlayerSpawn()
    {
        var board = Board();
        board.TryStep(new Cell(1, 0), out _);
        Assert.That(board.Reset(), Is.True);
        Assert.That(board.TableAtIndex(0), Is.EqualTo(new Cell(2, 2)));
        Assert.That(board.Player, Is.EqualTo(new Cell(1, 2)));
        Assert.That(board.OccupiedTargetCount, Is.EqualTo(0));
    }

    [Test]
    public void DuplicateTargetsAreRejected()
    {
        Assert.Throws<ArgumentException>(() => Board(new[] { new Cell(2, 2), new Cell(2, 3) },
            new[] { new Cell(4, 2), new Cell(4, 2) }));
    }

    [Test]
    public void DuplicateTablesAreRejected()
    {
        Assert.Throws<ArgumentException>(() => Board(new[] { new Cell(2, 2), new Cell(2, 2) },
            new[] { new Cell(4, 2), new Cell(4, 3) }));
    }

    [Test]
    public void InvalidSpawnAndTargetWallAreRejected()
    {
        Assert.Throws<ArgumentException>(() => Board(player: new Cell(2, 2)));
        Assert.Throws<ArgumentException>(() => Board(walls: new[] { new Cell(4, 2) }));
        Assert.Throws<ArgumentException>(() => Board(targets: new[] { new Cell(9, 2) }));
    }

    [Test]
    public void TableAndTargetCountsMustMatch()
    {
        Assert.Throws<ArgumentException>(() => Board(targets: Array.Empty<Cell>()));
    }

    [Test]
    public void RestoredRoomStaysSolvedAndDoesNotReset()
    {
        var board = ReorganizarLayout.CreateBoard();
        board.RestoreSolvedLayout();
        Assert.That(board.IsSolved, Is.True);
        Assert.That(board.OccupiedTargetCount, Is.EqualTo(11));
        Assert.That(board.TableAt(board.Player), Is.EqualTo(-1));
        Assert.That(board.BlockedByWall(board.Player), Is.False);
        Assert.That(board.Reset(), Is.False);
    }

    [Test]
    public void RestoringSolvedRoomRelocatesSpawnIfTargetOccupiesIt()
    {
        var board = Board(targets: new[] { new Cell(1, 2) });
        board.RestoreSolvedLayout();
        Assert.That(board.TableAt(board.Player), Is.EqualTo(-1));
    }

    [Test]
    public void SuppliedLayoutHasLegalSolutionAndFormsE()
    {
        var board = ReorganizarLayout.CreateBoard();
        int pushes = 0;
        foreach (char step in ReorganizarLayout.VerifiedSolution)
        {
            Assert.That(board.TryStep(ReorganizarLayout.Direction(step), out int table), Is.True,
                "Movimento inválido na solução: " + step + " em " + board.Player);
            if (table >= 0) pushes++;
        }
        Assert.That(board.IsSolved, Is.True);
        Assert.That(board.OccupiedTargetCount, Is.EqualTo(11));
        Assert.That(pushes, Is.EqualTo(26));
        var targets = ReorganizarLayout.CreateTargets();
        foreach (int y in new[] { 2, 3, 4, 5, 6 }) Assert.That(targets, Does.Contain(new Cell(4, y)));
        foreach (int y in new[] { 2, 4, 6 })
            foreach (int x in new[] { 5, 6 }) Assert.That(targets, Does.Contain(new Cell(x, y)));
    }
}
