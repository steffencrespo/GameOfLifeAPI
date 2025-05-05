using GameOfLifeAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:80");
builder.Services.AddSingleton<IBoardService, BoardService>();

// Setting swagger to generate xml comments
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

var boardService = app.Services.GetRequiredService<IBoardService>();
boardService.RetrieveBoardsFromLocalStorage(); // to fetch board data from local file 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); // ignoring https redirect to be able to run in a docker container
app.UseAuthorization();
app.MapControllers();
app.Run();
