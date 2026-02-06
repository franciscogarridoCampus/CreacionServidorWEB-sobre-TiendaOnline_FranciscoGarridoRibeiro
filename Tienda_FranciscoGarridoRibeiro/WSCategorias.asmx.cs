using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Web.Services;
using Tienda_FranciscoGarridoRibeiro.conexion;

namespace Tienda_FranciscoGarridoRibeiro
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class WSCategorias : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hola a todos";
        }

        [WebMethod]
        public string CrearCategorias(string nombre)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "INSERT INTO categorias (NombreCategoria) VALUES (@nombre)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.ExecuteNonQuery();
                }
                return "Categoría '" + nombre + "' creada con éxito.";
            }
            catch (Exception ex)
            {
                return "Error al crear: " + ex.Message;
            }
        }

        [WebMethod]
        public string ActualizarCategorias(int id, string nuevoNombre)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "UPDATE categorias SET NombreCategoria = @nombre WHERE CategoriaID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", nuevoNombre);
                    cmd.Parameters.AddWithValue("@id", id);
                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0 ? "Categoría actualizada." : "No se encontró el ID.";
                }
            }
            catch (Exception ex)
            {
                return "Error al actualizar: " + ex.Message;
            }
        }

        [WebMethod]
        public string BorrarCategorias(int id)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "DELETE FROM categorias WHERE CategoriaID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0 ? "Categoría eliminada." : "No se encontró el ID.";
                }
            }
            catch (Exception ex)
            {
                return "Error al borrar: " + ex.Message;
            }
        }

        [WebMethod]
        public List<string> BuscarCategorias()
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "SELECT NombreCategoria FROM categorias";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(reader["NombreCategoria"].ToString());
                        }
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                lista.Add("Error: " + ex.Message);
                return lista;
            }
        }
    }
}