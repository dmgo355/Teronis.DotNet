﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.



namespace Teronis.Threading.Tasks
{
    public interface IHasAsyncableEventSequence<KeyType>
        where KeyType : notnull
    {
        AsyncEventSequence<KeyType> AsyncEventSequence { get; }
    }
}
