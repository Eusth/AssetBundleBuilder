using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssetBundleBuilder
{
    class Program
    {
        static IDictionary<string, string> _Definitions = new Dictionary<string, string>()
        {
            //{  @"E:\Program Files\Unity 5.6.0f3\Editor\Unity.exe", "VRGIN_5_6" },
            //{  @"E:\Program Files\Unity 5.5.1\Editor\Unity.exe", "VRGIN_5_5" },
            //{  @"E:\Program Files\Unity 5.4.2p4\Editor\Unity.exe", "VRGIN_5_4" },
            //{  @"E:\Program Files\Unity 5.3.8f1\Editor\Unity.exe", "VRGIN_5_3" },
            //{  @"E:\Program Files\Unity 5.2.5f1\Editor\Unity.exe", "VRGIN_5_2" },
            {  @"E:\Program Files\Unity 5.0.0f4\Editor\Unity.exe", "VRGIN_5_0" },
        };

        static void Main(string[] args)
        {

            var filesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Files");
            Parallel.Invoke(new ParallelOptions()
            {
            }, _Definitions.Select(entrySet => new Action(delegate { ExportAssetBundles(entrySet.Key, filesPath, entrySet.Value); }) ).ToArray());
        }

        public static void ExportAssetBundles(string unityPath, string filesPath, string bundleName)
        {
           
            var projectPath = GetTemporaryDirectory();
            var assetsPath = Path.Combine(projectPath, "Assets");
            var contentPath = Path.Combine(assetsPath, "Content");
            var scriptPath = Path.Combine(assetsPath, "Editor", "CreateAssetBundles.cs");
            var createBundlesInvocation = "CreateAssetBundles.BuildAllAssetBundles";

            try
            {
                // Create temp project
                var process = Process.Start(unityPath, "-batchmode -quit -createProject \"" + projectPath + "\"");
                process.WaitForExit();

                // Copy files
                foreach (var file in Directory.EnumerateFiles(filesPath, "*", SearchOption.AllDirectories))
                {
                    var target = new FileInfo(Path.Combine(contentPath, file.Substring(filesPath.Length + 1)));
                    target.Directory.Create();
                    File.Copy(file, target.FullName, true);
                    Console.WriteLine("{0} -> {1}", file, target.FullName);
                }

                // Copy script file
                var scriptFile = PrepareScriptFile(Resources.CreateAssetBundles, contentPath, bundleName, Environment.CurrentDirectory);
                new FileInfo(scriptPath).Directory.Create();
                File.WriteAllText(scriptPath, scriptFile);

                // Execute script
                process = Process.Start(unityPath, "-batchmode -quit -projectPath \"" + projectPath + "\" -executeMethod " + createBundlesInvocation);
                Console.WriteLine("{0} {1}", unityPath, process.StartInfo.Arguments);
                process.WaitForExit();
            }
            finally
            {
                // Clean up
                try
                {
                    if (Directory.Exists(projectPath))
                    {
                        Directory.Delete(projectPath, true);
                    }
                } catch(Exception e)
                {
                    Console.Error.WriteLine("Could not clean up {0}: {1}", projectPath, e);
                }
            }
        }

        private static string PrepareScriptFile(string script, string contentPath, string bundleName, string outputDir)
        { 
            return script
                .Replace("{ASSETS_PATH}", contentPath)
                .Replace("{BUNDLE_NAME}", bundleName)
                .Replace("{OUTPUT_PATH}", outputDir);
        }

        static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
