using MySqlConnector;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileWatchingService
{
    public class FileWatcher
    {
        private static string _rutaSisgenBase = ConfigurationManager.AppSettings["sisgenBase"];
        private static string rutaBoleta = _rutaSisgenBase + @"procesos/";
        private static string rutMain = ConfigurationManager.AppSettings["rutClienteMain"];
        private static readonly string rutSinDV = rutMain.Substring(0, rutMain.Length - 2);
        private FileSystemWatcher _fileWatcher, _fileWatcherBoleta, _fileWatcherRcof;
        public const string connStr = "datasource=sisgenchile.com;Database=sisgenchile_sisgenfe;port=3306;username=sisgenchile_dbmanager;password=--d5!RWN[LIm;SslMode=none;persistsecurityinfo=True";
        private static string _segundos = ConfigurationManager.AppSettings["segundosEspera"];
        public FileWatcher()
        {
            Init();
        }

        private void Init()
        {
            //Creo directorio de boletas si es que no existe, luego procedo a iniciar el Watcher

            if (!Directory.Exists(_rutaSisgenBase + @"data/sisgendte/boleta/" + rutSinDV))
                Directory.CreateDirectory(_rutaSisgenBase + @"data/sisgendte/boleta/" + rutSinDV);

            _fileWatcherBoleta = new FileSystemWatcher(_rutaSisgenBase + @"data/sisgendte/boleta/" + rutSinDV);
            _fileWatcherBoleta.Filter = "*.txt";
            _fileWatcherBoleta.Created += new FileSystemEventHandler(_fileWatcherBoleta_Created);
            _fileWatcherBoleta.Changed += new FileSystemEventHandler(_fileWatcherBoleta_Changed);
            _fileWatcherBoleta.Renamed += new RenamedEventHandler(_fileWatcherBoleta_Renamed);
            _fileWatcherBoleta.EnableRaisingEvents = true;


            //Creo directorio de dte si es que no existe, luego procedo a iniciar el Watcher

            if (!Directory.Exists(_rutaSisgenBase + @"data/dte/" + rutSinDV))
                Directory.CreateDirectory(_rutaSisgenBase + @"data/dte/" + rutSinDV);

            _fileWatcher = new FileSystemWatcher(_rutaSisgenBase + @"data/dte/" + rutSinDV);
            _fileWatcher.Filter = "*.txt";
            _fileWatcher.Created += new FileSystemEventHandler(_fileWatcher_Created);
            _fileWatcher.Changed += new FileSystemEventHandler(_fileWatcher_Changed);
            _fileWatcher.Renamed += new RenamedEventHandler(_fileWatcher_Renamed);
            _fileWatcher.EnableRaisingEvents = true;


            //Creo directorio de rcof si es que no existe, luego procedo a iniciar el Watcher

            if (!Directory.Exists(_rutaSisgenBase + @"data/sisgendte/rcof/" + rutSinDV))
                Directory.CreateDirectory(_rutaSisgenBase + @"data/sisgendte/rcof/" + rutSinDV);

            _fileWatcherRcof = new FileSystemWatcher(_rutaSisgenBase + @"data/sisgendte/rcof/" + rutSinDV);
            _fileWatcherRcof.Filter = "*.txt";
            _fileWatcherRcof.Created += new FileSystemEventHandler(_fileWatcherRcof_Created);
            _fileWatcherRcof.Changed += new FileSystemEventHandler(_fileWatcherRcof_Changed);
            _fileWatcherRcof.Renamed += new RenamedEventHandler(_fileWatcherRcof_Renamed);
            _fileWatcherRcof.EnableRaisingEvents = true;



        }
        #region Eventos FileWatcher      

        private void _fileWatcherRcof_Renamed(object sender, RenamedEventArgs e)
        {
            TheWriter(string.Format("Archivo Renombrado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            ProcesaConsumos();

        }

        private void _fileWatcherRcof_Changed(object sender, FileSystemEventArgs e)
        {
            TheWriter(string.Format("Archivo cambiado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            ProcesaConsumos();
        }

        private void _fileWatcherRcof_Created(object sender, FileSystemEventArgs e)
        {
            TheWriter(string.Format("Archivo creado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            ProcesaConsumos();
        }

        private void _fileWatcherBoleta_Renamed(object sender, RenamedEventArgs e)
        {
            TheWriter(string.Format("Archivo renombrado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            GenBoleta();
        }

        private void _fileWatcherBoleta_Changed(object sender, FileSystemEventArgs e)
        {
            TheWriter(string.Format("Archivo cambiado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            GenBoleta();

        }

        private void _fileWatcherBoleta_Created(object sender, FileSystemEventArgs e)
        {
            TheWriter(string.Format("Archivo creado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            GenBoleta();
        }


        private void _fileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            TheWriter(string.Format("Archivo dte renombrado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            ProService10();

        }

        private void _fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            TheWriter(string.Format("Archivo dte cambiado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            ProService10();

        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            TheWriter(string.Format("Archivo dte creado: Ruta: {0}, Nombre: {1}", e.FullPath, e.Name));
            ProService10();

        }
        #endregion

        //Genera firma electrónica en formato pdf417
        private static void ProService10()
        {

            EliminaFirmas();
            ConsultaEstadoFacturacion();

            string rutEmpresa = rutSinDV;
            string rutRL = ConfigurationManager.AppSettings["rutRL"];
            string dataDTE = _rutaSisgenBase + @"data/dte/" + rutSinDV;
            string rutCliente = ConfigurationManager.AppSettings["rutCliente"];
            string camino1 = dataDTE + "/";
            string newFolder = camino1 + "pendientes/";

            if (!Directory.Exists(newFolder))
                Directory.CreateDirectory(newFolder);

            if (ConfigurationManager.AppSettings["isBloqued"].ToString() == "false")
            {
                try
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        WorkingDirectory = rutaBoleta
                    };

                    TheWriter("Segundos de espera: " + int.Parse(_segundos));
                    System.Threading.Thread.Sleep(int.Parse(_segundos));

                    string[] archivosTXT = Directory.GetFiles(dataDTE, "*.txt").Select(Path.GetFileName).ToArray();


                    foreach (string eachTextos in archivosTXT)
                    {
                        startInfo.Arguments = "/c php genFirma.php " + eachTextos + " " + rutEmpresa + " " + rutRL;
                        TheWriter("Argumentos: " + startInfo.Arguments);
                        process.StartInfo = startInfo;
                        process.Start();
                        process.WaitForExit();
                        process.Close();

                        //Leo el archivo para luego usar ciertos datos y guardarlos en base de datos.
                        string[] separador = { ";" };
                        string[] separadorLineas = { "~" };
                        string[] lines = File.ReadAllLines(_rutaSisgenBase + @"data/dte/" + rutSinDV + "/" + eachTextos);
                        string[] aCabecera = lines[0].Replace("[", "").Replace("]", "").Split(separador, StringSplitOptions.None);

                        string tipoDocumento = aCabecera[0];
                        string folio = aCabecera[1];
                        bool estado = false;


                        using (MySqlConnection conn = new MySqlConnection(connStr))
                        {
                            try
                            {
                                string insertData = "INSERT INTO Bitacora_core(Folio, TipoDocumento, Rut_Empresa, Estado) " +
                                    "values(@folio, @tipoDocumento, @rutEmpresa,@estado)";
                                MySqlCommand cmd = new MySqlCommand(insertData, conn);
                                cmd.Parameters.AddWithValue("@folio", folio);
                                cmd.Parameters.AddWithValue("@tipoDocumento", tipoDocumento);
                                cmd.Parameters.AddWithValue("@rutEmpresa", rutMain);
                                cmd.Parameters.AddWithValue("@estado", estado);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();

                            }
                            catch (Exception ex)
                            {
                                TheWriter("No se pudo conectar a la base de datos debido a " + ex.ToString());
                            }
                        }


                        if (File.Exists(newFolder + eachTextos))
                            File.Delete(newFolder + eachTextos);

                        File.Move(camino1 + eachTextos, newFolder + eachTextos);

                    }
                }
                catch (Exception ex)
                {
                    TheWriter("Watcher atrapo un error: " + ex.Message);
                }
            }
            else
                TheWriter("Warning: ****Cliente Bloqueado por Sisgen Chile Services****");

        }



        //metodo que mueve las firmas que son antiguas
        private static void EliminaFirmas()
        {
            string directorio = _rutaSisgenBase + @"procesos/";
            string path1 = directorio + @"firmas/";
            string path2 = directorio + @"firmas/respaldoFirmas/";


            DirectoryInfo d = new DirectoryInfo(path1);
            DateTime time = DateTime.Now.AddDays(-1);
            DateTime to_time = DateTime.Now;
            if (!Directory.Exists(path2))
                Directory.CreateDirectory(path2);

            DirectoryInfo info2 = new DirectoryInfo(path1);

            if (info2.Exists)
            {
                try
                {
                    (from file in info2.GetFiles("*.png")
                     where file.LastWriteTime < to_time.AddDays(-2.0)
                     select file).ToList().ForEach(f => File.Move(path1 + f, path2 + f));
                }
                catch (Exception ex)
                {
                    TheWriter("Excepcion en Elimina firmas: " + ex.ToString());
                }
            }
        }

        private static void ProcesaBoletas()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                WorkingDirectory = Path.GetDirectoryName(_rutaSisgenBase),
                Arguments = "/c START php procesos//procesaBoletas.php " + rutSinDV
            };
            Process.Start(processInfo);
        }
        private static void GenBoleta()
        {

            EliminaFirmas();
            //ConsultaEstadoFacturacion();
            string rutEmpresa = rutSinDV;
            string rutRL = ConfigurationManager.AppSettings["rutRL"];
            string dataDTE = _rutaSisgenBase + @"data/sisgendte/boleta/" + rutSinDV;

            if (ConfigurationManager.AppSettings["isBloqued"].ToString() == "false")
            {
                try
                {
                    Process process = new Process();

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        WorkingDirectory = rutaBoleta
                    };


                    TheWriter("Segundos de espera: " + int.Parse(_segundos));
                    System.Threading.Thread.Sleep(int.Parse(_segundos));

                    string[] archivosTXT = Directory.GetFiles(dataDTE, "*.txt").Select(Path.GetFileName).ToArray();

                    string pejec = rutaBoleta + "pejec.eje";

                    if (File.Exists(pejec))
                        File.Delete(pejec);

                    foreach (string eachTextos in archivosTXT)
                    {
                        startInfo.Arguments = "/c php genboleta.php " + eachTextos + " " + rutEmpresa + " " + rutRL;
                        process.StartInfo = startInfo;
                        process.Start();
                        process.WaitForExit();
                        process.Close();

                        if (File.Exists(dataDTE + "/" + eachTextos))
                            File.Delete(dataDTE + "/" + eachTextos);

                    }
                }
                catch (Exception ex)
                {
                    TheWriter("GenBoleta error: " + ex.Message);
                }
            }
            else
                TheWriter("Cliente Bloqueado en servidor Sisgen.");

        }

        private static void ProcesaConsumos()
        {
            ConsultaEstadoFacturacion();
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                WorkingDirectory = Path.GetDirectoryName(_rutaSisgenBase),
                Arguments = "/c START php procesos//procesaConsumos.php " + rutSinDV
            };
            Process.Start(processInfo);
        }

        private static void ConsultaEstadoFacturacion()
        {

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                TheWriter(DateTime.Now + ": Sisgen Watcher: Verificando bloqueo cliente");
                conn.Open();
                string sql = "select * from sis_contribuyente where sis_contribuyente_rut='" + rutMain + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr[9].ToString() != "1") //1 ES ACTIVO. CUALQUIER OTRO NUMERO ES BLOQUEO
                        UpdateConfig("true");
                    else
                        UpdateConfig("false");
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                TheWriter(ex.ToString());
            }
            conn.Close();

        }

        private static void UpdateConfig(string value)
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["isBloqued"].Value = value;
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                TheWriter("UpdateConfig error: " + ex.Message);
            }

        }

        //metodo que me permite escribir en un log
        public static void TheWriter(string mensaje)
        {
            try
            {
                string _mensaje = string.Format("{0} {1}", mensaje, Environment.NewLine);
                File.AppendAllText(_rutaSisgenBase + @"procesos/Sisgen_Watcher.log", _mensaje);
            }
            catch (Exception ex)
            {
                File.AppendAllText(_rutaSisgenBase + @"procesos/Sisgen_Watcher.log", ex.ToString());
            }

        }
    }
}
