using backendSuprimentosHKM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace backendSuprimentosHKM.Data.Context;

public class AppDbContext : IdentityDbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FornecedorDeMateriais> FornecedoresDeMateriais { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        //modelBuilder.Entity<FornecedorDeMateriais>(entity =>
        //{
        //    entity.ToTable(
        //        "fornecedores_de_materiais",
        //        "suprimentoshkm");
        //});
            

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
