﻿using System;

namespace Teronis
{
    public class CovariantTuple<T1, T2> : Tuple<T1, T2>, ICovariantTuple<T1,T2>
    {
        public CovariantTuple(T1 item1, T2 item2)
            : base(item1, item2) { }
    }
}
