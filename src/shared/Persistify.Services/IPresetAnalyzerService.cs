using System.ServiceModel;
using System.Threading.Tasks;
using Persistify.Requests.PresetAnalyzers;
using Persistify.Responses.PresetAnalyzers;
using ProtoBuf.Grpc;

namespace Persistify.Services;

[ServiceContract]
public interface IPresetAnalyzerService
{
    [OperationContract]
    ValueTask<CreatePresetAnalyzerResponse> CreatePresetAnalyzerAsync(CreatePresetAnalyzerRequest request,
        CallContext callContext);

    [OperationContract]
    ValueTask<GetPresetAnalyzerResponse> GetPresetAnalyzerAsync(GetPresetAnalyzerRequest request,
        CallContext callContext);

    [OperationContract]
    ValueTask<ListPresetAnalyzersResponse> ListPresetAnalyzersAsync(ListPresetAnalyzersRequest request,
        CallContext callContext);

    [OperationContract]
    ValueTask<DeletePresetAnalyzerResponse> DeletePresetAnalyzerAsync(DeletePresetAnalyzerRequest request,
        CallContext callContext);

    [OperationContract]
    ValueTask<ExistsPresetAnalyzerResponse> ExistsPresetAnalyzerAsync(ExistsPresetAnalyzerRequest request,
        CallContext callContext);
}
