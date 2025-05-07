using GameOfLifeAPI.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace GameOfLifeAPI.Services
{
    public class BoardService : IBoardService
    {
		private readonly ILogger<BoardService> _logger;
		private readonly string _storagePath = Path.Combine(AppContext.BaseDirectory, "boards.json");
        private readonly ConcurrentDictionary<Guid, Board> _boards = new();

		public BoardService(ILogger<BoardService> logger)
		{
    		_logger = logger;
    		_boards = new ConcurrentDictionary<Guid, Board>();
		}

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
			_logger.LogDebug("Retrieving board with ID {BoardId}", id);
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
			_logger.LogInformation("Creating new board with id {BoardId}", board.Id);
            PersistBoardsToLocalStorage();
            return board;
        }

        // to retrieve the next state of the board
        public List<List<bool>>? GetNextState(Guid id)
        {
			_logger.LogInformation("Getting next state for board {BoardId}", id);
            
            if (!_boards.TryGetValue(id, out var board) || board.State == null)
                return null;

            var nextState = ComputeNextState(board.State);

            _boards.AddOrUpdate(id, _ => board, (_boards, existingBoard) =>
            {
                existingBoard.State = nextState;
                return existingBoard;
            });

            PersistBoardsToLocalStorage();
            return nextState;
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
            _logger.LogInformation("Getting state after {Steps} steps for board {BoardId}", steps, id);
            
            if (steps < 0)
                throw new ArgumentException("Steps must be a positive integer");

            lock (_lock)
            {
                if (!_boards.TryGetValue(id, out var board) || board.State == null)
                    return null;

                var current = DeepCopy(board.State);
                
                for (int i = 0; i < steps; i++)
                    current = ComputeNextState(current);
                
                return current;
            }
        }
        
        public List<List<bool>>? GetFinalState(Guid id, int maxIterations = 1000)
        {
			_logger.LogInformation("Searching for final state for board {BoardId} with max {MaxSteps} steps", id, maxIterations);

            if (!_boards.TryGetValue(id, out var board) || board.State == null)
                return null;

            var previous = DeepCopy(board.State);
            var current = previous;

            for (int i = 0; i < maxIterations; i++)
            {
                current = ComputeNextState(current);

                if (AreBoardsEqual(previous, current))
                    return current;

                previous = DeepCopy(current);
            }

			_logger.LogWarning("Board {BoardId} did not stabilize after {MaxSteps} steps", id, maxIterations);
            throw new InvalidOperationException("Board did not reach a stable state after 1000 steps.");
        }

    	private bool AreBoardsEqual(List<List<bool>> a, List<List<bool>> b)
        {
            if (a.Count != b.Count || a[0].Count != b[0].Count) 
                return false;

            for (int i = 0; i < a.Count; i++)
            for (int j = 0; j < a[i].Count; j++)
                if (a[i][j] != b[i][j])
                    return false;

            return true;
        }

        // private methods
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
        
        public void PersistBoardsToLocalStorage()
        {
            try
            {
                var snapshot =
                    _boards.ToDictionary(entry => entry.Key,
                        entry => entry.Value); // enclosed copy of the data to avoid race condition while seriaslizing
                
                var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_storagePath, json);
                _logger.LogInformation("Saved {Count} boards to local storage", snapshot.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error persisting boards to local storage");
            }
        }

        public void RetrieveBoardsFromLocalStorage()
        {
            if (!File.Exists(_storagePath))
                return;

            var json = File.ReadAllText(_storagePath);
            var loaded = JsonSerializer.Deserialize<Dictionary<Guid, Board>>(json);
            if (loaded != null)
            {
                foreach (var kv in loaded)
                    _boards.AddOrUpdate(kv.Key, kv.Value, (key, oldValue) => kv.Value);
            }

			_logger.LogInformation("Loaded {Count} boards from local storage", _boards.Count);
        }
    }
}