using CurConvApp.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CurrencyRateRecord> CurrencyRateRecords { get; set; }

    public DbSet<DbUser> Users { get; set; }

}
