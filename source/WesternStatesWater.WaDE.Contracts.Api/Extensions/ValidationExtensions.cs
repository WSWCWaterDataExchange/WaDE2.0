using System;
using System.Linq;
using FluentValidation;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace WesternStatesWater.WaDE.Contracts.Api.Extensions;

public static class ValidationExtensions
{
    private const int SRID_WGS84 = 4326;
    private static readonly WKTReader WktReader = new(new NtsGeometryServices(PrecisionModel.Floating.Value, SRID_WGS84));
    
    private const string InvalidBBoxMessage =
        "Bounding box requires four values: minX, minY, maxX, and maxY, with longitudes " +
        "between {0} and {1} degrees and " +
        "latitudes between {2} and {3} degrees. " +
        "For example, \"-114.052,36.997,-109.041,42.001\"";

    private const string InvalidWktMessage =
        "Invalid '{PropertyName}'. Ensure the string is a valid POLYGON or MULTIPOLYGON WKT format, e.g., POLYGON((x y,x1 y1,x2 y2,...,xn yn x y)).";


    /// <summary>
    /// Validates that a string is a comma-separated list of exactly four numeric values, 
    /// ensuring it represents a valid bounding box (Bbox) structure.
    /// </summary>
    public static IRuleBuilderOptions<T, string> Bbox<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        double minX = -180,
        double maxX = 180,
        double minY = -90,
        double maxY = 90
    )
    {
        return ruleBuilder
            .NotEmpty()
            .Must(bbox =>
            {
                var parts = bbox.Split(',');
                return parts.Length == 4 && parts.All(v => double.TryParse(v, out _));
            })
            .WithMessage(string.Format(InvalidBBoxMessage, minX, maxX, minY, maxY));
    }

    /// <summary>
    /// Validates that the numeric values in a bounding box (Bbox) string are within the specified coordinate range.
    /// </summary>
    public static IRuleBuilderOptions<T, string> BboxInRange<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        double minX = -180,
        double maxX = 180,
        double minY = -90,
        double maxY = 90
    )
    {
        return ruleBuilder
            .Must(bboxCsv =>
            {
                var bbox = bboxCsv.Split(',').Select(double.Parse).ToArray();

                if ((bbox[0] < minX || bbox[0] > maxX) ||
                    (bbox[2] < minX || bbox[2] > maxX))
                {
                    return false;
                }

                return (bbox[1] >= minY && bbox[1] <= maxY) &&
                       bbox[3] >= minY && bbox[3] <= maxY;
            })
            .WithMessage(string.Format(InvalidBBoxMessage, minX, maxX, minY, maxY));
    }

    /// <summary>
    /// Validates that a string represents an integer within the specified inclusive range.
    /// </summary>
    public static IRuleBuilderOptions<T, string> LimitInRange<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int min,
        int max
    )
    {
        return ruleBuilder
            .NotEmpty()
            .Must(r => int.TryParse(r, out var val) && val >= min && val <= max)
            .WithMessage($"Limit must be a number between {min} and {max}.");
    }
    
    public static IRuleBuilderOptions<T, string> GreaterThan<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int min
    )
    {
        return ruleBuilder
            .NotEmpty()
            .Must(r => int.TryParse(r, out var val) && val > min)
            .WithMessage($"Limit must be a number greater than {min}.");
    }
    
    /// <summary>
    /// Validates that a string is a valid POLYGON or MULTIPOLYGON WellKnownText (WKT) string. Case insensitive.
    /// </summary>
    /// <param name="ruleBuilder"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> ValidAreaWkt<T>(
        this IRuleBuilder<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .Must(str => str.StartsWith("POLYGON", StringComparison.OrdinalIgnoreCase) ||
                         str.StartsWith("MULTIPOLYGON", StringComparison.OrdinalIgnoreCase))
            .WithMessage("'{PropertyName}' must be a valid POLYGON or MULTIPOLYGON string.")
            .Must(wkt =>
            {
                try
                {
                    WktReader.Read(wkt);
                    return true;
                }
                catch (ArgumentException)
                {
                    // Invalid WKT string
                    return false;
                }
            })
            .WithMessage(InvalidWktMessage);
    }
}