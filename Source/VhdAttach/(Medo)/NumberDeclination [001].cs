//Copyright (c) 2008 Josip Medved <jmedved@jmedved.com>

//2008-08-09: Initial version.
//2008-11-21: Added GetText method.
//            Added IDisposable.


using System;
using System.Globalization;

namespace Medo.Localization.Croatia {

    /// <summary>
    /// Declines numbers and text according to croatian language rules.
    /// </summary>
    public class NumberDeclination : IDisposable {

        private string _text1;
        private string _text2;
        private string _text5;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="text1">Suffix text as if it is used with number 1 (ex. 1 "tim").</param>
        /// <param name="text2">Suffix text as if it is used with number 2 (ex. 2 "tima").</param>
        /// <param name="text5">Suffix text as if it is used with number 5 (ex. 5 "timova").</param>
        public NumberDeclination(string text1, string text2, string text5) {
            this._text1 = text1;
            this._text2 = text2;
            this._text5 = text5;
        }

        /// <summary>
        /// Gets number with appropriate suffix.
        /// </summary>
        /// <param name="value">Value</param>
        public string this[int value] {
            get {
                return GetText(value);
            }
        }

        /// <summary>
        /// Gets number with appropriate suffix.
        /// </summary>
        /// <param name="value">Value</param>
        public string GetText(int value) {
            int desetice = value % 100;
            int jedinice = value % 10;
            if ((desetice >= 10) && (desetice <= 20)) {
                return string.Format(CultureInfo.CurrentCulture, "{0} {1}", value, this._text5);
            } else if (jedinice == 1) {
                return string.Format(CultureInfo.CurrentCulture, "{0} {1}", value, this._text1);
            } else if ((jedinice >= 2) && (jedinice <= 4)) {
                return string.Format(CultureInfo.CurrentCulture, "{0} {1}", value, this._text2);
            } else {
                return string.Format(CultureInfo.CurrentCulture, "{0} {1}", value, this._text5);
            }
        }


        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
            //no real dispose is neccessary, this is just for using() statement.
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    
    }

}
