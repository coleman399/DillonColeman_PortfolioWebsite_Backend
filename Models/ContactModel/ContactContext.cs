namespace DillonColeman_PortfolioWebsite.Models.ContactModel
{
    public class ContactContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        protected readonly IConfiguration Configuration;

        public ContactContext(IConfiguration configuration)
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
                entity.Property(e => e.Message)
                    .HasMaxLength(250);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt);
            });
        }
    }
}
