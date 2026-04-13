using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pedidos360Grupo4.Models;

namespace Pedidos360Grupo4.Data
{
    public class DatabaseLogic : IdentityDbContext

    {
        public DatabaseLogic(DbContextOptions<DatabaseLogic> options)
            : base(options)

        {

        }

        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Índices únicos recomendados
            builder.Entity<Cliente>()
                .HasIndex(x => x.Cedula)
                .IsUnique();

            builder.Entity<Cliente>()
                .HasIndex(x => x.Correo)
                .IsUnique();
        }


    }
}
