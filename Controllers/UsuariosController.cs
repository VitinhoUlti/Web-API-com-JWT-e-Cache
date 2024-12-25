using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Testezin.Contexto;
using Testezin.Entidades;
using Testezin.Servicos;

namespace Testezin.Controllers
{
    [ApiController]
    [Route("Usuario/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosContext contexto;
        private readonly IMemoryCache _memoryCache;

        public UsuariosController(UsuariosContext usuariosContext, IMemoryCache memorycache){
            contexto = usuariosContext;
            _memoryCache = memorycache;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Cadastrar(Usuarios usuario){
            contexto.Add(usuario);
            contexto.SaveChanges();
            var token = TokenService.GerarToken(usuario);
            return CreatedAtAction(nameof(ObterId), new {id = usuario.Id, token = token}, usuario);
        }

        [HttpGet("id/{id}")]
        [Authorize]
        public IActionResult ObterId(int id){
            var usuarioCache = _memoryCache.Get(id.ToString());
            if(_memoryCache.TryGetValue(id.ToString(), out usuarioCache)) {return Ok(usuarioCache);}

            var usuario = contexto.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            var memorycacheoptions = new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
                SlidingExpiration = TimeSpan.FromSeconds(1200)
            };
            _memoryCache.Set(id.ToString(), usuario, memorycacheoptions);
            return Ok(usuario);
        }
    }
}