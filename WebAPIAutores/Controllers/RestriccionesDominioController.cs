using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/restriccionesdominio")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesDominioController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public RestriccionesDominioController(ApplicationDbContext context)
        {
            this.context = context;
        }


    }
}
