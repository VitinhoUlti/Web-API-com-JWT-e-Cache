using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testezin.Entidades;

namespace Testezin.Contexto
{
    public class UsuariosContext : DbContext
    {
        public UsuariosContext(DbContextOptions<UsuariosContext> options) : base(options){}
        public DbSet<Usuarios> Usuario { get; set;}
    }
}