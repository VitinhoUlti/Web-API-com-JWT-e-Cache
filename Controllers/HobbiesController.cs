using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UsuariosContext contextoUsuario;

        public HobbiesController(HobbiesContext hobbiesContext, UsuariosContext usuariosContext){
            contexto = hobbiesContext;
            contextoUsuario = usuariosContext;
        }

        [HttpPost]
        public IActionResult CriarHobbie(Hobbies hobbie, int idDoUsuario){
            hobbie.Usuario = contextoUsuario.Usuario.Find(idDoUsuario);
            if (hobbie.Usuario == null) return NotFound();
            
            contexto.Add(hobbie);
            contexto.SaveChanges();
            return CreatedAtAction(nameof(ObterId), hobbie);
        }

        [HttpGet("id/{id}")]
        public IActionResult ObterId(int id){
            var hobbie = contexto.Hobbies.Find(id);
            if (hobbie == null) return NotFound();
            return Ok(hobbie);
        }

        [HttpGet("nome/{nome}")]
        public IActionResult ObterNome(string nome){
            var hobbie = from pessoa in contexto.Hobbies where pessoa.Nome.ToLower().Contains(nome.ToLower()) select pessoa;
            if (hobbie == null) return NotFound();
            return Ok(hobbie);
        }

        [HttpPut("{id}")]
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
        public IActionResult DeletarHobbie(int id){
            var hobbie = contexto.Hobbies.Find(id);
            if (hobbie == null) return NotFound();

            contexto.Hobbies.Remove(hobbie);
            contexto.SaveChanges();
            return NoContent();
        }
    }
}