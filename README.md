# Game of Life API

This project is a backend RESTful API that simulates Conway's Game of Life, implemented in C#. 
It supports board creation, evolution through generations, state stabilization, and simplified local persistence.

---

## Requirements

- .NET SDK 9.0

---

## Running the API

```bash
cd src/GameOfLifeAPI
dotnet run
```

API base URL: `http://localhost:80`  
Swagger UI: `http://localhost:80/swagger`

---

## Running Tests

```bash
cd src/GameOfLifeAPI.Tests
dotnet test
```

## Running Tests with Coverage Report

Test coverage reports are generated with coverlet.collector.

```bash
cd src/GameOfLifeAPI.Tests
dotnet test --collect:"XPlat Code Coverage"
```

Once the coverage report is ready, generate an HTML report with the xml file generated in TestResults/
```bash
~/.dotnet/tools/reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

And then, open the report
```bash
open coverage-report/index.html

```
---

## Endpoints

### POST /boards

Create a new board.

**Request body:**

```json
[
    [true, false, true],
    [false, true, false],
    [true, false, true]
]
```

**Response:**

```json
{
  "id": "guid"
}
```

---

### GET /boards/{id}

Returns the current state of a board given its unique ID.

---

### GET /boards/{id}/next

Returns tge next generation of the board.

---

### GET /boards/{id}/next/{steps}

Returns the board state after any given number of steps.

---

### GET /boards/{id}/final

Returns the stabilized version of the board, or returns HTTP 500 if the board is not stabilized after 1000 generations.

---

## Persistence

- Boards persisted locally to a `boards.json` file.
- Boards are reloaded on service startup.

---

## Project Structure

```
GameOfLifeSolution/
├── src/
│   ├── GameOfLifeAPI/
│   └── GameOfLifeAPI.Tests/
├── GameOfLifeSolution.sln
└── README.md
```

---

## Notes

- No authentication/authorization required;
- State is persisted using local JSON file;
- Unit tests are written using xUnit;
- See `ASSUMPTIONS.md` for key design decisions;
