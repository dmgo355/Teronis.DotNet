﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Teronis.DependencyInjection.Extensions
{
    public static class IReadOnlyLifetimeServiceCollectionExtensions
    {
        public static ServiceDescriptor CreateServiceDescriptor(this IReadOnlyLifetimeServiceCollection<LifetimeServiceDescriptor> collection, LifetimeServiceDescriptor descriptor)
        {
            if (!(descriptor.ImplementationInstance is null)) {
                return new ServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationInstance);
            }

            if (!(descriptor.ImplementationFactory is null)) {
                return new ServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationFactory, collection.Lifetime);
            }

            return new ServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationType!);
        }
    }
}
