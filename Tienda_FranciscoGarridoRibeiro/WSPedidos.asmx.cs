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
        // 1. CREAR PEDIDO (Punto 11)
        [WebMethod]
        public string CrearPedido(int usuarioID)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();

                // Insertamos el pedido con estado inicial 'Pendiente'
                string query = "INSERT INTO pedidos (UsuarioID, FechaPedido, Estado) VALUES (@user, @fecha, 'Pendiente'); SELECT LAST_INSERT_ID();";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@user", usuarioID);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);

                    // Ejecutamos y obtenemos el ID del pedido recién creado para poder usarlo en DetallePedidos
                    object idRecienCreado = cmd.ExecuteScalar();
                    return "Pedido creado con éxito. ID de Pedido: " + idRecienCreado.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Error al crear pedido: " + ex.Message;
            }
        }

        // 2. ACTUALIZAR ESTADO (Punto 13)
        [WebMethod]
        public string ActualizarEstadoPedido(int pedidoID, string nuevoEstado)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "UPDATE pedidos SET Estado = @estado WHERE PedidoID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@id", pedidoID);
                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0 ? "Estado actualizado a: " + nuevoEstado : "No se encontró el pedido.";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // 3. OBTENER PEDIDOS POR USUARIO (Punto 12)
        [WebMethod]
        public List<string> ObtenerPedidosPorUsuario(int usuarioID)
        {
            List<string> lista = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
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
            catch (Exception ex)
            {
                lista.Add("Error: " + ex.Message);
            }
            return lista;
        }
    }
}