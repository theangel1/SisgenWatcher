using FileWatcherService;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            string rutCli = "77725100-7";
            string connStr = "server=sisgenchile.com;user=sisgenchile_dbmanager;database=sisgenchile_sisgenfe;port=3306;password=--d5!RWN[LIm";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                //string sql = "select sis_contribuyente_estado from sis_contribuyente where sis_contribuyente_rut='" + rutCli + "'";
                string sql = "select * from sis_contribuyente where sis_contribuyente_rut='" + rutCli + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine("Estado: -- " + rdr[9]);
                    if (rdr[9].ToString() == "1")
                        Console.Write("Estado ok");
                    else
                        Console.Write("nook");

                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
            Console.Read();

        }
    }
}
