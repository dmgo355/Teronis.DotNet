﻿using System.Threading.Tasks;

namespace Teronis.Microsoft.JSInterop.Facade
{
    public interface IJSFacadesInitializer
    {
        /// <summary>
        /// Initializes the properties of the component that are decorated with a facade attribute.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        ValueTask<IJSFacades> InitializeFacadesAsync(object component);
        /// <summary>
        /// Creates a container for facades.
        /// </summary>
        /// <returns></returns>
        ValueTask<IJSFacades> InitializeFacadesAsync();
    }
}
