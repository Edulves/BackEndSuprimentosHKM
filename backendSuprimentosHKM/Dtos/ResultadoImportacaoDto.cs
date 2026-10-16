namespace backendSuprimentosHKM.Dtos;

public class ResultadoImportacaoDto
{
    public int TotalLinhasLidas { get; set; }

    public int TotalImportados { get; set; }

    public int TotalErros => Erros.Count;

    public List<LinhaComErroDto> Erros { get; set; } = [];
}
