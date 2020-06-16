using MessengerAPI.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerAPI.Data
{
    public class Model : IdentityDbContext
    {
        public Model(DbContextOptions<Model> options)
          : base(options)
        { }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ApplicationLog> ApplicationLogs { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ApplicationUserChat> UserChats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Identity table names changing methods

            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable(name: "Users");
                entity.Property(p => p.Id).HasColumnName("UserId");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
                entity.Property(p => p.Id).HasColumnName("RoleId");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
                entity.Property(p => p.Id).HasColumnName("UserClaimId");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
                entity.Property(p => p.Id).HasColumnName("RoleClaimId");

            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            #endregion

            #region Relationships

            //Defining primary keys for M-M table user and chat
            modelBuilder.Entity<ApplicationUserChat>()
                .HasKey(uc => new { uc.UserId, uc.ChatId });

            // Relationship M-M between user and chat
            modelBuilder.Entity<ApplicationUserChat>()
                .HasOne(bc => bc.ApplicationUser)
                .WithMany(b => b.ApplicationUserChats)
                .HasForeignKey(bc => bc.UserId);

            // Relationship M-M between chat and user
            modelBuilder.Entity<ApplicationUserChat>()
                .HasOne(bc => bc.Chat)
                .WithMany(c => c.ApplicationUserChats)
                .HasForeignKey(bc => bc.ChatId);

            //Relationship 1-M between user and message
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(c => c.Messages)
                .WithOne(e => e.ApplicationUser);

            //Relationship 1-M between chat and messages
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(e => e.Chat);

            #endregion

        }
    }
}
