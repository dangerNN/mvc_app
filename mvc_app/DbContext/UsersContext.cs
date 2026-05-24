
using Microsoft.EntityFrameworkCore;

public class UsersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=WIN-8G40ARFN4JP\DANGERNN;Database=UsersDB;Integrated Security=True; TrustServerCertificate=True;");
    }
}
