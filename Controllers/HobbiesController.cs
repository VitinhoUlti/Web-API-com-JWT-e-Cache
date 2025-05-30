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
    [Route("[controller]")]
    public class HobbiesController : ControllerBase
    {
        private readonly HobbiesContext contexto;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions memorycacheoptions = new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
            SlidingExpiration = TimeSpan.FromSeconds(1200)
        };

        public HobbiesController(HobbiesContext hobbiesContext, IMemoryCache memoryCache){
            contexto = hobbiesContext;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [Authorize]
        public IActionResult CriarHobbie(Hobbies hobbie, int idDoUsuario){
            hobbie.IdDoUsuario = idDoUsuario;

            contexto.Add(hobbie);
            contexto.SaveChanges();
            return CreatedAtAction(nameof(ObterId), new {id = hobbie.Id}, hobbie);
        }

        [HttpGet("id/{id}")]
        [Authorize]
        public IActionResult ObterId(int id){
            var hobbiesCache = _memoryCache.Get(id.ToString() + "HobbiesId");
            if(_memoryCache.TryGetValue(id.ToString() + "HobbiesId", out hobbiesCache)) {return Ok(hobbiesCache);}

            var hobbie = contexto.Hobbies.Find(id);
            if (hobbie == null) return NotFound();

            _memoryCache.Set(id.ToString() + "HobbiesId", hobbie, memorycacheoptions);
            return Ok(hobbie);
        }

        [HttpGet("nome/{nome}")]
        [Authorize]
        public IActionResult ObterNome(string nome){
            var hobbiesCache = _memoryCache.Get(nome.ToString() + "HobbiesNome");
            if(_memoryCache.TryGetValue(nome.ToString() + "HobbiesNome", out hobbiesCache)) {return Ok(hobbiesCache);}

            var hobbie = contexto.Hobbies.Where(pessoa => pessoa.Nome.ToLower() == nome.ToLower()).ToList();
            if (hobbie.Count == 0) return NotFound();

            _memoryCache.Set(nome.ToString() + "HobbiesNome", hobbie, memorycacheoptions);
            return Ok(hobbie);
        }

        [HttpGet("idusuario/{id}")]
        [Authorize]
        public IActionResult ObterIdUsuario(int id){
            var hobbie = contexto.Hobbies.Where(hobbie => hobbie.IdDoUsuario == id).ToList();
            if (hobbie == null) return NotFound();
            return Ok(hobbie);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult AtualizarHobbieporId(int id, Hobbies novoHobbie){
            var hobbie = contexto.Hobbies.Find(id);
            if (hobbie == null) return NotFound();

            hobbie.Nome = novoHobbie.Nome;
            hobbie.Aniversario = novoHobbie.Aniversario;
            hobbie.Hobbie = novoHobbie.Hobbie;
            hobbie.Gostos = novoHobbie.Gostos;
            hobbie.PossiveisPresentes = novoHobbie.PossiveisPresentes;

            contexto.Hobbies.Update(hobbie);
            contexto.SaveChanges();
            return Ok(hobbie);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeletarHobbie(int id){
            var hobbie = contexto.Hobbies.Find(id);
            if (hobbie == null) return NotFound();

            contexto.Hobbies.Remove(hobbie);
            contexto.SaveChanges();
            return NoContent();
        }
    }
}