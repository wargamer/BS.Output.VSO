﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BS.Output.VSO.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BS.Output.VSO.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Invalid URL.
        /// </summary>
        internal static string Invalid_URL {
            get {
                return ResourceManager.GetString("Invalid_URL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading....
        /// </summary>
        internal static string Loading {
            get {
                return ResourceManager.GetString("Loading", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please fill in all fields.
        /// </summary>
        internal static string Please_fill_in_all_fields {
            get {
                return ResourceManager.GetString("Please_fill_in_all_fields", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please select a valid project.
        /// </summary>
        internal static string Please_select_a_valid_project {
            get {
                return ResourceManager.GetString("Please_select_a_valid_project", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something went wrong while attempting to connect to VSO, please make sure the URL is correct.
        /// </summary>
        internal static string Something_went_wrong_while_attempting_to_connect_to_VSO {
            get {
                return ResourceManager.GetString("Something_went_wrong_while_attempting_to_connect_to_VSO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap vso_16_16 {
            get {
                object obj = ResourceManager.GetObject("vso_16_16", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap vso_64_32 {
            get {
                object obj = ResourceManager.GetObject("vso_64_32", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}