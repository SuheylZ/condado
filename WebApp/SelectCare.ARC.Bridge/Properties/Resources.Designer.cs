﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OLEBridge.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OLEBridge.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to USE_ARC_NEW_IMPLEMENTATION.
        /// </summary>
        internal static string ArcTypeKey {
            get {
                return ResourceManager.GetString("ArcTypeKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon Bridge {
            get {
                object obj = ResourceManager.GetObject("Bridge", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ApplicationServices.
        /// </summary>
        internal static string DBConnectionName {
            get {
                return ResourceManager.GetString("DBConnectionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to exit the application?.
        /// </summary>
        internal static string ExitMessage {
            get {
                return ResourceManager.GetString("ExitMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Confirm to exit.
        /// </summary>
        internal static string ExitTitle {
            get {
                return ResourceManager.GetString("ExitTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to selectCareHub.
        /// </summary>
        internal static string HubName {
            get {
                return ResourceManager.GetString("HubName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ARCHUBPATH.
        /// </summary>
        internal static string HubUrlKey {
            get {
                return ResourceManager.GetString("HubUrlKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OLEBridge.
        /// </summary>
        internal static string LogTitle {
            get {
                return ResourceManager.GetString("LogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OLEBridgeMutex.
        /// </summary>
        internal static string Mutex {
            get {
                return ResourceManager.GetString("Mutex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECTCAREASMX.
        /// </summary>
        internal static string ServiceKey {
            get {
                return ResourceManager.GetString("ServiceKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OLE Bridge.
        /// </summary>
        internal static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
    }
}
