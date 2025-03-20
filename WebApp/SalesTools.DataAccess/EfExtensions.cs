using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Text;

//namespace SalesTool.DataAccess.Models
//{
//    public partial class LeadEntities
//    {
//        [System.Data.Objects.DataClasses.EdmFunction("SQSalesToolLead.Store", "fn_Arc_GetChangeAgentArcHistoryIdByAccount")]
//        public  long? fn_Arc_GetChangeAgentArcHistoryIdByAccount(long? AcountId)
//        {
//            throw new  NotSupportedException();
//        }
//    }
//}
namespace SalesTool.DataAccess
{
    
    public static class EfExtensions
    {
        /// <summary>
        /// Increase object context time
        /// </summary>
        /// <param name="context"></param>
        /// <param name="timeOut"></param>
        /// <author>MH</author>
        public static void IncreaseObjectTimeout(this DbContext context, int timeOut)
        {
            if (context == null) throw new ArgumentNullException("context");
            ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = timeOut;
        }
        /// <summary>
        /// Increase object context time
        /// </summary>
        /// <param name="context"></param>
        /// <param name="timeOut"></param>
        /// <author>MH</author>
        public static void IncreaseObjectTimeout(this ObjectContext context, int timeOut)
        {
            if (context == null) throw new ArgumentNullException("context");
            context.CommandTimeout = timeOut;
            //((IObjectContextAdapter)context).ObjectContext.CommandTimeout = timeOut;
        }
        /// <summary>
        /// Evaluate state entry for property changes
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="propertyName"></param>
        /// <returns>return true if current value is different than original</returns>
        /// <author>MH:08 April 2014</author>
        public static bool HasPropertyChanged(this ObjectStateEntry entry, string propertyName)
        {
            bool hasChanges = false;
            if (entry != null)
            {
                var current = entry.GetCurrentValue(propertyName);
                var original = entry.GetOriginalValue(propertyName);
                hasChanges = !current.Equals(original);
            }
            return hasChanges;
        }

        public static bool HasChanged(this ObjectStateEntry entry)
        {
            return entry.GetModifiedProperties().Any(entry.HasPropertyChanged);
        }

        /// <summary>
        /// Gets the Original value from state entry
        /// </summary>
        /// <param name="stateEntry"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <author>MH:08 April 2014</author>
        public static object GetOriginalValue(this ObjectStateEntry stateEntry, string propertyName)
        {
            return stateEntry.OriginalValues.GetValue(stateEntry.OriginalValues.GetOrdinal(propertyName));
        }

        /// <summary>
        /// Gets the current value from state entry
        /// </summary>
        /// <param name="stateEntry"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <author>MH:08 April 2014</author>
        public static object GetCurrentValue(this ObjectStateEntry stateEntry, string propertyName)
        {
            return stateEntry.CurrentValues.GetValue(stateEntry.CurrentValues.GetOrdinal(propertyName));
        }

        /// <summary>
        /// Get Entities form object context with modified state
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <author>MH:08 April 2014</author>
        public static IEnumerable<T> GetModifiedEntitiesOfType<T>(this ObjectContext context)
        {
            IEnumerable<T> Ans = null;
            if (context != null)
            {
                Ans = context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Select(p => p.Entity).OfType<T>();
                //if(Ans.Count()==0)
                //    Ans = context.ObjectStateManager.GetObjectStateEntries(EntityState.Added).Select(p => p.Entity).OfType<T>();
            }
            return Ans;
        }
        public static IEnumerable<T> GetEntitiesOfTypeByState<T>(this ObjectContext context,EntityState state)
        {
            if (context != null)
                return
                    context.ObjectStateManager.GetObjectStateEntries(state).Select(p => p.Entity).OfType<T>();
            return null;
        }
        /// <summary>
        /// Helper Method to Execute Store Procedure that return ObjectResult.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <author>MH:16 April</author>
        public static ObjectResult<T> ExecuteStoreProcedure<T>(this ObjectContext context, string procedureName, object[] param)
        {
            var command = GenerateStoreCommand(procedureName, param);
            return context.ExecuteStoreQuery<T>(command, param);
        }
        /// <summary>
        /// Helper method to execute store procedure that return int equivalent to ExecuteStoreCommand
        /// </summary>
        /// <param name="context"></param>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <author>MH:16 April 2014</author>
        public static int ExecuteStoreProcedure(this ObjectContext context, string procedureName, object[] param)
        {
            var command = GenerateStoreCommand(procedureName, param);
            return context.ExecuteStoreCommand(command, param);
        } 
        
