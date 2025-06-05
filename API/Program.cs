using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDataContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Hello World!");

app.MapPost("/api/tarefas", async ([FromBody] Tarefa tarefa, AppDataContext context) =>
{
    if (string.IsNullOrWhiteSpace(tarefa.Titulo) || tarefa.Titulo.Length < 3)
        return Results.BadRequest(new { erro = "O título é obrigatório e deve ter no mínimo 3 caracteres." });

    if (tarefa.StatusId <= 0)
        return Results.BadRequest(new { erro = "O campo status é obrigatório." });

    if (tarefa.DataVencimento == default)
        return Results.BadRequest(new { erro = "A data de vencimento é obrigatória e deve estar no formato AAAA-MM-DD." });

    context.Tarefas.Add(tarefa);
    await context.SaveChangesAsync();

    return Results.Created($"/api/tarefas/{tarefa.Id}", tarefa);
});

app.MapGet("/api/tarefas", async (AppDataContext context) =>
{
    var tarefas = await context.Tarefas.Include(t => t.Status).ToListAsync();
    return Results.Ok(tarefas);
});

app.MapGet("/api/tarefas/{id}", async (int id, AppDataContext context) =>
{
    var tarefa = await context.Tarefas.Include(t => t.Status).FirstOrDefaultAsync(t => t.Id == id);
    return tarefa is null ? Results.NotFound(new { erro = "Tarefa não encontrada." }) : Results.Ok(tarefa);
});

app.MapPut("/api/tarefas/{id}", async (int id, [FromBody] Tarefa input, AppDataContext context) =>
{
    var tarefa = await context.Tarefas.FindAsync(id);

    if (tarefa is null)
        return Results.NotFound(new { erro = "Tarefa não encontrada." });

    if (string.IsNullOrWhiteSpace(input.Titulo) || input.Titulo.Length < 3)
        return Results.BadRequest(new { erro = "O título é obrigatório e deve ter no mínimo 3 caracteres." });

    if (input.StatusId <= 0)
        return Results.BadRequest(new { erro = "O campo status é obrigatório." });

    if (input.DataVencimento == default)
        return Results.BadRequest(new { erro = "A data de vencimento é obrigatória e deve estar no formato AAAA-MM-DD." });

    tarefa.Titulo = input.Titulo;
    tarefa.StatusId = input.StatusId;
    tarefa.DataVencimento = input.DataVencimento;

    await context.SaveChangesAsync();

    return Results.Ok(tarefa);
});

app.MapDelete("/api/tarefas/{id}", async (int id, AppDataContext context) =>
{
    var tarefa = await context.Tarefas.FindAsync(id);

    if (tarefa is null)
        return Results.NotFound(new { erro = "Tarefa não encontrada." });

    context.Tarefas.Remove(tarefa);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
