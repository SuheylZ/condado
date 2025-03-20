// Purpose of this class is to reduce the load of ApplicationGlobal settings 
// Design By: Muzammil H:
// Creation Date: 17 March 2014
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SalesTool.DataAccess
{
    public class ApplicationStorageHelper : IDisposable
    {
        DbContext _context;
        const string K_TableName = "Application_Storage";
        private string sqlQuery;

        public ApplicationStorageHelper(string connectionString)
        {
            _context = new DConext(connectionString);
        }
        public DbContext Context{get { return _context; }}

        private class DConext : DbContext
        {
            static DConext()
            {
                System.Data.Entity.Database.SetInitializer<DConext>(null);
            }
            public DConext(string connction)
                : base(connction)
            {
                
            }
        }
        

        public T Load<T>() where T : class
        {
            string sql = ConstructQuery<T>();
            var res = _context.Database.SqlQuery<T>(sql).SingleOrDefault<T>();
            return (T)res;
            //return default(T);
        }

        public TReturn LoadProperty<T, TReturn>(Expression<Func<T, object>> property)
        {
            PropertyInfo propertyInfo = GetPropertyInfo<T>(property);
            sqlQuery = GetPropertyQuery(propertyInfo);
            sqlQuery = sqlQuery.TrimEnd(',');
            sqlQuery = "SELECT\n" + sqlQuery;
            var res = _context.Database.SqlQuery<TReturn>(sqlQuery).FirstOrDefault<TReturn>();
            return res;
        }

        private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> property)
        {
            MemberExpression Exp = null;
            if (property.Body is UnaryExpression)
            {
                UnaryExpression UnExp = (UnaryExpression)property.Body;
                if (UnExp.Operand is MemberExpression)
                {
                    Exp = (MemberExpression)UnExp.Operand;
                }
                else
                    throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            else if (property.Body is MemberExpression)
            {
                Exp = (MemberExpression)property.Body;
            }
            else
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            PropertyInfo propertyInfo = (PropertyInfo)Exp.Member;
            return propertyInfo;
        }

        public string GetGeneratedSql()
        {
            return sqlQuery;
        }
        private string ConstructQuery<T>()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT");
            var type = typeof(T);
            //if (type.IsClass)
            //{
            var properties = type.GetProperties().Where(p => p.GetSetMethod() != null);
            foreach (var pro in properties)
            {
                sqlBuilder.AppendLine();
                string sql = GetPropertyQuery(pro);
                sqlBuilder.Append(sql);
            }
            //}
            //else
            //{

            //}
            sqlQuery = sqlBuilder.ToString();
            sqlQuery = sqlQuery.TrimEnd(',');
            return sqlQuery;
        }

        public object GetValueForDefaultValue(object obj)
        {
            if (obj is bool)
            {
                return (bool) obj ? 1 : 0;
            }
            if (obj is string)
            {
                return string.Format("'{0}'", obj);
            }
            return obj;
        }
        private string GetPropertyQuery(PropertyInfo pro)
        {
            string propertySqlQuery = "";
            var info = GetPropertyMetaData(pro);
            bool canBeNull = IsNullableOrString(pro);
            if (canBeNull)
            {
                // if nullable and has defaut value
                if (info.Item4 != null)
                {
                    propertySqlQuery = string.Format("(ISNULL((SELECT {0} FROM {1} WHERE [Key]='{2}'),{4})) AS '{3}',", info.Item1, K_TableName, info.Item2, info.Item3, GetValueForDefaultValue(info.Item4));
                }
                else
                    propertySqlQuery = string.Format("(SELECT {0} FROM {1} WHERE [Key]='{2}') AS '{3}',", info.Item1, K_TableName, info.Item2, info.Item3);
            }
            else
            {
                // if not nullable and default value specified

                if (info.Item4 != null)
                {
                    propertySqlQuery = string.Format("(ISNULL((SELECT {0} FROM {1} WHERE [Key]='{2}'),{4})) AS '{3}',", info.Item1, K_TableName, info.Item2, info.Item3, GetValueForDefaultValue( info.Item4));
                }
                else
                {
                    // calculate default value
                    var def = GetDefaultValue(pro);
                    propertySqlQuery = string.Format("(ISNULL((SELECT {0} FROM {1} WHERE [Key]='{2}'),{4})) AS '{3}',", info.Item1, K_TableName, info.Item2, info.Item3, def);
                }
            }
            return propertySqlQuery;
        }

        private object GetDefaultValue(System.Reflection.PropertyInfo pro)
        {
            var type = pro.PropertyType;
            if (IsNullableOrString(pro))
                return null;
            if (type == typeof(Boolean))
                return 0;
            else
                return GetDefault(type);
        }
        /// <summary>
        /// Item1=column Map
        /// item2=Key
        /// item3=PropertyName
        /// item4=default value
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private Tuple<string, string, string, object> GetPropertyMetaData(System.Reflection.PropertyInfo prop)
        {
            string columnMap = GetColumnName(prop.PropertyType);
            var keyValue = prop.GetCustomAttributes(true).OfType<KeyValue>().SingleOrDefault();
            string key = (keyValue != null) ? keyValue.Key : prop.Name;
            object defaultVal = keyValue != null ? keyValue.DefaultValue : null;
            return new Tuple<string, string, string, object>(columnMap, key, prop.Name, defaultVal);

        }

        //public void InvokeGenericMethod<T>(string method, BindingFlags binder, Type typeToPass, object obj, object[] param)
        //{
        //    var methodInfo = typeof(T).GetMethod(method, binder);
        //    //var genericMethod= method.MakeGenericMethod(typeToPass);
        //    //var result  genericMethod.invoke(obj,parm);
        //}
        public string GetColumnName(Type type)
        {
            var methedInfo = typeof(ApplicationStorageHelper).GetMethod("GetColumn");
            var genericMethod = methedInfo.MakeGenericMethod(type);
            var result = genericMethod.Invoke(this, null);
            return Convert.ToString(result);
        }
        public string GetColumn<T>()
        {
            T x = default(T);
            if (x is short || x is int || typeof(T) == typeof(Int16?) || typeof(T) == typeof(Int32?))
            {
                return "iValue";
            }
            if (x is float || x is double || typeof(T) == typeof(Double?) || typeof(T) == typeof(Single?))
            {
                return "fValue";
            }
            if (x is char || x is string)
            {
                return "tValue";
            }
            return
                (x is bool || typeof(T) == typeof(Boolean?)) ? "bValue" :
                    (x is Guid || typeof(T) == typeof(Guid?)) ? "uValue" :
                            (x is DateTime || typeof(T) == typeof(DateTime?)) ? "dValue"
                                    : "tValue";
        }

       
        private static bool IsNullableOrString(PropertyInfo info)
        {
            Type type = info.PropertyType;
            return type == typeof(String) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));

        }

        #region Helper Extension

        /// <summary>
        /// [ <c>public static T GetDefault&lt; T &gt;()</c> ]
        /// <para></para>
        /// Retrieves the default value for a given Type
        /// </summary>
        /// <typeparam name="T">The Type for which to get the default value</typeparam>
        /// <returns>The default value for Type T</returns>
        /// <remarks>
        /// If a reference Type or a System.Void Type is supplied, this method always returns null.  If a value type 
        /// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
        /// exception.
        /// </remarks>
        /// <seealso cref="GetDefault(Type)"/>
        public static T GetDefault<T>()
        {
            return (T)GetDefault(typeof(T));
        }

        /// <summary>
        /// [ <c>public static object GetDefault(Type type)</c> ]
        /// <para></para>
        /// Retrieves the default value for a given Type
        /// </summary>
        /// <param name="type">The Type for which to get the default value</param>
        /// <returns>The default value for <paramref name="type"/></returns>
        /// <remarks>
        /// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type 
        /// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
        /// exception.
        /// </remarks>
        /// <seealso cref="GetDefault&lt;T&gt;"/>
        public static object GetDefault(Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
                throw new ArgumentException(
                    "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                    "> contains generic parameters, so the default value cannot be retrieved");

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct), return a 
            //  default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message + "\")", e);
                }
            }

            // Fail with exception
            throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                                        "> is not a publicly-visible type, so the default value cannot be retrieved");
        }

        #endregion

        #region IDisposable

        private bool disposed = false;
        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
                _context.Dispose();
                _context = null;
                sqlQuery = null;
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

        #endregion

    }

    [Flags]
    public enum GlobalSettingAppMode {NotDefined=0, SQS = 1, SQAH = 2, SQL = 4, SQL_Split = 8 ,ALL=16}

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class KeyValue : Attribute
    {
        /// <summary>
        /// Kay to lookup in Database
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Default value for nullable property
        /// </summary>
        public object DefaultValue { get; set; }
        
        /// <summary>
        /// Setting applicable for 
        /// </summary>
        public GlobalSettingAppMode Mode { get; set; }
    }
}