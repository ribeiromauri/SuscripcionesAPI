using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Middlewares
{
    public static class LimitarPeticionesMiddlewareExtensions
    {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionesMiddleware>();
        }
    }

    public class LimitarPeticionesMiddleware
    {
        private readonly IConfiguration configuration;
        private readonly RequestDelegate siguiente;

        public LimitarPeticionesMiddleware(IConfiguration configuration, RequestDelegate siguiente)
        {
            this.configuration = configuration;
            this.siguiente = siguiente;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            var limitarPeticionesConfiguracion = new LimitarPeticionesConfiguracion();
            configuration.GetRequiredSection("limitarPeticiones").Bind(limitarPeticionesConfiguracion);            

            var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (llaveStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");
                return;
            }

            var llave = llaveStringValues[0];

            var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Llave == llave);

            if (llaveDB == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("No existe la llave");
                return;
            }

            if (!llaveDB.Activa)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La llave se encuentra inactiva");
                return;
            }

            //if (llaveDB.TipoLlave == TipoLlave.Gratis)
            //{
            //    var hoy = DateTime.Today;
            //    var mañana = hoy.AddDays(1);
            //    var cantidadPeticionesRealizadasHoy = await context.Peticiones.CountAsync(x =>
            //    x.LlaveId == llaveDB.Id && x.FechaPeticion >= hoy && x.FechaPeticion < mañana);

            //    if (cantidadPeticionesRealizadasHoy >= limitarPeticionesConfiguracion.PeticionesPorDiaGratuito)
            //    {
            //        httpContext.Response.StatusCode = 429; // Too many requests
            //        await httpContext.Response.WriteAsync("Ha excedido el límite de peticiones por día. Si desea " +
            //            "realizar más peticiones, " +
            //            "actualice su suscripción a una cuenta profesional");
            //        return;
            //    }
            //}

            await siguiente(httpContext);
        }
    }
}
