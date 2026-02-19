using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPanelGuiasApi.Dtos;
using SmartPanelGuiasApi.Models;
using SmartPanelGuiasApi.Services;
using System.Security.Claims;

using SmartPanelGuiasApi.Helpers;


namespace SmartPanelGuiasApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GuiaController : ControllerBase
    {
        private readonly GuiaService _service;
        private readonly IMapper _mapper;
        private readonly LogService _logService;

        public GuiaController(GuiaService service, IMapper mapper, LogService logService)
        {
            _service = service;
            _mapper = mapper;
            _logService = logService;
        }

        // =============================
        // MÉTODO PRIVADO DE AUDITORÍA
        // =============================
        private async Task RegistrarAuditoria(string accion, string descripcion)
        {
            var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idUsuarioClaim))
                return;

            var idUsuario = int.Parse(idUsuarioClaim);
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var navegador = Request.Headers["User-Agent"].ToString();

            await _logService.RegistrarLog(
                idUsuario,
                accion,
                descripcion,
                ip,
                navegador
            );
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var guias = _service.GetAll();
            return Ok(_mapper.Map<List<GuiaDto>>(guias));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var guia = _service.Get(id);
            if (guia == null) return NotFound();

            return Ok(_mapper.Map<GuiaDto>(guia));
        }

        [HttpGet("nextfolio/{tipo}")]
        public IActionResult GetNextFolio(string tipo)
        {
            int maxFolio = _service.GetMaxFolioByTipo(tipo);
            return Ok(new { Folio = maxFolio + 1 }); // ✅ Devuelve un objeto con propiedad "Folio"
        }



        // ===========================
        // PRUEBAS DE ERROR DEL SISTEMA
        // ===========================

        // 🔥 PRUEBA 500 - ERROR INTERNO DEL SERVIDOR
        // Lanza una excepción manual para comprobar que el middleware
        // captura y retorna correctamente un status 500.
        [HttpGet("error-test")]
        public IActionResult ErrorTest()
        {
            throw new Exception("Prueba de excepción manual");
        }



        // 🚫 PRUEBA 404 - RECURSO NO ENCONTRADO
        // Esta ruta NO existe en ningún método del controlador
        // por lo que intentar acceder a:
        //   GET /api/guia/no-existe-404
        // devolverá un 404 automáticamente.
        [HttpGet("no-existe-404")]
        public IActionResult NotFoundTest()
        {
            return NotFound("Ruta utilizada solo para probar el 404");
        }



        // 🔒 PRUEBA 401 - NO AUTORIZADO
        // Simula que el usuario no tiene autorización.
        // Esto devuelve un 401 para comprobar cómo lo maneja el middleware.
        [HttpGet("unauthorized-test")]
        public IActionResult UnauthorizedTest()
        {
            return Unauthorized("No tienes permisos para acceder a esta prueba");
        }



        // ❗ PRUEBA 400 - BAD REQUEST
        // Útil para verificar que el middleware maneja errores de validación.
        // Ejemplo:
        //   GET /api/guia/bad-request?valor=0
        //
        // Si valor == 0 → retorna un 400 BadRequest
        [HttpGet("bad-request")]
        public IActionResult BadRequestTest(int valor)
        {
            if (valor == 0)
            {
                return BadRequest("El valor no puede ser 0 — prueba de BAD REQUEST (400)");
            }

            return Ok($"Valor recibido correctamente: {valor}");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GuiaCreateDto dto)
        {
            var guia = _mapper.Map<Guia>(dto);
            _service.Create(guia);

            await RegistrarAuditoria(
                "Crear guía",
                $"Se creó la guía con folio {guia.Folio}"
            );

            return Ok();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GuiaUpdateDto dto)
        {
            var guia = _service.Get(id);
            if (guia == null) return NotFound();

            Console.WriteLine("========= DEBUG UPDATE =========");
            Console.WriteLine($"ANTES MAP - BD Descripcion: {guia.Descripcion}");
            Console.WriteLine($"DTO Descripcion: {dto.Descripcion}");
            Console.WriteLine($"ANTES MAP - BD Fecha: {guia.Fecha}");
            Console.WriteLine($"DTO Fecha: {dto.Fecha}");
            Console.WriteLine("================================");

            // 🔹 Clonar estado original
            var guiaOriginal = new Guia
            {
                NroInt = guia.NroInt,
                Tipo = guia.Tipo,
                Folio = guia.Folio,
                Fecha = guia.Fecha,
                Descripcion = guia.Descripcion
            };

            // 🔹 Aplicar cambios
            _mapper.Map(dto, guia);

            Console.WriteLine("========= DESPUÉS MAP =========");
            Console.WriteLine($"DESPUÉS MAP - Nueva Descripcion: {guia.Descripcion}");
            Console.WriteLine($"DESPUÉS MAP - Nueva Fecha: {guia.Fecha}");
            Console.WriteLine("================================");

            _service.Update(guia);

            // 🔹 Detectar cambios
            var cambios = AuditoriaHelper.ObtenerCambios(guiaOriginal, guia);

            Console.WriteLine("========= CAMBIOS DETECTADOS =========");
            foreach (var cambio in cambios)
            {
                Console.WriteLine(cambio);
            }
            Console.WriteLine("======================================");

            if (cambios.Any())
            {
                var descripcionLog = $"Se editó la guía ID {id}. {string.Join(" | ", cambios)}";
                await RegistrarAuditoria("Editar guía", descripcionLog);
            }

            return Ok();
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var guia = _service.Get(id);
            if (guia == null) return NotFound();

            _service.Delete(id);

            await RegistrarAuditoria(
                "Eliminar guía",
                $"Se eliminó la guía ID {id}"
            );

            return Ok();
        }
    }
}
