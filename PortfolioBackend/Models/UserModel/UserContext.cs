namespace PortfolioBackend.Models.UserModel
{
    public class UserContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<ForgotPasswordToken> ForgotPasswordTokens { get; set; }
        private readonly IConfiguration? _configuration;

        public UserContext() { }

        public UserContext(DbContextOptions<UserContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
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
                entity.OwnsOne(e => e.ForgotPasswordToken);
                entity.OwnsOne(e => e.RefreshToken);
                entity.Property(e => e.AccessToken);
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.UpdatedAt);
            });

            //OwnedNavigationBuilder<User, ForgotPasswordToken> forgotPasswordTokenNavigationBuilder = modelBuilder.Entity<User>().OwnsOne(e => e.ForgotPasswordToken);
            //OwnedNavigationBuilder<User, RefreshToken> refreshTokenNavigationBuilder = modelBuilder.Entity<User>().OwnsOne(e => e.RefreshToken);

            //forgotPasswordTokenNavigationBuilder.WithOwner().HasForeignKey("UserId");
            //refreshTokenNavigationBuilder.WithOwner().HasForeignKey("UserId");

            var test = _configuration!["SuperUserUserName"]!;

            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = 1,
                UserName = _configuration!["SuperUserUserName"]!,
                Email = _configuration["SuperUserEmail"]!,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(_configuration["SuperUserPassword"]!),
                Role = Roles.SuperUser.ToString(),
                AccessToken = string.Empty,
            });
        }
    }
}


