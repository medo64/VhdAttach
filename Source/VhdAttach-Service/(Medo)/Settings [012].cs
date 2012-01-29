//Josip Medved <jmedved@jmedved.com>  http://www.jmedved.com

//2007-10-15: New version.
//2007-11-15: When setting is written, it's cache is invalidated in order to force re-reading from registry.
//2007-11-21: State is thrown out.
//2007-12-23: Added trace for configuration settings.
//            Fixed error that prevented cache from working.
//2007-12-28: Added reading from command-line.
//            App.config is case insensitive.
//            Trace is culture insensitive.
//2008-01-03: Fixed bug with cache invalidation.
//            Added checks for null key.
//            Added Resources.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (CompoundWordsShouldBeCasedCorrectly).
//2008-04-26: Fixed case sensitivity bug when reading command line (introduced with FxCop cleaning).
//2008-11-07: Inserted Args [001] class in order to perform proper command line parsing.
//2009-07-04: Compatibility with Mono 2.4.
//2010-10-31: Added option to skip registry writes (NoRegistryWrites).
//2011-08-26: Added Defaults property.


using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;

namespace Medo.Configuration {

    /// <summary>
    /// Provides cached access to reading and writing settings.
    /// All settings are read in this order:
    ///   - Command line
    ///   - App.config
    ///   - registry (HKLM\Software\Company\Product)
    ///   - registry (HKCU\Software\Company\Product)
    /// Writing of settings is done in:
    ///   - registry (HKCU\Software\Company\Product)
    /// In case setting doesn't exist on reading, one is written with current value in:
    ///   - registry (HKCU\Software\Company\Product)
    /// Registry key contains company and (product|title|name).
    /// This class is thread-safe.
    /// </summary>
    public static class Settings {

        private static readonly object _syncRoot = new object(); //used for every access


