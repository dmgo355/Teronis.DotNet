﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Teronis.Collections.Generic
{
    public interface INullableKeyDictionary<KeyType, ValueType> : IDictionary<KeyType, ValueType>, IDictionary<StillNullable<KeyType>, ValueType>
        where KeyType : notnull
    {
        new ICollection<StillNullable<KeyType>> Keys { get; }
        new ICollection<ValueType> Values { get; }
        new int Count { get; }
        new bool IsReadOnly { get; }

        new void Clear();
        new IEnumerator<KeyValuePair<StillNullable<KeyType>, ValueType>> GetEnumerator();

        /// <summary>
        /// Adds an element associated with the nullable key.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException">Null key exists already.</exception>
        /// <exception cref="NotSupportedException"><see cref="INullableKeyDictionary{KeyType, ValueType}"/> is read-only.</exception>
        void Add([AllowNull] ValueType value);
        /// <summary>
        /// Removes the element associated with the nullable key.
        /// </summary>
        /// <returns>true if the element is successfully removed; otherwise, false.</returns>
        /// <exception cref="NotSupportedException"><see cref="INullableKeyDictionary{KeyType, ValueType}"/> is read-only.</exception>
        bool Remove();
    }
}
