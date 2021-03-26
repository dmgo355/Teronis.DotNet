﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Teronis.Mvc.ServiceResulting.Extensions
{
    public static class IServiceResultFactoryExtensions
    {
        [return: NotNullIfNotNull("serviceResultPostConfiguration")]
        public static IServiceResultPostConfiguration? WithHttpStatusCode<ServiceResultPostConfigurationType>(this ServiceResultPostConfigurationType serviceResultPostConfiguration, HttpStatusCode statusCode)
            where ServiceResultPostConfigurationType : IServiceResultPostConfiguration
        {
            if (!ReferenceEquals(serviceResultPostConfiguration, default(ServiceResultPostConfigurationType))) {
                serviceResultPostConfiguration.WithStatusCode((int)statusCode);
            }

            return serviceResultPostConfiguration;
        }
    }
}
