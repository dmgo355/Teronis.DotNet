﻿using Teronis.Collections.Synchronization.Example1.Models;

namespace Teronis.Collections.Synchronization.Example1.ViewModels.ModelCollections
{
    /// <summary>
    /// It holds an observable collection of <see cref="DeviceHeaderSyntheticEntity"/>. Its purpose
    /// is to have a long running synced collection of <see cref="DeviceHeaderSyntheticEntity"/>.
    /// </summary>
    public class DeviceHeaderCollectionSynchronization : SynchronizingCollectionBase<DeviceHeaderEntity, DeviceHeaderViewModel>
    {
        public DeviceHeaderViewModel? SelectedItem { get; set; }

#pragma warning disable IDE0052 // Ungelesene private Member entfernen
        private readonly AddRemoveResetBehaviourForCollectionItemByAddRemoveParents<DeviceHeaderEntity, DeviceHeaderViewModel> collectionItemParentsBehaviour;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen

        public DeviceHeaderCollectionSynchronization() =>
            collectionItemParentsBehaviour = new AddRemoveResetBehaviourForCollectionItemByAddRemoveParents<DeviceHeaderEntity, DeviceHeaderViewModel>(this);

        protected override void ConfigureItems(Options options)
        {
            options.SetSequentialSynchronizationMethod(DeviceHeaderEntityEqualityComparer.Default);
            options.SuperItemsOptions.SetItems(CollectionChangeHandler<DeviceHeaderEntity>.DecoupledItemReplacingHandler.Default);
            options.SubItemsOptions.UpdateItem = (subItem, getSuperItem) => subItem.Header = getSuperItem();
        }

        protected override DeviceHeaderViewModel CreateSubItem(DeviceHeaderEntity newItem) =>
            new DeviceHeaderViewModel(newItem);
    }
}
