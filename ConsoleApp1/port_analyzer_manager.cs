using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace mednet.joshua.port
{
    public class port_analyzer_manager
    {
        public delegate bool OnTestablaufEndeEventHandler(bool status, string message);
        public event OnTestablaufEndeEventHandler OnTestablaufEnde = null;

        private string m_StrVerzeichnis1 = "";   // Messdaten
        private string m_StrVerzeichnis2 = "";   // Vergleichsverzeichnis mit den neuen Testdaten
        private string m_StrBDTNummerVon = "000";
        private string m_StrBDTNummerBis = "999";
        private bool m_bTestablaufEnde = false;

        public port_analyzer_manager(string p_StrVerzeichnis1, string p_StrVerzeichnis2)
        {
            m_StrVerzeichnis1 = p_StrVerzeichnis1;
            m_StrVerzeichnis2 = p_StrVerzeichnis2;
        }

        public void OnStartTestablauf()
        {
            try
            {
                //**************************************************************************************
                // 1:
                // Start Vergleich Directory
                // Stimmt die Anzahl der Dateien überein. Starke Größenabweichungen und Dateigröße
                // identisch; gibt es neue Dateien
                //**************************************************************************************
                mednet.joshua.jsp.jsDump.msg("starte Directory-Vergleich");
                DateTime dtStart = DateTime.Now;
                JSDirectoryAnalyzer da = new JSDirectoryAnalyzer();
                da.Initialize(m_StrVerzeichnis1, m_StrVerzeichnis2);
                da.OnStartVergleich();
                TimeSpan ts = DateTime.Now - dtStart;
                mednet.joshua.jsp.jsDump.msg("Directory-Vergleich ende:" + ts.TotalMilliseconds.ToString() +"ms" );
                mednet.joshua.jsp.jsDump.msg(da.StrLOG);

                //**************************************************************************************
                // 2:
                // Start LDT-Datei vergleichen
                // Ist der Inhalt der Datei identisch bis auf Datums- und Zeitangaben.
                //**************************************************************************************
                mednet.joshua.jsp.jsDump.msg("starte LDT-Vergleich");
                dtStart = DateTime.Now;
                JSLDTAnalyzer ldta = new JSLDTAnalyzer();
                ldta.Initialize(m_StrVerzeichnis1, m_StrVerzeichnis2);
                ldta.OnStartVergleich();
                ts = DateTime.Now - dtStart;
                mednet.joshua.jsp.jsDump.msg("LDT-Vergleich ende:" + ts.TotalMilliseconds.ToString() + "ms");
                mednet.joshua.jsp.jsDump.msg(da.StrLOG);

                //**************************************************************************************
                // 3:
                // Start PDF-Datei vergleichen
                // Ist der Inhalt der PDF-Datei bis auf die Datums- und Zeitangaben gleich
                //**************************************************************************************

                //**************************************************************************************
                // 4:
                // Start Protokoll-Datei vergleichen
                // Sind die gesuchten Event-Codes vorhanden, also sind alle Ergeignisse
                // eingetroffen (stimmt der Ablauf)
                //**************************************************************************************

                //**************************************************************************************
                // 5:
                // Start Etiketten-Datei vergleichen
                // EPL2-Dateivergleich
                //**************************************************************************************

                //Program a = new Program();
                //// a.TestOrdner(@"D:\ab\1" , @"D:\ab\2");
                //a.Compare("D:\\ab\\2\\a.txt", "D:\\ab\\2\\a.txt");
                //a.Compare("D:\\ab\\2\\c.txt", "D:\\ab\\2\\d.txt");
                //a.CompareFileBytes("D:\\ab\\2\\Praxisphasenvertrag.pdf", "D:\\ab\\2\\Zulassungsantrag_Praxisphase_FB_Technik.pdf");

                // Test
                System.Threading.Thread.Sleep(15000);
                if (OnTestablaufEnde != null)
                {
                    OnTestablaufEnde(true, "Testablauf-Ende: ok");
                }
                m_bTestablaufEnde = true;
            }
            catch (Exception x21)
            {
                mednet.joshua.jsp.jsDump.errx("OnStartTestablauf", x21);
            }
            // Aber Fehlerstatus
            m_bTestablaufEnde = true;
            if (OnTestablaufEnde != null)
            {
                OnTestablaufEnde(false, "Testablauf-Ende: Fehler");
            }
            return;
        }
        #region Attribute
        public bool IsTestablaufEnde
        {
            get { return m_bTestablaufEnde; }
        }
        public string StrBDTNummerVon
        {
            set { m_StrBDTNummerVon = value; }
        }
        public string StrBDTNummerBis
        {
            set { m_StrBDTNummerBis = value; }
        }
        #endregion
    }
}
