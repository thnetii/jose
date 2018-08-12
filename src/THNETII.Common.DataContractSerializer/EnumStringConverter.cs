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
    /// Helper class that provides Enum-String Conversions that honour the
    /// <see cref="EnumMemberAttribute"/> applied to values of the enumeration
    /// type.
    /// </summary>
    public static class EnumStringConverter
    {
        private static class EnumValues<T> where T : struct, Enum
        {
            public static readonly Type TypeRef = typeof(T);
            public static readonly IDictionary<string, T> StringToValue = InitializeStringToValueDictionary();
            public static readonly IDictionary<T, string> ValueToString = InitializeValueToStringDictionary();

            static void InitializeConversionDictionary(Action<string, T> dictionaryAddValueAction)
            {
                var ti = TypeRef.GetTypeInfo();
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
                    if (s == null)
                        return;
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
        }

        /// <summary>
        /// Converts the string representation of the constant name, serialization name or the numeric value of one
        /// or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert to.</typeparam>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <returns>The converted value as an instance of <typeparamref name="T"/>.</returns>
        /// <remarks>
        /// The serialization name refers to the value specified in for the <see cref="EnumMemberAttribute.Value"/> member of an 
        /// <see cref="EnumMemberAttribute"/> applied to one of the enumerated constants of the <typeparamref name="T"/> enumeration type.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T Parse<T>(string s) where T : struct, Enum
        {
            if (s != null && EnumValues<T>.StringToValue.TryGetValue(s, out T value))
                return value;
            return (T)Enum.Parse(EnumValues<T>.TypeRef, s, ignoreCase: true);
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns the default value for <typeparamref name="T"/> in case the string cannot be converted.</para>
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert to.</typeparam>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or the default value of <typeparamref name="T"/> 
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T ParseOrDefault<T>(string s) where T : struct, Enum =>
            ParseOrDefault<T>(s, @default: default);

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns the specified alternate value in case the string cannot be converted.</para>
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert to.</typeparam>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <param name="default">The default value to return if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or <paramref name="default"/>
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T ParseOrDefault<T>(string s, T @default) where T : struct, Enum
        {
            if (TryParse(s, out T value))
                return value;
            return @default;
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns the specified alternate value in case the string cannot be converted.</para>
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert to.</typeparam>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <param name="defaultFactory">The factory that produces the value to return if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>. Must not be <c>null</c>.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or the return value from <paramref name="defaultFactory"/>
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="defaultFactory"/> is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T ParseOrDefault<T>(string s, Func<T> defaultFactory)
            where T : struct, Enum
        {
            if (TryParse(s, out T value))
                return value;
            else if (defaultFactory == null)
                throw new ArgumentNullException(nameof(defaultFactory));
            return defaultFactory();
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns the specified alternate value in case the string cannot be converted.</para>
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert to.</typeparam>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <param name="defaultFactory">The factory that produces the value to return if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>. Must not be <c>null</c>.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or the return value from <paramref name="defaultFactory"/>
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="defaultFactory"/> is <c>null</c>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T ParseOrDefault<T>(string s, Func<string, T> defaultFactory)
            where T : struct, Enum
        {
            if (TryParse(s, out T value))
                return value;
            else if (defaultFactory == null)
                throw new ArgumentNullException(nameof(defaultFactory));
            return defaultFactory(s);
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns <c>null</c> in case the string cannot be converted.</para>
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert to.</typeparam>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <returns>
        /// The converted value as an instance of <typeparamref name="T"/>, or <c>null</c>
        /// if <paramref name="s"/> cannot be converted to <typeparamref name="T"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static T? ParseOrNull<T>(string s) where T : struct, Enum
        {
            if (TryParse(s, out T value))
                return value;
            return null;
        }

        /// <summary>
        /// Attempts to convert the string representation of the constant name, serialization name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated value of <typeparamref name="T"/>.
        /// <para>This operation is always case-insensitive using ordinal string comparison.</para>
        /// <para>Returns <c>null</c> in case the string cannot be converted.</para>
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert to.</typeparam>
        /// <param name="s">A string containing the name, serialization name or value to convert.</param>
        /// <param name="value">The converted value of <paramref name="s"/> if the method returns <c>true</c>.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="s"/> was successfully converted to a value of <typeparamref name="T"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>If this method returns <c>false</c>, the out-value of the <paramref name="value"/> parameter is not defined.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static bool TryParse<T>(string s, out T value) where T : struct, Enum
        {
            if (s != null && EnumValues<T>.StringToValue.TryGetValue(s, out value))
                return true;
            return Enum.TryParse(s, out value);
        }

        /// <summary>
        /// Returns the serialized name or the default string representation of the specified value.
        /// </summary>
        /// <typeparam name="T">The enumeration type to convert from.</typeparam>
        /// <param name="value">The value of <typeparamref name="T"/> to serialize.</param>
        /// <returns>
        /// A string containing either the serialization name if the constant equal to <paramref name="value"/> 
        /// has an <see cref="EnumMemberAttribute"/> applied to it; otherwise, the return value of <see cref="Enum.ToString()"/> for
        /// the specified value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static string ToString<T>(T value) where T : struct, Enum
        {
            if (EnumValues<T>.ValueToString.TryGetValue(value, out string s))
                return s;
            return value.ToString();
        }
    }
}
