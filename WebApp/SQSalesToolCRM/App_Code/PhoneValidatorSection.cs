using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Web;


namespace SalesTool.Web.Configuration
{
    /// <summary>
    /// Class to read custom configuration section
    /// </summary>
    public class PhoneValidatorSection: ConfigurationSection
    {
        public static string SectionName { get { return "phoneValidator"; } }
        [ConfigurationProperty("url", DefaultValue = "", IsRequired = false)]
        //[StringValidator(/*InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|",*/ MinLength = 1, MaxLength = 128)]
        public String Uri
        {
            get { return (String)this["url"]; }
            set { this["url"] = value; }
        }
        
        [ConfigurationProperty("user", DefaultValue="", IsRequired = true)]
        //[StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|", MinLength = 1, MaxLength = 64)]
        public String User
        {
            get{  return (String)this["user"];  }
            set{  this["user"] = value;        }
        }

        [ConfigurationProperty("password", DefaultValue="", IsRequired = true)]
        //[StringValidator(InvalidCharacters = "", MinLength = 1, MaxLength = 64)]
        public String Password
        {
            get{  return (String)this["password"];  }
            set{  this["password"] = value;        }
        }

        [ConfigurationProperty("timeout", DefaultValue=5, IsRequired = true)]
        //[IntegerValidator(ExcludeRange=false, MinValue=0, MaxValue=32655)]
        public int Timeout
        {
            get{  return (int)this["timeout"];  }
            set{  this["timeout"] = value;        }
        }

        [ConfigurationProperty("protocol", DefaultValue=1, IsRequired = true)]
        //[IntegerValidator(ExcludeRange=false, MinValue=0, MaxValue=32655)]
        public int Protocol
        {
            get{  return (int)this["protocol"];  }
            set{  this["protocol"] = value;        }
        }

        [ConfigurationProperty("debug", DefaultValue=false, IsRequired = false)]
        public bool IsDebugged
        {
            get { return (bool)this["debug"]; }
            set { this["debug"] = value; }
        }

        [ConfigurationProperty("actionOn", DefaultValue = PhoneActionOn.Cell_Unknown, IsRequired = true)]
        [System.ComponentModel.TypeConverter(typeof(CaseInsensitiveEnumConfigConverter<PhoneActionOn>))]
        public PhoneActionOn ActionOn
        {
            get {
                 return (PhoneActionOn)this["actionOn"];
            }
            set {
                this["actionOn"] = value.ToString(); 
            }
        }


    };

    public class CaseInsensitiveEnumConfigConverter<T> : ConfigurationConverterBase
    {
        public override object ConvertFrom(
        ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            return Enum.Parse(typeof(T), (string)data, true);
        }
    }
}