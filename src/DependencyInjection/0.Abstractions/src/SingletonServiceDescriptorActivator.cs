﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Teronis.DependencyInjection
{
    internal sealed class SingletonServiceDescriptorActivator : DescriptorActivatorBase<SingletonServiceDescriptor>
    {
        internal protected override SingletonServiceDescriptor CreateDescriptor(ServiceDescriptor serviceDescriptor) =>
            new SingletonServiceDescriptor(serviceDescriptor);

        internal protected override SingletonServiceDescriptor CreateDescriptor(Type serviceType, Type implementationType) =>
            new SingletonServiceDescriptor(serviceType, implementationType);

        internal protected override SingletonServiceDescriptor CreateDescriptor(Type serviceType, object implementationInstance) =>
            new SingletonServiceDescriptor(serviceType, implementationInstance);

        internal protected override SingletonServiceDescriptor CreateDescriptor(Type serviceType, Func<IServiceProvider, object> implementationFactory) =>
            new SingletonServiceDescriptor(serviceType, implementationFactory);
    }
}
