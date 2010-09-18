//Josip Medved <jmedved@jmedved.com> http://www.jmedved.com

//2007-12-31: New version.
//2008-05-22: Added Description.
//2008-06-06: Added Copyright.


using System.Reflection;

namespace Medo.Reflection {

	/// <summary>
	/// Returns various info about assembly that started process.
	/// </summary>
	public static class EntryAssembly {

        private readonly static Assembly Assembly = System.Reflection.Assembly.GetEntryAssembly();
        //private readonly static AssemblyName AssemblyName = Assembly.GetName();

		/// <summary>
		/// Gets entry assembly's full name.
		/// </summary>
		public static string FullName {
			get { return System.Reflection.Assembly.GetEntryAssembly().GetName().FullName; }
		}

		/// <summary>
		/// Gets entry assembly's application name.
		/// </summary>
		public static string Name {
			get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Name; }
		}

		/// <summary>
		/// Gets entry assembly's version.
		/// </summary>
		public static System.Version Version {
			get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Version; }
		}

		/// <summary>
		/// Returns entry assembly's version in x.xx format.
		/// </summary>
		public static string ShortVersionString {
			get {
				System.Version version = EntryAssembly.Version;
				return version.Major.ToString("0", System.Globalization.CultureInfo.InvariantCulture) + "." + version.Minor.ToString("00", System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Returns entry assembly's version in x.xx.xxx.xxxx format.
		/// </summary>
		public static string LongVersionString {
			get {
				System.Version version = EntryAssembly.Version;
				return version.Major.ToString("0", System.Globalization.CultureInfo.CurrentCulture) + "." + version.Minor.ToString("00", System.Globalization.CultureInfo.CurrentCulture) + "." + version.Build.ToString("000", System.Globalization.CultureInfo.CurrentCulture) + "." + version.Revision.ToString("0000", System.Globalization.CultureInfo.CurrentCulture); 
			}
		}

		/// <summary>
		/// Returns entry assembly's company or null if not found.
		/// </summary>
		public static string Company {
			get{
				object[] companyAttributes = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyCompanyAttribute), true);
				if ((companyAttributes != null) && (companyAttributes.Length >= 1)) {
					return ((System.Reflection.AssemblyCompanyAttribute)companyAttributes[companyAttributes.Length - 1]).Company;
				}
				return null;
			}
		}

		/// <summary>
		/// Returns entry assembly's title or name if title is not found.
		/// </summary>
		public static string Title {
			get{
				object[] titleAttributes = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), true);
				if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
					return ((System.Reflection.AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
				} else {
					return Name;
				}
			}
		}

		/// <summary>
		/// Retuns entry's assembly product. If product is not found, title is returned.
		/// </summary>
		public static string Product {
			get{
				object[] productAttributes = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), true);
				if ((productAttributes != null) && (productAttributes.Length >= 1)) {
					return ((System.Reflection.AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
				} else {
					return Title;
				}
			}
		}

        /// <summary>
        /// Retuns entry's assembly description. If description is not found, empty string is returned.
        /// </summary>
        public static string Description {
            get {
                object[] descriptionAttributes = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), true);
                if ((descriptionAttributes != null) && (descriptionAttributes.Length >= 1)) {
                    return ((System.Reflection.AssemblyDescriptionAttribute)descriptionAttributes[descriptionAttributes.Length - 1]).Description;
                } else {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Retuns entry's assembly copyright. If copyright is not found, empty string is returned.
        /// </summary>
        public static string Copyright {
            get {
                object[] copyrightAttributes = Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), true);
                if ((copyrightAttributes != null) && (copyrightAttributes.Length >= 1)) {
                    return ((System.Reflection.AssemblyCopyrightAttribute)copyrightAttributes[copyrightAttributes.Length - 1]).Copyright;
                } else {
                    return string.Empty;
                }
            }
        }

	}

}
