﻿using Teronis.AspNetCore.Identity.Authentication.Utils;
using Teronis.AspNetCore.Identity.Entities;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace Teronis.AspNetCore.Identity.Authentication.Extensions
{
    public static class BasicAuthenticationEventsExtensions
    {
        /// <summary>
        /// The property <see cref="BasicAuthenticationEvents.OnValidatePrincipal"/> will be overridden.
        /// </summary>
        public static BasicAuthenticationEvents UseAuthenticateWhenValidatePrincipal<UserType>(this BasicAuthenticationEvents events)
            where UserType : class, IBearerUserEntity
        {
            events.OnValidatePrincipal = BasicAuthenticationEventsUtils.ValidatePrincipal<UserType>;
            return events;
        }
    }
}
