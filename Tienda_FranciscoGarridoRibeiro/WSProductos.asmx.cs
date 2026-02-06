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
    public class WSProductos : System.Web.Services.WebService
    {
        // 1. CREAR PRODUCTO (Punto 6)
        [WebMethod]
        public string CrearProducto(string nombre, string descripcion, decimal precio, int stock, int categoriaID)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "INSERT INTO productos (Nombre, Descripcion, Precio, Stock, CategoriaID) VALUES (@nom, @des, @pre, @sto, @cat)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@des", descripcion);
                    cmd.Parameters.AddWithValue("@pre", precio);
                    cmd.Parameters.AddWithValue("@sto", stock);
                    cmd.Parameters.AddWithValue("@cat", categoriaID);
                    cmd.ExecuteNonQuery();
                }
                return "Producto '" + nombre + "' creado correctamente.";
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        // 2. OBTENER PRODUCTOS (Punto 9)
        [WebMethod]
        public List<string> ObtenerProductos()
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "SELECT ProductoID, Nombre, Precio, Stock FROM productos";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add($"ID: {reader["ProductoID"]} | {reader["Nombre"]} | Precio: {reader["Precio"]} | Stock: {reader["Stock"]}");
                    }
                }
            }
            catch (Exception ex) { lista.Add("Error: " + ex.Message); }
            return lista;
        }

        // 3. BUSCAR PRODUCTOS POR NOMBRE (Punto 10)
        [WebMethod]
        public List<string> BuscarProductos(string termino)
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "SELECT Nombre, Precio FROM productos WHERE Nombre LIKE @termino";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@termino", "%" + termino + "%");
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add($"{reader["Nombre"]} - ${reader["Precio"]}");
                        }
                    }
                }
            }
            catch (Exception ex) { lista.Add("Error: " + ex.Message); }
            return lista;
        }

        // 4. ACTUALIZAR PRODUCTO (Punto 7)
        [WebMethod]
        public string ActualizarProducto(int id, decimal nuevoPrecio, int nuevoStock)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "UPDATE productos SET Precio = @pre, Stock = @sto WHERE ProductoID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@pre", nuevoPrecio);
                    cmd.Parameters.AddWithValue("@sto", nuevoStock);
                    cmd.Parameters.AddWithValue("@id", id);
                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0 ? "Producto actualizado." : "No se encontró el producto.";
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        // 5. ELIMINAR PRODUCTO (Punto 8)
        [WebMethod]
        public string EliminarProducto(int id)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "DELETE FROM productos WHERE ProductoID = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0 ? "Producto eliminado." : "No se encontró el ID.";
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }
    }
}