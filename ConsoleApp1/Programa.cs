using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
namespace GenFirmas
{
    public class Programa
    {
        public static void Main(string[] args)
        {
            string rutEmpresa = ConfigurationManager.AppSettings["rut"];
            string rutRL = ConfigurationManager.AppSettings["rutRL"];
            string dataDTE = ConfigurationManager.AppSettings["ruta"];
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();            
            startInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            //string rutaDTE = @"c:\toquihua\data\dte\"+args[0].ToString();            
            //string rutaDTE = @"c:\toquihua\data\dte\" + rutEmpresa;
            string rutaDTE =  dataDTE+ rutEmpresa;
            string[] archivosTXT = Directory.GetFiles(rutaDTE, "*.txt").Select(Path.GetFileName).ToArray();
            
             
            foreach (string eachTextos in archivosTXT)
            {
                //startInfo.Arguments = "/c php genFirma.php "+ eachTextos+ " "+ args[0]+ " "+ args[1];
                startInfo.Arguments = "/c php genFirma.php " + eachTextos + " " + rutEmpresa + " " + rutRL;
                process.StartInfo = startInfo;
                process.Start();
            }
            System.Threading.Thread.Sleep(2000);
            pendientes();
            eliminaFirmas();
        }

        private static void pendientes()
        {
            string sisgen = ConfigurationManager.AppSettings["ruta"];
            string rutCliente = ConfigurationManager.AppSettings["rut"];
            string camino1 =sisgen + rutCliente + "/";
            string newFolder = camino1 + "pendientes/";
            
            //creacion de la carpeta pendientes
            if (!Directory.Exists(newFolder))            
                Directory.CreateDirectory(newFolder);

            DirectoryInfo dir2 = new DirectoryInfo(camino1);
            if (dir2.Exists)
            {
                string[] archivosTXT = Directory.GetFiles(camino1, "*.txt").Select(Path.GetFileName).ToArray();
                
                
                foreach (string eachTextos in archivosTXT)
                {
                  //  Console.WriteLine(eachTextos+" proyecto:nacho");
                    if (File.Exists(newFolder + eachTextos))
                        File.Delete(newFolder + eachTextos);

                    File.Move(camino1 + eachTextos, newFolder + eachTextos);
                }
                //Console.ReadKey();

            }
                      

        }
        private static void eliminaFirmas()
        {
            string directorio = ConfigurationManager.AppSettings["rutaGenfirma"];
            /*
                Console.WriteLine("Directorio: " + directorio);
                Console.ReadKey();
            */
            string path1 = directorio + "/firmas/";          
            string path2 = directorio + "/firmas/respaldoFirmas/";
           

            DirectoryInfo d = new DirectoryInfo(path1);
            DateTime time = DateTime.Now.AddDays(-1);
            DateTime to_time = DateTime.Now;
                if (!Directory.Exists(path2))
                {
                    Directory.CreateDirectory(path2);
                }
            DirectoryInfo info2 = new DirectoryInfo(path1);
                if (info2.Exists)
                {
                    (from file in info2.GetFiles("*.png")
                     where file.LastWriteTime < to_time.AddDays(-2.0)
                     select file).ToList<FileInfo>().ForEach(f => File.Move(path1 + f, path2 + f));
                }
        }

        /*
        private void watch()
        {
            FileSystemWatcher watch = new FileSystemWatcher();
            watch.Path = @"c:\toquihua\data\dte\76451885";
            watch.NotifyFilter = NotifyFilters.CreationTime;
            watch.Filter = "*.txt";
            watch.EnableRaisingEvents = true;
        }
        */
        
    }
}
