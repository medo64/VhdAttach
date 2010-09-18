/* Josip Medved <jmedved@jmedved.com>  http://www.jmedved.com  http://medo64.blogspot.com */

//2009-05-23: New version.
//2009-07-04: Compatibility with Mono 2.4.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Reflection;
using System.Text;

namespace Medo.Configuration {

    /// <summary>
    /// Enables loading and saving of files list.
    /// It is written in State key at HKEY_CURRENT_USER branch withing defined SubKeyPath.
    /// </summary>
    public class RecentFiles {

        /// <summary>
        /// Creates new instance with "Default" as group name and maximum of 16 files.
        /// </summary>
        public RecentFiles()
            : this(16, null) {
        }

        /// <summary>
        /// Creates new instance with "Default" as group name.
        /// </summary>
        /// <param name="maximumCount">Maximum number of items to load or save.</param>
        public RecentFiles(int maximumCount)
            : this(maximumCount, null) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="maximumCount">Maximum number of items to load or save.</param>
        /// <param name="groupName">Name of group. If omitted, "Default" is used.</param>
        public RecentFiles(int maximumCount, string groupName) {
            //get paths
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

            string basePath = "Software";
            if (!string.IsNullOrEmpty(company)) { basePath += "\\" + company; }
            if (!string.IsNullOrEmpty(product)) { basePath += "\\" + product; }

            this.SubkeyPath = basePath + "\\RecentFiles";

            this.MaximumCount = maximumCount;
            if (string.IsNullOrEmpty(groupName)) {
                this.GroupName = "Default";
            } else {
                this.GroupName = groupName;
            }

            this.Load();
        }


        /// <summary>
        /// Gets maximum number of file names to be saved.
        /// </summary>
        public int MaximumCount { get; private set; }

        /// <summary>
        /// Gets number of file names.
        /// </summary>
        public int Count {
            get { return this._items.Count; }
        }

        /// <summary>
        /// Group name.
        /// </summary>
        public string GroupName { get; private set; }


        private List<RecentFile> _items = new List<RecentFile>();

        /// <summary>
        /// Gets file name at given index.
        /// </summary>
        /// <param name="index">Index.</param>
        public RecentFile this[int index] {
            get { return this._items[index]; }
        }

        public ReadOnlyCollection<RecentFile> AsReadOnly() {
            return this._items.AsReadOnly();
        }

        /// <summary>
        /// Inserts file name on top of list if one does not exist or moves it to top if one does exist.
        /// All changes are immediately saved.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Push(string fileName) {
            _items.Insert(0, new RecentFile(fileName));

            //remove duplicate of it
            for (int i = _items.Count - 1; i >= 1; --i) {
                if (_items[i].Equals(fileName)) {
                    this._items.RemoveAt(i);
                }
            }

            this.Save();
        }

        /// <summary>
        /// Removes all occurrances of given file.
        /// All changes are immediately saved.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Remove(string fileName) {
            for (int i = _items.Count - 1; i >= 0; --i) {
                if (_items[i].Equals(fileName)) {
                    this._items.RemoveAt(i);
                }
            }
            this.Save();
        }

        /// <summary>
        /// Removes all files from list.
        /// All changes are immediately saved.
        /// </summary>
        public void Clear() {
            this._items.Clear();
            this.Save();
        }


        /// <summary>
        /// Reloads file list from registry.
        /// </summary>
        public void Load() {
            this._items.Clear();
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(this.SubkeyPath, false)) {
                if (rk != null) {

                    object valueCU = rk.GetValue(this.GroupName, null);
                    if (valueCU != null) {
                        var valueKind = RegistryValueKind.MultiString;
                        if (!RecentFiles.IsRunningOnMono) { valueKind = rk.GetValueKind(this.GroupName); }
                        if (valueKind == RegistryValueKind.MultiString) {
                            string[] valueArr = valueCU as string[];
                            if (valueArr != null) {
                                for (int i = 0; i < valueArr.Length; ++i) {
                                    if (!string.IsNullOrEmpty(valueArr[i])) {
                                        _items.Add(new RecentFile(valueArr[i]));
                                    }
                                }
                            }
                        }


                    }
                }
            }
        }

        /// <summary>
        /// Saves current list to registry.
        /// This is automaticaly done on each insert.
        /// </summary>
        public void Save() {
            if (this._items.Count > this.MaximumCount) { this._items.RemoveRange(this.MaximumCount, this._items.Count - this.MaximumCount); }

            string[] fileNames = new string[this._items.Count];
            for (int i = 0; i < this._items.Count; ++i) {
                fileNames[i] = this._items[i].FileName;
            }

            using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(this.SubkeyPath)) {
                rk.SetValue(this.GroupName, fileNames, RegistryValueKind.MultiString);
            }
        }



        /// <summary>
        /// Gets/sets subkey used for registry storage.
        /// </summary>
        private string SubkeyPath { get; set; }

        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

    }



    /// <summary>
    /// Single recent file
    /// </summary>
    public class RecentFile {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="fileName">Full file name.</param>
        internal RecentFile(string fileName) {
            this.FileName = fileName;
        }

        /// <summary>
        /// Gets full file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets title of current file.
        /// </summary>
        public string Title {
            get {
                var info = new System.IO.FileInfo(this.FileName);

                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", false)) {
                        if (rk != null) {
                            var valueKind = RegistryValueKind.DWord;
                            if (!RecentFile.IsRunningOnMono) { valueKind = rk.GetValueKind("HideFileExt"); }
                            if (valueKind == RegistryValueKind.DWord) {
                                int hideFileExt = (int)(rk.GetValue("HideFileExt", 1));
                                if (hideFileExt != 0) {
                                    return info.Name.Substring(0, info.Name.Length - info.Extension.Length);
                                }
                            }
                        }
                }

                return info.Name;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj) {
            var other = obj as RecentFile;
            if (other != null) {
                return (string.Compare(this.FileName, other.FileName, System.StringComparison.OrdinalIgnoreCase) == 0);
            }
            var otherString = obj as string;
            if (otherString != null) {
                return (string.Compare(this.FileName, otherString, StringComparison.OrdinalIgnoreCase) == 0);
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode() {
            return this.FileName.GetHashCode();
        }

        /// <summary>
        /// Returns a System.String that represents the current object.
        /// </summary>
        public override string ToString() {
            return this.Title;
        }


        private static bool IsRunningOnMono {
            get {
                return (Type.GetType("Mono.Runtime") != null);
            }
        }

    }

}
