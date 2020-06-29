﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Teronis.Text.Json.Serialization
{
    public static class OnlyIncludedVariablesJsonConverter
    {
        private static JsonConverter wrap(JsonConverter converter, out IVariablesClusionHelper variablesHelper)
        {
            variablesHelper = (IVariablesClusionHelper)converter;
            return converter;
        }

        private static JsonConverter createNonGeneric(Type type, params object[] args)
        {
            var genericTemplateType = typeof(OnlyIncludedVariablesJsonConverter<>);
            var genericType = genericTemplateType.MakeGenericType(type);
            return (JsonConverter)Activator.CreateInstance(genericType, args)!;
        }

        public static JsonConverter CreateNonGeneric(Type type, out IVariablesClusionHelper variablesInclusionHelper) =>
            wrap(createNonGeneric(type), out variablesInclusionHelper);

        public static JsonConverter CreateNonGeneric(Type type, IEnumerable<KeyValuePair<Type, string>> includedVariables, out IVariablesClusionHelper variablesInclusionHelper) =>
            wrap(createNonGeneric(type, includedVariables), out variablesInclusionHelper);
    }
}
