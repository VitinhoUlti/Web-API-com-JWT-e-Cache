using Testezin.Entidades;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testezin.Contexto;

namespace Testezin.Services
{
    public static class DatabaseManagementService
    {
        public static void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<HobbiesContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<UsuariosContext>().Database.Migrate();
            }
        }
    }
}

