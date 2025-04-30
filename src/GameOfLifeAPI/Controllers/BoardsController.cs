using GameOfLifeAPI.Models;
using GameOfLifeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLifeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController : ControllerBase
    {
        private readonly BoardService _boardService;

        public BoardsController(BoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpPost]
        public IActionResult UploadBoard([FromBody] bool[,] state)
        {
            var id = _boardService.UploadBoard(state);
            return CreatedAtAction(nameof(GetBoard), new { id }, new { id });
        }

        [HttpGet("{id}")]
        public IActionResult GetBoard(Guid id)
        {
            var board = _boardService.GetBoard(id);
            if (board == null)
                return NotFound();
            return Ok(board);
        }
        
    }
}