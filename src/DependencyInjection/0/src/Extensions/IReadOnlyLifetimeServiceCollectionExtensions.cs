﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Teronis.DependencyInjection.Extensions
{
    public static class ILifetimeServiceCollectionExtensions
    {
        public static IServiceCollection CreateServiceCollection(this IReadOnlyLifetimeServiceCollection<LifetimeServiceDescriptor> collection)
        {
            var serviceCollection = new ServiceCollection();
            var singletonsCount = collection.Count;

            for (int index = 0; index < singletonsCount; index++) {
                var lifetimeServiceDescriptor = collection[index];
                var serviceDescriptor = collection.CreateServiceDescriptor(lifetimeServiceDescriptor);
                serviceCollection.Add(serviceDescriptor);
            }

            return serviceCollection;
        }
    }
}
