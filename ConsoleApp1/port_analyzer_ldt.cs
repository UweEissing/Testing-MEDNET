using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mednet.joshua.port
{
    /// <summary>
    /// Klasse zum Vergleich einer LDT-Datei
    /// </summary>
    public class JSLDTAnalyzer : JSPortAnalyzerA
    {
        public JSLDTAnalyzer():base()
        {
            _log = new log.jsLogWriter("JSLDTAnalyzer");
        }

        public override void Initialize(string a, string b)
        {
        }
        public override bool OnStartVergleich()
        {
            return false;
        }

    }
}
