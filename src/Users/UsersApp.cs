using Microsoft.OpenApi.Models;
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

var userNotFound = "������������ �� ������";
UsersDb users = new UsersDb(new ApplicationContext(DbSource.PROD));

app.MapGet("/users", () => users.Get())
.WithName("GetUsers")
.WithSummary("�������� ���� �������������")
.WithDescription("���������� ������ �������������.")
.WithOpenApi();


app.MapGet("/user", (int id) => users.Get(id))
.WithName("GetUser")
.WithSummary("�������� ������������")
.WithDescription("���������� ������ ������������ �� id.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi(operation =>
{
    operation.Responses["404"].Description = userNotFound;
    return operation;
});

app.MapPost("/user", (string name, string email) => users.Add(name, email))
.WithName("AddUser")
.WithSummary("�������� ������������")
.WithDescription("�������� ������ ������������.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi(operation =>
{
    operation.Responses["200"].Description = "������������ ��������";
    operation.Responses["400"].Description = "�������� ������ ����������� �����";
    return operation;
});

app.MapPut("/user", (int id, string name, string email) => users.Update(id, name, email))
.WithName("UpdateUser")
.WithSummary("������������� ������������� ������������")
.WithDescription("������������� ������������� ������������ �� id.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi(operation =>
{
    operation.Responses["200"].Description = "������������ ��������������";
    operation.Responses["404"].Description = userNotFound;
    return operation;
});

app.MapDelete("/user", (int id) => users.Delete(id))
.WithName("DeleteUser")
.WithSummary("������� ������������") 
.WithDescription("������� ������������ �� id.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi(operation =>
{
    operation.Responses["204"].Description = "������������ �����";
    operation.Responses["404"].Description = userNotFound;
    return operation;
});

app.Run();