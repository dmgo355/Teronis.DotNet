﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace Teronis.Collections.Generic
{
    public interface ICovariantKeyValuePair<out KeyType, out ValueType>
    {
        KeyType Key { get; }
        [MaybeNull]
        ValueType Value { get; }
    }
}
