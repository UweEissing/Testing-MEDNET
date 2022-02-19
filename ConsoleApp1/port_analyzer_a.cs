using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace mednet.joshua.port
{
    public abstract class JSPortAnalyzerA
    {
        // abstract oder Basisklasse von der wir alle Klassen für Vergleiche ableiten
        // hier sollen alle allgemeinen funktionen stehen, die n-fach verwendet werden.
        public mednet.joshua.log.jsLogWriter _log = null;
        public string m_StrObject1 = "";
        public string m_StrObject2 = "";

        public JSPortAnalyzerA()
        {
        }
        public abstract void Initialize(string a, string b);
        public abstract bool OnStartVergleich();

        public string StrLOG
        {
            get
            {
                return _log.ToString();
            }
        }
    }  


}
