using System;
using ChoETL;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

// Test Commit UE
public class Program
{
    public void TestOrdner(string pathA, string pathB)
    {
        // Erstelen zwei Ordner mit gleichen oder unterschiedlichen Pfad
         

        System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(pathA);
        System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(pathB);

        // Ermöglichst alle DateinTyp.  
        IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
        IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

        //Ein benutzerdefinierter Dateivergleich, der unten definiert ist  
        FileCompare myFileCompare = new FileCompare();

        // Diese Abfrage bestimmt, ob die beiden Ordner enthalten  
          
        bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

        if (areIdentical == true)
        {
            Console.WriteLine("Die beiden Ordner sind identisch ");
        }
        else
        {
            Console.WriteLine("Die beiden Ordner sind nicht identische");
        }

        // zu finden die gemeinsamen Dateien
           
        var queryCommonFiles = list1.Intersect(list2, myFileCompare);

        if (queryCommonFiles.Any())
        {
            Console.WriteLine("Die folgenden Dateien befinden sich in beiden Ordnern:");
            foreach (var v in queryCommonFiles)
            {
                Console.WriteLine(v.FullName); 
                //zeigt welche Dateien in der Liste gibt 
            }
        }
        else
        {
            Console.WriteLine("Es gibt keine gemeinsamen Dateien in den beiden Ordnern!!");
        }

        // unterschied zwischen den beiden Ordnern.  
          
        var queryList1Only = (from file in list1
                              select file).Except(list2, myFileCompare);

        Console.WriteLine("Die folgenden Dateien befinden sich in list1, aber nicht in list2:");
        foreach (var v in queryList1Only)
        {
            Console.WriteLine(v.FullName);
        }

        //  debug mode.  
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
    //Vergleich zwischen die Dateien
    public bool Compare(string filePathOne, string filePathTwo)
    {
        var s = new StringBuilder();
        string[] fileContentsOne = File.ReadAllLines(filePathOne);
        string[] fileContentsTwo = File.ReadAllLines(filePathTwo);
        List<string> firstNotSecond = fileContentsOne.Except(fileContentsTwo).ToList();
        var secondNotFirst = fileContentsTwo.Except(fileContentsOne).ToList();
        Console.WriteLine(fileContentsOne.SequenceEqual(fileContentsTwo));
        if (!fileContentsOne.Length.Equals(fileContentsTwo.Length))
        {
            Console.WriteLine("Die Datein Sind nicht Identisch!!");
            return false;
        }
        Console.WriteLine("In Liste1 vorhanden, aber nicht in Liste2");
        foreach (var x in firstNotSecond)
        {
            Console.WriteLine(x);
        }
        Console.WriteLine("In Liste2 vorhanden, aber nicht in Liste1");
        foreach (var y in secondNotFirst)
        {
            Console.WriteLine(y);
        }
        // muss in einer CSV Datei die Ergebnisse erstellen
        File.WriteAllText("D:\\Error.txt", s.ToString());
        return true;
    }
    class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public FileCompare() { }

        // Andere Methode zu Vergleichen Name && Length

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            return (f1.Name == f2.Name && f1.Length == f2.Length);
        }

        // gibt einen Hash zurück, wenn Equals wahr ist, dann müssen die Hash-Codes.  
        public int GetHashCode(System.IO.FileInfo fi)
        {
            string s = $"{fi.Name}{fi.Length}";
            return s.GetHashCode();
        }
    }
        // Compare file sizes
    private bool CompareFileSizes(string fileName1, string fileName2)
    {
        bool fileSizeEqual = true;

        // ertsellen System.IO.FileInfo objects für die Dateien
        FileInfo fileInfo1 = new FileInfo(fileName1);
        FileInfo fileInfo2 = new FileInfo(fileName2);
        Console.WriteLine("File1: " + fileInfo1.Length);
        Console.WriteLine("File2: " + fileInfo2.Length);
        // Compare file sizes
        if (fileInfo1.Length != fileInfo2.Length)
        {

            // Dateigrößen sind nicht gleich, daher sind Dateien nicht identisch
            fileSizeEqual = false;
            Console.WriteLine("Sorry nicht gleiche Größe !!");
        }
        else
        {
            //fileSizeEqual = true;
            Console.WriteLine("gleiche Größe");
        }
        Console.WriteLine("Ergebniss:" + " " + fileSizeEqual);
        return fileSizeEqual;

    }
    private bool CompareFileBytes(string fileName1, string fileName2)
    {
         
        // If sizes sind equal then compare bytes.
        if (CompareFileSizes(fileName1, fileName2))
        {
            int file1byte = 0;
            int file2byte = 0;
           
            // öffnen a System.IO.FileStream for jede file.
            
            using (FileStream fileStream1 = new FileStream(fileName1, FileMode.Open),
                              fileStream2 = new FileStream(fileName2, FileMode.Open))
            {
                // Liest und vergleicht ein Byte aus jeder Datei, 
               //bis ein nicht übereinstimmender Satz von Bytes gefunden 
               //wird oder das Ende der Datei erreicht ist.
                do
                {
                    file1byte = fileStream1.ReadByte();
                    file2byte = fileStream2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));
            }
            Console.WriteLine("Gleiche FileBytes");
            return ((file1byte - file2byte) == 0);

        }
        else
        {
            Console.WriteLine("Nicht dieselben FileBytes!!");
            return false;

        }
        
    }
   //hallo Woerld
    public static void Main(string[] args)
        {
        Program a = new Program();
       // a.TestOrdner(@"D:\ab\1" , @"D:\ab\2");
        a.Compare("D:\\ab\\2\\a.txt", "D:\\ab\\2\\a.txt");
        a.Compare("D:\\ab\\2\\c.txt", "D:\\ab\\2\\d.txt");
        a.CompareFileBytes("D:\\ab\\2\\Praxisphasenvertrag.pdf", "D:\\ab\\2\\Zulassungsantrag_Praxisphase_FB_Technik.pdf");
      
       



    }
}

