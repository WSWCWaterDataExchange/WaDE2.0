using System.Runtime.Serialization;

namespace WesternStatesWater.Shared.Exceptions;

[Serializable]
public class WaDENotFoundException : WaDEException
{
    public WaDENotFoundException() : base("Record not found.")
    {
    }

    public WaDENotFoundException(string message)
        : base(message)
    {
    }

    public WaDENotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected WaDENotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}