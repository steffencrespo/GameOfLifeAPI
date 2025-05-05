using GameOfLifeAPI.Models;
using GameOfLifeAPI.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace GameOfLifeAPI.Tests
{
    public class BoardServiceTests
    {

		[Fact]
		public void GetNextState_ShouldReturnSameState_WhenStepsIsZero()
		{
		    // Arrange
    		var service = CreateService();
    		var initialState = new List<List<bool>>
   	 		{
   	     		new() { true, false },
   	     		new() { false, true }
    		};
    		
			var board = service.CreateBoard(initialState);

    		// Act
    		var result = service.GetStateAfterSteps(board.Id, 0);
	
    		// Assert
    		Assert.NotNull(result);
    		Assert.Equal(initialState.Count, result!.Count);

	    	for (int i = 0; i < initialState.Count; i++)
    		{
        		Assert.Equal(initialState[i], result[i]);
    		}
		}

		[Fact]
		public void GetFinalState_ShouldReturnError_WhenBoardDoesNotStabilize()
		{
    		// Arrange
    		var service = CreateService();
    		var blinker = new List<List<bool>>
    		{
        		new() { false, true, false },
        		new() { false, true, false },
        		new() { false, true, false }
    		};
    
    		var board = service.CreateBoard(blinker);

    		// Act & Assert
    		var exception = Assert.Throws<InvalidOperationException>(() =>
    		{
        		service.GetFinalState(board.Id);
    		});

    		Assert.Equal("Board did not reach a stable state after 1000 steps.", exception.Message);
		}

		[Fact]
		public void CreateBoard_ShouldThrowException_WhenStateIsNull()
		{
    		// Arrange
    		var service = CreateService();

    		// Act & Assert
    		var exception = Assert.Throws<ArgumentException>(() =>
    		{
        		service.CreateBoard(null!);
    		});

    		Assert.Equal("State must be a non-empty 2D list", exception.Message);
		}

		[Fact]
		public void CreateBoard_ShouldThrowException_WhenStateIsEmpty()
		{
    		// Arrange
    		var service = CreateService();
    		var emptyState = new List<List<bool>>();

    		// Act & Assert
    		var exception = Assert.Throws<ArgumentException>(() =>
    		{
        		service.CreateBoard(emptyState);
    		});

    		Assert.Equal("State must be a non-empty 2D list", exception.Message);
		}

        [Fact]
        public void GetNextState_ShouldCorrectlyComputeNextGeneration()
        {
            // Arrange
            var service = CreateService();
            var initialState = new List<List<bool>>
            {
                new() { false, true, false },
                new() { false, true, false },
                new() { false, true, false }
            };

            var board = service.CreateBoard(initialState);

            // Act
            var next = service.GetNextState(board.Id);

            // Assert
            var expected = new List<List<bool>>
            {
                new() { false, false, false },
                new() { true,  true,  true  },
                new() { false, false, false }
            };

            Assert.NotNull(next);
            Assert.Equal(expected.Count, next!.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], next[i]);
            }
        }

        [Fact]
        public void GetStateAfterSteps_ShouldReturnCorrectResult_AfterMultipleGenerations()
        {
            // Arrange
            var service = CreateService();
            var initialState = new List<List<bool>>
            {
                new() { false, true, false },
                new() { false, true, false },
                new() { false, true, false }
            };

            var board = service.CreateBoard(initialState);
            
            // Act
            var next = service.GetStateAfterSteps(board.Id, 3);
                
            // Assert
            var expected = new List<List<bool>>
            {
                new() { false, false, false },
                new() { true,  true,  true  },
                new() { false, false, false }
            };

            Assert.NotNull(next);
            Assert.Equal(expected.Count, next!.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], next[i]);
            }
        }
        
        [Fact]
        public void CreateBoard_ShouldStoreBoardWithCorrectInitialState()
        {
            // Arrange
            var service = CreateService();
            var initialState = new List<List<bool>>
            {
                new() { true, false },
                new() { false, true }
            };

            // Act
            var board = service.CreateBoard(initialState);
            var retrieved = service.GetBoard(board.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(board.Id, retrieved!.Id);
            Assert.Equal(initialState.Count, retrieved.State!.Count);
            for (int i = 0; i < initialState.Count; i++)
                Assert.Equal(initialState[i], retrieved.State[i]);
        }

        [Fact]
        public void GetBoard_ShouldReturnNull_IfBoardDoesNotExist()
        {
            // Arrange
            var service = CreateService();
            var fakeId = Guid.NewGuid();

            // Act
            var result = service.GetBoard(fakeId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void GetNextState_ShouldReturnNull_ForInvalidBoardId()
        {
            // Arrange
            var service = CreateService();
            var invalidId = Guid.NewGuid();

            // Act
            var result = service.GetNextState(invalidId);

            // Assert
            Assert.Null(result);
        }
        
        [Theory]
        [MemberData(nameof(GetBlinkerTestCases))]
        public void GetStateAfterSteps_ShouldReturnExpectedResult(List<List<bool>> initial, int steps, List<List<bool>> expected)
        {
            // Arrange
            var service = CreateService();
            var board = service.CreateBoard(initial);

            // Act
            var result = service.GetStateAfterSteps(board.Id, steps);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Count, result!.Count);
            for (int i = 0; i < expected.Count; i++)
                Assert.Equal(expected[i], result[i]);
        }
        
        public static IEnumerable<object[]> GetBlinkerTestCases()
        {
            yield return new object[]
            {
                // initial state (vertical blinker)
                new List<List<bool>>
                {
                    new() { false, true, false },
                    new() { false, true, false },
                    new() { false, true, false }
                },
                1, // steps
                // expected state (horizontal blinker)
                new List<List<bool>>
                {
                    new() { false, false, false },
                    new() { true,  true,  true  },
                    new() { false, false, false }
                }
            };

            yield return new object[]
            {
                // same initial state
                new List<List<bool>>
                {
                    new() { false, true, false },
                    new() { false, true, false },
                    new() { false, true, false }
                },
                2, // steps
                // should return to original (after full cycle)
                new List<List<bool>>
                {
                    new() { false, true, false },
                    new() { false, true, false },
                    new() { false, true, false }
                }
            };
        }
		
		private BoardService CreateService()
		{
			var mockLogger = new Mock<ILogger<BoardService>>();
			return new BoardService(mockLogger.Object);
		}
    }
}