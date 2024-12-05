using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Testezin.Contexto;
using Testezin.Entidades;

namespace Testezin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HobbiesController : ControllerBase
    {
        private readonly HobbiesContext contexto;

        public HobbiesController(HobbiesContext hobbiesContext){
            contexto = hobbiesContext;
        }

        [HttpPost]
        public IActionResult CriarHobbie(Hobbies hobbie){
            contexto.Add(hobbie);
            contexto.SaveChanges();
            return Ok(hobbie);
        }

        [HttpGet("AcharHobbies/id/{id}")]
        public IActionResult ObterId(int id){
            var hobbie = contexto.Hobbies.Find(id);
            if (hobbie == null) return NotFound();
            return Ok(hobbie);
        }

        [HttpGet("AcharHobbies/nome/{nome}")]
        public IActionResult ObterNome(string nome){
            var hobbie = from pessoa in contexto.Hobbies where pessoa.Nome.Contains(nome) select pessoa;
            if (hobbie == null) return NotFound();
            return Ok(hobbie);
        }
    }
}