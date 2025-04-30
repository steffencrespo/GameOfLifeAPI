namespace GameOfLifeAPI.Models
{
    public class Board
    {
        public Guid Id { get; set; }
        public bool[,]? State { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}