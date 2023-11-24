using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace back.Models;

public partial class BddContext : DbContext
{
    public BddContext()
    {
    }

    public BddContext(DbContextOptions<BddContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Personne> Personnes { get; set; }

    public virtual DbSet<Personnerestauranthistorique> Personnerestauranthistoriques { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<Typerestaurant> Typerestaurants { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Personne>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("personne");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Login).HasMaxLength(200);
            entity.Property(e => e.Mdp).HasMaxLength(400);
            entity.Property(e => e.Nom).HasMaxLength(200);
        });

        modelBuilder.Entity<Personnerestauranthistorique>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("personnerestauranthistorique");

            entity.HasIndex(e => e.IdPersonne, "IdPersonne");

            entity.HasIndex(e => e.IdRestaurant, "IdRestaurant");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.IdPersonne).HasColumnType("int(11)");
            entity.Property(e => e.IdRestaurant).HasColumnType("int(11)");
            entity.Property(e => e.Prix).HasPrecision(5, 2);

            entity.HasOne(d => d.IdPersonneNavigation).WithMany(p => p.Personnerestauranthistoriques)
                .HasForeignKey(d => d.IdPersonne)
                .HasConstraintName("personnerestauranthistorique_ibfk_1");

            entity.HasOne(d => d.IdRestaurantNavigation).WithMany(p => p.Personnerestauranthistoriques)
                .HasForeignKey(d => d.IdRestaurant)
                .HasConstraintName("personnerestauranthistorique_ibfk_2");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("restaurant");

            entity.HasIndex(e => e.IdTypeRestaurant, "IdTypeRestaurant");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Adresse).HasMaxLength(300);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IdTypeRestaurant).HasColumnType("int(11)");
            entity.Property(e => e.Nom).HasMaxLength(200);
            entity.Property(e => e.Telephone)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Url).HasMaxLength(1000);

            entity.HasOne(d => d.IdTypeRestaurantNavigation).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.IdTypeRestaurant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("restaurant_ibfk_1");

            entity.HasMany(d => d.IdPersonnes).WithMany(p => p.IdRestaurants)
                .UsingEntity<Dictionary<string, object>>(
                    "Personnerestaurantaimer",
                    r => r.HasOne<Personne>().WithMany()
                        .HasForeignKey("IdPersonne")
                        .HasConstraintName("personnerestaurantaimer_ibfk_1"),
                    l => l.HasOne<Restaurant>().WithMany()
                        .HasForeignKey("IdRestaurant")
                        .HasConstraintName("personnerestaurantaimer_ibfk_2"),
                    j =>
                    {
                        j.HasKey("IdRestaurant", "IdPersonne")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("personnerestaurantaimer");
                        j.HasIndex(new[] { "IdPersonne" }, "IdPersonne");
                        j.IndexerProperty<int>("IdRestaurant").HasColumnType("int(11)");
                        j.IndexerProperty<int>("IdPersonne").HasColumnType("int(11)");
                    });
        });

        modelBuilder.Entity<Typerestaurant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("typerestaurant");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Nom).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
