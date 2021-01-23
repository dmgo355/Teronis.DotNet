﻿using System;
using Newtonsoft.Json;

namespace Teronis.Json.Extensions
{
    public static class StringExtensions
    {
        public static object DeserializeJson(this string str, Type type) => JsonConvert.DeserializeObject(str, type);
        public static T DeserializeJson<T>(this string str) => JsonConvert.DeserializeObject<T>(str);
        public static object DeserializeJson(this string str, Type type, JsonSerializerSettings settings) => JsonConvert.DeserializeObject(str, type, settings);
        public static T DeserializeJson<T>(this string str, JsonSerializerSettings settings) => JsonConvert.DeserializeObject<T>(str, settings);
    }
}
