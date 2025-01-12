namespace WesternStatesWater.WaDE.Engines.Contracts.Attributes;

/// <summary>
/// Used to mark a property with the name of the property to be serialized in the GeoJson "properties" object.
/// </summary>
/// <param name="name"></param>
[AttributeUsage(AttributeTargets.Property)]
public class FeaturePropertyNameAttribute(string name) : Attribute
{
    public string GetName() => name;
}