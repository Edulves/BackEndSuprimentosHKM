using backendSuprimentosHKM.Dtos;
using backendSuprimentosHKM.Dtos.Requests;

namespace backendSuprimentosHKM.Services;

public interface IFornecedoresXMateriaisService
{
    Task<ResultadoImportacaoDto> ImportarAsync(Stream arquivo, CancellationToken cancellationToken);
    Task<ICollection<MateriaisDTO>> GetMateriaisDTOAsync();
    Task <ICollection<FornecedoresDTO>> GetFornecedoresAsync(GetFornecdoresDto request);
}
