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
        // -------------------------------
        // 1. CREAR PRODUCTO
        // -------------------------------
        [WebMethod]
        public string CrearProducto(string nombre, string descripcion, decimal precio, int stock, int categoriaID)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();

                // Comprobar si hay alguna categoría en la base
                string queryCountCategorias = "SELECT COUNT(*) FROM categorias";
                using (MySqlCommand cmdCount = new MySqlCommand(queryCountCategorias, conexion))
                {
                    int totalCategorias = Convert.ToInt32(cmdCount.ExecuteScalar());
                    if (totalCategorias == 0)
                        return "Error: No existen categorías. Crea al menos una categoría antes de crear un producto.";
                }

                // Validar que la categoría seleccionada exista
                string queryValidar = "SELECT COUNT(*) FROM categorias WHERE CategoriaID=@catID";
                using (MySqlCommand cmdVal = new MySqlCommand(queryValidar, conexion))
                {
                    cmdVal.Parameters.AddWithValue("@catID", categoriaID);
                    int count = Convert.ToInt32(cmdVal.ExecuteScalar());
                    if (count == 0)
                        return "Error: La categoría seleccionada no existe.";
                }

                // Insertar producto
                string query = "INSERT INTO productos (Nombre, Descripcion, Precio, Stock, CategoriaID) " +
                               "VALUES (@nom, @des, @pre, @sto, @cat)";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@des", descripcion);
                    cmd.Parameters.AddWithValue("@pre", precio);
                    cmd.Parameters.AddWithValue("@sto", stock);
                    cmd.Parameters.AddWithValue("@cat", categoriaID);
                    cmd.ExecuteNonQuery();
                }

                return $"Producto '{nombre}' creado correctamente.";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // -------------------------------
        // 2. OBTENER PRODUCTOS (filtros opcionales)
        // -------------------------------
        [WebMethod]
        public List<string> ObtenerProductos(string nombreCategoria = "", string precioMin = "", string precioMax = "")
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();

                string query = "SELECT p.ProductoID, p.Nombre, p.Precio, p.Stock, c.NombreCategoria " +
                               "FROM productos p " +
                               "INNER JOIN categorias c ON p.CategoriaID = c.CategoriaID " +
                               "WHERE 1=1";

                // Aplicar filtros solo si se rellena algo
                if (!string.IsNullOrEmpty(nombreCategoria))
                    query += " AND c.NombreCategoria LIKE @nombreCat";
                if (!string.IsNullOrEmpty(precioMin))
                    query += " AND p.Precio >= @precioMin";
                if (!string.IsNullOrEmpty(precioMax))
                    query += " AND p.Precio <= @precioMax";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    if (!string.IsNullOrEmpty(nombreCategoria))
                        cmd.Parameters.AddWithValue("@nombreCat", "%" + nombreCategoria + "%");
                    if (!string.IsNullOrEmpty(precioMin))
                        cmd.Parameters.AddWithValue("@precioMin", decimal.Parse(precioMin));
                    if (!string.IsNullOrEmpty(precioMax))
                        cmd.Parameters.AddWithValue("@precioMax", decimal.Parse(precioMax));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add($"ID: {reader["ProductoID"]} | {reader["Nombre"]} | Precio: {reader["Precio"]} | Stock: {reader["Stock"]} | Categoria: {reader["NombreCategoria"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista.Add("Error: " + ex.Message);
            }
            return lista;
        }


        // -------------------------------
        // 3. BUSCAR PRODUCTOS (por nombre de producto o nombre de categoría)
        // -------------------------------
        [WebMethod]
        public List<string> BuscarProductos(string nombreProducto = "", string nombreCategoria = "")
        {
            List<string> lista = new List<string>();

            // Si no se rellena ninguno, no devuelve nada
            if (string.IsNullOrEmpty(nombreProducto) && string.IsNullOrEmpty(nombreCategoria))
                return lista;

            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();

                string query = "SELECT p.Nombre, p.Precio, p.Stock, c.NombreCategoria " +
                               "FROM productos p " +
                               "INNER JOIN categorias c ON p.CategoriaID = c.CategoriaID " +
                               "WHERE 1=1";

                if (!string.IsNullOrEmpty(nombreProducto))
                    query += " AND p.Nombre LIKE @nombreProd";
                if (!string.IsNullOrEmpty(nombreCategoria))
                    query += " AND c.NombreCategoria LIKE @nombreCat";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    if (!string.IsNullOrEmpty(nombreProducto))
                        cmd.Parameters.AddWithValue("@nombreProd", "%" + nombreProducto + "%");
                    if (!string.IsNullOrEmpty(nombreCategoria))
                        cmd.Parameters.AddWithValue("@nombreCat", "%" + nombreCategoria + "%");

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add($"Nombre: {reader["Nombre"]} | Precio: {reader["Precio"]} | Stock: {reader["Stock"]} | Categoria: {reader["NombreCategoria"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista.Add("Error: " + ex.Message);
            }

            return lista;
        }

        // -------------------------------
        // 4. ACTUALIZAR PRODUCTO
        // -------------------------------
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
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // -------------------------------
        // 5. ELIMINAR PRODUCTO
        // -------------------------------
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
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
