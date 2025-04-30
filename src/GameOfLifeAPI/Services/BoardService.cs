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
    }
}