using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Face;
using MagicMedia.Messaging;
using MagicMedia.Security;
using MagicMedia.Store;
using MagicMedia.Thumbnail;
using MassTransit;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MagicMedia;

public class MediaEditorService : IMediaEditorService
{
    private readonly IMediaService _mediaService;
    private readonly IMediaStore _store;
    private readonly IFaceService _faceService;
    private readonly IThumbnailService _thumbnailService;
    private readonly IWebPreviewImageService _webPreviewImageService;
    private readonly IBus _bus;

    public MediaEditorService(
        IMediaService mediaService,
        IMediaStore store,
        IFaceService faceService,
        IThumbnailService thumbnailService,
        IWebPreviewImageService webPreviewImageService,
        IBus bus)
    {
        _mediaService = mediaService;
        _store = store;
        _faceService = faceService;
        _thumbnailService = thumbnailService;
        _webPreviewImageService = webPreviewImageService;
        _bus = bus;
    }

    public async Task<Media> SaveEditedImageAsync(
        Guid id,
        string type,
        Stream stream,
        CancellationToken cancellationToken)
    {
        Media media = await _mediaService.GetByIdAsync(id, cancellationToken);
        EnsureOriginalBackup(media);

        Stream existing = _mediaService.GetMediaStream(media);
        Image origImage = await Image.LoadAsync(existing, cancellationToken);
        Image editedImage = await Image.LoadAsync(stream, cancellationToken);
        editedImage.Metadata.ExifProfile = origImage.Metadata.ExifProfile;

        await SaveAndResetNewImage(media, editedImage, cancellationToken);

        return media;
    }

    public async Task<Media> RotateMediaAsync(
        Guid id,
        int degrees,
        CancellationToken cancellationToken)
    {
        Media media = await _mediaService.GetByIdAsync(id, cancellationToken);
        EnsureOriginalBackup(media);

        Stream data = _mediaService.GetMediaStream(media);
        Image? image = await Image.LoadAsync(data, cancellationToken);

        Image rotated = image.Clone(x => x.Rotate(degrees));

        await SaveAndResetNewImage(media, rotated, cancellationToken);

        return media;
    }

    private async Task SaveAndResetNewImage(Media media, Image image, CancellationToken cancellationToken)
    {
        var originalFilename = _mediaService.GetFilename(media, MediaFileType.Original);
        File.Delete(originalFilename);

        await image.SaveAsJpegAsync(originalFilename, cancellationToken);

        var fileInfo = new FileInfo(originalFilename);
        media.Size = fileInfo.Length;

        await ResetImageAsync(media, image, cancellationToken);

        await _store.UpdateAsync(media, cancellationToken);
        await _webPreviewImageService.SavePreviewImageAsync(media, cancellationToken);

        await _bus.Publish(new MediaEditedMessage(media.Id), cancellationToken);
    }

    private async Task ResetImageAsync(
        Media media,
        Image image,
        CancellationToken cancellationToken)
    {
        media.Dimension = new MediaDimension { Height = image.Height, Width = image.Width };
        media.FaceCount = null;
        media.AISummary = new MediaAISummary();

        IEnumerable<ThumbnailResult> thumbnails = await _thumbnailService
            .GenerateAllThumbnailAsync(image, cancellationToken);

        var mediaThumbnails = thumbnails.Select(x => new MediaThumbnail
        {
            Id = Guid.NewGuid(),
            Data = x.Data,
            Format = x.Format,
            Dimensions = x.Dimensions,
            Size = x.Size
        }).ToList();

        await _store.UpdateThumbnailsAsync(media, mediaThumbnails, cancellationToken);
        await _faceService.DeleteByMediaIdAsync(media.Id, cancellationToken);
        await _store.DeleteMediaAIAsync(media.Id, cancellationToken);
    }

    private void EnsureOriginalBackup(Media media)
    {
        var originalLocation = _mediaService.GetFilename(media, MediaFileType.Original);
        var backupLocation = _mediaService.GetFilename(media, MediaFileType.OriginalBackup);

        if (!File.Exists(backupLocation))
        {
            File.Copy(originalLocation, backupLocation);
        }
    }
}
