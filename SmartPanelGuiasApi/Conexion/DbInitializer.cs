using System;
using System.Data;
using Npgsql;
using SmartPanelGuiasApi.Conexion;

namespace SmartPanelGuiasApi.Conexion
{
    public static class DbInitializer
    {
        public static void Initialize(DbConexion db)
        {
            using var conn = db.GetSmartPanelConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();

            // Crear tabla Privilegios si no existe
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS privilegio (
    id_privilegio SERIAL PRIMARY KEY,
    nombre_pri VARCHAR(25) NOT NULL,
    descripcion VARCHAR(50)
);";
            cmd.ExecuteNonQuery();

            // Crear tabla Usuarios si no existe
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS usuario (
    id_usuario SERIAL PRIMARY KEY,
    id_privilegio INT REFERENCES privilegio(id_privilegio),
    rut VARCHAR(12) NOT NULL,
    password VARCHAR(100) NOT NULL,
    nombre VARCHAR(50) NOT NULL,
    correo VARCHAR(50) NOT NULL,
    estado VARCHAR(1) NOT NULL
);";
            cmd.ExecuteNonQuery();

            // Poblar Privilegios si está vacío
            cmd.CommandText = "SELECT COUNT(*) FROM privilegio;";
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count == 0)
            {
                cmd.CommandText = @"
INSERT INTO privilegio (nombre_pri, descripcion)
VALUES ('Admin', 'Administrador del sistema'),
       ('User', 'Usuario normal');";
                cmd.ExecuteNonQuery();
            }

            // Poblar Usuario de prueba si está vacío
            cmd.CommandText = "SELECT COUNT(*) FROM usuario;";
            count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count == 0)
            {
                string passwordHash = CryptSharp.Crypter.Blowfish.Crypt("admin");

                cmd.CommandText = $@"
INSERT INTO usuario (id_privilegio, rut, password, nombre, correo, estado)
VALUES (1, '00000000-0', '{passwordHash}', 'Test User', 'test@test.cl', '0');";
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("✅ Database initialized successfully.");
        }
    }
}