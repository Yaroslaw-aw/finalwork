using Microsoft.EntityFrameworkCore;

namespace ApiRegistration.Db
{
    public partial class UserContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(user => user.userId).HasName("user_pkey");
                entity.HasIndex(user => user.Email).IsUnique();

                entity.ToTable("users");

                entity.Property(user => user.userId).HasColumnName("user_id");
                entity.Property(user => user.Email)
                      .HasMaxLength(255)
                      .HasColumnName("email");

                entity.Property(user => user.Password).HasColumnName("password");
                entity.Property(user => user.Salt).HasColumnName("salt");

                entity.Property(user => user.RoleId).HasConversion<int>();
            });

            modelBuilder
                .Entity<Role>()
                .Property(role => role.RoleId)
                .HasConversion<int>();

            modelBuilder
                .Entity<Role>().HasData(
                 Enum.GetValues(typeof(RoleId))
                 .Cast<RoleId>()
                 .Select(roleId => new Role()
                 {
                     RoleId = roleId,
                     RoleName = roleId.ToString()
                 }));

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
