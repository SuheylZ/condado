// Purpose of this file is to support fluentSql like functionality to keep code code compact and consistent.
// Author : Muzammil Hussain
// Created: 06 March 2014

using System.Collections.Generic;
using System.Reflection;
using System.Linq;
namespace System.Data.SqlClient
{

    public static class SqlCommandExtensions
    {
        #region SqlParameter

        /// <summary>
        /// Add Parameter to command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand AddParameter(this SqlCommand command, SqlParameter parameter)
        {
            command.Parameters.Add(parameter);
            return command;
        }

        /// <summary>
        /// Add Parameter to command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand AddParameter(this SqlCommand command, string name, int value)
        {
            command.AddParameter(name, value, SqlDbType.Int, ParameterDirection.Input);
            return command;
        }

        /// <summary>
        /// Add Parameter to command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand AddParameter(this SqlCommand command, string name, bool value)
        {
            command.AddParameter(name, value ? 1 : 0, SqlDbType.TinyInt, ParameterDirection.Input);
            return command;
        }

        /// <summary>
        /// Add Parameter to command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand AddParameter(this SqlCommand command, string name, string value)
        {
            command.AddParameter(name, value, SqlDbType.NVarChar, ParameterDirection.Input);
            return command;
        }

        /// <summary>
        /// Add Parameter to command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand AddParameter(this SqlCommand command, string name, object value)
        {
            var parameter = new SqlParameter
                {
                    ParameterName = name,
                    Value = value,
                    Direction = ParameterDirection.Input
                };

            command.AddParameter(parameter);
            return command;
        }

        /// <summary>
        /// Add Parameter to command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand AddParameter(this SqlCommand command, string name, object value, SqlDbType type,
                                              ParameterDirection direction)
        {
            var parameter = new SqlParameter
                {
                    ParameterName = name,
                    Value = value,
                    SqlDbType = type,
                    Direction = direction
                };
            command.Parameters.Add(parameter);
            return command;
        }

        /// <summary>
        /// Add Parameters collection to command Parameters
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand AddParameters(this SqlCommand command, IEnumerable<SqlParameter> parameters)
        {
            var commandParameters = command.Parameters;

            foreach (var parameter in parameters)
            {
                commandParameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// convert source object to command parameters
        /// </summary>
        /// <param name="command"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand ParseParameters(this SqlCommand command, object source)
        {
            var commandParameters = command.Parameters;

            var properties =
                source.GetType()
                      .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance |
                                     BindingFlags.FlattenHierarchy);
            var sourceType = source.GetType();

            // The entity's unique identifier is added here because the previous flags only detect the declared properties
            // and we dont want to be declaring the guid on every entity
            // command_parameters.Add( Create.Parameter( "@id", source.Id ) );

            // TODO: ensure that the list of properties correspond the the actual object
            // and that there's an Id field

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var propertyName = property.Name;

                var propertyInfo = sourceType.GetProperty(property.Name);
                var propertyValue = propertyInfo.GetValue(source, null);

                var value = Convert.ChangeType(propertyValue, Nullable.GetUnderlyingType(propertyType) ?? propertyType);

                if (value == null) continue;

                commandParameters.Add(SqlHelper.Parameter("@" + propertyName, value));
            }

            return command;
        }

        /// <summary>
        /// Removes the command parameter 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand ReleaseParameters(this SqlCommand command)
        {
            var parameters = command.Parameters;

            var i = 0;
            var length = parameters.Count;

            while (i < length)
            {
                var parameter = parameters[i];
                parameters.Remove(parameter);
                i++;
            }

            return command;
        }

        #endregion

        /// <summary>
        /// Sets the CommandTimeout value default 30 seconds
        /// </summary>
        /// <param name="command"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand SetTimeout(this SqlCommand command, int seconds = 30)
        {
            command.CommandTimeout = seconds;
            return command;
        }

