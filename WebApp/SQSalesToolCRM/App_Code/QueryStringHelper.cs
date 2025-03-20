// This class is designed to improve the structure of the application 
// Provides the model base query string manipulation
// You can use either HttpRequest.ReadQueryStringAs<T> method to support model base query parsing or
// QueryString class to manipulate direct uri 
// Design By Muzammil H:
// Date Created:27 April 2014
using System.Linq;
using System.Web;

//MH
namespace SalesTool.Web
{

    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.Collections.Specialized;
    using System;
    public class UrlKeyValue : Attribute
    {
        string _name;
        public UrlKeyValue(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public bool IsIgnoreCase
        {
            get;
            set;
        }

        public bool IsIgnoreOnNull
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Convert Dictionary to Storgly typed class
    /// </summary>
    /// <typeparam name="T">Class type</typeparam>
    class DictionaryConverter<T> where T : class, new()
    {
        public T ToObject(Dictionary<string, string> value)
        {
            T t = new T();

            var properties = t.GetType().GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes(true).OfType<UrlKeyValue>().FirstOrDefault();

                string key = attribute != null ? attribute.Name : property.Name;


                if (attribute != null && attribute.IsIgnoreCase)
                {
                    if (value.Keys.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        key = value.Keys.FirstOrDefault(item => item.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                        object val = ConvertValue(value[key], property.PropertyType);
                        property.SetValue(t, val, null);
                    }
                }
                else
                {
                    if (value.ContainsKey(key))
                    {
                        object val = ConvertValue(value[key], property.PropertyType);
                        property.SetValue(t, val, null);
                    }
                }

            }

            return t;
        }

        private object ConvertValue(string s, Type type)
        {
            MethodInfo openMethod = typeof(ExtensionMethods).GetMethod("ConvertOrDefault");
            MethodInfo typedMethod = openMethod.MakeGenericMethod(type);
            var a = typedMethod.Invoke(null, new object[] { s });
            return a;
        }

        public Dictionary<string, string> ToDictionary(T t)
        {
            PropertyInfo[] properties = t.GetType().GetProperties();

            var dictionary = new Dictionary<string, string>(properties.Count());

            foreach (PropertyInfo prop in properties)
            {
                var attribute = prop.GetCustomAttributes(true).OfType<UrlKeyValue>().FirstOrDefault();

                string key = attribute != null ? attribute.Name : prop.Name;

                var value = prop.GetValue(t, null);

                if (attribute != null && attribute.IsIgnoreOnNull)
                {
                    if (value != null)
                    {
                        dictionary.Add(key, (string)value);
                    }
                }
                else
                {
                    dictionary.Add(key, (string)value);
                }
            }

            return dictionary;
        }
    }

    /// <summary>
    /// Serialize and Deserialize key value string
    /// </summary>
    /// <typeparam name="T">class type</typeparam>
    class KeyValueStringSerializer<T> where T : class, new()
    {
        DictionaryConverter<T> _dictionaryConverter = new DictionaryConverter<T>();

        public T Deserialize(string keyValueString)
        {
            var dictionary = keyValueString.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);

            T t = _dictionaryConverter.ToObject(dictionary);

            return t;
        }

        public string Serialize(T t)
        {
            var dictionary = _dictionaryConverter.ToDictionary(t);

            return string.Join("&", dictionary.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
        }
    }

    class NameValueCollectionSerializer<T> where T : class, new()
    {
        DictionaryConverter<T> _dictionaryConverter = new DictionaryConverter<T>();

        public T Deserialize(NameValueCollection value)
        {
            var dictionary = value.ToDictionary();

            return _dictionaryConverter.ToObject(dictionary);
        }
    }
  
    /// <summary>
    /// A chainable query string helper class.
    /// Example usage :
    /// string strQuery = QueryString.Current.Add("id", "179").ToString();
    /// string strQuery = new QueryString().Add("id", "179").ToString();
    /// </summary>
    public class QueryString : NameValueCollection
    {
        public QueryString() { }

        public QueryString(string queryString)
        {
            FillFromString(queryString);
        }

        public static QueryString Current
        {
            get
            {
                return new QueryString().FromCurrent();
            }
        }

        /// <summary>
        /// extracts a querystring from a full URL
        /// </summary>
        /// <param name="s">the string to extract the querystring from</param>
        /// <returns>a string representing only the querystring</returns>
        public string ExtractQueryString(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Contains("?"))
                    return s.Substring(s.IndexOf("?") + 1);
            }
            return s;
        }

