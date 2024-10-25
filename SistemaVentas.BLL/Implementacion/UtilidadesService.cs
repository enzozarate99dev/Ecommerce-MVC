using SistemaVentas.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVentas.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        //Encripta la clave para guardarla en la bd
        public string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();
            
            using (SHA256 hash = SHA256.Create()) 
            { 
            
                Encoding enc = Encoding.UTF8;

                byte[] res = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in res) 
                { 
                    sb.Append(b.ToString("x2")); // Conversión a hexadecimal
                }
            }
            return sb.ToString();
        }

        public string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);
            return clave;
        }
    }
}