        /// <summary>
        /// Sets the command timeout 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlCommand SetTimeout(this SqlCommand command, TimeSpan time)
        {
            command.CommandTimeout = (int)time.TotalSeconds;
            return command;
        }
    }

    public class SqlHelper
    {
        /// <summary>
        /// Creates Sql Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlParameter Parameter(string name, object value, SqlDbType type, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter { ParameterName = name, Value = value, SqlDbType = type, Direction = direction };
        }

        /// <summary>
        /// Creates Sql Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlParameter Parameter(string name, string value)
        {
            return Parameter(name, value, SqlDbType.NVarChar);
        }

        /// <summary>
        /// Creates Sql Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlParameter Parameter(string name, int value)
        {
            return Parameter(name, value, SqlDbType.Int);
        }

        /// <summary>
        /// Creates Sql Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlParameter Parameter(string name, bool value)
        {
            return Parameter(name, value ? 1 : 0, SqlDbType.TinyInt);
        }

        /// <summary>
        /// Creates Sql Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlParameter Parameter(string name, object value)
        {
            return new SqlParameter { ParameterName = name, Value = value, Direction = ParameterDirection.Input };
            ;
        }

        public static SqlParameter Parameter(string name, DateTime value)
        {
            return new SqlParameter
                {
                    ParameterName = name,
                    Value = value,
                    Direction = ParameterDirection.Input,
                    DbType = DbType.DateTime
                };
        }

        /// <summary>
        /// Creates SqlConnection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        ///<author>Muzammil H</author>
        public static SqlConnection Connection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// 4.5 working
        ///// <summary>
        ///// Creates SqlConnection
        ///// </summary>
        ///// <param name="connectionString"></param>
        ///// <param name="credential"></param>
        ///// <returns></returns>
        /////<author>Muzammil H</author>
        //public static SqlConnection Connection(string connectionString,SqlCredential credential)
        //{
        //    return new SqlConnection(connectionString,credential);
        //}

        ////TODO:need to test 4.5 wokring
        /////  <summary>
        /////  Creates SqlConnection
        /////  </summary>
        /////  <param name="connectionString"></param>
        /////  <param name="userId"></param>
        /////  <param name="password"></param>
        ///// <param name="isReadOnly"></param>
        ///// <returns></returns>
        ///// <author>Muzammil H</author>
        //public static SqlConnection Connection(string connectionString,string userId,string password,bool isReadOnly=true)
        //{
        //    var credential = new SqlCredential(userId, password.ToSecureString(isReadOnly));
        //    return new SqlConnection(connectionString,credential);
        //}

        ///// <summary>
        ///// Convert password to SecureString
        ///// </summary>
        ///// <param name="password"></param>
        ///// <returns></returns>
        /////<author>Muzammil H</author>
        //private static SecureString ConvertToSecureString(string password)
        //{
        //    var secureString = new SecureString();
        //    if (password.Length > 0)
        //    {
        //        foreach (var c in password.ToCharArray()) secureString.AppendChar(c);
        //    }
        //    return secureString;
        //}
    }

    public class SqlParameterFluent
    {
        private readonly List<SqlParameter> _collection;

        public SqlParameterFluent()
        {
            _collection = new List<SqlParameter>();
        }

        public SqlParameterFluent Add(SqlParameter parameter)
        {
            _collection.Add(parameter);
            return this;
        }
        public SqlParameterFluent Add(string name, object value)
        {
            _collection.Add(SqlHelper.Parameter(name,value));
            return this;
        }
        public SqlParameterFluent Add(string name, object value, SqlDbType dbType)
        {
            _collection.Add(SqlHelper.Parameter(name, value??DBNull.Value, dbType));
            return this;
        }
        public SqlParameterFluent Add(string name, string value, SqlDbType dbType, int size)
        {
            var param = SqlHelper.Parameter(name, value, dbType);
            param.Size = size;
            _collection.Add(param);
            return this;
        }

        public SqlParameterFluent Add(string name, object value, SqlDbType dbType, ParameterDirection direction)
        {
            _collection.Add(SqlHelper.Parameter(name, value, dbType, direction));
            return this;
        }

        public SqlParameter[] ToArray()
        {
            return _collection.ToArray();
        }

        public object[] ToObjectArray()
        {
            return _collection.Select(p => p as object).ToArray();
        }
    }
}