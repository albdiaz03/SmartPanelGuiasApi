using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartPanelGuiasApi.Dtos;
using SmartPanelGuiasApi.Models;
using SmartPanelGuiasApi.Services;

namespace SmartPanelGuiasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuiaController : ControllerBase
    {
        private readonly GuiaService _service;
        private readonly IMapper _mapper;

        public GuiaController(GuiaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
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
        public IActionResult Create([FromBody] GuiaCreateDto dto)
        {
            var guia = _mapper.Map<Guia>(dto);
            _service.Create(guia);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] GuiaUpdateDto dto)
        {
            var guia = _service.Get(id);
            if (guia == null) return NotFound();

            _mapper.Map(dto, guia);
            _service.Update(guia);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var guia = _service.Get(id);
            if (guia == null) return NotFound();

            _service.Delete(id);
            return Ok();
        }
    }
}
