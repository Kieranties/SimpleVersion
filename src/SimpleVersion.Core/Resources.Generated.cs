// Hash: 0A-A5-5D-3E-34-7D-56-6F-91-37-77-41-4F-C7-4A-69
// Auto generated - Do not modify this file!
// Modify  then invoke Update-Resource to recreate
// For members that require formatting provide a csv of the argument names
// 09/26/2019 09:37:54
namespace SimpleVersion {

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Update-Resource", "0.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {

        private static global::System.Resources.ResourceManager resourceMan;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Resources"/> class.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {}

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SimpleVersion.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture { get; set; }

                /// <summary>
        ///   Looks up a localized string similar to: Could not convert given context to {0}.
        /// </summary>
        internal static string Exception_CouldNotConvertContextType(object contextType) => string.Format(Culture, GetString(nameof(Exception_CouldNotConvertContextType)), contextType);

        /// <summary>
        ///   Looks up a localized string similar to: Could not find the current branch tip. Unable to identify the current commit.
        /// </summary>
        internal static string Exception_CouldNotFindBranchTip => GetString(nameof(Exception_CouldNotFindBranchTip));

        /// <summary>
        ///   Looks up a localized string similar to: Could not find git repository at '{0}' or any parent directory.
        /// </summary>
        internal static string Exception_CouldNotFindGitRepository(object path) => string.Format(Culture, GetString(nameof(Exception_CouldNotFindGitRepository)), path);

        /// <summary>
        ///   Looks up a localized string similar to: Could not read '{0}', has it been committed?
        /// </summary>
        internal static string Exception_CouldNotReadSettingsFile(object fileName) => string.Format(Culture, GetString(nameof(Exception_CouldNotReadSettingsFile)), fileName);

        /// <summary>
        ///   Looks up a localized string similar to: Unexpected JsonToken '{0}' in converter {1}.
        /// </summary>
        internal static string Exception_InvalidJsonToken(object token, object converter) => string.Format(Culture, GetString(nameof(Exception_InvalidJsonToken)), token, converter);

        /// <summary>
        ///   Looks up a localized string similar to: Version '{0}' is not in a valid format.
        /// </summary>
        internal static string Exception_InvalidVersionFormat(object value) => string.Format(Culture, GetString(nameof(Exception_InvalidVersionFormat)), value);

        /// <summary>
        ///   Looks up a localized string similar to: Path must be provided.
        /// </summary>
        internal static string Exception_PathMustBeProvided => GetString(nameof(Exception_PathMustBeProvided));

        /// <summary>
        ///    Returns the resource string for the given key
        /// </summary>
        private static string GetString(string key) => ResourceManager.GetString(key, Culture);
    }
}
