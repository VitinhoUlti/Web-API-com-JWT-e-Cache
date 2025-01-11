using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        private readonly Hash hash = new Hash(SHA512.Create());

        public UsuariosController(UsuariosContext usuariosContext, IMemoryCache memorycache, IConfiguration configuration){
            contexto = usuariosContext;
            _memoryCache = memorycache;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Cadastrar(Usuarios usuario){
            usuario.Senha = hash.CriptografarSenha(usuario.Senha);
            contexto.Add(usuario);
            contexto.SaveChanges();

            var tokenService = new TokenService(_configuration);
            var token = tokenService.GerarToken(usuario);
            return CreatedAtAction(nameof(ObterId), new {id = usuario.Id, token = token}, usuario);
        }

        [HttpGet("login/{nome}/{senha}")]
        [AllowAnonymous]
        public IActionResult Login(string nome, string senha){
            var usuarioCache = _memoryCache.Get(new {nome, senha});
            if(_memoryCache.TryGetValue(new {nome, senha}, out usuarioCache)) {return Ok(usuarioCache);}

            var login = from pessoa in contexto.Usuarios where pessoa.Nome.ToLower().Contains(nome.ToLower()) select pessoa;

            foreach(var usuario in login) {
                if(hash.VerificarSenha(senha, usuario.Senha)){
                    var memorycacheoptions = new MemoryCacheEntryOptions {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
                        SlidingExpiration = TimeSpan.FromSeconds(1200)
                    };
                    _memoryCache.Set(new {nome, senha}, usuario, memorycacheoptions);

                    return Ok(new {usuario});
                }
            }

            return NotFound();
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