        /// <summary>
        /// returns a querystring object based on a string
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the QueryString object </returns>
        public QueryString FillFromString(string s)
        {
            base.Clear();
            if (string.IsNullOrEmpty(s)) return this;
            foreach (string keyValuePair in ExtractQueryString(s).Split('&'))
            {
                if (string.IsNullOrEmpty(keyValuePair)) continue;
                string[] split = keyValuePair.Split('=');
                base.Add(split[0],
                    split.Length == 2 ? split[1] : "");
            }
            return this;
        }

        /// <summary>
        /// returns a QueryString object based on the current querystring of the request
        /// </summary>
        /// <returns>the QueryString object </returns>
        public QueryString FromCurrent()
        {
            if (HttpContext.Current != null)
            {
                return FillFromString(HttpContext.Current.Request.QueryString.ToString());
            }
            base.Clear();
            return this;
        }

        /// <summary>
        /// add a name value pair to the collection
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="value">the value associated to the name</param>
        /// <returns>the QueryString object </returns>
        public new QueryString Add(string name, string value)
        {
            return Add(name, value, false);
        }

        /// <summary>
        /// adds a name value pair to the collection
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="value">the value associated to the name</param>
        /// <param name="isUnique">true if the name is unique within the querystring. This allows us to override existing values</param>
        /// <returns>the QueryString object </returns>
        public QueryString Add(string name, string value, bool isUnique)
        {
            string existingValue = base[name];
            if (string.IsNullOrEmpty(existingValue))
                base.Add(name, HttpUtility.UrlEncodeUnicode(value));
            else if (isUnique)
                base[name] = HttpUtility.UrlEncodeUnicode(value);
            else
                base[name] += "," + HttpUtility.UrlEncodeUnicode(value);
            return this;
        }

        /// <summary>
        /// removes a name value pair from the querystring collection
        /// </summary>
        /// <param name="name">name of the querystring value to remove</param>
        /// <returns>the QueryString object</returns>
        public new QueryString Remove(string name)
        {
            string existingValue = base[name];
            if (!string.IsNullOrEmpty(existingValue))
                base.Remove(name);
            return this;
        }

        /// <summary>
        /// clears the collection
        /// </summary>
        /// <returns>the QueryString object </returns>
        public QueryString Reset()
        {
            base.Clear();
            return this;
        }

        ///// <summary>
        ///// Encrypts the keys and values of the entire querystring acc. to a key you specify
        ///// </summary>
        ///// <param name="key">the key to use in the encryption</param>
        ///// <returns>an encrypted querystring object</returns>
        //public QueryString Encrypt(string key)
        //{
        //    QueryString qs = new QueryString();
        //    Utils.Cryptography.Encryption enc = new Utils.Cryptography.Encryption();
        //    enc.Password = key;
        //    for (var i = 0; i < base.Keys.Count; i++)
        //    {
        //        if (!string.IsNullOrEmpty(base.Keys[i]))
        //        {
        //            foreach (string val in base[base.Keys[i]].Split(','))
        //                qs.Add(enc.Encrypt(base.Keys[i]), enc.Encrypt(HttpUtility.UrlDecode(val)));
        //        }
        //    }
        //    return qs;
        //}

        ///// <summary>
        ///// Decrypts the keys and values of the entire querystring acc. to a key you specify
        ///// </summary>
        ///// <param name="key">the key to use in the decryption</param>
        ///// <returns>a decrypted querystring object</returns>
        //public QueryString Decrypt(string key)
        //{
        //    QueryString qs = new QueryString();
        //    Utils.Cryptography.Encryption enc = new Utils.Cryptography.Encryption();
        //    enc.Password = key;
        //    for (var i = 0; i < base.Keys.Count; i++)
        //    {
        //        if (!string.IsNullOrEmpty(base.Keys[i]))
        //        {
        //            foreach (string val in base[base.Keys[i]].Split(','))
        //                qs.Add(enc.Decrypt(HttpUtility.UrlDecode(base.Keys[i])), enc.Decrypt(HttpUtility.UrlDecode(val)));
        //        }
        //    }
        //    return qs;
        //}

