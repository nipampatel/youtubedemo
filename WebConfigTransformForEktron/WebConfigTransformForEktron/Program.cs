using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace WebConfigTransformForEktron
{
    class Program
    {
        
        private static readonly string _TRANSFORM_KEY = "transformPath";
        private static readonly string _CONFIG_PATH_FILE = "configPathFile";
        private static string _transformPath = null;
        private static string _configPath = null;
        static void Main(string[] args)
        {
            ReadAppSettings();

            // Get all web.config paths and trasform same
            List<string> webConfigPaths = ParseConfigPaths();
            List<string> failures = new List<string>();
            foreach (string webConfigPath in webConfigPaths)
            {
                try
                {
                    Console.WriteLine("New File..................");
                    Console.WriteLine(Transform(webConfigPath));
                }
                catch (Exception exception)
                {
                    failures.Add(string.Format("{0} -> {1}", webConfigPath, exception.Message));
                }
            }
            PrintFailures(failures);
        }

        private static void PrintFailures(List<string> failures)
        {
            foreach (string failure in failures)
            {
                Console.WriteLine(failure);
            }
        }

        private static void ReadAppSettings()
        {
            _configPath = ConfigurationManager.AppSettings[_CONFIG_PATH_FILE];
            Debug.Assert(_configPath != null);
            Console.WriteLine("Configs path {0}", _configPath);
            // Make sure transform file exist
            Debug.Assert(File.Exists(_configPath));

            _transformPath = ConfigurationManager.AppSettings[_TRANSFORM_KEY];
            Debug.Assert(_transformPath != null);
            Console.WriteLine("Transform path {0}", _transformPath);
            // Make sure transform file exist
            Debug.Assert(File.Exists(_transformPath));
        }
        private static string Transform(string filePath)
        {   
            string output = null;
            if (File.Exists(filePath))
            {
                // Get file path
                string root = Path.GetDirectoryName(filePath);
                string filename = Path.GetFileName(filePath);
                Console.WriteLine("Destination {0}\\{1}", root, filename); 
                Process transformProcess = new Process();
                transformProcess.StartInfo.UseShellExecute = false;
                transformProcess.StartInfo.RedirectStandardOutput = false;
                transformProcess.StartInfo.FileName = "ctt.exe";
                transformProcess.StartInfo.Arguments = string.Format(" source:{0} transform:{1} destination:{2}\\{3} pw", filePath, _transformPath, root, filename);
                Console.WriteLine(string.Format("ctt.exe source:{0} transform:{1} destination:{2}\\{3} pw", filePath, _transformPath, root, filename));
                transformProcess.StartInfo.CreateNoWindow = false;
                transformProcess.StartInfo.RedirectStandardOutput = true;
                transformProcess.Start();
                output = transformProcess.StandardOutput.ReadToEnd();
                transformProcess.WaitForExit();
            }
            return output;
        }

        private static List<string> ParseConfigPaths()
        {
            List<string> paths = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(_configPath))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        paths.Add(line);
                    }
                }
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Console.WriteLine("Could not find file. Please check path and try again");
                Console.WriteLine(fileNotFoundException.Message);
            }
            return paths;
        }
    }
}
