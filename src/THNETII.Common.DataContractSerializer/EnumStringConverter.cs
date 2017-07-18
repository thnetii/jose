using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace THNETII.Common.DataContractSerializer
{
    public static class EnumStringConverter<T> where T : struct
    {
        private static readonly Type typeRef;
        private static readonly IDictionary<string, T> stringToValue = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        private static readonly IDictionary<T, string> valueToString = new Dictionary<T, string>();

        static EnumStringConverter()
        {
            typeRef = typeof(T);
            var ti = typeRef.GetTypeInfo();
            if (!ti.IsEnum)
                throw new ArgumentException($"Type Argument must represent an Enum type", nameof(T));

            foreach (var fi in ti.DeclaredFields.Where(i => i.IsStatic))
            {
                var enumMemberAttr = fi.GetCustomAttribute<EnumMemberAttribute>();
                if (enumMemberAttr == null)
                    continue;
                T v = (T)fi.GetValue(null);
                string s = enumMemberAttr.IsValueSetExplicitly ? enumMemberAttr.Value : fi.Name;
                stringToValue[s] = v;
                valueToString[v] = s;
            }
        }

        public static T Parse(string s)
        {
            if (stringToValue.TryGetValue(s, out T value))
                return value;
            return (T)Enum.Parse(typeRef, s, ignoreCase: true);
        }

        public static bool TryParse(string s, out T value)
        {
            if (stringToValue.TryGetValue(s, out value))
                return true;
            return Enum.TryParse(s, out value);
        }

        public static string ToString(T value)
        {
            if (valueToString.TryGetValue(value, out string s))
                return s;
            return value.ToString();
        }
    }
}
