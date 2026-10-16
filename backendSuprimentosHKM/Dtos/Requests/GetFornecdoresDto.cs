using System.ComponentModel.DataAnnotations;

namespace backendSuprimentosHKM.Dtos.Requests;

public class GetFornecdoresDto
{
    [Required]
    public string CodInternoProduto { get; set; } = string.Empty;
}
