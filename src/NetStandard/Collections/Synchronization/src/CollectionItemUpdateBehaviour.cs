﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Teronis.Collections.Changes;
using Teronis.ObjectModel.Updates;

namespace Teronis.Collections.Synchronization
{
    public class CollectionItemUpdateBehaviour<ItemType, ContentType>
        where ItemType : IApplyContentUpdateBy<ContentType>
    {
        public INotifyCollectionChangeApplied<ItemType, ContentType> CollectionChangeAppliedNotifier { get; private set; }

        public CollectionItemUpdateBehaviour(INotifyCollectionChangeApplied<ItemType, ContentType> collectionChangeNotifier)
        {
            CollectionChangeAppliedNotifier = collectionChangeNotifier;
            CollectionChangeAppliedNotifier.CollectionChangeApplied += NotifiableCollectionContainer_CollectionChangeAppliedAsync;
        }

        private IEnumerable<UpdateWithTargetContainer<ContentType, ItemType>> getOldItemUpdateContainerIterator(ICollectionChange<ItemType, ContentType> change)
        {
            if (change.Action == NotifyCollectionChangedAction.Replace) {
                var oldItems = change.OldItems ??
                throw new ArgumentException("The old item-item-items were not given that can be processed as collection change");

                var newItems = change.NewItems ??
                    throw new ArgumentException("The new item-item-items were not given that can be processed as collection change");

                var oldItemsEnumerator = oldItems.GetEnumerator();
                var newItemsEnumerator = newItems.GetEnumerator();

                while (oldItemsEnumerator.MoveNext() && newItemsEnumerator.MoveNext()) {
                    var oldItem = oldItemsEnumerator.Current;
                    var newItem = newItemsEnumerator.Current;
                    var oldItemUpdate = new ContentUpdate<ContentType>(newItem, this);
                    var oldItemUpdateContainer = new UpdateWithTargetContainer<ContentType, ItemType>(oldItemUpdate, oldItem);
                    yield return oldItemUpdateContainer;
                }
            }
        }

        private async void NotifiableCollectionContainer_CollectionChangeAppliedAsync(object sender, CollectionChangeAppliedEventArgs<ItemType, ContentType> args)
        {
            var change = args.ItemContentChange;
            var tcs = args.AsyncEventSequence.RegisterDependency();

            try {
                foreach (var oldItemUpdateContainer in getOldItemUpdateContainerIterator(change)) {
                    await oldItemUpdateContainer.Target.ApplyContentUpdateByAsync(oldItemUpdateContainer.Update);
                }

                tcs.SetResult();
            } catch (Exception error) {
                tcs.SetException(error);
            }
        }
    }
}
