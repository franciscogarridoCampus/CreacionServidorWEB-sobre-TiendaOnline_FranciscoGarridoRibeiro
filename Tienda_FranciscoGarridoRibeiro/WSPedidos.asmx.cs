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
    public class WSPedidos : System.Web.Services.WebService
    {
        // 1. CREAR PEDIDO CON DETALLES (transacción)
        [WebMethod]
        public string CrearPedido(int usuarioID, string idsProductosCSV, string cantidadesProductosCSV)
        {
            try
            {
                // Convertir CSV a listas
                List<int> productos = new List<int>();
                List<int> cantidades = new List<int>();

                string[] prodSplit = idsProductosCSV.Split(',');
                string[] cantSplit = cantidadesProductosCSV.Split(',');

                if (prodSplit.Length != cantSplit.Length)
                    return "Error: La cantidad de productos y cantidades no coincide.";

                for (int i = 0; i < prodSplit.Length; i++)
                {
                    productos.Add(int.Parse(prodSplit[i]));
                    cantidades.Add(int.Parse(cantSplit[i]));
                }

                Conexion oConexion = new Conexion();
                using (MySqlConnection conexion = oConexion.Conector())
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                        conexion.Open();

                    MySqlTransaction transaccion = conexion.BeginTransaction();
                    try
                    {
                        // Insertar pedido
                        string queryPedido = "INSERT INTO pedidos (UsuarioID, FechaPedido, Estado) VALUES (@user, @fecha, 'Pendiente'); SELECT LAST_INSERT_ID();";
                        int pedidoID;
                        using (MySqlCommand cmd = new MySqlCommand(queryPedido, conexion, transaccion))
                        {
                            cmd.Parameters.AddWithValue("@user", usuarioID);
                            cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                            pedidoID = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Insertar detalles usando precios desde productos
                        string queryDetalle = "INSERT INTO detallepedidos (PedidoID, ProductoID, Cantidad, PrecioUnitario) " +
                                              "SELECT @pedido, ProductoID, @cant, Precio FROM productos WHERE ProductoID=@prod";

                        using (MySqlCommand cmdDetalle = new MySqlCommand(queryDetalle, conexion, transaccion))
                        {
                            for (int i = 0; i < productos.Count; i++)
                            {
                                cmdDetalle.Parameters.Clear();
                                cmdDetalle.Parameters.AddWithValue("@pedido", pedidoID);
                                cmdDetalle.Parameters.AddWithValue("@prod", productos[i]);
                                cmdDetalle.Parameters.AddWithValue("@cant", cantidades[i]);
                                cmdDetalle.ExecuteNonQuery();
                            }
                        }

                        transaccion.Commit();
                        return "Pedido creado con éxito. ID de Pedido: " + pedidoID;
                    }
                    catch (Exception exTrans)
                    {
                        transaccion.Rollback();
                        return "Error al crear pedido con detalles: " + exTrans.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // 2. ACTUALIZAR ESTADO DE PEDIDO
        [WebMethod]
        public string ActualizarEstadoPedido(int pedidoID, string nuevoEstado)
        {
            try
            {
                Conexion oConexion = new Conexion();
                using (MySqlConnection conexion = oConexion.Conector())
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                        conexion.Open();

                    string query = "UPDATE pedidos SET Estado = @estado WHERE PedidoID = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                        cmd.Parameters.AddWithValue("@id", pedidoID);
                        int filas = cmd.ExecuteNonQuery();
                        return filas > 0 ? "Estado actualizado a: " + nuevoEstado : "No se encontró el pedido.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // 3. OBTENER PEDIDOS POR USUARIO
        [WebMethod]
        public List<string> ObtenerPedidosPorUsuario(int usuarioID)
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                using (MySqlConnection conexion = oConexion.Conector())
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                        conexion.Open();

                    string query = "SELECT PedidoID, FechaPedido, Estado FROM pedidos WHERE UsuarioID = @user";
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@user", usuarioID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add($"ID: {reader["PedidoID"]} | Fecha: {reader["FechaPedido"]} | Estado: {reader["Estado"]}");
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

        // 4. ELIMINAR PEDIDO
        [WebMethod]
        public string EliminarPedido(int pedidoID)
        {
            try
            {
                Conexion oConexion = new Conexion();
                using (MySqlConnection conexion = oConexion.Conector())
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                        conexion.Open();

                    string query = "DELETE FROM pedidos WHERE PedidoID = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", pedidoID);
                        int filas = cmd.ExecuteNonQuery();
                        return filas > 0 ? "Pedido eliminado." : "No se encontró el pedido.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
