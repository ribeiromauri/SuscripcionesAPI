﻿using Microsoft.AspNetCore.Mvc;

namespace WebAPIAutores.Controllers
{
    
    public class CustomBaseController : ControllerBase
    {
        protected string ObtenerUsuarioId()
        {
            var usuarioClaims = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            var usuarioId = usuarioClaims.Value;
            return usuarioId;
        }
    }
}
