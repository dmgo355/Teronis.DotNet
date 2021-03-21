﻿namespace Teronis.AspNetCore.Identity.AccountManaging.Controllers.Datransjects.Converters
{
    /// <summary>
    /// Provides role entity conversion.
    /// </summary>
    /// <typeparam name="RoleType"></typeparam>
    /// <typeparam name="RoleCreationType"></typeparam>
    public interface IConvertRole<RoleType, RoleCreationType>
    {
        /// <summary>
        /// Converts role entity to role view.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        RoleCreationType Convert(RoleType source);
    }
}
