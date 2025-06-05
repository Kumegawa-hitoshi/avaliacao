using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/tarefas", async ([FromBody] Tarefa tarefa, AppDataContext context) =>
{
    // Validações
    if (string.IsNullOrWhiteSpace(tarefa.Titulo) || tarefa.Titulo.Length < 3)
    {
        return Results.BadRequest(new { erro = "O título é obrigatório e deve ter no mínimo 3 caracteres." });
    }

    if (tarefa.StatusId == 0)
    {
        return Results.BadRequest(new { erro = "O campo StatusId é obrigatório." });
    }

    if (tarefa.DataVencimento == default)
    {
        return Results.BadRequest(new { erro = "A data de vencimento é obrigatória e deve estar no formato AAAA-MM-DD." });
    }

    var statusExiste = await context.Status.AnyAsync(s => s.Id == tarefa.StatusId);
    if (!statusExiste)
    {
        return Results.BadRequest(new { erro = "StatusId inválido. Nenhum status com esse ID foi encontrado." });
    }

    context.Tarefas.Add(tarefa);
    await context.SaveChangesAsync();

    return Results.Created($"/api/tarefas/{tarefa.Id}", tarefa);
});

app.MapGet("/api/tarefas", async (AppDataContext context) =>
{
    var tarefas = await context.Tarefas
        .Include(t => t.Status) // Inclui o nome do status na resposta
        .ToListAsync();

    return Results.Ok(tarefas);
});

app.Run();
