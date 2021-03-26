﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Teronis.Microsoft.JSInterop.Locality;

namespace Teronis.Microsoft.JSInterop.Module
{
    public interface IJSModule : IJSLocalObject
    {
        public string ModuleNameOrPath { get; }
    }
}
