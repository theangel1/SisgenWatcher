using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            
            startInfo.WorkingDirectory = rutaLoc() + "/procesos";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            string[] archivosTXT = Directory.GetFiles(rutaDTE(), "*.txt").Select(Path.GetFileName).ToArray();
            
            foreach (string eachTextos in archivosTXT)
            {                
                startInfo.Arguments = "/c php genFirma.php "+ eachTextos+ " "+ args[1]+ " "+ args[2];
                process.StartInfo = startInfo;
                process.Start();              
            }
            eliminaFirmas();
        }

        private static void eliminaFirmas()
        {
            string directorio = Directory.GetCurrentDirectory();
            string str2 = "Provider=VFPOLEDB.1; Data Source =" + directorio + " ; Extended Properties = dBASE III; User ID=;Password=";
            OleDbConnection conexion = new OleDbConnection();
            conexion.Open();
            string consulta = "select Unidad from arcvia";
            OleDbDataAdapter adapter = new OleDbDataAdapter(consulta, conexion);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            string ruta = Convert.ToString(dataSet.Tables[0].Rows[0]["Unidad"].ToString()).Trim();
            conexion.Close();

            string path1 = ruta + "/procesos/firmas/";
            string path2 = ruta + "/procesos/firmas/respaldoFirmas/";

            DirectoryInfo d = new DirectoryInfo(path1);
            DateTime time = DateTime.Now.AddDays(-1.0);
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

        private static string rutaDTE()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string str2 = "Provider=VFPOLEDB.1; Data Source =" + currentDirectory + " ; Extended Properties = dBASE III; User ID=;Password=";
            OleDbConnection selectConnection = new OleDbConnection
            {
                ConnectionString = str2
            };
            selectConnection.Open();
            string selectCommandText = "select Rutadte from arcvia";
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommandText, selectConnection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            string str4 = Convert.ToString(dataSet.Tables[0].Rows[0]["Rutadte"].ToString()).Trim();
            selectConnection.Close();
            return str4;
        }

        private static string rutaLoc()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string str2 = "Provider=VFPOLEDB.1; Data Source =" + currentDirectory + " ; Extended Properties = dBASE III; User ID=;Password=";
            OleDbConnection selectConnection = new OleDbConnection
            {
                ConnectionString = str2
            };
            selectConnection.Open();
            string selectCommandText = "select Unidad from arcvia";
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommandText, selectConnection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            string str4 = Convert.ToString(dataSet.Tables[0].Rows[0]["Unidad"].ToString()).Trim();
            selectConnection.Close();
            return str4;
        }
    }
}
