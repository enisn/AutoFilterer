var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Source=todos.db";
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

await EnsureDb(connectionString);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "Hello World!");
app.MapGet("/hello", () => new { Hello = "World" });
app.MapGet("/hello/{name}", (string name) => new { Message = "Hello " + name });


app.MapGet("/todos", async (http) =>
    await http.Response.WriteAsJsonAsync(
        await http.RequestServices.GetService<TodoDbContext>().Todos.ToListAsync()
        )
    );

app.Run();

Task EnsureDb(string connectionString)
{
    var options = new DbContextOptionsBuilder<TodoDbContext>().UseSqlite(connectionString).Options;
    using var db = new TodoDbContext(options);
    return db.Database.EnsureCreatedAsync();
}

class Todo
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public bool IsComplete { get; set; }
}

class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}
