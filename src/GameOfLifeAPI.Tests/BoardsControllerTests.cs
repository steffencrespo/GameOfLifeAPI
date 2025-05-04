using Xunit;
using Moq;
using GameOfLifeAPI.Controllers;
using GameOfLifeAPI.Services;
using GameOfLifeAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLifeAPI.Tests;

public class BoardsControllerTests
{
    [Fact]
    public void PostBoards_ShouldReturnOkAndBoard_WhenStateIsValid()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var input = new List<List<bool>>
        {
            new() { true, false },
            new() { false, true }
        };

        var expectedBoard = new Board
        {
            Id = Guid.NewGuid(),
            State = input
        };

        mockService.Setup(s => s.CreateBoard(input)).Returns(expectedBoard);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.CreateBoard(input);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(expectedBoard, createdResult.Value);
        Assert.Equal(nameof(BoardsController.GetBoard), createdResult.ActionName);
    }
    
    [Fact]
    public void GetBoard_ShouldReturnBoard_WhenBoardExists()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var boardId = Guid.NewGuid();
        var board = new Board
        {
            Id = boardId,
            State = new List<List<bool>>
            {
                new() { true, false },
                new() { false, true }
            }
        };

        mockService.Setup(s => s.GetBoard(boardId)).Returns(board);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetBoard(boardId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(board, okResult.Value);
    }
    
    [Fact]
    public void GetBoard_ShouldReturnNotFound_WhenBoardDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var nonExistentId = Guid.NewGuid();

        mockService.Setup(s => s.GetBoard(nonExistentId)).Returns((Board?)null);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetBoard(nonExistentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public void GetNextState_ShouldReturnNextState_WhenBoardExists()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var boardId = Guid.NewGuid();
        var nextState = new List<List<bool>>
        {
            new() { false, true },
            new() { true, false }
        };

        mockService.Setup(s => s.GetNextState(boardId)).Returns(nextState);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetNextState(boardId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(nextState, okResult.Value);
    }
    
    [Fact]
    public void GetNextState_ShouldReturnNotFound_WhenBoardDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var boardId = Guid.NewGuid();

        mockService.Setup(s => s.GetNextState(boardId)).Returns((List<List<bool>>?)null);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetNextState(boardId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public void GetStateAfterSteps_ShouldReturnState_WhenBoardExists()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var boardId = Guid.NewGuid();
        var steps = 3;

        var resultingState = new List<List<bool>>
        {
            new() { false, true },
            new() { true, false }
        };

        mockService.Setup(s => s.GetStateAfterSteps(boardId, steps)).Returns(resultingState);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetStateAfterSteps(boardId, steps);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(resultingState, okResult.Value);
    }
    
    [Fact]
    public void GetStateAfterSteps_ShouldReturnNotFound_WhenBoardDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var boardId = Guid.NewGuid();
        var steps = 3;

        mockService.Setup(s => s.GetStateAfterSteps(boardId, steps)).Returns((List<List<bool>>?)null);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetStateAfterSteps(boardId, steps);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public void GetFinalState_ShouldReturnFinalState_WhenBoardStabilizes()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var boardId = Guid.NewGuid();

        var finalState = new List<List<bool>>
        {
            new() { true, false },
            new() { false, true }
        };

        mockService.Setup(s => s.GetFinalState(boardId, It.IsAny<int>())).Returns(finalState);
        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetFinalState(boardId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(finalState, okResult.Value);
    }

    [Fact]
    public void GetFinalState_ShouldReturnNotFound_WhenBoardDoesNotStabilize()
    {
        // Arrange
        var mockService = new Mock<IBoardService>();
        var boardId = Guid.NewGuid();

        mockService.Setup(s => s.GetFinalState(boardId, It.IsAny<int>()))
            .Returns((List<List<bool>>?)null);

        var controller = new BoardsController(mockService.Object);

        // Act
        var result = controller.GetFinalState(boardId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, objectResult.StatusCode);
        Assert.Equal("Unable to stabilize board after max steps", objectResult.Value);
    }
}