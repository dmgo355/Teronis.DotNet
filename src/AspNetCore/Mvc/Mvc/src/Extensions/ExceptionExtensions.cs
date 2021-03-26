﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Teronis.Mvc.ServiceResulting;

namespace Teronis.AspNetCore.Identity.Extensions
{
    public static class ExceptionExtensions
    {
        [return: NotNullIfNotNull("error")]
        [return: NotNullIfNotNull("errorCode")]
        public static JsonError? ToJsonError(this Exception? error, string? errorCode = null)
        {
            if (error is null && errorCode is null) {
                return null;
            }

            return new JsonError(error, errorCode ?? JsonError.DefaultErrorCode);
        }

        [return: NotNullIfNotNull("error")]
        [return: NotNullIfNotNull("errorCode")]
        public static JsonErrors? ToJsonErrors(this Exception? error, string? errorCode = null)
        {
            var jsonError = ToJsonError(error, errorCode);

            if (jsonError is null) {
                return null;
            }

            return new JsonErrors(jsonError);
        }
    }
}
