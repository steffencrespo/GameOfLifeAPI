using GameOfLifeAPI.Models;

namespace GameOfLifeAPI.Services
{
    public class BoardService
    {
        private readonly Dictionary<Guid, Board> _boards = new();

        public Board UploadBoard(List<List<bool>> state)
        {
            if (state == null || state.Count == 0 || state[0].Count == 0)
                throw new ArgumentException("State must be a non-empty 2D list");
            
            var board = new Board
            {
                Id = Guid.NewGuid(),
                State = state,
                CreatedAt = DateTime.UtcNow
            };
            _boards[board.Id] = board;
            return board;
        }

        public Board? GetBoard(Guid id)
        {
            _boards.TryGetValue(id, out var board);
            return board;
        }
        
        public Board CreateBoard(List<List<bool>> state)
        {
            if (state == null || state.Count == 0 || state[0].Count == 0)
                throw new ArgumentException("State must be a non-empty 2D list");

            var board = new Board
            {
                Id = Guid.NewGuid(),
                State = state,
                CreatedAt = DateTime.UtcNow
            };

            _boards[board.Id] = board;
            return board;
        }

        // to retrieve the next state of the board
        public List<List<bool>>? GetNextState(Guid id)
        {
            if (!_boards.TryGetValue(id, out var board) || board.State == null)
                return null;

            var current = board.State;
            int rows = current.Count;
            int columns = current[0].Count;

            var next = new List<List<bool>>();

            for (int row = 0; row < rows; row++)
            {
                var newRow = new List<bool>();
                for (int col = 0; col < columns; col++)
                {
                    int aliveNeighbors = CountAliveNeighbors(current, row, col, rows, columns);
                    bool cellIsAlive = current[row][col];

                    if (cellIsAlive && (aliveNeighbors == 2 || aliveNeighbors == 3))
                        newRow.Add(true);
                    else if (!cellIsAlive && aliveNeighbors == 3)
                        newRow.Add(true);
                    else
                        newRow.Add(false);
                }
                next.Add(newRow);
            }

            return next;
        }

        private int CountAliveNeighbors(List<List<bool>> grid, int row, int col, int rows, int cols)
        {
            int count = 0;
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0)
                        continue;

                    int r = row + dr;
                    int c = col + dc;

                    if (r >= 0 && r < rows && c >= 0 && c < cols && grid[r][c])
                        count++;
                }
            }
            return count;
        }
        
        public List<List<bool>>? GetStateAfterSteps(Guid id, int steps)
        {
            if (!_boards.TryGetValue(id, out var board) || board.State == null || steps < 0)
                return null;

            var current = DeepCopy(board.State);

            for (int i = 0; i < steps; i++)
            {
                current = ComputeNextState(current);
            }

            return current;
        }

        private List<List<bool>> ComputeNextState(List<List<bool>> state)
        {
            int rows = state.Count;
            int cols = state[0].Count;
            var next = new List<List<bool>>();

            for (int row = 0; row < rows; row++)
            {
                var newRow = new List<bool>();
                for (int col = 0; col < cols; col++)
                {
                    int aliveNeighbors = CountAliveNeighbors(state, row, col, rows, cols);
                    bool cellIsAlive = state[row][col];

                    if (cellIsAlive && (aliveNeighbors == 2 || aliveNeighbors == 3))
                        newRow.Add(true);
                    else if (!cellIsAlive && aliveNeighbors == 3)
                        newRow.Add(true);
                    else
                        newRow.Add(false);
                }
                next.Add(newRow);
            }

            return next;
        }

        private List<List<bool>> DeepCopy(List<List<bool>> source)
        {
            return source.Select(row => new List<bool>(row)).ToList();
        }
    }
}