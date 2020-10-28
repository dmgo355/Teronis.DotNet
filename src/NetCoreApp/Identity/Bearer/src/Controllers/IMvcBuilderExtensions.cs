﻿using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Teronis.Mvc;
using Teronis.Mvc.ApplicationModels;

namespace Teronis.Identity.Controllers
{
    public static partial class IMvcBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="BearerSignInController"/> to <see cref="IMvcBuilder"/>.
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <param name="applicationPartName">Sets <see cref="ApplicationPart.Name". If null the name is <see cref="TypesProvidingApplicationPart"/>.</param>
        /// <param name="configureControllerModel"></param>
        /// <returns></returns>
        public static IMvcBuilder AddBearerSignInControllers(this IMvcBuilder mvcBuilder, string? applicationPartName, 
            Action<ISelectedControllerModelConfiguration>? configureControllerModel = null)
        {
            mvcBuilder.ConfigureApplicationPartManager(setup => {
                var controllerType = typeof(BearerSignInController<Singleton>).GetTypeInfo();
                var typesProvider = TypesProvidingApplicationPart.Create(applicationPartName, controllerType);
                setup.ApplicationParts.Add(typesProvider);

                var controllerModelConfiguration = new ControllerModelConfiguration(controllerType);

                controllerModelConfiguration.AddScopedRouteConvention("api/sign-in",
                    (configuration, convention) => configuration.AddControllerConvention(convention));

                configureControllerModel?.Invoke(controllerModelConfiguration);
                mvcBuilder.Services.ApplyControllerModelConfiguration(controllerModelConfiguration);
            });

            return mvcBuilder;
        }

        /// <summary>
        /// Adds <see cref="BearerSignInController"/> to <see cref="IMvcBuilder"/>.
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <param name="configureControllerModel"></param>
        /// <returns></returns>
        public static IMvcBuilder AddBearerSignInControllers(this IMvcBuilder mvcBuilder, Action<ISelectedControllerModelConfiguration>? configureControllerModel = null) =>
            AddBearerSignInControllers(mvcBuilder, null, configureControllerModel: configureControllerModel);
    }
}
