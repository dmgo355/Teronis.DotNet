﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace Teronis.Windows.Forms.Extensions
{
    public static class PaddingExtensions
    {
        public static Padding Subtract(this Padding padding, Padding paddingSubtraction)
        {
            return new Padding(padding.Left - paddingSubtraction.Left, padding.Top - paddingSubtraction.Top, padding.Right - paddingSubtraction.Right, padding.Bottom - paddingSubtraction.Bottom);
        }

        public static Padding Add(this Padding padding, Padding paddingSubtraction)
        {
            return new Padding(padding.Left + paddingSubtraction.Left, padding.Top + paddingSubtraction.Top, padding.Right + paddingSubtraction.Right, padding.Bottom + paddingSubtraction.Bottom);
        }
    }
}
