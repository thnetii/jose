using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace THNETII.Common.DataContractSerializer
{
    /// <summary>
    /// Helper class that provides Enum-String Conversions that honour the <see cref="EnumMemberAttribute"/> applied to values of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The enumeration type to convert to and from strings during serialization.</typeparam>
    /// <remarks>
    /// It is not possible to constrain <typeparamref name="T"/> to an enum, making it possible to specify any value type for <typeparamref name="T"/>.
    /// During execution of the static contructor of the <see cref="EnumStringConverter{T}"/> type, <typeparamref name="T"/> is checked and the static 
    /// constructor will throw an <see cref="ArgumentException"/>.
    /// <para>
    /// Specifying a non-enumeration type for <typeparamref name="T"/> will trigger a <see cref="TypeInitializationException"/> in the calling code
    /// where the <see cref="Exception.InnerException"/> member is set to the <see cref="ArgumentException"/> instance thrown by the static constructor of
    /// the <see cref="EnumStringConverter{T}"/> type.
    /// </para>
    /// </remarks>
    public static class EnumStringConverter<T> where T : struct
    {
        private static readonly Type typeRef = typeof(T);
        private static readonly IDictionary<string, T> stringToValue = InitializeStringToValueDictionary();
        private static readonly IDictionary<T, string> valueToString = InitializeValueToStringDictionary();

        [SuppressMessage("Microsoft.Usage", "CA2208", Target = "System.ArgumentException")]
        static void InitializeConversionDictionary(Action<string, T> dictionaryAddValueAction)
        {
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
                dictionaryAddValueAction(s, v);
            }
        }

        static IDictionary<string, T> InitializeStringToValueDictionary()
        {
            var stringToValue = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            InitializeConversionDictionary((s, v) =>
            {
                if (!stringToValue.ContainsKey(s))
                    stringToValue[s] = v;
            });
            return stringToValue;
        }

        static IDictionary<T, string> InitializeValueToStringDictionary()
        {
            var valueToString = new Dictionary<T, string>();
            InitializeConversionDictionary((s, v) =>
            {
                if (!valueToString.ContainsKey(v))
                    valueToString[v] = s;
            });
            return valueToString;
        }

        /// <summary>
        /// Converts the string representation of the constant name, serialization name or the numeric value of one
        /// or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// </summary>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <returns>The converted value as an instance of <typeparamref name="T"/>.</returns>
        /// <remarks>
        /// The serialization name refers to the value specified in for the <see cref="EnumMemberAttribute.Value"/> member of an 
        /// <see cref="EnumMemberAttribute"/> applied to one of the enumerated constants of the <typeparamref name="T"/> enumeration type.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T Parse(string s)
        {
            if (stringToValue.TryGetValue(s, out T value))
                return value;
            return (T)Enum.Parse(typeRef, s, ignoreCase: true);
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns the default value for <typeparamref name="T"/> in case the string cannot be converted.</para>
        /// </summary>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or the default value of <typeparamref name="T"/> 
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T ParseOrDefault(string s) => ParseOrDefault(s, default(T));

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns the specified alternate value in case the string cannot be converted.</para>
        /// </summary>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <param name="default">The default value to return if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or <paramref name="default"/>
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T ParseOrDefault(string s, T @default)
        {
            if (stringToValue.TryGetValue(s, out T value))
                return value;
            else if (Enum.TryParse(s, out value))
                return value;
            return @default;
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns <c>null</c> in case the string cannot be converted.</para>
        /// </summary>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or <c>null</c>
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T? ParseOrNull(string s)
        {
            if (stringToValue.TryGetValue(s, out T value))
                return value;
            else if (Enum.TryParse(s, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns <c>null</c> in case the string cannot be converted.</para>
        /// </summary>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <param name="value">The converted value of <paramref name="s"/> if the method returns <c>true</c>.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="s"/> was successfully converted to a value of <typeparamref name="T"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>If this method returns <c>false</c>, the out-value of the <paramref name="value"/> parameter is not defined.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static bool TryParse(string s, out T value)
        {
            if (stringToValue.TryGetValue(s, out value))
                return true;
            return Enum.TryParse(s, out value);
        }

        /// <summary>
        /// Returns the serialized name or the default string representation of the specified value.
        /// </summary>
        /// <param name="value">The value of <typeparamref name="T"/> to serialize.</param>
        /// <returns>
        /// A string containing either the serialization name if the constant equal to <paramref name="value"/> 
        /// has an <see cref="EnumMemberAttribute"/> applied to it; otherwise, the return value of <see cref="Enum.ToString()"/> for
        /// the specified value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static string ToString(T value)
        {
            if (valueToString.TryGetValue(value, out string s))
                return s;
            return value.ToString();
        }
    }
}
