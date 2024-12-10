namespace WesternStatesWater.WaDE.Common.Contracts;

public interface IRequestHandlerResolver
{
    IRequestHandler<T> Resolve<T>() where T : RequestBase;
}