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
