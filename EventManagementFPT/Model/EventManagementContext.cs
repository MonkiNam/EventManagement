﻿#nullable disable

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EventManagementFPT.Model
{
    public partial class EventManagementContext : DbContext
    {
        public EventManagementContext()
        {
        }

        public EventManagementContext(DbContextOptions<EventManagementContext> options) : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventLike> EventLikes { get; set; }
        public virtual DbSet<FollowEvent> FollowEvents { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserEvent> UserEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.ToTable("tblCategory");

                entity.Property(e => e.CategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("CategoryID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.CommentId);

                entity.ToTable("tblComment");

                entity.HasIndex(e => e.EventId, "IX_tblComment_EventID");

                entity.HasIndex(e => e.ParentId, "IX_tblComment_ParentID");

                entity.HasIndex(e => e.UserId, "IX_tblComment_UserID");

                entity.Property(e => e.CommentId)
                    .ValueGeneratedNever()
                    .HasColumnName("CommentID");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.TblComments)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblComment_tblEvent");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_tblComment_tblComment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblComment_tblUser");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.ToTable("tblEvent");

                entity.HasIndex(e => e.Category, "IX_tblEvent_Category");

                entity.Property(e => e.EventId)
                    .ValueGeneratedNever()
                    .HasColumnName("EventID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Venue)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.TblEvents)
                    .HasForeignKey(d => d.Category)
                    .HasConstraintName("FK_tblEvent_tblCategory");
            });

            modelBuilder.Entity<EventLike>(entity =>
            {
                entity.HasKey(e => new {e.EventId, e.UserId});

                entity.ToTable("tblEventLike");

                entity.HasIndex(e => e.UserId, "IX_tblEventLike_UserID");

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.TblEventLikes)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblEventLike_tblEvent");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblEventLikes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblEventLike_tblUser");
            });

            modelBuilder.Entity<FollowEvent>(entity =>
            {
                entity.HasKey(e => new {e.EventId, e.UserId});

                entity.ToTable("tblFollowEvent");

                entity.HasIndex(e => e.UserId, "IX_tblFollowEvent_UserID");

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.TblFollowEvents)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblFollowEvent_tblEvent");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblFollowEvents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblFollowEvent_tblUser");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId);

                entity.ToTable("tblReport");

                entity.HasIndex(e => e.Author, "IX_tblReport_Author");

                entity.HasIndex(e => e.EventId, "IX_tblReport_EventID");

                entity.Property(e => e.ReportId)
                    .ValueGeneratedNever()
                    .HasColumnName("ReportID");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.TblReports)
                    .HasForeignKey(d => d.Author)
                    .HasConstraintName("FK_tblReport_tblUser");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.TblReports)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_tblReport_tblEvent");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("tblUser");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("UserID");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsBlocked).HasColumnName("isBlocked");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserEvent>(entity =>
            {
                entity.HasKey(e => new {e.EventId, e.UserId});

                entity.ToTable("tblUserEvent");

                entity.HasIndex(e => e.UserId, "IX_tblUserEvent_UserID");

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.TblUserEvents)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblUserEvent_tblEvent");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserEvents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblUserEvent_tblUser");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}