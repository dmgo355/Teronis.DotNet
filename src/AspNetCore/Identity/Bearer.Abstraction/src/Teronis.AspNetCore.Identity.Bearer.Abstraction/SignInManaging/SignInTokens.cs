﻿using System;

namespace Teronis.AspNetCore.Identity.Bearer
{
    [Serializable]
    public class SignInTokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        // TODO: Check if deserializable.
        internal SignInTokens()
        {
            AccessToken = null!;
            RefreshToken = null!;
        }

        public SignInTokens(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
