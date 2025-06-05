using System.ComponentModel.DataAnnotations;

namespace API.Modelos;

public class Tarefa
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo StatusId é obrigatório.")]
    public int StatusId { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [MinLength(3, ErrorMessage = "O título deve ter no mínimo 3 caracteres.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime DataVencimento { get; set; }

    public Status Status { get; set; }
}
