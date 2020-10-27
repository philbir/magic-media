using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace MagicMedia.Api.Host.Tests
{
    public class ThumbnailSizeNameValueSerializer
        : IValueSerializer
    {
        public string Name => "ThumbnailSizeName";

        public ValueKind Kind => ValueKind.Enum;

        public Type ClrType => typeof(ThumbnailSizeName);

        public Type SerializationType => typeof(string);

        public object Serialize(object value)
        {
            if (value is null)
            {
                return null;
            }

            var enumValue = (ThumbnailSizeName)value;

            switch(enumValue)
            {
                case ThumbnailSizeName.Xs:
                    return "XS";
                case ThumbnailSizeName.S:
                    return "S";
                case ThumbnailSizeName.M:
                    return "M";
                case ThumbnailSizeName.L:
                    return "L";
                case ThumbnailSizeName.SqXs:
                    return "SQ_XS";
                case ThumbnailSizeName.SqS:
                    return "SQ_S";
                default:
                    throw new NotSupportedException();
            }
        }

        public object Deserialize(object serialized)
        {
            if (serialized is null)
            {
                return null;
            }

            var stringValue = (string)serialized;

            switch(stringValue)
            {
                case "XS":
                    return ThumbnailSizeName.Xs;
                case "S":
                    return ThumbnailSizeName.S;
                case "M":
                    return ThumbnailSizeName.M;
                case "L":
                    return ThumbnailSizeName.L;
                case "SQ_XS":
                    return ThumbnailSizeName.SqXs;
                case "SQ_S":
                    return ThumbnailSizeName.SqS;
                default:
                    throw new NotSupportedException();
            }
        }

    }
}
