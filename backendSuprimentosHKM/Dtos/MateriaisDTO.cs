namespace backendSuprimentosHKM.Dtos;

public class MateriaisDTO
{
    public int CodProduto { get; set; }
    public string CodInternoDoProduto { get; set; } = string.Empty;
    public int CodMatFornecedor { get; set; }
    public string NomeDoMaterial { get; set; } = string.Empty;
}
