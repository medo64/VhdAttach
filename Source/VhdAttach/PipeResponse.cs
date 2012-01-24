using System;
using System.Collections.Generic;
using System.Text;

namespace VhdAttach {
    internal class PipeResponse {

        public PipeResponse(int exitCode, string message) {
            this.ExitCode = exitCode;
            this.Message = message;
        }

        public int ExitCode { get; private set; }
        public string Message { get; private set; }

    }
}