        private static string _subkeyPath;
        /// <summary>
        /// Gets/sets subkey used for registry storage.
        /// </summary>
        public static string SubkeyPath {
            get {
                lock (_syncRoot) {
                    if (Settings._subkeyPath == null) {
                        Assembly assembly = Assembly.GetEntryAssembly();

                        string company = null;
                        object[] companyAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                        if ((companyAttributes != null) && (companyAttributes.Length >= 1)) {
                            company = ((AssemblyCompanyAttribute)companyAttributes[companyAttributes.Length - 1]).Company;
                        }

                        string product = null;
                        object[] productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                        if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                            product = ((AssemblyProductAttribute)productAttributes[productAttributes.Length - 1]).Product;
                        } else {
                            object[] titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                            if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                                product = ((AssemblyTitleAttribute)titleAttributes[titleAttributes.Length - 1]).Title;
                            } else {
                                product = assembly.GetName().Name;
                            }
                        }

                        string path = "Software";
                        if (!string.IsNullOrEmpty(company)) { path += "\\" + company; }
                        if (!string.IsNullOrEmpty(product)) { path += "\\" + product; }

                        _subkeyPath = path;
                    }
                    return _subkeyPath;
                }
            }
            set { lock (_syncRoot) { _subkeyPath = value; } }
        }

        /// <summary>
        /// Gets/sets whether settings should be written to registry.
        /// </summary>
        public static bool NoRegistryWrites { get; set; }

        /// <summary>
        /// Clears all cached data so on next access re-read of configuration data will occur.
        /// </summary>
        public static void ClearCachedData() {
            lock (_syncRoot) {
                Cache.Clear();
            }
        }


        #region String

        /// <summary>
        /// Retrieves the value associated with the specified key. If the key is not found in app.config, registry is checked (HKLM, then HKCU), if key is still not found returns the default value that you provide and creates entry in registry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">The value to return if key does not exist.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static string Read(string key, string defaultValue) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            string currKey = key.ToUpperInvariant();
            lock (_syncRoot) {
                string retValue = null;

                if (Cache.Contains(currKey)) {
                    retValue = Cache.Read(currKey);
                    goto Done;
                }

                try {
                    //CommandLine
                    if (_args.ContainsKey(currKey)) {
                        retValue = _args.GetValue(currKey);
                        goto Done;
                    }

                    //AppConfig
                    if (AppConfig.ContainsKey(currKey)) {
                        retValue = AppConfig[currKey];
                        goto Done;
                    }

                    //Registry (HKLM)
                    try {
                        using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueLM = rk.GetValue(currKey, null);
                                if (valueLM != null) {
                                    var valueKind = RegistryValueKind.String;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.String:
                                        case RegistryValueKind.ExpandString:
                                            retValue = valueLM as string;
                                            goto Done;
                                        case RegistryValueKind.MultiString:
                                            retValue = string.Join("\n", (valueLM as string[]));
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //Registry (HKCU)
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueCU = rk.GetValue(currKey, null);
                                if (valueCU != null) {
                                    var valueKind = RegistryValueKind.String;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.String:
                                        case RegistryValueKind.ExpandString:
                                            retValue = valueCU as string;
                                            goto Done;
                                        case RegistryValueKind.MultiString:
                                            retValue = string.Join("\n", (valueCU as string[]));
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //Defaults
                    if ((Settings.Defaults != null) && (Settings.Defaults.ContainsKey(currKey))) {
                        retValue = Settings.Defaults[currKey];
                        goto Done;
                    }

                    //Default
                    retValue = defaultValue;
                    goto Done;

                } finally {
                    Cache.Write(currKey, retValue);
                    Trace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\"", key, retValue) + "  {Medo.Configuration.Settings}");
                }

            Done:
                return (string)retValue;
            }
        }

        /// <summary>
        /// Sets the value for specified key. If the specified key does not exist, it is created.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static void Write(string key, string value) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            lock (_syncRoot) {
                Cache.Invalidate(key.ToUpperInvariant());
                if (Settings.NoRegistryWrites) {
                    Cache.Write(key.ToUpperInvariant(), value);
                } else {
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(Settings.SubkeyPath)) {
                            if (rk != null) {
                                rk.SetValue(key, value, RegistryValueKind.String);
                            }
                        }
                    } catch (IOException) { //key is deleted. 
                    } catch (UnauthorizedAccessException) { } //key is write protected. 
                }
            }
        }

        #endregion

        #region Integer

        /// <summary>
        /// Retrieves the value associated with the specified key. If the key is not found in app.config, registry is checked (HKLM, then HKCU), if key is still not found returns the default value that you provide and creates entry in registry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">The value to return if key does not exist.</param>
        /// <exception cref="System.FormatException">Input string was not in a correct format.</exception>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static int Read(string key, int defaultValue) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            string currKey = key.ToUpperInvariant();
            lock (_syncRoot) {
                int retValue = defaultValue;

                if (Cache.Contains(currKey)) {
                    retValue = int.Parse(Cache.Read(currKey), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    goto Done;
                }

                try {
                    //CommandLine
                    if (_args.ContainsKey(currKey)) {
                        retValue = int.Parse(_args.GetValue(currKey), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        goto Done;
                    }

                    //AppConfig
                    if (AppConfig.ContainsKey(currKey)) {
                        retValue = int.Parse(AppConfig[currKey], NumberStyles.Integer, CultureInfo.InvariantCulture);
                        goto Done;
                    }

                    //registry (HKLM)
                    try {
                        using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueLM = rk.GetValue(currKey, null);
                                if (valueLM != null) {
                                    var valueKind = RegistryValueKind.DWord;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.DWord:
                                            retValue = (int)valueLM;
                                            goto Done;
                                        case RegistryValueKind.String:
                                            retValue = int.Parse(valueLM as string, NumberStyles.Integer, CultureInfo.InvariantCulture);
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //registry (HKCU)
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueCU = rk.GetValue(currKey, null);
                                if (valueCU != null) {
                                    var valueKind = RegistryValueKind.DWord;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.DWord:
                                            retValue = (int)valueCU;
                                            goto Done;
                                        case RegistryValueKind.String:
                                            retValue = int.Parse(valueCU as string, NumberStyles.Integer, CultureInfo.InvariantCulture);
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //Defaults
                    if ((Settings.Defaults != null) && (Settings.Defaults.ContainsKey(currKey))) {
                        retValue = int.Parse(Settings.Defaults[currKey], NumberStyles.Integer, CultureInfo.InvariantCulture);
                        goto Done;
                    }

                    //Default
                    retValue = defaultValue;
                    goto Done;

                } finally {
                    Cache.Write(currKey, retValue.ToString(CultureInfo.InvariantCulture));
                    Trace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\"", key, retValue) + "  {Medo.Configuration.Settings}");
                }

            Done:
                return retValue;
            }
        }

        /// <summary>
        /// Sets the value for specified key. If the specified key does not exist, it is created.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static void Write(string key, int value) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            lock (_syncRoot) {
                Cache.Invalidate(key.ToUpperInvariant());
                if (Settings.NoRegistryWrites) {
                    Cache.Write(key.ToUpperInvariant(), value.ToString(CultureInfo.InvariantCulture));
                } else {
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(Settings.SubkeyPath)) {
                            if (rk != null) {
                                rk.SetValue(key, value, RegistryValueKind.DWord);
                            }
                        }
                    } catch (IOException) { //key is deleted. 
                    } catch (UnauthorizedAccessException) { } //key is write protected.
                }
            }
        }

        #endregion

        #region Boolean

        /// <summary>
        /// Retrieves the value associated with the specified key. If the key is not found in app.config, registry is checked (HKLM, then HKCU), if key is still not found returns the default value that you provide and creates entry in registry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">The value to return if key does not exist.</param>
        /// <exception cref="System.FormatException">Input string was not in a correct format.</exception>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static bool Read(string key, bool defaultValue) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            string currKey = key.ToUpperInvariant();
            lock (_syncRoot) {
                bool retValue = defaultValue;

                if (Cache.Contains(currKey)) {
                    retValue = bool.Parse(Cache.Read(currKey));
                    goto Done;
                }

                try {
                    //CommandLine
                    if (_args.ContainsKey(currKey)) {
                        string valueCL = _args.GetValue(currKey);
                        if (string.IsNullOrEmpty(valueCL)) { //if only /debug is specified than it is set.
                            retValue = true;
                        } else {
                            int intValue;
                            if (int.TryParse(valueCL, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue)) {
                                retValue = intValue != 0;
                            } else {
                                retValue = bool.Parse(valueCL);
                            }
                        }
                        goto Done;
                    }

                    //AppConfig
                    if (AppConfig.ContainsKey(currKey)) {
                        string valueAC = AppConfig[currKey];
                        if (valueAC != null) {
                            int intValue;
                            if (int.TryParse(valueAC, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue)) {
                                retValue = intValue != 0;
                            } else {
                                retValue = bool.Parse(valueAC);
                            }
                            goto Done;
                        }
                    }

                    //registry (HKLM)
                    try {
                        using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueRM = rk.GetValue(currKey, null);
                                if (valueRM != null) {
                                    var valueKind = RegistryValueKind.DWord;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.DWord:
                                            retValue = (int)valueRM != 0;
                                            goto Done;
                                        case RegistryValueKind.String:
                                            string strValue = valueRM as string;
                                            int intValue;
                                            if (int.TryParse(strValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue)) {
                                                retValue = intValue != 0;
                                            } else {
                                                retValue = bool.Parse(strValue);
                                            }
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //registry (HKCU)
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueCU = rk.GetValue(currKey, null);
                                if (valueCU != null) {
                                    var valueKind = RegistryValueKind.DWord;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.DWord:
                                            retValue = (int)valueCU != 0;
                                            goto Done;
                                        case RegistryValueKind.String:
                                            string strValue = valueCU as string;
                                            int intValue;
                                            if (int.TryParse(strValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue)) {
                                                retValue = intValue != 0;
                                            } else {
                                                retValue = bool.Parse(strValue as string);
                                            }
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //Defaults
                    if ((Settings.Defaults != null) && (Settings.Defaults.ContainsKey(currKey))) {
                        string strValue = Settings.Defaults[currKey];
                        int intValue;
                        if (int.TryParse(strValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue)) {
                            retValue = intValue != 0;
                        } else {
                            retValue = bool.Parse(strValue as string);
                        }
                        goto Done;
                    }

                    //Default
                    retValue = defaultValue;
                    goto Done;

                } finally {
                    Cache.Write(currKey, retValue.ToString(CultureInfo.InvariantCulture));
                    Trace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\"", key, retValue) + "  {Medo.Configuration.Settings}");
                }

            Done:
                return retValue;
            }
        }

        /// <summary>
        /// Sets the value for specified key. If the specified key does not exist, it is created.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">The value to write.</param>4
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static void Write(string key, bool value) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            lock (_syncRoot) {
                Cache.Invalidate(key.ToUpperInvariant());
                if (Settings.NoRegistryWrites) {
                    Cache.Write(key.ToUpperInvariant(), value.ToString(CultureInfo.InvariantCulture));
                } else {
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(Settings.SubkeyPath)) {
                            if (rk != null) {
                                if (value) {
                                    rk.SetValue(key, 1, RegistryValueKind.DWord);
                                } else {
                                    rk.SetValue(key, 0, RegistryValueKind.DWord);
                                }
                            }
                        }
                    } catch (IOException) { //key is deleted. 
                    } catch (UnauthorizedAccessException) { } //key is write protected. 
                }
            }
        }

        #endregion

        #region Double

        /// <summary>
        /// Retrieves the value associated with the specified key. If the key is not found in app.config, registry is checked (HKLM, then HKCU), if key is still not found returns the default value that you provide and creates entry in registry.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">The value to return if key does not exist.</param>
        /// <exception cref="System.FormatException">Input string was not in a correct format.</exception>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static double Read(string key, double defaultValue) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            string currKey = key.ToUpperInvariant();
            lock (_syncRoot) {
                double retValue = defaultValue;

                if (Cache.Contains(currKey)) {
                    retValue = double.Parse(Cache.Read(currKey), NumberStyles.Float, CultureInfo.InvariantCulture);
                    goto Done;
                }

                try {
                    //CommandLine
                    if (_args.ContainsKey(currKey)) {
                        retValue = double.Parse(_args.GetValue(currKey), NumberStyles.Float, CultureInfo.InvariantCulture);
                        goto Done;
                    }

                    //AppConfig
                    if (AppConfig.ContainsKey(currKey)) {
                        retValue = double.Parse(AppConfig[currKey], NumberStyles.Float, CultureInfo.InvariantCulture);
                        goto Done;
                    }

                    //registry (HKLM)
                    try {
                        using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueLM = rk.GetValue(currKey, null);
                                if (valueLM != null) {
                                    var valueKind = RegistryValueKind.String;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.String:
                                            retValue = double.Parse(valueLM as string, NumberStyles.Float, CultureInfo.InvariantCulture);
                                            goto Done;
                                        case RegistryValueKind.DWord:
                                            retValue = (int)valueLM;
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //registry (HKCU)
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(Settings.SubkeyPath, false)) {
                            if (rk != null) {
                                object valueCU = rk.GetValue(currKey, null);
                                if (valueCU != null) {
                                    var valueKind = RegistryValueKind.String;
                                    if (!Settings.IsRunningOnMono) { rk.GetValueKind(currKey); }
                                    switch (valueKind) {
                                        case RegistryValueKind.String:
                                            retValue = double.Parse(valueCU as string, NumberStyles.Float, CultureInfo.InvariantCulture);
                                            goto Done;
                                        case RegistryValueKind.DWord:
                                            retValue = (int)valueCU;
                                            goto Done;
                                    }
                                }
                            }
                        }
                    } catch (SecurityException) { }

                    //Defaults
                    if ((Settings.Defaults != null) && (Settings.Defaults.ContainsKey(currKey))) {
                        retValue = double.Parse(Settings.Defaults[currKey], NumberStyles.Float, CultureInfo.InvariantCulture);
                        goto Done;
                    }

                    //Default
                    retValue = defaultValue;
                    goto Done;

                } finally {
                    Cache.Write(currKey, retValue.ToString(CultureInfo.InvariantCulture));
                    Trace.TraceInformation(string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\"", key, retValue) + "  {Medo.Configuration.Settings}");
                }

            Done:
                return retValue;
            }
        }

        /// <summary>
        /// Sets the value for specified key. If the specified key does not exist, it is created.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">The value to write.</param>4
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        public static void Write(string key, double value) {
            if (key == null) { throw new ArgumentNullException("key", Resources.ExceptionKeyCannotBeNull); }

            lock (_syncRoot) {
                Cache.Invalidate(key.ToUpperInvariant());
                if (Settings.NoRegistryWrites) {
                    Cache.Write(key.ToUpperInvariant(), value.ToString(CultureInfo.InvariantCulture));
                } else {
                    try {
                        using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(Settings.SubkeyPath)) {
                            if (rk != null) {
                                rk.SetValue(key, value.ToString(CultureInfo.InvariantCulture), RegistryValueKind.String);
                            }
                        }
                    } catch (IOException) { //key is deleted. 
                    } catch (UnauthorizedAccessException) { } //key is write protected.
                }
            }
        }

        #endregion


        #region Cache (private)

        private static class Cache {

            private static System.Collections.Generic.Dictionary<string, string> _cache = new System.Collections.Generic.Dictionary<string, string>();
            private static readonly object _cacheSyncRoot = new object();


            internal static void Clear() {
                lock (_cacheSyncRoot) {
                    _cache.Clear();
                }
            }

            internal static bool Contains(string key) {
                lock (_cacheSyncRoot) {
                    return _cache.ContainsKey(key);
                }
            }

            internal static string Read(string key) {
                lock (_cacheSyncRoot) {
                    if (_cache.ContainsKey(key)) {
                        return _cache[key];
                    }
                    return null;
                }
            }

            internal static void Write(string key, string value) {
                lock (_cacheSyncRoot) {
                    if (_cache.ContainsKey(key)) {
                        _cache[key] = value;
                    } else {
                        _cache.Add(key, value);
                    }
                }
            }

            internal static void Invalidate(string key) {
                lock (_cacheSyncRoot) {
                    if (_cache.ContainsKey(key)) {
                        _cache.Remove(key); //forcing re-reading from disc or other device.
                    }
                }
            }

        }

        #endregion


        #region AppConfig

        private static System.Collections.Generic.Dictionary<string, string> _appConfig;

        private static System.Collections.Generic.Dictionary<string, string> AppConfig {
            get {
                if (_appConfig == null) {
                    _appConfig = new System.Collections.Generic.Dictionary<string, string>();
                    for (int i = 0; i < System.Configuration.ConfigurationManager.AppSettings.Count; ++i) {
                        string currKey = System.Configuration.ConfigurationManager.AppSettings.GetKey(i).ToUpperInvariant();
                        string[] currValues = System.Configuration.ConfigurationManager.AppSettings.GetValues(i);
                        string currValue = string.Empty;
                        if (currValues.Length > 0) { currValue = currValues[currValues.Length - 1]; }
                        if (!string.IsNullOrEmpty(currKey)) {
                            if (_appConfig.ContainsKey(currKey)) {
                                _appConfig[currKey] = currValue;
                            } else {
                                _appConfig.Add(currKey, currValue);
                            }
                        }
                    }

                }
                return _appConfig;
            }
        }

        #endregion

        #region Args

        private static Args _args = Args.Current;

        private class Args {

            private static Args _current;
            /// <summary>
            /// Gets command-line arguments for current application.
            /// </summary>
            public static Args Current {
                get {
                    if (_current == null) {
                        string[] envArgs = Environment.GetCommandLineArgs();
                        _current = new Args(envArgs, 1, envArgs.Length - 1);
                    }
                    return _current;
                }
            }


            /// <summary>
            /// Creates new instance.
            /// </summary>
            /// <param name="array">Array of all arguments.</param>
            /// <param name="offset">Index of starting item.</param>
            /// <param name="count">Number of items.</param>
            public Args(string[] array, int offset, int count) {
                InitializeFromArray(array, offset, count, new string[] { "/", "--", "-" }, new char[] { ':', '=' });
            }


            private Dictionary<string, List<string>> _items;

            private void InitializeFromArray(string[] array, int offset, int count, string[] prefixes, char[] separators) {
                _items = new Dictionary<string, List<string>>();

                for (int i = 0; i < count; ++i) {
                    string curr = array[offset + i];
                    string key = null;
                    string value = null;

                    bool isDone = false;

                    //named
                    for (int j = 0; j < prefixes.Length; ++j) {
                        string currPrefix = prefixes[j];
                        if (curr.StartsWith(currPrefix, StringComparison.Ordinal)) {
                            int iSep = curr.IndexOfAny(separators);
                            if (iSep >= 0) {
                                key = curr.Substring(currPrefix.Length, iSep - currPrefix.Length);
                                value = curr.Remove(0, iSep + 1);
                            } else {
                                key = curr.Substring(currPrefix.Length, curr.Length - currPrefix.Length);
                                value = string.Empty;
                            }
                            isDone = true;
                            break;
                        }
                    }

                    //noname
                    if (!isDone) {
                        key = string.Empty;
                        value = curr;
                    } else {
                        key = key.ToUpperInvariant();
                    }

                    List<string> currList;
                    if (_items.ContainsKey(key)) {
                        currList = _items[key];
                    } else {
                        currList = new List<string>();
                        _items.Add(key, currList);
                    }
                    currList.Add(value);
                }
            }


            /// <summary>
            /// Return true if key exists in current list.
            /// </summary>
            /// <param name="key">Key.</param>
            public bool ContainsKey(string key) {
                if (key == null) {
                    key = string.Empty;
                } else {
                    key = key.ToUpperInvariant();
                }
                return _items.ContainsKey(key);
            }


            /// <summary>
            /// Returns single value connected to given key.
            /// If key is not found, null is returned.
            /// If multiple values exist, last one is returned.
            /// </summary>
            /// <param name="key">Key.</param>
            public string GetValue(string key) {
                if (key == null) {
                    key = string.Empty;
                } else {
                    key = key.ToUpperInvariant();
                }

                if (_items.ContainsKey(key)) {
                    return _items[key][_items[key].Count - 1];
                } else {
                    return null;
                }
            }


            private static class Helper {

                internal enum State {
                    Default = 0,
                    Quoted = 1
                }

            }

        }

        #endregion



        private static Dictionary<string, string> Defaults = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Sets defaults to be used as last priority.
        /// </summary>
        /// <param name="key">Setting key.</param>
        /// <param name="value">Setting value.</param>
        public static void SetDefaults(string key, string value) {
            if (Settings.Defaults.ContainsKey(key)) {
                Settings.Defaults[key] = value;
            } else {
                Settings.Defaults.Add(key, value);
            }
        }
        /// <summary>
        /// Sets defaults to be used as last priority.
        /// </summary>
        /// <param name="defaults">Name/value collection of settings.</param>
        public static void SetDefaults(IDictionary<string, string> defaults) {
            if (defaults != null) {
                foreach (var item in defaults) {
                    SetDefaults(item.Key, item.Value);
                }
            }
        }


        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }


        private static class Resources {

            internal static string ExceptionKeyCannotBeNull {
                get { return "Key cannot be null."; }
            }
        }

    }

}
