using System;
using System.Collections.Generic;
using System.Linq;

namespace SelectCare.ArcApi
{
    public static class GenericExtensionMethod
    {
        public const string UtcDateFormat = "yyyy-MM-dd HH:mm:ss UTC";
        
        public static string ConvertToUtcDateTimeString(this DateTime date)
        {
            return date.ToUniversalTime().ToString(UtcDateFormat);
        }

        public static string ConvertToUtcDateTimeString(this DateTime? date)
        {
           return  date.HasValue?date.Value.ConvertToUtcDateTimeString():"";
        }
        //used by LINQ to SQL
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        //used by LINQ
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
        public static T GetValue<T>(string input, string toRemove, out bool isPassed)
        {
            var temp = input.Trim(toRemove.ToArray());
            var output = Parse<T>(temp, out isPassed);
            return output;
        }

        public static T Parse<T>(string sourceValue, out bool isPassed) //where T : IConvertible 
        {
            Type t = typeof(T);

            if (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                if (sourceValue == null)
                {
                    isPassed = true;
                    return (T)(object)null;
                }
                try
                {
                    isPassed = true;
                    return (T)Convert.ChangeType(sourceValue, Nullable.GetUnderlyingType(t));
                }
                catch (Exception e)
                {
                    isPassed = false;
                    return (T)(object)null;
                }
            }
            try
            {
                isPassed = true;
                return (T)Convert.ChangeType(sourceValue, typeof(T));
            }
            catch (Exception)
            {
                isPassed = false;
                return (T)(object)null;
            }
        }

        public static T Parse<T>(string sourceValue, IFormatProvider provider) where T : IConvertible
        {
            return (T)Convert.ChangeType(sourceValue, typeof(T), provider);
        }

        public static string ApplyUtcFormat(this DateTime time)
        {
            return time.ToString(ArcDataConversion.UtcDateFormat);
        }

        /// <summary>
        /// Gets the comma separated ids from success response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetCommaSeparatedIds(this ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.Where(p => p.Accepted == "Y").Select(p => p.AccountID).ToList();

                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets the comma separated ids from success response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetCommaSeparatedIds(this OpResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.OpResults.Where(p => p.Accepted == "Y").Select(p => p.ID).ToList();

                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets comma separated string from successful Response results
        /// </summary>
        /// <param name="response">Arc Response</param>
        /// <returns></returns>
        public static string GetCommaSeparatedReferences(this ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.Where(p => p.Accepted == "Y").Select(p => p.Reference).ToList();

                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets the comma separated string from Successful response string
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetCommaSeparatedIdsFromActions(this ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.SelectMany(p => p.SubResults.Where(s => s.Accepted == "Y"))
                         .Select(m => m.ID.Replace("Action:", ""))
                         .ToList();
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets the comma separated string from Successful response string
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetCommaSeparatedIdsFromMessage(this ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.SelectMany(p => p.SubResults.Where(s => s.Accepted == "Y"))
                         .Select(m => m.ID.Replace("Message:", ""))
                         .ToList();
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
    }
}