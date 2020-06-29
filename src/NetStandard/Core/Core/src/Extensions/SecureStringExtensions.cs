﻿using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Teronis.Extensions
{
    public static class SecureStringExtensions
    {
        public static string? ToUnsecureString(this SecureString securedString)
        {
            if (securedString == null) {
                throw new ArgumentNullException(nameof(securedString));
            }

            var unmanagedString = IntPtr.Zero;

            try {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securedString);
                return Marshal.PtrToStringUni(unmanagedString);
            } finally {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
