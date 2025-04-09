using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace InverPaper.Utilities
{
    public class DBContextUtility
    {

        static string SERVER = "LENOVO24\\KARITH";
        static string DB_NAME = "ProyectoInverPaper";
        static string DB_USER = "prueba";
        static string DB_PASSWD = "281106";
        static string Conn = "server=" + SERVER + ";database=" + DB_NAME + ";user id =" + DB_USER + ";password=" + DB_PASSWD + ";MultipleActiveResultSets=true;";

        SqlConnection Con = new SqlConnection(Conn);

        public void Connect()
        {
            Debug.WriteLine("Holaaa");
            try
            {
                Con.Open();
                Debug.WriteLine("CONEXION EXITOSA");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public void Disconnect()
        {
            Con.Close();
        }
        public SqlConnection CONN()
        {
            return Con;
        }
    }
}