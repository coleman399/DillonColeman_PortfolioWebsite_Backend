namespace PortfolioWebsite_Backend.Models.UserModel
{
    public class UserContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ForgotPasswordToken> ForgotPasswordTokens { get; set; }

        protected readonly IConfiguration _configuration;

        public UserContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            // Connection string to MySql using user secrets 
            var connectionString = _configuration["ConnectionStrings:LocalMySqlDb"];

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
                entity.Property(e => e.AccessToken);
                entity.OwnsOne(e => e.RefreshToken);
                entity.OwnsOne(e => e.ForgotPasswordToken);
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.UpdatedAt);
            }).Entity<User>().HasData(new User
            {
                Id = 1,
                UserName = _configuration["SuperUser:UserName"]!,
                Email = _configuration["SuperUser:Email"]!,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(_configuration["SuperUser:Password"]!),
                Role = Roles.SuperUser.ToString(),
                AccessToken = string.Empty,
                CreatedAt = DateTime.Now,
            });
        }
    }
}
