﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Options;

namespace Teronis.Microsoft.JSInterop.Facades
{
    internal class JSFacadesActivatorOptionsPostConfiguration : IPostConfigureOptions<JSFacadesActivatorOptions>
    {
        private readonly IServiceProvider serviceProvider;

        public JSFacadesActivatorOptionsPostConfiguration(IServiceProvider serviceProvider) =>
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public void PostConfigure(string name, JSFacadesActivatorOptions options)
        {
            if (name != string.Empty) {
                return;
            }

            options.SetServiceProvider(serviceProvider);
        }
    }
}
