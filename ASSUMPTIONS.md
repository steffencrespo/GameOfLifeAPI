# Assumptions and Technical Decisions

This doc outlines key design decisions and assumptions I made during the implementation of this API.

---

## Assumptions

- The board is represented as a 2D list of booleans `List<List<bool>>`;
- An initial attempt was made using `bool[,]`, but caused runtime errors (`System.NotSupportedException`) deserializing multidimensional arrays, so I replaced it with the `List<List<bool>>`;
- When a new board is created it receives a `Guid` as its unique identifier;
- Boards are considered stable when two consecutive generations are equal (i.e., no changes between them);
- A maximum of 1000 iterations is allowed in the `/final` endpoint to detect board stabilization. If this limit is exceeded, the system returns an error (HTTP 500);

---

## Persistence

- No storage technology was specified, so to keep things simple I went with a JSON file to persist boards locally using `System.Text.Json`;
- The persistence takes the hard-coded name `boards.json`, and created/updated automatically as part of a `POST /boards` request;
- Boards are reloaded from the storage file via `RetrieveBoardsFromLocalStorage()` method on service startup;

---

## Testing

- Unit tests use `xUnit`;
- Critical logic paths such as:
    - `CreateBoard`
    - `GetNextState`
    - `GetStateAfterSteps`
    - Board stabilization
    - Invalid ID scenarios
- `Theory` was used to validate repeatable board evolution (e.g., blinker pattern).

---

## Project Structure

- Both the API project and the test projects are grouped under `GameOfLifeSolution.sln`;
- Project folders follow the recommented structure `Controllers/`, `Services/`, `Models/`, which is common practice;

---

## Security

- No authentication or authorization used;
- Input validation is basic — the model binder expects well-formed JSON.

---

## Notes

- Additional enhancements (e.g., Docker support, Swagger customization, async I/O) were deprioritized to focus on functional correctness, code clarity, and testability.
