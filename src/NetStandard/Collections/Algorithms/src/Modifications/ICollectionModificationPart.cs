﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Teronis.Collections.Algorithms.Modifications
{
    public interface ICollectionModificationPart<out OwnerNewItemType, out OwnerOldItemType, out ItemType, out OtherItemType>
    {
        ICollectionModification<OwnerNewItemType, OwnerOldItemType> Owner { get; }
        ICollectionModificationPart<OwnerNewItemType, OwnerOldItemType, OtherItemType, ItemType> OtherPart { get; }
        IReadOnlyList<ItemType>? Items { get; }
        int Index { get; }
    }
}
