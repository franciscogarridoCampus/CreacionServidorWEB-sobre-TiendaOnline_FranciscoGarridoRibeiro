using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tienda_FranciscoGarridoRibeiro.conexion
{
    public class Conexion
    {
        string connString = "Server=localhost;Database=tiendadb;Uid=root;Pwd=;";
        MySqlConnection conexion;

        public Conexion() { 
            conexion= new MySqlConnection(connString);
            conexion.Open();
        }

        public MySqlConnection Conector() {
            return conexion;
        }

    }
}