﻿// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Teronis.EntityFrameworkCore.Query
{
    internal interface IChildCollectionConstantPredicateBuilder
    {
        void AppendConcatenatedExpressionToParent();
    }
}
