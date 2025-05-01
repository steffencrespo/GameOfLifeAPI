using Microsoft.AspNetCore.Mvc;
using GameOfLifeAPI.Models;
using GameOfLifeAPI.Services;

namespace GameOfLifeAPI.Controllers
{
    [ApiController]
    [Route("boards")]
    public class BoardsController : ControllerBase
    {
        private readonly BoardService _boardService;

        public BoardsController(BoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpPost]
        public IActionResult CreateBoard([FromBody] List<List<bool>> state)
        {
            if (state == null)
            {
                return BadRequest("State cannot be null");
            }

            var board = _boardService.CreateBoard(state);
            return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
        }

        [HttpGet("{id}")]
        public IActionResult GetBoard(Guid id)
        {
            var board = _boardService.GetBoard(id);
            if (board == null)
            {
                return NotFound();
            }
            return Ok(board);
        }
        
        [HttpGet("{id}/next")]
        public IActionResult GetNextState(Guid id)
        {
            var nextSstate = _boardService.GetNextState(id);
            if (nextSstate == null)
                return NotFound();

            return Ok(nextSstate);
        }

        [HttpGet("{id}/next/{steps:int}")]
        public IActionResult GetStateAfterSteps(Guid id, int steps)
        {
            var result = _boardService.GetStateAfterSteps(id, steps);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}