        /// <summary>
        /// overrides the default
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the associated decoded value for the specified name</returns>
        public new string this[string name]
        {
            get
            {
                return HttpUtility.UrlDecode(base[name]);
            }
        }

        /// <summary>
        /// overrides the default indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns>the associated decoded value for the specified index</returns>
        public new string this[int index]
        {
            get
            {
                return HttpUtility.UrlDecode(base[index]);
            }
        }

        /// <summary>
        /// checks if a name already exists within the query string collection
        /// </summary>
        /// <param name="name">the name to check</param>
        /// <returns>a boolean if the name exists</returns>
        public bool Contains(string name)
        {
            string existingValue = base[name];
            return !string.IsNullOrEmpty(existingValue);
        }

        /// <summary>
        /// outputs the querystring object to a string
        /// </summary>
        /// <returns>the encoded querystring as it would appear in a browser</returns>
        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (var i = 0; i < base.Keys.Count; i++)
            {
                if (!string.IsNullOrEmpty(base.Keys[i]))
                {
                    foreach (string val in base[base.Keys[i]].Split(','))
                        builder.Append((builder.Length == 0) ? "?" : "&").Append(HttpUtility.UrlEncodeUnicode(base.Keys[i])).Append("=").Append(val);
                }
            }
            return builder.ToString();
        }
    }
}

//MH
public static partial class ExtensionMethods
{

    /// <summary>
    /// Get QueryString as strongly typed object.
    /// </summary>
    /// <typeparam name="T">Storngly typed class</typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public static T ReadQueryStringAs<T>(this System.Web.HttpRequest request) where T : class, new()
    {
        SalesTool.Web.NameValueCollectionSerializer<T> _dictionaryserializer = new SalesTool.Web.NameValueCollectionSerializer<T>();
        return _dictionaryserializer.Deserialize(request.QueryString);
    }
    /// <summary>
    /// Read specific object from QueryString,form,cokies or from server collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <author>MH: 09 April 2014</author>
    public static T ReadValueAs<T>(this System.Web.HttpRequest request, string key)
    {
        string value = request[key];
        return value.ConvertTo(default(T));
    }

    /// <summary>
    /// Read specific object from QueryString
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <author>MH: 09 April 2014</author>
    public static T ReadQueryStringAs<T>(this System.Web.HttpRequest request, string key)
    {
        string value = request.QueryString[key];
        return value.ConvertTo(default(T));
    }
    /// <summary>
    /// Create Key Value String from object 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string CreateKeyValueString(this object obj)
    {
        SalesTool.Web.KeyValueStringSerializer<object> _keyvalueserializer = new SalesTool.Web.KeyValueStringSerializer<object>();
        return _keyvalueserializer.Serialize(obj);
    }
    /// <summary>
    /// Convert value or return default value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <author>MH:27 April 2014</author>
    public static T ConvertOrDefault<T>(this object value)
    {
        return ConvertTo<T>(value, default(T));
    }
    /// <summary>
    /// Convert NameValueCollection to dictionary
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static System.Collections.Generic.Dictionary<string, string> ToDictionary
            (this System.Collections.Specialized.NameValueCollection source)
    {
        return source.Cast<string>()
                     .Select(s => new { Key = s, Value = source[s] })
                     .ToDictionary(p => p.Key, p => p.Value);
    }
}

//Example Model class 
//public class LeadsPageQueryModel
//{
//    [SalesTool.Web.UrlKeyValue(Konstants.K_ACCOUNT_ID, IsIgnoreCase = true)]
//    public long? AccountId { get; set; }

//    [SalesTool.Web.UrlKeyValue("campaignid", IsIgnoreCase = false)]
//    public long? CampaignKey { get; set; }

//    [SalesTool.Web.UrlKeyValue(Konstants.K_PARENT_ACCOUNT_ID, IsIgnoreCase = true)]
//    public long? ParentAccountId { get; set; }
//}