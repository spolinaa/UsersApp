using UsersApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

UsersDb users = new UsersDb(new ApplicationContext(DbSource.PROD));

app.MapGet("/users", () => users.Get())
.WithName("GetUsers")
.WithOpenApi();

app.MapGet("/user", (int id) => users.Get(id))
.WithName("GetUser")
.WithOpenApi();

app.MapPost("/user", (string name, string email) => users.Add(name, email))
.WithName("AddUser")
.WithOpenApi();

app.MapPut("/user", (int id, string name, string email) => users.Update(id, name, email))
.WithName("UpdateUser")
.WithOpenApi();

app.MapDelete("/user", (int id) => users.Delete(id))
.WithName("DeleteUser")
.WithOpenApi();

app.Run();