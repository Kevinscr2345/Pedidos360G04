using Microsoft.AspNetCore.Identity;

namespace Pedidos360Grupo4.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Vendedor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string correoAdmin = "admin@pedidos360.com";
            string claveAdmin = "Admin123";

            var usuarioAdmin = await userManager.FindByEmailAsync(correoAdmin);
            if (usuarioAdmin == null)
            {
                usuarioAdmin = new IdentityUser
                {
                    UserName = correoAdmin,
                    Email = correoAdmin,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuarioAdmin, claveAdmin);
                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuarioAdmin, "Admin");
                }
            }

            string correoVendedor = "vendedor@pedidos360.com";
            string claveVendedor = "Vendedor123";

            var usuarioVendedor = await userManager.FindByEmailAsync(correoVendedor);
            if (usuarioVendedor == null)
            {
                usuarioVendedor = new IdentityUser
                {
                    UserName = correoVendedor,
                    Email = correoVendedor,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuarioVendedor, claveVendedor);
                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuarioVendedor, "Vendedor");
                }
            }
        }
    }
}