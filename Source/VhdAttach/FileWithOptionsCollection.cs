using System;
using System.Collections.Generic;
using VhdAttachCommon;

namespace VhdAttach {
    internal class FileWithOptionsCollection : List<FileWithOptions> {

        public FileWithOptionsCollection(IEnumerable<FileWithOptions> collection) : base(collection) { }

        public bool Remove(string fileName) {
            FileWithOptions selectedItem = null;
            foreach (var item in this) {
                if (string.Equals(item.FileName, fileName, StringComparison.OrdinalIgnoreCase)) {
                    selectedItem = item;
                    break;
                }
            }
            return this.Remove(selectedItem);
        }

    }
}
