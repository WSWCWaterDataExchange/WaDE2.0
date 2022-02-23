using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Contracts.Api;

namespace WaDEApiFunctions
{
    internal static class RequestDataParser
    {
        public static string GetQueryString(this HttpRequest req, string key)
        {
            return (string)req.Query[key];
        }

        public static DateTime? ParseDate(string value)
        {
            return DateTime.TryParse(value, out var date) ? date : (DateTime?)null;
        }

        public static int? ParseInt(string value)
        {
            return int.TryParse(value, out var date) ? date : (int?)null;
        }

        public static GeometryFormat? ParseGeometryFormat(string value)
        {
            if(Enum.TryParse(typeof(GeometryFormat), value, true, out var result))
            {
                return (GeometryFormat)result;
            }
            return null;
        }
    }
}
