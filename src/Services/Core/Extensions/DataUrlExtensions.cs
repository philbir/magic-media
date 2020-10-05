using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Extensions
{
    public static class DataUrlExtensions
    {
        public static string ToDataUrl(this byte[] image, string type)
        {
            return $"data:image/{type};base64,{Convert.ToBase64String(image)}";
        }
    }
}
