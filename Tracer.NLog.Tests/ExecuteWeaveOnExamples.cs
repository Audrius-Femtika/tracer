using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Tracer.Fody;
using Tracer.Fody.Filters;
using Tracer.Fody.Helpers;
using Tracer.Fody.Weavers;


namespace Tracer.Log4Net.Tests
{
    [TestFixture]
    public class ExecuteWeaveOnExamples
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [Test, Explicit, Category("manual")]
        public void WeaveMyApplication()
        {

            string src = @"D:\Work\TestNlogAutoLogger3\TestApplication.NLog\bin\Debug\TestApplication.NLog.exe";
            //string src = @"D:\Work\TestNlogAutoLogger3\TestApplication.NLog\bin\Release\TestApplication.NLog224.exe";
            string dst = @"D:\Work\TestNlogAutoLogger3\TestApplication.NLog\bin\Debug\TestApplication.NLog335.exe";
            System.IO.File.Copy(src, dst, true);
            System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(dst));
            Stopwatch stp = Stopwatch.StartNew();

            var configPath = @"D:\Work\Femtikon spaced\femtikon3d\C#\Femtikon3D\PostBuildToolsBins\LogWrapper\default.xaml";
            var configXML = XDocument.Load(configPath);
            
            var parser = FodyConfigParser.Parse(configXML.Root, configXML.Root);
            var config = parser.Result;
            AssemblyWeaver.Execute(dst, config);
            stp.Stop();
            Debug.WriteLine("-------------------took " + stp.ElapsedMilliseconds.ToString() + " ms");

        }
    }
}
