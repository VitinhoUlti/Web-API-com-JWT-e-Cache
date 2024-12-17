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
    [Route("Usuario/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosContext contexto;

        public UsuariosController(UsuariosContext usuariosContext){
            contexto = usuariosContext;
        }

        [HttpPost]
        public IActionResult Cadastrar(Usuarios usuario){
            contexto.Add(usuario);
            contexto.SaveChanges();
            var token = TokenService.GerarToken(usuario);
            return CreatedAtAction(nameof(ObterId), new {id = usuario.Id, token}, usuario);
        }

        [HttpGet("id/{id}")]
        public IActionResult ObterId(int id){
            var usuario = contexto.Usuarios.Find(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }
    }
}