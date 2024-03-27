using Microsoft.EntityFrameworkCore;

namespace ApiMailServer.Db
{
    public partial class MessageContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public MessageContext(DbContextOptions<MessageContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("message_pkey");

                entity.Property(e => e.ProducerId).HasColumnName("producer_id");
                entity.Property(e => e.ConsumerId).HasColumnName("consumer_id");
                entity.Property(e => e.ProducerEmail).HasColumnName("producer_email");
                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
            });


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
