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

        // 1. CONSULTAR DETALLES DE UN PEDIDO
        [WebMethod]
        public List<string> ConsultarDetallePedido(int pedidoID)
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                using (MySqlConnection conexion = oConexion.Conector())
                {
                    string query = "SELECT p.Nombre, d.Cantidad, d.PrecioUnitario, (d.Cantidad*d.PrecioUnitario) as Subtotal " +
                                   "FROM detallepedidos d INNER JOIN productos p ON d.ProductoID = p.ProductoID WHERE d.PedidoID=@pedido";
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@pedido", pedidoID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add($"Producto: {reader["Nombre"]} | Cant: {reader["Cantidad"]} | Precio Unitario: {reader["PrecioUnitario"]} | Subtotal: {reader["Subtotal"]}");
                            }
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

        // 2. ELIMINAR DETALLE
        [WebMethod]
        public string EliminarDetalle(int detalleID)
        {
            try
            {
                Conexion oConexion = new Conexion();
                using (MySqlConnection conexion = oConexion.Conector())
                {
                    string query = "DELETE FROM detallepedidos WHERE DetalleID = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", detalleID);
                        int filas = cmd.ExecuteNonQuery();
                        return filas > 0 ? "Detalle eliminado." : "No se encontró el ID.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // 3. HISTORIAL DE COMPRAS (Pedidos + Detalles con subtotal)
        [WebMethod]
        public List<string> HistorialCompras(int usuarioID)
        {
            List<string> historial = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                using (MySqlConnection conexion = oConexion.Conector())
                {
                    string queryPedidos = "SELECT PedidoID, FechaPedido, Estado FROM pedidos WHERE UsuarioID=@user";
                    using (MySqlCommand cmdPedidos = new MySqlCommand(queryPedidos, conexion))
                    {
                        cmdPedidos.Parameters.AddWithValue("@user", usuarioID);
                        using (MySqlDataReader readerPedidos = cmdPedidos.ExecuteReader())
                        {
                            while (readerPedidos.Read())
                            {
                                int pedidoID = Convert.ToInt32(readerPedidos["PedidoID"]);
                                historial.Add($"Pedido ID: {pedidoID} | Fecha: {readerPedidos["FechaPedido"]} | Estado: {readerPedidos["Estado"]}");

                                // Detalles
                                using (MySqlConnection conexionDetalle = oConexion.Conector())
                                {
                                    string queryDetalle = "SELECT p.Nombre, d.Cantidad, d.PrecioUnitario, (d.Cantidad*d.PrecioUnitario) as Subtotal " +
                                                          "FROM detallepedidos d INNER JOIN productos p ON d.ProductoID=p.ProductoID WHERE d.PedidoID=@pedido";
                                    using (MySqlCommand cmdDetalle = new MySqlCommand(queryDetalle, conexionDetalle))
                                    {
                                        cmdDetalle.Parameters.AddWithValue("@pedido", pedidoID);
                                        using (MySqlDataReader readerDetalle = cmdDetalle.ExecuteReader())
                                        {
                                            while (readerDetalle.Read())
                                            {
                                                historial.Add($"   Producto: {readerDetalle["Nombre"]} | Cant: {readerDetalle["Cantidad"]} | Precio Unitario: {readerDetalle["PrecioUnitario"]} | Subtotal: {readerDetalle["Subtotal"]}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                historial.Add("Error: " + ex.Message);
            }
            return historial;
        }
    }
}
