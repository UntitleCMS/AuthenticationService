using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuthenticationService.Extentions;

public static partial class GuidExtention
{
    public static string ToBase64Url(this Guid id) =>
        Convert.ToBase64String(id.ToByteArray())
            .TrimEnd('=')
            .Replace("/", "-")
            .Replace("+", "_");

    public static Guid ToGuid(this string b64guidurl)
    {
        switch (b64guidurl.Length % 4)
        {
            case 2: b64guidurl += "=="; break;
            case 3: b64guidurl += "="; break;
        }
        return new Guid(Convert
            .FromBase64String(b64guidurl
            .Replace("-", "/")
            .Replace("_", "+")));
    }

}

