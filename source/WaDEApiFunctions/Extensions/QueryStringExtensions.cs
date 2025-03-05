using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using WesternStatesWater.Shared.DataContracts;

namespace WaDEApiFunctions.Extensions;

public static class QueryStringExtensions
{
    public static (bool, List<string>) ContainsUnmatchedParameters<T>(this NameValueCollection query) where T : RequestBase
    {
        var properties = typeof(T).GetProperties();
        var unmatchedParameters = new List<string>();

        // Filter out query string parameter key. This is attached to requests for security, but is not a property of the request.
        foreach (var key in query.AllKeys.Where(x => !x.Equals("key", StringComparison.OrdinalIgnoreCase)))
        {
            if (properties.All(p => !p.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                unmatchedParameters.Add(key);
            }
        }

        return (unmatchedParameters.Any(), unmatchedParameters);
    }
}