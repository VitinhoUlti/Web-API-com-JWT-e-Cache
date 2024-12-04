using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testezin.Entidades;

namespace Testezin.Contexto
{
    public class HobbiesContext : DbContext
    {
        public HobbiesContext(DbContextOptions<HobbiesContext> options) : base(options){}
        public DbSet<Hobbies> Hobbies { get; set;}
    }
}