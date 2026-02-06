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
    public class WSDetallePedidos : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hola a todos";
        }

        // 1. AGREGAR PRODUCTO AL DETALLE
        [WebMethod]
        public string AgregarDetalle(int pedidoID, int productoID, int cantidad, decimal precioUnitario)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();

                string query = "INSERT INTO detallepedidos (PedidoID, ProductoID, Cantidad, PrecioUnitario) " +
                               "VALUES (@pedido, @prod, @cant, @precio)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@pedido", pedidoID);
                    cmd.Parameters.AddWithValue("@prod", productoID);
                    cmd.Parameters.AddWithValue("@cant", cantidad);
                    cmd.Parameters.AddWithValue("@precio", precioUnitario);
                    cmd.ExecuteNonQuery();
                }
                return "Producto agregado al pedido correctamente.";
            }
            catch (Exception ex)
            {
                return "Error al agregar detalle: " + ex.Message;
            }
        }

        // 2. OBTENER DETALLES DE UN PEDIDO ESPECÍFICO (Punto 14 de la actividad)
        [WebMethod]
        public List<string> ConsultarDetallePedido(int pedidoID)
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();

                // Hacemos un JOIN simple para mostrar el nombre del producto en lugar de solo el ID
                string query = "SELECT p.Nombre, d.Cantidad, d.PrecioUnitario " +
                               "FROM detallepedidos d " +
                               "INNER JOIN productos p ON d.ProductoID = p.ProductoID " +
                               "WHERE d.PedidoID = @pedido";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@pedido", pedidoID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fila = $"Producto: {reader["Nombre"]} | Cant: {reader["Cantidad"]} | Precio: {reader["PrecioUnitario"]}";
                            lista.Add(fila);
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

        // 3. ELIMINAR UN ITEM DEL DETALLE
        [WebMethod]
        public string EliminarDetalle(int detalleID)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "DELETE FROM detallepedidos WHERE DetalleID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", detalleID);
                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0 ? "Detalle eliminado." : "No se encontró el ID.";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}