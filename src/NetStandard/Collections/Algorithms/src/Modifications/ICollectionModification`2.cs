﻿using System.Collections.Generic;

namespace Teronis.Collections.Algorithms.Modifications
{
    public interface ICollectionModification<out NewItemType, out OldItemType> : ICollectionModificationParameters
    {
        new int OldIndex { get; }
        ICollectionModificationPart<NewItemType, OldItemType, OldItemType, NewItemType> OldPart { get; }
        IReadOnlyList<OldItemType>? OldItems { get; }

        new int NewIndex { get; }
        ICollectionModificationPart<NewItemType, OldItemType, NewItemType, OldItemType> NewPart { get; }
        IReadOnlyList<NewItemType>? NewItems { get; }
    }
}
