using Microsoft.AspNetCore.Identity;

namespace Pedidos360Grupo4.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Agregamos "Operaciones" al arreglo de roles
            string[] roles = { "Admin", "Vendedor", "Operaciones" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Creamos los usuarios usando el método auxiliar para mantener el código limpio
            await CrearUsuarioSiNoExiste(userManager, "admin@pedidos360.com", "Admin123", "Admin");
            await CrearUsuarioSiNoExiste(userManager, "vendedor@pedidos360.com", "Vendedor123", "Vendedor");
            await CrearUsuarioSiNoExiste(userManager, "operaciones@pedidos360.com", "Operaciones123", "Operaciones");
        }

        // Método auxiliar que encapsula la lógica repetitiva de crear un usuario y asignarle un rol
        private static async Task CrearUsuarioSiNoExiste(UserManager<IdentityUser> userManager, string correo, string clave, string rol)
        {
            var usuario = await userManager.FindByEmailAsync(correo);

            if (usuario == null)
            {
                usuario = new IdentityUser
                {
                    UserName = correo,
                    Email = correo,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuario, clave);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuario, rol);
                }
            }
        }
    }
}