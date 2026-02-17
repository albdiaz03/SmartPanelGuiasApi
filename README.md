# SmartPanelGuiasApi

SmartPanelGuiasApi es una API RESTful desarrollada en C# con ASP.NET Core para gestionar guías internas y registrar auditoría de cambios. Permite crear, leer, actualizar y eliminar guías, registrando automáticamente los cambios en un sistema de logs con información de usuario, fecha, IP y navegador.

## Tecnologías
- Lenguaje: C#
- Framework: ASP.NET Core Web API
- Mapeo de objetos: AutoMapper
- Base de datos: SQL Server
- Acceso a datos: ADO.NET
- Seguridad: JWT (JSON Web Tokens)
- Auditoría: Registro de cambios en las guías, incluyendo propiedades editadas y texto largo de descripción resumido.

## Funcionalidades
- CRUD completo de guías
- Registro de auditoría de cambios detectando automáticamente modificaciones
- Endpoints de prueba para manejar errores 400, 401, 404 y 500
- Generación automática de próximos folios por tipo de guía

## Instalación
1. Clona el repositorio:
   git clone https://github.com/tu-usuario/SmartPanelGuiasApi.git
2. Configura la cadena de conexión a SQL Server en LogService y GuiaService.
3. Restaura paquetes NuGet: dotnet restore
4. Ejecuta la API: dotnet run

## Uso
- Consumir la API usando cualquier cliente HTTP (Postman, Blazor frontend, etc.)
- Autenticarse mediante JWT en los endpoints protegidos
- Crear, editar o eliminar guías y revisar los logs generados automáticamente

## Licencia
MIT
