using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace WesternStatesWater.WaDE.Managers.Api.Mapping;

public class CommaStringToListConverter : IValueConverter<string, List<string>>
{
    public List<string> Convert(string sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
        {
            return null;
        }

        return sourceMember.Split(",").Select(str => str.Trim()).ToList();
    }
}