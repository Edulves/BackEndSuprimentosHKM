using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace backendSuprimentosHKM.Models;

public class FornecedorDeMateriais
{
    [Key]
    public int Id { get; set; }
    public int CodEmpresa { get; set; }
    public int CodFornecedor { get; set; }
    public int CodProduto { get; set; }
    public string? CodMatNoFornecedor { get; set; }
    public string? CodInterno { get; set; }
    public string? Fornecedor { get; set; }
    public string? Produto { get; set; }
    public string GuidLinha { get; set; } = string.Empty;
}