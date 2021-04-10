﻿// Copyright (c) Manuel Römer.
// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// <auto-generated>
//   This code file has automatically been added by the "Teronis.Nullable" NuGet package (https://www.nuget.org/packages/Nullable).
//   Please see https://github.com/manuelroemer/Nullable for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The StaticAnalyseAttributes-folder and the [..]Attribute[.ExcludeFromCodeCoverage].cs-files don't appear in your project.
//   * The added files are immutable and can therefore not be modified by coincidence.
//   * Updating/Uninstalling the package will work flawlessly.
// </auto-generated>

#if !NULLABLE_ATTRIBUTES_DISABLE
#nullable enable
#pragma warning disable

namespace System.Diagnostics.CodeAnalysis
{
    using global::System;

#if DEBUG
    /// <summary>
    ///     Specifies that the method or property will ensure that the listed field and property members have
    ///     not-<see langword="null"/> values.
    /// </summary>
#endif
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
#if !NULLABLE_ATTRIBUTES_INCLUDE_IN_CODE_COVERAGE
    [DebuggerNonUserCode]
#endif
    internal sealed partial class MemberNotNullAttribute : Attribute
    {
#if DEBUG
        /// <summary>
        ///     Gets field or property member names.
        /// </summary>
#endif
        public string[] Members { get; }

#if DEBUG
        /// <summary>
        ///     Initializes the attribute with a field or property member.
        /// </summary>
        /// <param name="member">
        ///     The field or property member that is promised to be not-null.
        /// </param>
#endif
        public MemberNotNullAttribute(string member)
        {
            Members = new[] { member };
        }

#if DEBUG
        /// <summary>
        ///     Initializes the attribute with the list of field and property members.
        /// </summary>
        /// <param name="members">
        ///     The list of field and property members that are promised to be not-null.
        /// </param>
#endif
        public MemberNotNullAttribute(params string[] members)
        {
            Members = members;
        }
    }
}

#pragma warning restore
#nullable restore
#endif // NULLABLE_ATTRIBUTES_DISABLE
