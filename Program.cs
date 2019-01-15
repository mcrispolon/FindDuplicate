using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;

namespace FindDuplicateFile
{
    class Program
    {

        static private SHA256 Sha256 = SHA256.Create();
        static private readonly string _json = "1";
        static private readonly string _xml = "2";

        static private string _folderPath;
        static private string _outputFormat;


        static void Main(string[] args)
        {
            ProcessFolderInput();
            ProcessFormatInput();

            ConsoleKeyInfo k;
            Console.WriteLine("\n Press ESC to exit...");
            while (true)
            {
                k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.Escape)
                    break;
            }

        }


        /// <summary>
        /// Get Sha256 Hash
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static private string GetHash(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                byte[] sha256 = Sha256.ComputeHash(stream);
                return Convert.ToBase64String(sha256);
            }
        }


        private static void CheckDuplicate()
        {
            List<string> files = new List<string>(Directory.EnumerateFiles(_folderPath, "*.*", SearchOption.AllDirectories));

            List<DuplicateFile> duplicateFiles = new List<DuplicateFile>();

            foreach (var f in files)
            {
                var dFile = new DuplicateFile()
                {
                    FileHash = GetHash(f),
                    FileName = Path.GetFileName(f),
                    FileDirectory = Path.GetDirectoryName(f)
                };

                duplicateFiles.Add(dFile);
            }

            var result = duplicateFiles.GroupBy(d => d.FileHash)
                                    .Where(fh => fh.Count() > 1)
                                    .Select(fg => new FileGrouping() { File = fg.Key, Files = fg.ToList() })
                                    .ToList();

            if(result.Count < 1)
            {
                Console.WriteLine(String.Format("\n No duplicate files in {0}", _folderPath));
            }
            else
            {
                if (_outputFormat == _json)
                {
                    //json
                    string jsonResult = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
                    string jsonPath = Path.Combine(_folderPath, "output.json");
                    File.WriteAllText(jsonPath, jsonResult);
                    Console.WriteLine(String.Format("\n Check output.json file in {0}", _folderPath));
                }
                else
                {
                    //xml
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<FileGrouping>));
                    string xmlPath = Path.Combine(_folderPath, "output.xml");
                    using (StreamWriter sw = new StreamWriter(xmlPath))
                    {
                        xmlSerializer.Serialize(sw, result);
                    }

                    Console.WriteLine(String.Format("\n Check output.xml file in {0}", _folderPath));
                }
            }
        }

        private static void ProcessFolderInput()
        {
            Console.Write("\n Enter folder path : ");
            _folderPath = Console.ReadLine();

            try
            {
                //Check if valid path
                FileAttributes attr = File.GetAttributes(_folderPath);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    if (!Directory.Exists(_folderPath))
                    {
                        Console.WriteLine("Folder path does not exist");
                        ProcessFolderInput();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid folder path");
                    ProcessFolderInput();
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("\n" + ex.Message);
                ProcessFolderInput();
            }

        }

        private static void ProcessFormatInput()
        {
            Console.Write("\n Output format options: \n  \t 1 - JSON \n \t 2 - XML \n Enter number for output format: ");
            //Console.Write("\n Enter number for output format: ");
            _outputFormat = Console.ReadLine();

            if ((_outputFormat == _json || _outputFormat == _xml))
            {
                CheckDuplicate();
            }
            else
            {
                Console.WriteLine("\n Invalid output format");
                ProcessFormatInput();
            }


            
        }
    }
}
