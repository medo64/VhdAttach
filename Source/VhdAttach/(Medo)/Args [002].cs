//Copyright (c) 2008 Josip Medved <jmedved@jmedved.com>

//2008-11-04: First version.
//2012-11-24: Validating arguments.


using System;
using System.Collections.Generic;
using System.Text;

namespace Medo.Application {

    /// <summary>
    /// Parsing and reading of command-line arguments.
    /// </summary>
    public class Args {

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

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="line">Line with all arguments.</param>
        /// <param name="removeFirstMember">If true, first member will be removed.</param>
        public Args(string line, bool removeFirstMember) {
            Helper.State state = Helper.State.Default;
            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();

            if (line != null) {
                for (int i = 0; i < line.Length; ++i) {
                    char currChar = line[i];
                    switch (currChar) {
                        case ' ':
                            if (state == Helper.State.Default) {
                                if (sb.Length > 0) {
                                    list.Add(sb.ToString());
                                    sb.Length = 0;
                                }
                            } else {
                                sb.Append(currChar);
                            }
                            break;

                        case '\"':
                            if (state == Helper.State.Default) {
                                state = Helper.State.Quoted;
                            } else if (state == Helper.State.Quoted) {
                                if ((i + 1 < line.Length) && (line[i + 1] == '\"')) {
                                    sb.Append("\"");
                                    i++;
                                } else {
                                    state = Helper.State.Default;
                                }
                            }
                            break;

                        default:
                            sb.Append(currChar);
                            break;
                    }
                }
            }

            if (sb.Length > 0) {
                list.Add(sb.ToString());
                sb.Length = 0;
            }

            if (removeFirstMember) {
                InitializeFromArray(list.ToArray(), 1, list.Count - 1, new string[] { "/", "--", "-" }, new char[] { ':', '=' });
            } else {
                InitializeFromArray(list.ToArray(), 0, list.Count, new string[] { "/", "--", "-" }, new char[] { ':', '=' });
            }
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
        /// Returns all keys.
        /// </summary>
        public string[] GetKeys() {
            List<string> list = new List<string>();
            Dictionary<string, List<string>>.Enumerator iEnum = _items.GetEnumerator();
            while (iEnum.MoveNext()) {
                list.Add(iEnum.Current.Key);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Returns values connected to given key.
        /// If key cannot be found, empty array will be returned.
        /// </summary>
        /// <param name="key">Key.</param>
        public string[] GetValues(string key) {
            if (key == null) {
                key = string.Empty;
            } else {
                key = key.ToUpperInvariant();
            }
            if (_items.ContainsKey(key)) {
                return _items[key].ToArray();
            } else {
                return new string[] { };
            }
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

        /// <summary>
        /// Returns single value connected to given key.
        /// If key does not exist, default value is returned.
        /// If multiple values exist, only last one is taken into account.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public string GetValue(string key, string defaultValue) {
            string value = GetValue(key);
            if (value != null) {
                return value;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns single value connected to given key.
        /// Value is true if it is different from 0, has string "true" or when there is no value for found key.
        /// If key does not exist, default value is returned.
        /// If multiple values exist, only last one is taken into account.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public bool GetValue(string key, bool defaultValue) {
            string value = GetValue(key);
            if (value != null) {
                if (value.Length == 0) { return true; }

                int valueInt;
                if (int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out valueInt)) {
                    return valueInt != 0;
                }

                bool valueBool;
                if (bool.TryParse(value, out valueBool)) {
                    return valueBool;
                }

            }
            return defaultValue;
        }

        /// <summary>
        /// Returns single value connected to given key.
        /// If key does not exist, default value is returned.
        /// If multiple values exist, only last one is taken into account.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public int GetValue(string key, int defaultValue) {
            string value = GetValue(key);
            if (value != null) {
                int valueInt;
                if (int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out valueInt)) {
                    return valueInt;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns single value connected to given key.
        /// If key does not exist, default value is returned.
        /// If multiple values exist, only last one is taken into account.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public double GetValue(string key, double defaultValue) {
            string value = GetValue(key);
            if (value != null) {
                double valueX;
                if (double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out valueX)) {
                    return valueX;
                }
            }
            return defaultValue;
        }


        private static class Helper {

            internal enum State {
                Default = 0,
                Quoted = 1
            }

        }

    }

}
