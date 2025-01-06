using System;

namespace WesternStatesWater.WaDE.Tests.Helpers;

/// <summary>
/// CV name generator.
/// Each CV table has different column sizes. This class generates a unique name from A-Z, then AA-ZZ, ect.
/// </summary>
public static class CvNameGenerator
{
    /// <summary>
    /// Creates a new unique name from A-Z, then AA-ZZ, ect.
    /// </summary>
    /// <param name="maxLength">The next name size cannot exceed max length.</param>
    /// <returns>Unique Name used for CV Name column.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetNextName(int index, int maxLength)
    {
        if (maxLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Max length must be greater than 0.");

        const int alphabetLength = 26;
        const char firstChar = 'A';
        
        var result = string.Empty;

        while (index >= 0)
        {
            result = (char)(firstChar + (index % alphabetLength)) + result;

            // Stop if adding another character exceeds maxLength
            if (result.Length > maxLength)
                throw new InvalidOperationException("Global index exceeded the maxLength limit.");

            index = (index / alphabetLength) - 1;
        }

        return result;
    }
}