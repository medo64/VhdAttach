using System;
using System.Collections.Generic;
using System.Text;

namespace VhdAttach {
    internal class PipeResponse {

        public PipeResponse(bool isError, string message) {
            this.IsError = isError;
            this.Message = message;
        }

        public bool IsError { get; private set; }
        public string Message { get; private set; }

    }
}
