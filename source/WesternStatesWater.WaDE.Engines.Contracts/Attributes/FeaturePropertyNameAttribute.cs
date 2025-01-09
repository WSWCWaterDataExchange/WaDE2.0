namespace WesternStatesWater.WaDE.Engines.Contracts.Attributes;

public class FeaturePropertyNameAttribute(string name) : Attribute
{
    public string GetName() => name;
}