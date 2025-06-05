using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o contexto do banco de dados
builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();

// Endpoint para criar uma tarefa (POST)
app.MapPost("/api/tarefas", async (Tarefa tarefa, AppDataContext context) =>
{
    if (string.IsNullOrWhiteSpace(tarefa.Titulo))
        return Results.BadRequest(new { mensagem = "Título é obrigatório." });

    if (tarefa.Titulo.Length > 100)
        return Results.BadRequest(new { mensagem = "Título deve ter no máximo 100 caracteres." });

    if (tarefa.DataVencimento.Date < DateTime.Now.Date)
        return Results.BadRequest(new { mensagem = "A data de vencimento não pode ser no passado." });

    var status = await context.Status.FindAsync(tarefa.StatusId);
    if (status == null)
        return Results.BadRequest(new { mensagem = "StatusId inválido." });

    context.Tarefas.Add(tarefa);
    await context.SaveChangesAsync();

    return Results.Created($"/api/tarefas/{tarefa.Id}", tarefa);
});

// Endpoint para listar todas as tarefas (GET)
app.MapGet("/api/tarefas", async (AppDataContext context) =>
{
    var tarefas = await context.Tarefas.ToListAsync();
    return Results.Ok(tarefas);
});

// Endpoint para obter tarefa por ID (GET)
app.MapGet("/api/tarefas/{id:int}", async (int id, AppDataContext context) =>
{
    var tarefa = await context.Tarefas.FindAsync(id);

    if (tarefa == null)
        return Results.NotFound(new { mensagem = "Tarefa não encontrada." });

    return Results.Ok(tarefa);
});

// Endpoint para atualizar uma tarefa (PUT)
app.MapPut("/api/tarefas/{id:int}", async (int id, Tarefa tarefaEditada, AppDataContext context) =>
{
    if (string.IsNullOrWhiteSpace(tarefaEditada.Titulo))
        return Results.BadRequest(new { mensagem = "Título é obrigatório." });

    if (tarefaEditada.Titulo.Length > 100)
        return Results.BadRequest(new { mensagem = "Título deve ter no máximo 100 caracteres." });

    if (tarefaEditada.DataVencimento.Date < DateTime.Now.Date)
        return Results.BadRequest(new { mensagem = "A data de vencimento não pode ser no passado." });

    var status = await context.Status.FindAsync(tarefaEditada.StatusId);
    if (status == null)
        return Results.BadRequest(new { mensagem = "StatusId inválido." });

    var tarefaExistente = await context.Tarefas.FindAsync(id);
    if (tarefaExistente == null)
        return Results.NotFound(new { mensagem = "Tarefa não encontrada." });

    tarefaExistente.Titulo = tarefaEditada.Titulo;
    tarefaExistente.DataVencimento = tarefaEditada.DataVencimento;
    tarefaExistente.StatusId = tarefaEditada.StatusId;

    await context.SaveChangesAsync();

    return Results.Ok(tarefaExistente);
});

app.Run();
