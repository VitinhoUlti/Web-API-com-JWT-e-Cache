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
        private readonly IConfiguration _configuration;

        public UsuariosController(UsuariosContext usuariosContext, IMemoryCache memorycache, IConfiguration configuration){
            contexto = usuariosContext;
            _memoryCache = memorycache;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Cadastrar(Usuarios usuario){
            contexto.Add(usuario);
            contexto.SaveChanges();

            var tokenService = new TokenService(_configuration);
            var token = tokenService.GerarToken(usuario);
            return CreatedAtAction(nameof(ObterId), new {id = usuario.Id, token = token}, usuario);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(Usuarios usuario){
            var usuarioCache = _memoryCache.Get(usuario.ToString());
            if(_memoryCache.TryGetValue(usuario.ToString(), out usuarioCache)) {return Ok(usuarioCache);}

            var login = from pessoa in contexto.Usuarios where pessoa.Nome.ToLower().Contains(usuario.Nome.ToLower()) && pessoa.Senha.Contains(usuario.Senha) select pessoa;
            if(login == null) return NotFound();

            var memorycacheoptions = new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
                SlidingExpiration = TimeSpan.FromSeconds(1200)
            };
            _memoryCache.Set(usuario.ToString(), login, memorycacheoptions);
            return Ok(usuario);
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

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult AtualizarUsuarioporId(int id, Usuarios novoUsuario){
            var usuario = contexto.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            usuario.Nome = novoUsuario.Nome;
            usuario.Senha = novoUsuario.Senha;

            contexto.Usuarios.Update(usuario);
            contexto.SaveChanges();
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeletarUsuario(int id){
            var hobbie = contexto.Usuarios.Find(id);
            if (hobbie == null) return NotFound();

            contexto.Usuarios.Remove(hobbie);
            contexto.SaveChanges();
            return NoContent();
        }
    }
}