namespace WesternStatesWater.WaDE.Common.Contracts;

public interface IRequestHandler<in T> where T : RequestBase
{
    Task<ResponseBase> Handle(T request);
}