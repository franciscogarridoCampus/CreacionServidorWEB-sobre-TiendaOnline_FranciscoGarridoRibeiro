using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Tienda_FranciscoGarridoRibeiro.conexion;

namespace Tienda_FranciscoGarridoRibeiro
{
    /// <summary>
    /// Descripción breve de WSConectarBase
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WSConectarBase : System.Web.Services.WebService
    {
        /// <summary>
        /// Este metodo para conectarse a la base de datos de forma correcta a traves de la clase Conexion.cs
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string PruebaConexion()
        {
            Conexion oConexion = new Conexion();
            MySqlConnection conexion = oConexion.Conector();
            // 1. Cadena de conexión
            //string connString = "Server=localhost;Database=tiendadb;Uid=root;Pwd=;";
            //using (MySqlConnection conexion = new MySqlConnection(connString))
            {
                try
                {
                    
                   
                    Console.WriteLine("Conexión exitosa!");
                    

                    return "Conexion correcta";

                    
                }
                catch (Exception ex)
                {
 
                    return "Conexion incorrecta";
                }
            }

        }






























        //METODOS DENTRO DEL ARCHIVO DURANTE LAS CLASES
        
        /// <summary>
        /// Este metodo para conectarse a la base de datos de forma correcta
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string PruebaConexionAqui()
        {
           
            // 1. Cadena de conexión
            string connString = "Server=localhost;Database=tiendadb;Uid=root;Pwd=;";
            using (MySqlConnection conexion = new MySqlConnection(connString))
            {
                try
                {
                    
                    
                    Console.WriteLine("Conexión exitosa!");


                    return "Conexion correcta";


                }
                catch (Exception ex)
                {

                    return "Conexion incorrecta";
                }
            }

        }




        /// <summary>
        /// Conexion de base de datos con el select categoria, ejemplo de carlos
        /// </summary>
        /// <returns></returns>
        [WebMethod]
 
public string PruebaConexionCategorias()
{
    try
    {
        // Usar tu clase Conexion.cs para abrir la conexión
        Conexion oConexion = new Conexion();
        MySqlConnection conexion = oConexion.Conector();

        // Abrir la conexión (si no lo hiciera tu clase ya)
        if (conexion.State != System.Data.ConnectionState.Open)
            conexion.Open();

        string query = "SELECT NombreCategoria FROM categorias";

        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
        {
            // El parámetro @activo no existe en tu query, así que lo eliminamos

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Aquí podrías construir un string o lista si quieres mostrar resultados
                    // Por ahora lo dejamos vacío para mantener tu código original
                }
            }
        }

        return "Conexion correcta";
    }
    catch (Exception ex)
    {
        return "Conexion incorrecta: " + ex.Message;
    }
}

        
    }
}
