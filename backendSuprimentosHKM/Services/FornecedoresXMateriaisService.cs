using backendSuprimentosHKM.Data.Context;
using backendSuprimentosHKM.Dtos;
using backendSuprimentosHKM.Dtos.Requests;
using backendSuprimentosHKM.Models;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace backendSuprimentosHKM.Services;

public class FornecedoresXMateriaisService(AppDbContext context) : IFornecedoresXMateriaisService
{
    private readonly AppDbContext _context = context;

    public async Task<ICollection<MateriaisDTO>> GetMateriaisDTOAsync()
    {
        return await _context.FornecedoresDeMateriais.Select(m => 
                        new MateriaisDTO { 
                            NomeDoMaterial = m.Produto!, 
                            CodInternoDoProduto = m.CodInterno!, 
                            CodMatFornecedor = m.CodFornecedor, 
                            CodProduto = m.CodProduto 
                            })
                            .GroupBy(m => m.CodInternoDoProduto)
                            .Select(g => g.First())
                            .ToListAsync();
    }

    public async Task<ICollection<FornecedoresDTO>> GetFornecedoresAsync(GetFornecdoresDto request)
    {
        return await _context.FornecedoresDeMateriais.Where(x => x.CodInterno!.Equals(request.CodInternoProduto)).Select(f =>
            new FornecedoresDTO
            {
                NomeFornecedor = f.Fornecedor!,
                CodFornecedor = f.CodFornecedor,
                NomeDoProduto = f.Produto!,
                CodMatFornecedor = f.CodMatNoFornecedor!,
            }
        ).Distinct().ToListAsync();
    }

    public async Task<ResultadoImportacaoDto> ImportarAsync(Stream arquivo, CancellationToken cancellationToken)
    {
        using var workbook = new XLWorkbook(arquivo);

        var planilha = workbook.Worksheet(1);
        var resultado = new ResultadoImportacaoDto();

        var ultimaLinhaUsada = planilha.LastRowUsed();

        if (ultimaLinhaUsada is null)
        {
            resultado.Erros.Add(new LinhaComErroDto
            {
                Linha = 1,
                Mensagem = "A planilha está vazia."
            });

            return resultado;
        }

        var ultimaLinha = ultimaLinhaUsada.RowNumber();

        if (ultimaLinha < 2)
        {
            resultado.Erros.Add(new LinhaComErroDto
            {
                Linha = 1,
                Mensagem = "A planilha não contém linhas de dados."
            });

            return resultado;
        }

        var cabecalhos = MapearCabecalhos(planilha);

        ValidarCabecalhos(cabecalhos);

        var relacaoFornecedorProduto = new List<FornecedorDeMateriais>();

        const int tamanhoLote = 1000;

        var lote = new List<FornecedorDeMateriais>(tamanhoLote);

        for (int linha = 2; linha <= ultimaLinha; linha++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            resultado.TotalLinhasLidas++;

            var cod_empresa = planilha.Cell(linha, cabecalhos["cod_empresa"]).GetValue<int>();
            var cod_fornecedor = planilha.Cell(linha, cabecalhos["cod_fornecedor"]).GetValue<int>();
            var cod_produto = planilha.Cell(linha, cabecalhos["cod_produto"]).GetValue<int>();
            var cod_mat_no_fornecedor = LerTexto(planilha, linha, cabecalhos["cod_mat_no_fornecedor"]);
            var cod_interno = LerTexto(planilha, linha, cabecalhos["cod_interno"]);
            var fornecedor = LerTexto(planilha, linha, cabecalhos["fornecedor"]);
            var produto = LerTexto(planilha, linha, cabecalhos["produto"]);
            var guid_linha = LerTexto(planilha, linha, cabecalhos["guid_linha"]);

            lote.Add(new FornecedorDeMateriais
            {
                CodEmpresa = cod_empresa,
                CodFornecedor = cod_fornecedor,
                CodProduto = cod_produto,
                CodMatNoFornecedor = cod_mat_no_fornecedor,
                CodInterno = cod_interno,
                Fornecedor = fornecedor,
                Produto = produto,
                GuidLinha = guid_linha
            });

            if (lote.Count >= tamanhoLote)
            {
                await _context.FornecedoresDeMateriais.AddRangeAsync(lote);

                await _context.SaveChangesAsync(cancellationToken);

                _context.ChangeTracker.Clear();
                lote.Clear();
            }
        }

        if (lote.Count > 0)
        {
            await _context.FornecedoresDeMateriais.AddRangeAsync(lote);

            await _context.SaveChangesAsync(cancellationToken);

            _context.ChangeTracker.Clear();
        }

        resultado.TotalImportados = relacaoFornecedorProduto.Count;

        return resultado;
    }

    private static Dictionary<string, int> MapearCabecalhos(IXLWorksheet planilha)
    {
        var ultimaColuna = planilha.Row(1)
            .LastCellUsed()?.Address.ColumnNumber ?? 0;

        var cabecalhos = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

        for (int coluna = 1; coluna <= ultimaColuna; coluna++)
        {
            var nome = planilha.Cell(1, coluna)
                .GetString()
                .Trim();

            if (nome.Length == 0)
                continue;

            if (!cabecalhos.TryAdd(nome, coluna))
            {
                throw new InvalidDataException(
                    $"Cabeçalho duplicado: '{nome}'.");
            }
        }

        return cabecalhos;
    }

    private static void ValidarCabecalhos(Dictionary<string, int> cabecalhos)
    {
        string[] obrigatorios =
        [
            "cod_empresa",
            "cod_fornecedor",
            "cod_produto",
            "cod_mat_no_fornecedor",
            "cod_interno",
            "fornecedor",
            "produto",
            "guid_linha"
        ];

        var ausentes = obrigatorios
            .Where(c => !cabecalhos.ContainsKey(c))
            .ToList();

        if (ausentes.Count > 0)
        {
            throw new InvalidDataException(
                "Colunas obrigatórias ausentes: " +
                string.Join(", ", ausentes));
        }
    }

    private static string LerTexto(IXLWorksheet planilha, int linha, int coluna)
    {
        return planilha.Cell(linha, coluna)
            .GetFormattedString()
            .Trim();
    }

    private static void AdicionarErro(ResultadoImportacaoDto resultado, int linha, string mensagem)
    {
        resultado.Erros.Add(new LinhaComErroDto
        {
            Linha = linha,
            Mensagem = mensagem
        });
    }
}