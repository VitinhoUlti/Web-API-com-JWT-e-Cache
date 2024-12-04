using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hobbies-Web-API-EntityFramework.Entidades
{
    public class Hobbies
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Date Aniversario { get; set; }
        public string Hobbies { get; set; }
        public string Gostos { get; set; }
        public string PossiveisPresentes { get; set; }
    }
}