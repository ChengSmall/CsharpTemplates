using System;

namespace Cheng.Algorithm.HashCodes
{


    internal class NullableValueTypeHashCode<TS> : BaseHashCode64<Nullable<TS>> where TS : struct
    {

        public NullableValueTypeHashCode()
        {
            p_def = BaseHashCode64<TS>.Default;
        }

        private BaseHashCode64<TS> p_def;

        public override long GetHashCode64(TS? value)
        {
            return value.HasValue ? p_def.GetHashCode64(value.Value) : 0;
        }
    }

}
