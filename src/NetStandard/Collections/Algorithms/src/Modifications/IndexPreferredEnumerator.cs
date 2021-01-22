﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Teronis.Collections.Specialized;

namespace Teronis.Collections.Algorithms.Modifications
{
    internal class IndexPreferredEnumerator<ItemType> : IEnumerator<ItemType>
    {
        public int CurrentLength { get; private set; }

        private readonly IEnumerator<ItemType> enumerator;

        public IndexPreferredEnumerator(IEnumerable<ItemType> enumerable, LastIndexDirectoryEntry lastIndex)
        {
            if (enumerable is YieldIteratorInfluencedReadOnlyList<ItemType> list) {
                enumerator = new IndexedEnumerator(list, lastIndex);
            } else {
                enumerator = enumerable.GetEnumerator();
            }
        }

        public ItemType Current =>
            enumerator.Current;

        public bool MoveNext()
        {
            if (enumerator.MoveNext()) {
                CurrentLength++;
                return true;
            } else {
                return false;
            }
        }

        public void Reset() =>
            enumerator.Reset();

        object? IEnumerator.Current =>
            ((IEnumerator)enumerator).Current;

        public void Dispose() =>
            enumerator.Dispose();

        public class IndexedEnumerator : IEnumerator<ItemType>
        {
            private readonly IReadOnlyList<ItemType> list;
            private readonly LastIndexDirectoryEntry lastIndex;

            public ItemType Current { get; private set; }

            public IndexedEnumerator(IReadOnlyList<ItemType> list, LastIndexDirectoryEntry lastIndex)
            {
                Current = default!;
                this.list = list;
                this.lastIndex = lastIndex;
            }

            public bool MoveNext()
            {
                if (lastIndex + 1 < list.Count) {
                    Current = list[lastIndex + 1];
                    return true;
                } else {
                    return false;
                }
            }

            public void Reset() =>
                throw new NotImplementedException();

            object IEnumerator.Current =>
                Current!;

            public void Dispose() { }
        }
    }
}
