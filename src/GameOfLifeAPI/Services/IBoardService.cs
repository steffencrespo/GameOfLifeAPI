namespace GameOfLifeAPI.Services;

using GameOfLifeAPI.Models;

public interface IBoardService
{
    Board CreateBoard(List<List<bool>> state);
    Board? GetBoard(Guid id);
    List<List<bool>>? GetNextState(Guid id);
    List<List<bool>>? GetStateAfterSteps(Guid id, int steps);
    List<List<bool>>? GetFinalState(Guid id, int maxIterations = 1000);
    void PersistBoardsToLocalStorage();
    void RetrieveBoardsFromLocalStorage();
}