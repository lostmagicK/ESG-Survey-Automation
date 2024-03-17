using System;
using System.Collections.Generic;
using ESG_Survey_Automation.Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace ESG_Survey_Automation.Infrastructure.EntityFramework;

public partial class ESGSurveyContext : DbContext
{
    public ESGSurveyContext(DbContextOptions<ESGSurveyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Token1);

            entity.ToTable("Token");

            entity.Property(e => e.Token1)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Token");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Token_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EncryptedPassword)
                .IsRequired()
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
