namespace WesternStatesWater.WaDE.Common.Contracts;

public abstract class ResponseBase
{
    public ErrorBase? Error { get; set; }
}

public class ErrorBase
{
    public string? PublicMessage { get; init; }
}