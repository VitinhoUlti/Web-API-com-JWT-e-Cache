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
        private readonly MemoryCacheEntryOptions memorycacheoptions = new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
            SlidingExpiration = TimeSpan.FromSeconds(1200)
        };

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
            return Ok(new {usuario = usuario, token = token});
        }

        [HttpGet("login/{nome}/{senha}")]
        [AllowAnonymous]
        public IActionResult Login(string nome, string senha){
            var usuarioCache = _memoryCache.Get(new {nome, senha});
            
            if(_memoryCache.TryGetValue(new {nome, senha}, out usuarioCache)) {
                var tokenService = new TokenService(_configuration);
                var token = tokenService.GerarToken((Usuarios)usuarioCache);

                return Ok(new {usuario = usuarioCache, token = token});
            }

            var login = from pessoa in contexto.Usuarios where pessoa.Nome.ToLower() == nome.ToLower() select pessoa;

            foreach(var usuario in login) {
                if(hash.VerificarSenha(senha, usuario.Senha)){
                    _memoryCache.Set(new {nome, senha}, usuario, memorycacheoptions);

                    var tokenService = new TokenService(_configuration);
                    var token = tokenService.GerarToken(usuario);
                    return Ok(new {usuario = usuario, token = token});
                }
            }

            return NotFound();
        }

        [HttpGet("nome/{nome}")]
        [AllowAnonymous]
        public IActionResult ObterPorNome(string nome){
            var usuarioCache = _memoryCache.Get(nome.ToString() + "UsuarioNome");
            if(_memoryCache.TryGetValue(nome.ToString() + "UsuarioNome", out usuarioCache)) {return Ok(usuarioCache);}

            var usuario = contexto.Usuarios.Where(pessoa => pessoa.Nome.ToLower().Equals(nome.ToLower())).ToList(); //deixei diferente do obter nome do hobbies para testar performace, porém os dois deram resultados iguais
            if(usuario.Count == 0) return NotFound();

            _memoryCache.Set(nome.ToString() + "UsuarioNome", usuario, memorycacheoptions);

            return Ok(usuario);
        }

        [HttpGet("id/{id}")]
        [Authorize]
        public IActionResult ObterPorId(int id){
            var usuarioCache = _memoryCache.Get(id.ToString() + "UsuarioId");
            if(_memoryCache.TryGetValue(id.ToString() + "UsuarioId", out usuarioCache)) {return Ok(usuarioCache);}

            var usuario = contexto.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            _memoryCache.Set(id.ToString() + "UsuarioId", usuario, memorycacheoptions);
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