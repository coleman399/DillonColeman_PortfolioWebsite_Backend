namespace PortfolioWebsite_Backend.Models.UserModel
{
    public class UserContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected readonly IConfiguration Configuration;

        public UserContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            // connect to mysql with connection string from app settings
            var connectionString = Configuration["ConnectionStrings:LocalMySqlDb"];

            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // write fluent API configurations here
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.PasswordHash)
                    .IsRequired();
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Role)
                    .IsRequired();
                entity.Property(e => e.AccessToken)
                    .IsRequired();
                entity.OwnsOne(e => e.RefreshToken);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt);
            });
        }
    }
}
