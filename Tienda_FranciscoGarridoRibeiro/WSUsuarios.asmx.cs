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
    public class WSUsuarios : System.Web.Services.WebService
    {
        // 1. ValidarUsuario (Punto 1): Consulta credenciales y retorna el nombre si es correcto
        [WebMethod]
        public string ValidarUsuario(string nombreUsuario, string contrasena)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "SELECT Nombre FROM usuarios WHERE NombreUsuario = @user AND Contrasena = @pass";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@user", nombreUsuario);
                    cmd.Parameters.AddWithValue("@pass", contrasena);
                    object resultado = cmd.ExecuteScalar();

                    return (resultado != null) ? "Bienvenido, " + resultado.ToString() : "Error: Usuario o contraseña incorrectos";
                }
            }
            catch (Exception ex) { return "Error de conexión: " + ex.Message; }
        }

        // 2. RegistrarUsuario (Punto 2): Inserta un nuevo usuario en la tabla
        [WebMethod]
        public string RegistrarUsuario(string user, string pass, string nombre, string apellido, string email)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "INSERT INTO usuarios (NombreUsuario, Contrasena, Nombre, Apellido, Email, FechaRegistro) " +
                               "VALUES (@u, @p, @n, @a, @e, @f)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", pass);
                    cmd.Parameters.AddWithValue("@n", nombre);
                    cmd.Parameters.AddWithValue("@a", apellido);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@f", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
                return "Usuario registrado con éxito.";
            }
            catch (Exception ex) { return "Error al registrar: " + ex.Message; }
        }

        // 3. ActualizarUsuario (Punto 3): Modifica la información de un usuario existente
        [WebMethod]
        public string ActualizarUsuario(int id, string nombre, string apellido, string email)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "UPDATE usuarios SET Nombre=@n, Apellido=@a, Email=@e WHERE UsuarioID=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@n", nombre);
                    cmd.Parameters.AddWithValue("@a", apellido);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@id", id);
                    int filas = cmd.ExecuteNonQuery();
                    return (filas > 0) ? "Datos actualizados." : "No se encontró el usuario.";
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        // 4. EliminarUsuario (Punto 4): Elimina por identificador
        [WebMethod]
        public string EliminarUsuario(int id)
        {
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "DELETE FROM usuarios WHERE UsuarioID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int filas = cmd.ExecuteNonQuery();
                    return (filas > 0) ? "Usuario eliminado." : "El ID no existe.";
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        // 5. ObtenerUsuarios (Punto 5): Devuelve lista de usuarios registrados
        [WebMethod]
        public List<string> ObtenerUsuarios()
        {
            List<string> usuarios = new List<string>();
            try
            {
                Conexion oConexion = new Conexion();
                MySqlConnection conexion = oConexion.Conector();
                string query = "SELECT NombreUsuario, Email FROM usuarios";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add($"{reader["NombreUsuario"]} ({reader["Email"]})");
                    }
                }
            }
            catch (Exception ex) { usuarios.Add("Error: " + ex.Message); }
            return usuarios;
        }
    }
}