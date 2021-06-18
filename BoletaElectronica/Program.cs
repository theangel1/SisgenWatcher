using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BoletaElectronica
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string rutCliente = ConfigurationManager.AppSettings["rutCliente"];            
            string rutaBoleta = ConfigurationManager.AppSettings["ruta"]+rutCliente;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.FileName = "cmd.exe";
            //string rutaDTE = @"c:\toquihua\data\dte\"+args[0].ToString();            
            //string rutaDTE = @"c:\toquihua\data\dte\" + rutEmpresa;
            

            //rutaPHP
            
            
                //startInfo.Arguments = "/c php genFirma.php "+ eachTextos+ " "+ args[0]+ " "+ args[1];
                startInfo.Arguments = "/c php procesos//procesaBoletas.php " +  " " + rutCliente;
           
                process.StartInfo = startInfo;
                process.Start();
            
        }
    }
}
