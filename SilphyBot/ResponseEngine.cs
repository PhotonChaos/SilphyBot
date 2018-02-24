using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilphyBot
{
    class ResponseEngine {
        private Dictionary<string, string> responses = new Dictionary<string, string>();
        private readonly bool isKamtro;

        public ResponseEngine(bool isKamtro) {
            this.isKamtro = isKamtro;
        }
    }
}
