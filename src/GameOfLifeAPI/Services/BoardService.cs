using GameOfLifeAPI.Models;

namespace GameOfLifeAPI.Services
{
    public class BoardService
    {
        private readonly Dictionary<Guid, Board> _boards = new();

        public Guid UploadBoard(bool[,] state)
        {
            var board = new Board
            {
                Id = Guid.NewGuid(),
                State = state,
                CreatedAt = DateTime.UtcNow
            };
            _boards[board.Id] = board;
            return board.Id;
        }

        public Board? GetBoard(Guid id)
        {
            _boards.TryGetValue(id, out var board);
            return board;
        }
    }
}