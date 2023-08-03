using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/llaves")]
    public class LlavesAPIController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ServicioLlaves servicioLlaves;

        public LlavesAPIController(ApplicationDbContext context, IMapper mapper, ServicioLlaves servicioLlaves)
        {
            this.context = context;
            this.mapper = mapper;
            this.servicioLlaves = servicioLlaves;
        }

        [HttpGet]
        public async Task<ActionResult<List<LlaveDTO>>> MisLLaves()
        {
            var usuarioId = ObtenerUsuarioId();
            var llavesUsuario = await context.LlavesAPI.Where(x => x.UsuarioId== usuarioId).ToListAsync();
            return mapper.Map<List<LlaveDTO>>(llavesUsuario);
        }

        [HttpPost]
        public async Task<ActionResult> CrearLlave(CrearLlaveDTO crearLlaveDTO)
        {
            var usuarioId = ObtenerUsuarioId();

            if(crearLlaveDTO.TipoLlave == Entidades.TipoLlave.Gratis)
            {
                var existeLlaveGratuita = await context.LlavesAPI.AnyAsync(x => x.UsuarioId == usuarioId && x.TipoLlave == Entidades.TipoLlave.Gratis);
                if (existeLlaveGratuita) return BadRequest("El usuario ya tiene una llave gratuita");
            }

            await servicioLlaves.CrearLlave(usuarioId, crearLlaveDTO.TipoLlave);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> ActualizarLlave(ActualizarLlaveDTO actualizarLlaveDTO)
        {
            var usuarioId = ObtenerUsuarioId();

            var llaveDb = await context.LlavesAPI.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

            if (llaveDb == null) return NotFound();

            if (usuarioId != llaveDb.UsuarioId) return Forbid();

            if (actualizarLlaveDTO.ActualizarLlave) llaveDb.Llave = servicioLlaves.GenerarLlave();

            llaveDb.Activa = actualizarLlaveDTO.Activa;

            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
