using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MSC.API.Helpers
{
    public static class ExtensionMethods
    {
        #region Enumerable

        public static string ToCsv(this IEnumerable<int> list)
        {
            return string.Join(",", list.Select(x => x.ToString()).ToArray());
        }

        public static string ToCsv(this IEnumerable<string> list)
        {
            return string.Join(",", list.Select(x => x).ToArray());
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }

            return collection;
        }

        #endregion

        #region Object

        public static object ToDbNull(this object value)
        {
            return value ?? DBNull.Value;
        }

        public static int ToInt(this object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (value is decimal)
                return Convert.ToInt32(value);

            return ToInt(value.ToString());
        }

        public static short ToShort(this object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (value is short)
                return Convert.ToInt16(value);

            return ToShort(value.ToString());
        }

        public static long ToLong(this object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (value is decimal)
                return Convert.ToInt64(value);

            return ToInt(value.ToString());
        }

        public static decimal ToDecimal(this object value)
        {
            return value == null || value == DBNull.Value ? 0 : ToDecimal(value.ToString());
        }

        public static bool ToBool(this object value)
        {
            if (value == null || value == DBNull.Value)
                return false;

            if (value is int)
                return Convert.ToBoolean(value);

            return ToBool(value.ToString());
        }

        public static DateTime? ToDateTime(this object value)
        {
            if (value == null || value == DBNull.Value || value.ToString() == "" || value.ToString() == "From" || value.ToString() == "To")
                return null;

            return Convert.ToDateTime(value);
        }

        public static string ToShortDatestring(this object date)
        {
            if (date == DBNull.Value || date == null)
                return "";

            return ((DateTime)date).ToShortDatestring();
        }

        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static char ToChar(this object value)
        {
            return Convert.ToChar(value);
        }

        #endregion

        #region String

        public static int ToInt(this string value)
        {
            int.TryParse(value, out int ret);

            return ret;
        }

        public static int ToInt(this string value, int result)
        {
            return int.TryParse(value, out int ret)
                ? ret
                : result;
        }

        public static short ToShort(this string value)
        {
            short.TryParse(value, out short ret);

            return ret;
        }

        public static short ToShort(this string value, short result)
        {
            return short.TryParse(value, out short ret)
                ? ret
                : result;
        }

        public static long ToLong(this string value)
        {
            long.TryParse(value, out long ret);

            return ret;
        }

        public static long ToLong(this string value, long result)
        {
            return long.TryParse(value, out long ret)
                ? ret
                : result;
        }

        public static object ToDbNull(this string value)
        {
            return string.IsNullOrEmpty(value)
                ? (object)DBNull.Value
                : value;
        }

        public static decimal ToDecimal(this string value)
        {
            decimal.TryParse(value, out decimal ret);

            return ret;
        }

        public static decimal ToDecimal(this string value, decimal result)
        {
            return decimal.TryParse(value, out decimal ret) ? ret : result;
        }

        public static decimal ToCurrency(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            value = value.Replace("$", "");
            value = value.Replace(",", "");
            decimal.TryParse(value, out decimal ret);

            return ret;
        }

        public static bool ToBool(this string value)
        {
            if (value.IsInt())
                return Convert.ToBoolean(ToInt(value));

            bool.TryParse(value, out var ret);

            return ret;
        }

        public static DateTime? ToDate(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return DateTime.Parse(value);
        }

        public static object ToDbNull(this int value)
        {
            return value <= 0
                ? (object)DBNull.Value
                : value;
        }

        public static object ToDbNull(this int? value)
        {
            return !value.HasValue || value.Value <= 0
                ? (object)DBNull.Value
                : value;
        }

        public static bool IsInt(this string s)
        {
            return !string.IsNullOrEmpty(s) && s.All(char.IsNumber);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool HasChanged(this string valueA, string valueB)
        {
            if (string.IsNullOrEmpty(valueA) && string.IsNullOrEmpty(valueB))
                return false;

            return valueA != valueB;
        }

        public static string NullEmptyString(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string ToSentence(this string input)
        {
            return new string(input.ToCharArray().SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new[] { ' ', c } : new[] { c }).ToArray());
        }

        public static string RemovePunctuation(this string s)
        {
            var sb = new StringBuilder();

            foreach (var c in s.Where(c => !char.IsPunctuation(c)))
                sb.Append(c);

            return sb.ToString();
        }

        public static string SpacesToUnderscores(this string s)
        {
            return s.Replace(' ', '_');
        }

        public static bool In(this string s, StringComparison comparisonType = StringComparison.Ordinal, params string[] values)
        {
            if (values == null || values.Length == 0)
                return false;

            StringComparer comparerType;

            switch (comparisonType)
            {
                case StringComparison.CurrentCulture:
                    comparerType = StringComparer.CurrentCulture;
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                    comparerType = StringComparer.CurrentCultureIgnoreCase;
                    break;
                case StringComparison.InvariantCulture:
                    comparerType = StringComparer.InvariantCulture;
                    break;
                case StringComparison.InvariantCultureIgnoreCase:
                    comparerType = StringComparer.InvariantCultureIgnoreCase;
                    break;
                case StringComparison.OrdinalIgnoreCase:
                    comparerType = StringComparer.OrdinalIgnoreCase;
                    break;
                case StringComparison.Ordinal:
                    comparerType = StringComparer.Ordinal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null);
            }

            return values.Contains(s, comparerType);
        }

        public static bool In(this string s, params string[] values)
        {
            return s.In(StringComparison.Ordinal, values);
        }

        public static bool IsValidIp(this string ip)
        {
            if (!Regex.IsMatch(ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
                return false;

            var ips = ip.Split('.');

            if (ips.Length == 4 || ips.Length == 6)
            {
                return int.Parse(ips[0]) < 256 && int.Parse(ips[1]) < 256 & int.Parse(ips[2]) < 256 & int.Parse(ips[3]) < 256;
            }

            return false;
        }

        public static bool IsValidUrl(this string text)
        {
            var rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");

            return rx.IsMatch(text);
        }

        public static string ToMd5(this string s)
        {
            var hash = new StringBuilder();
            var md5provider = new MD5CryptoServiceProvider();
            var bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(s));

            foreach (var b in bytes)
            {
                hash.Append(b.ToString("x2"));
            }

            return hash.ToString();
        }

        #endregion

        #region Int

        public static string ToNA(this int? value)
        {
            return value.HasValue ? value.ToString() : "N/A";
        }

        #endregion

        #region Bool

        public static string ToYesNoLetter(this bool value)
        {
            return value ? "Y" : "N";
        }

        #endregion

        #region DateTime

        public static object ToDbNull(this DateTime value)
        {
            return value == DateTime.MinValue ? (object)DBNull.Value : value;
        }

        public static object ToDbNull(this DateTime? value)
        {
            return value ?? (object)DBNull.Value;
        }

        public static string ToShortDate(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToShortDatestring() : string.Empty;
        }

        public static bool IsBetween(this DateTime value, DateTime from, DateTime to)
        {
            return value >= from && value <= to;
        }

        #endregion

        #region Class

        public static T Clone<T>(this T source) where T : class
        {
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static object Copy(this object o)
        {
            var t = o.GetType();
            var properties = t.GetProperties();

            var p = t.InvokeMember("", BindingFlags.CreateInstance, null, o, null);

            foreach (var pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }

            return p;
        }

        public static Dictionary<string, object> ToDictionaryProperties(this object atype, BindingFlags flags)
        {
            if (atype == null) return new Dictionary<string, object>();
            var t = atype.GetType();
            var props = t.GetProperties(flags);
            var dict = new Dictionary<string, object>();
            foreach (PropertyInfo prp in props)
            {
                object value = prp.GetValue(atype, new object[] { });
                dict.Add(prp.Name, value);
            }

            return dict;
        }


        public static bool EqualProperties<T>(this T self, T to, params string[] ignore) where T : class
        {
            if (self == null || to == null)
                return self == to;

            var type = typeof(T);
            var ignoreList = new List<string>(ignore);

            return !(from pi
                        in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     where !ignoreList.Contains(pi.Name)
                     let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
                     let toValue = type.GetProperty(pi.Name).GetValue(to, null)
                     where selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue))
                     select selfValue)
                .Any();
        }

        public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                var element = collection.ElementAt(i);

                if (!predicate(element))
                    continue;

                collection.Remove(element);
                i--;
            }
        }

        #endregion

        #region Misc

        public static bool In(this int value, params int[] values)
        {
            return values.Any(i => value == i);
        }

        #endregion
    }
}
