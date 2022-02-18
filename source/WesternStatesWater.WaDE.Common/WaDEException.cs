using System.Runtime.Serialization;

namespace WesternStatesWater.WaDE.Common
{
    [Serializable]
    public class WaDEException : Exception
    {
        public WaDEException()
        {
        }

        public WaDEException(string message)
            : base(message)
        {
        }

        public WaDEException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected WaDEException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}