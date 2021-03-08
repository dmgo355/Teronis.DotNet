﻿using System;
using System.Reflection;

namespace Teronis.Microsoft.JSInterop.Facade
{
    public class JSFacadeDefaults
    {
        public const BindingFlags COMPONENT_PROPERTY_BINDING_FLAGS = BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic;

        public const BindingFlags PROXY_INTERFACE__METHOD_BINDING_FLAGS = BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic;
    }
}
