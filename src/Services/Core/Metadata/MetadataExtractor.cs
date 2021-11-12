using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NGeoHash;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace MagicMedia
{
    public class MetadataExtractor : IMetadataExtractor
    {
        private readonly IGeoDecoderService _geoDecoderService;
        private const string ExifDateFormat = "yyyy:MM:dd HH:mm:ss";

        public MetadataExtractor(IGeoDecoderService geoDecoderService)
        {
            _geoDecoderService = geoDecoderService;
        }

        public async Task<MediaMetadata> GetMetadataAsync(
            Stream stream,
            CancellationToken cancellationToken)
        {
            Image image = await Image.LoadAsync(stream);

            return await GetMetadataAsync(image, cancellationToken);
        }

        public async Task<MediaMetadata> GetMetadataAsync(
            Image image,
            CancellationToken cancellationToken)
        {
            var metadata = new MediaMetadata();
            metadata.Dimension = new MediaDimension
            {
                Height = image.Height,
                Width = image.Width
            };

            ExifProfile? exifProfile = image.Metadata.ExifProfile;

            if (exifProfile != null)
            {
                metadata.GeoLocation = await GetGeoLocationDataAsync(exifProfile, cancellationToken);
                metadata.Camera = GetCameraData(exifProfile);
                metadata.DateTaken = GetDateTaken(exifProfile);
                metadata.Orientation = exifProfile.GetValue(ExifTag.Orientation)?.ToString();
                metadata.ImageId = exifProfile.GetValue(ExifTag.ImageUniqueID)?.ToString();
            }

            return metadata;
        }

        private DateTime? GetDateTaken(ExifProfile exifProfile)
        {
            IExifValue<string>? originalDate = exifProfile.GetValue(ExifTag.DateTimeOriginal);
            if (originalDate != null)
            {
                DateTime dateTaken;
                if (DateTime.TryParseExact(
                    originalDate.Value.ToString(),
                    ExifDateFormat,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out dateTaken))
                {
                    return dateTaken;
                }
            }

            return null;
        }

        private CameraData? GetCameraData(ExifProfile exifProfile)
        {
            IExifValue<string> model = exifProfile.GetValue(ExifTag.Model);
            IExifValue<string> make = exifProfile.GetValue(ExifTag.Make);

            if (model?.GetValue() != null)
            {
                return new CameraData
                {
                    Model = model.ToString(),
                    Make = make.ToString()
                };
            }

            return null;
        }

        private async Task<GeoLocation?> GetGeoLocationDataAsync(
            ExifProfile exifProfile,
            CancellationToken cancellationToken)
        {
            IExifValue<Rational[]> lat = exifProfile.GetValue(ExifTag.GPSLatitude);
            IExifValue<Rational[]> lon = exifProfile.GetValue(ExifTag.GPSLongitude);
            IExifValue<string> latRef = exifProfile.GetValue(ExifTag.GPSLatitudeRef);
            IExifValue<string> lonRef = exifProfile.GetValue(ExifTag.GPSLongitudeRef);
            IExifValue<Rational> alt = exifProfile.GetValue(ExifTag.GPSAltitude);

            if (lat != null)
            {
                var gps = new GeoLocation()
                {
                    Type = "gps",
                    Point = new GeoPoint()
                };
                var latValue = ConvertToLocation(lat.Value);
                var lonValue = ConvertToLocation(lon.Value);

                if (latRef.Value.ToString() == "S")
                {
                    latValue = latValue * -1;
                }
                if (lonRef.Value.ToString() == "W")
                {
                    lonValue = lonValue * -1;
                }

                gps.Point = GeoPoint.Create(latValue, lonValue);
                if (alt != null)
                {
                    var dominator = (int)alt.Value.Denominator;
                    if (dominator == 0)
                        dominator = 1;
                    gps.Altitude = (int)alt.Value.Numerator / dominator;
                }

                gps.GeoHash = GeoHash.Encode(gps.Point.Coordinates[1], gps.Point.Coordinates[0]);
                gps.Address = await _geoDecoderService.DecodeAsync(
                    gps.Point.Coordinates[1],
                    gps.Point.Coordinates[0],
                    cancellationToken);

                return gps;
            }

            return null;
        }

        private double ConvertToLocation(Rational[] rational)
        {
            return Math.Round(
                rational[0].GetValue() + rational[1].GetValue() /
                60.0 + rational[2].GetValue() / 3600.0, 6);
        }
    }
}
