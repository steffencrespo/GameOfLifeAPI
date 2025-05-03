using GameOfLifeAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BoardService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var boardService = app.Services.GetRequiredService<BoardService>();
boardService.RetrieveBoardsFromLocalStorage(); // to fetch board data from local file 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
