using System;
using System.Globalization;
using AutoMapper;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

/// <summary>
/// This converter will convert a string following the OGC API datetime filter https://datatracker.ietf.org/doc/html/rfc3339#section-5.6 and convert it to a DateFilter. 
/// </summary>
/// <remarks>This converter assumes the string is valid.</remarks>
public class OgcDateTimeConverter : IValueConverter<string, AccessorApi.DateRangeFilter>
{
    public AccessorApi.DateRangeFilter Convert(string sourceMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(sourceMember))
        {
            return new AccessorApi.DateRangeFilter();
        }

        var path = sourceMember.Split("/");

        if (path.Length == 1)
        {
            return new AccessorApi.DateRangeFilter
            {
                StartDate = DateTimeOffset.Parse(path[0], CultureInfo.InvariantCulture),
                EndDate = DateTimeOffset.Parse(path[0], CultureInfo.InvariantCulture)
            };
        }

        if (path[1].Length == 0 || path[1] == "..")
        {
            return new AccessorApi.DateRangeFilter
            {
                StartDate = DateTimeOffset.Parse(path[0], CultureInfo.InvariantCulture),
                EndDate = null
            };
        }

        if (path[0].Length == 0 || path[0] == "..")
        {
            return new AccessorApi.DateRangeFilter
            {
                StartDate = null,
                EndDate = DateTimeOffset.Parse(path[1], CultureInfo.InvariantCulture)
            };
        }

        return new AccessorApi.DateRangeFilter
        {
            StartDate = DateTimeOffset.Parse(path[0], CultureInfo.InvariantCulture),
            EndDate = DateTimeOffset.Parse(path[1], CultureInfo.InvariantCulture)
        };
    }
}