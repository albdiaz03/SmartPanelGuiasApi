using System;
using System.Data;

namespace SmartPanelGuiasApi.Conexion
{
    public static class DbInitializer
    {
        public static void Initialize(DbConexion db)
        {
            using var conn = db.GetSmartPanelConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();

            // 1️⃣ Privilegios
            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'privilegio')
BEGIN
    CREATE TABLE privilegio(
        id_privilegio INT IDENTITY(1,1) PRIMARY KEY,
        nombre_pri NVARCHAR(25) NOT NULL,
        descripcion NVARCHAR(50) NULL
    )
END";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM privilegio WHERE nombre_pri = 'Admin')
    INSERT INTO privilegio(nombre_pri, descripcion) VALUES ('Admin', 'Administrador del sistema');
IF NOT EXISTS (SELECT 1 FROM privilegio WHERE nombre_pri = 'User')
    INSERT INTO privilegio(nombre_pri, descripcion) VALUES ('User', 'Usuario normal');";
            cmd.ExecuteNonQuery();

            // 2️⃣ Usuario
            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'usuario')
BEGIN
    CREATE TABLE usuario(
        id_usuario INT IDENTITY(1,1) PRIMARY KEY,
        id_privilegio INT NULL,
        rut NVARCHAR(12) NOT NULL,
        password NVARCHAR(100) NOT NULL,
        nombre NVARCHAR(50) NOT NULL,
        correo NVARCHAR(50) NOT NULL,
        estado NVARCHAR(1) NOT NULL,
        FOREIGN KEY(id_privilegio) REFERENCES privilegio(id_privilegio)
    )
END";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM usuario WHERE correo = 'test@test.cl')
BEGIN
    INSERT INTO usuario(id_privilegio, rut, password, nombre, correo, estado)
    VALUES (
        (SELECT TOP 1 id_privilegio FROM privilegio ORDER BY id_privilegio),
        '11111111-1',
        'admin', -- 🔹 opcional: cambiar por hash
        'Test User',
        'test@test.cl',
        '0'
    )
END";
            cmd.ExecuteNonQuery();

            // 3️⃣ ApiKeys
            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'api_keys')
BEGIN
    CREATE TABLE api_keys(
        id INT IDENTITY(1,1) PRIMARY KEY,
        api_key_hash VARCHAR(128) NOT NULL,
        name VARCHAR(100),
        active BIT DEFAULT 1,
        scopes VARCHAR(255),
        created_by VARCHAR(50),
        created_at DATETIME DEFAULT GETDATE()
    )
END";

            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM api_keys WHERE name='DefaultKey')
BEGIN
    INSERT INTO api_keys(api_key_hash, name, active, created_by)
    VALUES ('123456', 'DefaultKey', 1, 'Seeder')
END";
            cmd.ExecuteNonQuery();

            // 4️⃣ Logs
            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Logs')
BEGIN
    CREATE TABLE Logs(
        IdLog INT IDENTITY(1,1) PRIMARY KEY,
        IdUsuario INT NOT NULL,
        Accion NVARCHAR(200) NOT NULL,
        Descripcion NVARCHAR(MAX) NULL,
        IP NVARCHAR(50) NULL,
        Navegador NVARCHAR(500) NULL,
        Fecha DATETIME DEFAULT GETDATE()
    )
END";
            cmd.ExecuteNonQuery();

            // 5️⃣ Sucursales
            cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'sucursales')
BEGIN
    CREATE TABLE sucursales(
        cod_comercio VARCHAR(20) PRIMARY KEY,
        cod_softland VARCHAR(20) NOT NULL
    )
END";
            cmd.ExecuteNonQuery();

            Console.WriteLine("✅ Database initialized successfully (tables + seed data).");
        }
    }
}