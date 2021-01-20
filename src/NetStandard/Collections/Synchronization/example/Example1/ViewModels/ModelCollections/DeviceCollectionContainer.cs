﻿using System.ComponentModel;
using Teronis.ObjectModel;

namespace Teronis.Collections.Synchronization.Example1.ViewModels.ModelCollections
{
    /// <summary>
    /// It holds an observable collection of <see cref="DeviceHeaderSyntheticEntity"/>. Its purpose
    /// is to have a long running synced collection of <see cref="DeviceHeaderSyntheticEntity"/>.
    /// </summary>
    public class DeviceCollectionSynchronisation : SyncingCollectionViewModel<DeviceViewModel, DeviceHeaderViewModel>
    {
        public DeviceViewModel SelectedItem {
            get => deviceHeaderCollectionContainer.SelectedItem?.CreateParentsCollector().SingleParent<DeviceViewModel>();
            private set => deviceHeaderCollectionContainer.SelectedItem = value.HeaderContainer;
        }

        private readonly PropertyChangedForwarder propertyChangedRelay;
        private readonly DeviceHeaderCollectionSynchronisation deviceHeaderCollectionContainer;
        private readonly CollectionSynchronisationMirror<DeviceHeaderCollectionSynchronisation.SubItemCollection> collectionModifiedImitator;
#pragma warning disable IDE0052 // Ungelesene private Member entfernen
        private readonly AddRemoveResetBehaviourForCollectionItemByAddRemoveParents<DeviceViewModel, DeviceHeaderViewModel> itemParentsBehaviour;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen

        public DeviceCollectionSynchronisation(DeviceHeaderCollectionSynchronisation deviceHeaderCollectionContainer)
        {
            propertyChangedRelay = new PropertyChangedForwarder(deviceHeaderCollectionContainer, notifyPropertyChange: OnPropertyChanged);
            propertyChangedRelay.AddPropertyChangeForwarding(nameof(DeviceHeaderCollectionSynchronisation.SelectedItem));
            collectionModifiedImitator = CreateCollectionSynchronisationMirror(deviceHeaderCollectionContainer.SubItems);
            itemParentsBehaviour = new AddRemoveResetBehaviourForCollectionItemByAddRemoveParents<DeviceViewModel, DeviceHeaderViewModel>(this);
            this.deviceHeaderCollectionContainer = deviceHeaderCollectionContainer;
        }

        protected override DeviceViewModel CreateSubItem(DeviceHeaderViewModel newItem) =>
            new DeviceViewModel(newItem);
    }
}
