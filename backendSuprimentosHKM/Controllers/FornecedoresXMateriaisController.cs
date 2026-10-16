using backendSuprimentosHKM.Data.Context;
using backendSuprimentosHKM.Dtos;
using backendSuprimentosHKM.Dtos.Requests;
using backendSuprimentosHKM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendSuprimentosHKM.Controllers;

[Route("[controller]")]
[ApiController]
public class FornecedoresXMateriaisController(AppDbContext context, IFornecedoresXMateriaisService service) : ControllerBase
{
    private readonly IFornecedoresXMateriaisService _service = service;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<MateriaisDTO>))]
    public async Task<IActionResult> GetMateriaisAsync()
    {
        try
        {
            var resultado = await _service.GetMateriaisDTOAsync();

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                mensagem = $"Algo deu errado{ex}"
            });
        }
    }

    [HttpGet("FornecedoresDeUmMaterial")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<FornecedoresDTO>))]
    public async Task<IActionResult> GetFornecedoresAsync([FromQuery] GetFornecdoresDto request)
    {
        try
        {
            var resultado = await _service.GetFornecedoresAsync(request);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                mensagem = $"Algo deu errado{ex}"
            });
        }
    }

    //[HttpPost]
    //[Consumes("multipart/form-data")]
    //[ProducesResponseType(typeof(ResultadoImportacaoDto), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> Importar(
    //[FromForm] IFormFile? arquivo,
    //CancellationToken cancellationToken)
    //{
    //    if (arquivo is null || arquivo.Length == 0)
    //    {
    //        return BadRequest(new
    //        {
    //            mensagem = "Envie um arquivo Excel válido."
    //        });
    //    }

    //    if (!string.Equals(
    //            Path.GetExtension(arquivo.FileName),
    //            ".xlsx",
    //            StringComparison.OrdinalIgnoreCase))
    //    {
    //        return BadRequest(new
    //        {
    //            mensagem = "Apenas arquivos .xlsx são aceitos."
    //        });
    //    }

    //    try
    //    {
    //        await using var stream = arquivo.OpenReadStream();

    //        var resultado = await _service.ImportarAsync(
    //            stream, cancellationToken);

    //        return Ok(resultado);
    //    }
    //    catch (InvalidDataException ex)
    //    {
    //        return BadRequest(new
    //        {
    //            mensagem = ex.Message
    //        });
    //    }
    //    catch (IOException)
    //    {
    //        return BadRequest(new
    //        {
    //            mensagem = "Não foi possível ler o arquivo enviado."
    //        });
    //    }
    //}
}
