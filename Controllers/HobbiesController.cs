using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Testezin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HobbiesController : ControllerBase
    {
        [HttpGet("ObterHobbie/{nome}/{hobbies}")]
        public IActionResult ObterHobbie(string nome, string hobbies){
            var obj = new {
                NomeDaPessoa = nome,
                Hobbies = hobbies
            };

            return Ok(obj);
        }

        [HttpGet("HobbieDe/{nome}")]
        public IActionResult HobbieDe(string nome){
            var pessoas = nome.Split(',');
            List<string> mensagem = new List<string>();

            foreach (var pessoa in pessoas)
            {
                mensagem.Add("O hobbie de " + pessoa + " é amar;");
            }

            return Ok(new {mensagem});
        }
    }
}