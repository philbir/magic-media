using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using StrawberryShake;
using StrawberryShake.Http;
using StrawberryShake.Http.Subscriptions;
using StrawberryShake.Transport;

namespace MagicMedia.Api.Host.Tests
{
    public class MediaDetailsResultParser
        : JsonResultParserBase<IMediaDetails>
    {
        private readonly IValueSerializer _uuidSerializer;
        private readonly IValueSerializer _stringSerializer;
        private readonly IValueSerializer _dateTimeSerializer;
        private readonly IValueSerializer _intSerializer;
        private readonly IValueSerializer _thumbnailSizeNameSerializer;

        public MediaDetailsResultParser(IValueSerializerResolver serializerResolver)
        {
            if (serializerResolver is null)
            {
                throw new ArgumentNullException(nameof(serializerResolver));
            }
            _uuidSerializer = serializerResolver.GetValueSerializer("Uuid");
            _stringSerializer = serializerResolver.GetValueSerializer("String");
            _dateTimeSerializer = serializerResolver.GetValueSerializer("DateTime");
            _intSerializer = serializerResolver.GetValueSerializer("Int");
            _thumbnailSizeNameSerializer = serializerResolver.GetValueSerializer("ThumbnailSizeName");
        }

        protected override IMediaDetails ParserData(JsonElement data)
        {
            return new MediaDetails
            (
                ParseMediaDetailsMediaById(data, "mediaById")
            );

        }

        private IMedia ParseMediaDetailsMediaById(
            JsonElement parent,
            string field)
        {
            JsonElement obj = parent.GetProperty(field);

            return new Media
            (
                DeserializeUuid(obj, "id"),
                DeserializeNullableString(obj, "filename"),
                DeserializeNullableDateTime(obj, "dateTaken"),
                DeserializeNullableUuid(obj, "cameraId"),
                ParseMediaDetailsMediaByIdDimension(obj, "dimension"),
                ParseMediaDetailsMediaByIdCamera(obj, "camera"),
                ParseMediaDetailsMediaByIdFaces(obj, "faces"),
                ParseMediaDetailsMediaByIdThumbnail(obj, "thumbnail")
            );
        }

        private ICamera ParseMediaDetailsMediaByIdCamera(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new Camera
            (
                DeserializeUuid(obj, "id"),
                DeserializeNullableString(obj, "model"),
                DeserializeNullableString(obj, "make")
            );
        }

        private IMediaDimension ParseMediaDetailsMediaByIdDimension(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new MediaDimension
            (
                DeserializeInt(obj, "height"),
                DeserializeInt(obj, "width")
            );
        }

        private IReadOnlyList<IMediaFace> ParseMediaDetailsMediaByIdFaces(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            int objLength = obj.GetArrayLength();
            var list = new IMediaFace[objLength];
            for (int objIndex = 0; objIndex < objLength; objIndex++)
            {
                JsonElement element = obj[objIndex];
                list[objIndex] = new MediaFace
                (
                    DeserializeUuid(element, "id"),
                    ParseMediaDetailsMediaByIdFacesBox(element, "box"),
                    ParseMediaDetailsMediaByIdFacesThumbnail(element, "thumbnail"),
                    ParseMediaDetailsMediaByIdFacesPerson(element, "person"),
                    DeserializeNullableUuid(element, "personId")
                );

            }

            return list;
        }

        private IMediaThumbnail ParseMediaDetailsMediaByIdThumbnail(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new MediaThumbnail
            (
                DeserializeUuid(obj, "id"),
                DeserializeThumbnailSizeName(obj, "size"),
                DeserializeNullableString(obj, "dataUrl"),
                ParseMediaDetailsMediaByIdThumbnailDimensions(obj, "dimensions")
            );
        }

        private IImageBox ParseMediaDetailsMediaByIdFacesBox(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new ImageBox
            (
                DeserializeInt(obj, "left"),
                DeserializeInt(obj, "top"),
                DeserializeInt(obj, "right"),
                DeserializeInt(obj, "bottom")
            );
        }

        private IPerson ParseMediaDetailsMediaByIdFacesPerson(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new Person
            (
                DeserializeUuid(obj, "id"),
                DeserializeNullableString(obj, "name"),
                DeserializeNullableDateTime(obj, "dateOfBirth"),
                DeserializeNullableString(obj, "group")
            );
        }

        private IMediaThumbnail1 ParseMediaDetailsMediaByIdFacesThumbnail(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new MediaThumbnail1
            (
                DeserializeUuid(obj, "id")
            );
        }

        private IMediaDimension1 ParseMediaDetailsMediaByIdThumbnailDimensions(
            JsonElement parent,
            string field)
        {
            if (!parent.TryGetProperty(field, out JsonElement obj))
            {
                return null;
            }

            if (obj.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new MediaDimension1
            (
                DeserializeInt(obj, "height"),
                DeserializeInt(obj, "width")
            );
        }

        private System.Guid DeserializeUuid(JsonElement obj, string fieldName)
        {
            JsonElement value = obj.GetProperty(fieldName);
            return (System.Guid)_uuidSerializer.Deserialize(value.GetString());
        }

        private string DeserializeNullableString(JsonElement obj, string fieldName)
        {
            if (!obj.TryGetProperty(fieldName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return (string)_stringSerializer.Deserialize(value.GetString());
        }

        private System.DateTimeOffset? DeserializeNullableDateTime(JsonElement obj, string fieldName)
        {
            if (!obj.TryGetProperty(fieldName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return (System.DateTimeOffset?)_dateTimeSerializer.Deserialize(value.GetString());
        }

        private System.Guid? DeserializeNullableUuid(JsonElement obj, string fieldName)
        {
            if (!obj.TryGetProperty(fieldName, out JsonElement value))
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return (System.Guid?)_uuidSerializer.Deserialize(value.GetString());
        }
        private int DeserializeInt(JsonElement obj, string fieldName)
        {
            JsonElement value = obj.GetProperty(fieldName);
            return (int)_intSerializer.Deserialize(value.GetInt32());
        }
        private ThumbnailSizeName DeserializeThumbnailSizeName(JsonElement obj, string fieldName)
        {
            JsonElement value = obj.GetProperty(fieldName);
            return (ThumbnailSizeName)_thumbnailSizeNameSerializer.Deserialize(value.GetString());
        }
    }
}
