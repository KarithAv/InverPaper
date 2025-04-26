using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace InverPaper.Utilidades
{
    public class EncriptadorUtilidad
    {
            public static string EncriptarPassword(string password)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }

            public static bool VerificarPassword(string passwordIngresada, string hashAlmacenado)
            {
                return BCrypt.Net.BCrypt.Verify(passwordIngresada, hashAlmacenado);
            }
    }
}


