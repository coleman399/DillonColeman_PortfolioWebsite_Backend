namespace PortfolioWebsite_Backend.Models.ContactModel
{
    public class ContactContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactContext(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = _configuration["ConnectionStrings:LocalMySqlDb"];
            try
            {
                if (_webHostEnvironment.EnvironmentName.Equals("Testing"))
                {
                    options.UseInMemoryDatabase("PerformanceTestingDB");
                }
                else
                {
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }
            }
            catch (Exception exception)
            {
                throw new DatabaseFailedToConnectException($"{exception.Message} {exception}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // write fluent API configurations here

            //Property Configurations
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Phone)
                    .HasMaxLength(50);
                entity.Property(e => e.Message);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt);
            });
        }
    }
}