        /// <summary>
        /// Helper method to execute store procedure that return int equivalent to ExecuteStoreCommand
        /// </summary>
        /// <param name="context"></param>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <author>MH:16 April 2014</author>
        public static IEnumerable<T> ExecuteStoreProcedure<T>(this DbContext context, string procedureName, object[] param)
        {
            var command = GenerateStoreCommand(procedureName, param);
            return context.Database.SqlQuery<T>(command, param);
        }
        
        /// <summary>
        /// Helper method to generate store procedure 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        ///  <author>MH:16 April 2014</author>
        public static string GenerateStoreCommand(string procedureName, IEnumerable<object> param)
        {
            string command = string.Format("EXEC {0} ", procedureName);
            command = param.OfType<SqlParameter>().Select(o => (o as System.Data.SqlClient.SqlParameter).ParameterName).Where(parameterName => !string.IsNullOrEmpty(parameterName)).Aggregate(command, (current, parameterName) => current + (" @" + parameterName + ","));
            command = command.TrimEnd(',');
            return command;
        }
        public static bool IsNew(this System.Data.Objects.DataClasses.EntityObject entity)
        {
            return entity.EntityKey == null || entity.EntityKey.IsTemporary;
        }
        
    }



}

public static class GeneralExtensionMethods
{
   #region Others

    // <summary>
    /// Check guid if is empty
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    /// <author>MH:21 April 2014</author>
    public static bool IsEmpty(this Guid guid)
    {
        return guid == Guid.Empty;
    }

    /// <summary>
    /// Check guid if is empty
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    /// <author>MH:21 April 2014</author>
    public static bool IsNullOrEmpty(this Guid? guid)
    {
        return guid == null || guid == Guid.Empty;
    }

    /// <summary>
    /// 	Converts an object to the specified target type or returns the default value if
    ///     those 2 types are not convertible.
    ///     <para>
    ///     If the <paramref name="value"/> can't be convert even if the types are 
    ///     convertible with each other, an exception is thrown.</para>
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <param name = "defaultValue">The default value.</param>
    /// <returns>The target type</returns>
    private static T ConvertTo<T>(this object value, T defaultValue)
    {
        if (value != null)
        {
            var targetType = typeof(T);

            if (value.GetType() == targetType) return (T)value;

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(value);
            if (converter != null)
            {
                if (converter.CanConvertTo(targetType))
                    return (T)converter.ConvertTo(value, targetType);
            }

            converter = System.ComponentModel.TypeDescriptor.GetConverter(targetType);
            if (converter != null)
            {
                if (converter.CanConvertFrom(value.GetType()))
                    return (T)converter.ConvertFrom(value);
            }
        }
        return defaultValue;
    }


    /// <summary>
    /// Convert string to Guid Default null
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <author>MH:21 April</author>
    public static Guid? AsGuid(this string input)
    {
        return ConvertTo(input, default(Guid?));
    }

    //MH:04 June 2014
    // Null proppogation.
    public static U IfNotNull<T, U>(this T t, Func<T, U> fn)
    {
        return t != null ? fn(t) : default(U);
    }

    public static string InsertNewLineAtEnd(this string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            input=input + Environment.NewLine;
        }
        return input;
    }
    public static string InsertAtNewLine(this string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            input = Environment.NewLine+input;
        }
        return input;
    }

    #endregion
}
