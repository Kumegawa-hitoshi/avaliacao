using System.ComponentModel.DataAnnotations;

namespace API.Modelos;

public class Tarefa
{

    public int Id { get; set; }

    public int StatusId { get; set; }

    public string Titulo { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime DataVencimento { get; set; }

    public Status Status { get; set; }

}