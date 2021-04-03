﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Teronis.DependencyInjection
{
    public interface INonSingletonServiceCollection<TNonSingletonServiceDescriptor> : ILifetimeServiceCollection<TNonSingletonServiceDescriptor>
        where TNonSingletonServiceDescriptor : NonSingletonServiceDescriptor
    { }
}
