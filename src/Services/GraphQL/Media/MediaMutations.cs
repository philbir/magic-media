using HotChocolate.AspNetCore.Authorization;
using MagicMedia.Authorization;
using MagicMedia.Operations;
using MagicMedia.Store;

namespace MagicMedia.GraphQL;

[ExtendObjectType(RootTypes.Mutation)]
[Authorize(Apply = ApplyPolicy.BeforeResolver, Policy = AuthorizationPolicies.Names.MediaEdit)]
public class MediaMutations
{
    private readonly IMediaOperationsService _operationsService;
    private readonly IMediaAIService _cloudAIMediaProcessing;

    public MediaMutations(
        IMediaOperationsService operationsService,
        IMediaAIService cloudAIMediaProcessing)
    {
        _operationsService = operationsService;
        _cloudAIMediaProcessing = cloudAIMediaProcessing;
    }

    public async Task<MediaOperationPayload> MoveMediaAsync(
        MoveMediaRequest request,
        CancellationToken cancellationToken)
    {
        request.OperationId = request.OperationId ?? Guid.NewGuid().ToString("N");
        await _operationsService.MoveMediaAsync(request, cancellationToken);

        return new MediaOperationPayload(request.OperationId);
    }

    public async Task<ToggleMediaFavoritePayload> ToggleMediaFavoriteAsync(
        ToggleMediaFavoriteInput input, CancellationToken cancellationToken)
    {
        await _operationsService.ToggleFavoriteAsync(
            input.Id,
            input.IsFavorite,
            cancellationToken);

        return new ToggleMediaFavoritePayload(input.Id, input.IsFavorite);
    }

    public async Task<MediaOperationPayload> RecycleMediaAsync(
        RecycleMediaRequest input,
        CancellationToken cancellationToken)
    {
        RecycleMediaRequest request = input with { OperationId = input.OperationId ?? Guid.NewGuid().ToString("N") };

        await _operationsService.RecycleAsync(request, cancellationToken);

        return new MediaOperationPayload(request.OperationId);
    }

    public async Task<MediaOperationPayload> DeleteMediaAsync(
        DeleteMediaRequest input,
        CancellationToken cancellationToken)
    {
        DeleteMediaRequest request = input with { OperationId = input.OperationId ?? Guid.NewGuid().ToString("N") };

        await _operationsService.DeleteAsync(request, cancellationToken);

        return new MediaOperationPayload(request.OperationId);
    }

    public async Task<MediaOperationPayload> UpdateMediaMetadataAsync(
        UpdateMediaMetadataRequest input,
        CancellationToken cancellationToken)
    {
        input.OperationId = input.OperationId ?? Guid.NewGuid().ToString("N");
        await _operationsService.UpdateMetadataAsync(input, cancellationToken);

        return new MediaOperationPayload(input.OperationId);
    }

    public async Task<MediaOperationPayload> ReScanFacesAsync(
        RescanFacesRequest input,
        CancellationToken cancellationToken)
    {
        RescanFacesRequest request = input with { OperationId = input.OperationId ?? Guid.NewGuid().ToString("N") };

        await _operationsService.ReScanFacesAsync(request, cancellationToken);

        return new MediaOperationPayload(request.OperationId);
    }

    public async Task<AnalyseMediaPayload> AnalyseMediaAsync(
        AnalyseMediaInput input,
        CancellationToken cancellationToken)
    {
        MediaAI? mediaAi = await _cloudAIMediaProcessing
            .AnalyseMediaAsync(input.Id, cancellationToken);

        return new AnalyseMediaPayload(mediaAi);
    }

    public async Task<ExportMediaPayload> QuickExportMediaAsync(
        [Service] IMediaExportService service,
        QuickExportMediaInput input,
        CancellationToken cancellationToken)
    {
        MediaExportResult export = await service.ExportAsync(
            input.Id,
            new MediaExportOptions(),
            cancellationToken);

        return new ExportMediaPayload(export.Path);
    }

    public async Task<MediaOperationPayload> ExportMediaAsync(
        ExportMediaRequest input,
        CancellationToken cancellationToken)
    {
        ExportMediaRequest request = input with { OperationId = input.OperationId ?? Guid.NewGuid().ToString("N") };

        await _operationsService.ExportAsync(request, cancellationToken);

        return new MediaOperationPayload(request.OperationId);
    }

    public async Task<ExecuteMediaRepairPayload> ExecuteMediaRepairAsync(
        [Service] IMediaRepairService service,
        RepairMediaRequest input,
        CancellationToken cancellationToken)
    {
        Media media = await service.ExecuteRepairAsync(input, cancellationToken);

        return new ExecuteMediaRepairPayload(media);
    }

    public async Task<RotateMediaPayload> RotateMediaAsync(
        [Service] IMediaEditorService service,
        RotateMediaInput input,
        CancellationToken cancellationToken)
    {
        Media media = await service.RotateMediaAsync(
            input.Id,
            input.Degrees,
            cancellationToken);

        return new RotateMediaPayload(media);
    }
}

public record RotateMediaInput(Guid Id, int Degrees);

public class RotateMediaPayload
{
    public RotateMediaPayload(Media media)
    {
        Media = media;
    }

    public Media Media { get; set; }
}

public class ExecuteMediaRepairPayload
{
    public ExecuteMediaRepairPayload(Media media)
    {
        Media = media;
    }

    public Media Media { get; set; }
}

public record QuickExportMediaInput(Guid Id);

public record ExportMediaPayload(string Path);
