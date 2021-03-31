﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Teronis.Microsoft.JSInterop.Interception
{
    public class JSIteratingInterceptorBuilderOptions<DerivedType>
        where DerivedType : JSIteratingInterceptorBuilderOptions<DerivedType>
    {
        internal IServiceProvider ServiceProvider {
            set => serviceProvider = value;
        }

        internal virtual JSIteratingInterceptorBuilder InterceptorBuilder { get; }

        private IServiceProvider? serviceProvider;
        private IJSObjectInterceptor? interceptor;

        public JSIteratingInterceptorBuilderOptions() =>
            InterceptorBuilder = new JSIteratingInterceptorBuilder();

        /// <summary>
        /// Configures an implementation of <see cref="IJSIteratingInterceptorBuilder"/>
        /// to create an implementation of <see cref="IJSObjectInterceptor"/> for being used as 
        /// <see cref="interceptor"/> when it is null.
        /// </summary>
        /// <param name="configure"></param>
        public DerivedType ConfigureInterceptorBuilder(Action<IJSIteratingInterceptorBuilder> configure)
        {
            configure?.Invoke(InterceptorBuilder);
            return (DerivedType)this;
        }

        private IServiceProvider GetServiceProvider() =>
            serviceProvider ?? throw new InvalidOperationException("Service provider has not been set.");

        public IJSObjectInterceptor BuildInterceptor(
            Action<IJSIteratingInterceptorBuilder>? configureBuilder)
        {
            if (configureBuilder is null && !(interceptor is null)) {
                return interceptor;
            }

            var serviceProvider = GetServiceProvider();

            if (configureBuilder is null) {
                return InterceptorBuilder.Build(serviceProvider);
            }

            var mutatingInterceptorBuilder = new JSIteratingInterceptorBuilder(InterceptorBuilder.InterceptorDescriptors);
            mutatingInterceptorBuilder.SetRegistrationPhase(InterceptorDescriptorRegistrationPhase.FacadeActivation);
            configureBuilder(mutatingInterceptorBuilder);
            return mutatingInterceptorBuilder.Build(serviceProvider);
        }
    }
}
