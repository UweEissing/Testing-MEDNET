using System;
using ChoETL;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using mednet.joshua.port;

namespace mednet.joshua.porta
{
    // Test Commit UE
    public class port_main
    {
        [DllImport("kernel32.dll")]
        private static extern int SetConsoleTitle(string header);

        private static bool bOnTestablaufEndeErreicht = false;

        //hallo Woerld
        [STAThread]
        public static void Main(string[] args)
        {
            DateTime dtStart = DateTime.Now;
            try
            {
                //*************************************************************************************
                // Unhandled Exceptions, da wir sehr viele IO-Operationen durchführen
                // und durch die Datenmenge eine Fehlersuche nicht leicht wird, StackTrace in
                // der Fehlerausgabe.
                //*************************************************************************************
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                string l_StrVerzeichnis1 = "";
                string l_StrVerzeichnis2 = "";
                string l_StrBDTNummerVon = "000";
                string l_StrBDTNummerBis = "999";

                //*************************************************************************************
                // Logging starten nach BaseDirectory\log
                //*************************************************************************************
                mednet.joshua.jsp.jsDump.StrPrefix = "port";
                mednet.joshua.jsp.jsDump d = new jsp.jsDump(AppDomain.CurrentDomain.BaseDirectory);
                mednet.joshua.jsp.jsDump.msg("start PortTester");

                string l_StrWinTitle = "porttester " + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                string l_StrFileVersionInfo = "";

                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    mednet.joshua.jsp.jsDump.msg("Versionsnummer:" + fvi.FileVersion);
                    l_StrFileVersionInfo = fvi.FileVersion;
                }
                catch (Exception x21)
                {
                    mednet.joshua.jsp.jsDump.errx("Assembly.Version=", x21);
                }
                SetConsoleTitle(l_StrWinTitle + " (v" + l_StrFileVersionInfo + ")");
                Console.WriteLine(l_StrWinTitle + " (v" + l_StrFileVersionInfo + ")");

                //*************************************************************************************
                // Befehlzeilenparameter analysieren, aufnehmen, gegebenenfalls Defaultwerte
                //*************************************************************************************
                if (args.Length == 0)
                {
                    mednet.joshua.jsp.jsDump.msg("Keine Befehlszeilenparameter");
                    ShowHilfe();
                    return;
                }

                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];
                    mednet.joshua.jsp.jsDump.msg("arg=" + arg);
                    Console.WriteLine("arg=" + arg);
                    if (arg.StartsWith("-vz1:") == true)
                    {
                        l_StrVerzeichnis1 = arg.Substring(4).TrimStart(new char[] { ':' });
                    }
                    else if (arg.StartsWith("-vz2:") == true)
                    {
                        l_StrVerzeichnis2 = arg.Substring(4).TrimStart(new char[] { ':' });
                    }
                    else if (arg.StartsWith("-von:") == true)
                    {
                        l_StrBDTNummerVon = arg.Substring(4).TrimStart(new char[] { ':' });
                    }
                    else if (arg.StartsWith("-bis:") == true)
                    {
                        l_StrBDTNummerBis = arg.Substring(4).TrimStart(new char[] { ':' });
                    }
                }

                //*************************************************************************************
                // Gibt die Vergleichsverzeichnisse überhaupt?
                //*************************************************************************************
                DirectoryInfo di1 = new DirectoryInfo(l_StrVerzeichnis1);
                DirectoryInfo di2 = new DirectoryInfo(l_StrVerzeichnis2);
                if (di1.Exists == false || di2.Exists == false ||
                    String.Compare(di1.FullName, di2.FullName, true) == 0)
                {
                    mednet.joshua.jsp.jsDump.msg("Verzeichnisse fehlen, nicht gefunden oder identisch");
                    ShowHilfe();
                    return;
                }

                //*************************************************************************************
                // port_analyzer_manager verwaltet die Testaufrufe
                // und wird als Thread gestartet.
                //*************************************************************************************
                Console.WriteLine("");
                Console.WriteLine("start Vergleich: " + l_StrVerzeichnis1 + " << " + l_StrVerzeichnis2);
                port_analyzer_manager m = new port_analyzer_manager(l_StrVerzeichnis1, l_StrVerzeichnis2);
                m.OnTestablaufEnde += M_OnTestablaufEnde;
                Thread tr = new Thread(new ThreadStart(m.OnStartTestablauf));
                tr.Start();

                //*************************************************************************************
                // Jetzt warten wir auf das Endesignal des Testablaufs oder den Anwenderabbruch
                //*************************************************************************************
                ConsoleKeyInfo ki;
                do
                {
                    while (Console.KeyAvailable == false)
                    {
                        Thread.Sleep(250); // Loop until input is entered.
                        if (!(m.IsTestablaufEnde == false && bOnTestablaufEndeErreicht == false))
                        {
                            return;
                        }
                    }

                    ki = Console.ReadKey(true);
                    if (ki.KeyChar == '.')
                    {
                        mednet.joshua.jsp.jsDump.msg("Anwenderabbruch.");
                        break;
                    }
                }
                while (true);

                TimeSpan ts = DateTime.Now - dtStart;
                mednet.joshua.jsp.jsDump.msg("Zeitdauer gesamt:" + ts.TotalSeconds.ToString());
                mednet.joshua.jsp.jsDump.ShutDown = true;
            }
            catch (Exception x21)
            {
                mednet.joshua.jsp.jsDump.DebugDumpLine("port_main:" + x21.Message);
            }
            finally
            {
            }
            return;
        }

        private static bool M_OnTestablaufEnde(bool status, string message)
        {
            mednet.joshua.jsp.jsDump.msg(message);
            bOnTestablaufEndeErreicht = true;
            return true;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            mednet.joshua.jsp.jsDump.DebugDumpLine("CurrentDomain_ProcessExit."+Environment.StackTrace.ToString());
            return;
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                mednet.joshua.jsp.jsDump.DebugDumpLine("CurrentDomain_UnhandledException");
                mednet.joshua.jsp.jsDump.DebugDumpLine("ExceptionObject:" + e.ExceptionObject.ToString());
                mednet.joshua.jsp.jsDump.DebugDumpLine("IsTerminating:" + e.IsTerminating.ToString());
                mednet.joshua.jsp.jsDump.DebugDumpLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"));
                return;
            }
            catch (Exception x21)
            {
                mednet.joshua.jsp.jsDump.DebugDumpLine(x21.Message);
            }
            return;
        }
        private static void ShowHilfe()
        {
            Console.WriteLine("Hilfe");
            return;
        }
    }

}