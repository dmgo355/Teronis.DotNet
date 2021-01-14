﻿using System.Collections.Generic;

namespace Teronis.Collections.Generic
{
    public interface IReadOnlyNullableKeyDictionary<KeyType, ValueType> : IReadOnlyDictionary<KeyType, ValueType>, IReadOnlyDictionary<YetNullable<KeyType>, ValueType>,
        IReadOnlyCollection<KeyValuePair<IYetNullable<KeyType>, ValueType>>, ICovariantReadOnlyNullabkeKeyDictionary<KeyType, ValueType>
        where KeyType : notnull
    {
        new ValueType this[YetNullable<KeyType> key] { get; }

        new int Count { get; }
        new ICollection<YetNullable<KeyType>> Keys { get; }
        new ICollection<ValueType> Values { get; }

        new bool ContainsKey(YetNullable<KeyType> key);
        new bool TryGetValue(YetNullable<KeyType> key, out ValueType value);

        new IEnumerator<KeyValuePair<YetNullable<KeyType>, ValueType>> GetEnumerator();
    }
}
