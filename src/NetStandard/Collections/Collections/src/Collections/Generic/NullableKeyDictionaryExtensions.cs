﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Teronis.Collections.Generic
{
    public static class NullableKeyDictionaryExtensions
    {
        [return: NotNullIfNotNull("dictionary")]
        public static IReadOnlyDictionary<KeyType, ValueType>? AsReadOnlyDictionary<KeyType, ValueType>(this IReadOnlyNullableKeyDictionary<KeyType, ValueType>? dictionary)
            where KeyType : notnull =>
            dictionary;

        [return: NotNullIfNotNull("dictionary")]
        public static IReadOnlyDictionary<StillNullable<KeyType>, ValueType>? AsReadOnlyDictionaryWithNullableKeys<KeyType, ValueType>(this IReadOnlyNullableKeyDictionary<KeyType, ValueType>? dictionary)
            where KeyType : notnull =>
            dictionary;

        [return: NotNullIfNotNull("dictionary")]
        public static IReadOnlyCollection<KeyValuePair<IStillNullable<KeyType>, ValueType>>? AsReadOnlyCollectionWithPairsHavingCovariantNullableKey<KeyType, ValueType>(this NullableKeyDictionary<KeyType, ValueType>? dictionary)
            where KeyType : notnull =>
            dictionary;
    }
}
