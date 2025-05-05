using Microsoft.AspNetCore.Mvc;
using GameOfLifeAPI.Models;
using GameOfLifeAPI.Services;

namespace GameOfLifeAPI.Controllers
{
    [ApiController]
    [Route("boards")]
    public class BoardsController : ControllerBase
    {
		private readonly IBoardService _boardService;

        public BoardsController(IBoardService boardService)
        {
            _boardService = boardService;
        }

		/// <summary>
		/// Creates a new board with the given initial state.
		/// </summary>
		/// <param name="state">A 2D list representing the board's initial configuration.</param>
		/// <returns>Returns the created board with its unique ID.</returns>
		/// <response code="201">Board successfully created</response>
		/// <response code="400">Invalid input</response>
		[HttpPost]
		[ProducesResponseType(typeof(Board), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateBoard([FromBody] List<List<bool>> state)
        {
            if (state == null)
            {
                return BadRequest("State cannot be null");
            }

            var board = _boardService.CreateBoard(state);
            return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
        }

		/// <summary>
		/// Returns a board by its unique id.
		/// </summary>
		/// <param name="id">The id of the board</param>
		/// <returns>The board with its current state.</returns>
		/// <response code="200">Board found</response>
		/// <response code="404">Board not found</response>
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
        
		/// <summary>
		/// Generates the next state of the board.
		/// </summary>
		/// <param name="id">The id of the board.</param>
		/// <returns>The next state of the board.</returns>
		/// <response code="200">Successfullu computed the next state</response>
		/// <response code="404">Board not found</response>
        [HttpGet("{id}/next")]
        public IActionResult GetNextState(Guid id)
        {
            var nextSstate = _boardService.GetNextState(id);
            if (nextSstate == null)
                return NotFound();

            return Ok(nextSstate);
        }

		/// <summary>
		/// Generates the state of the board after a given number of steps.
		/// </summary>
		/// <param name="id">The id of the board.</param>
		/// <param name="steps">Number of steps forward.</param>
		/// <returns>The board state after a number of steps.</returns>
		/// <response code="200">State after the number of steps</response>
		/// <response code="404">Board not found</response>
        [HttpGet("{id}/next/{steps:int}")]
        public IActionResult GetStateAfterSteps(Guid id, int steps)
        {
            var result = _boardService.GetStateAfterSteps(id, steps);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        
		/// <summary>
		/// Generates the final stabilized state of the board.
		/// </summary>
		/// <param name="id">The id of the board.</param>
		/// <returns>The final board state, or error if the board never stabilizes after the max number of steps.</returns>
		/// <response code="200">Board stabilized</response>
		/// <response code="404">Board not found</response>
		/// <response code="400">Board did not stabilize</response>
        [HttpGet("{id}/final")]
        public IActionResult GetFinalState(Guid id)
        {
            var final = _boardService.GetFinalState(id);

            if (final == null)
                return StatusCode(404, "Unable to stabilize board after max steps");

            return Ok(final);
        }
    }
}