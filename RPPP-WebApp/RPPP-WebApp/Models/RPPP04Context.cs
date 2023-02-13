﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RPPP_WebApp.Models
{
    public partial class RPPP04Context : DbContext
    {
        public RPPP04Context()
        {
        }

        public RPPP04Context(DbContextOptions<RPPP04Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Autocesta> Autocesta { get; set; }
        public virtual DbSet<CestovniObjekt> CestovniObjekt { get; set; }
        public virtual DbSet<Dionica> Dionica { get; set; }
        public virtual DbSet<Incident> Incident { get; set; }
        public virtual DbSet<Kamera> Kamera { get; set; }
        public virtual DbSet<KategorijaVozila> KategorijaVozila { get; set; }
        public virtual DbSet<Koncesionar> Koncesionar { get; set; }
        public virtual DbSet<NaplatnaKucica> NaplatnaKucica { get; set; }
        public virtual DbSet<NaplatnaPostaja> NaplatnaPostaja { get; set; }
        public virtual DbSet<Odmoriste> Odmoriste { get; set; }
        public virtual DbSet<OdrzavanjeObjekta> OdrzavanjeObjekta { get; set; }
        public virtual DbSet<PopratniSadrzaj> PopratniSadrzaj { get; set; }
        public virtual DbSet<ProlazakVozila> ProlazakVozila { get; set; }
        public virtual DbSet<Reakcija> Reakcija { get; set; }
        public virtual DbSet<VrstaIncidenta> VrstaIncidenta { get; set; }
        public virtual DbSet<VrstaKamere> VrstaKamere { get; set; }
        public virtual DbSet<VrstaNaplate> VrstaNaplate { get; set; }
        public virtual DbSet<VrstaOdrzavanja> VrstaOdrzavanja { get; set; }
        public virtual DbSet<VrstaPopratnog> VrstaPopratnog { get; set; }
        public virtual DbSet<VrstaReakcije> VrstaReakcije { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Autocesta>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Oznaka)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.HasOne(d => d.Koncesionar)
                    .WithMany(p => p.Autocesta)
                    .HasForeignKey(d => d.KoncesionarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Autocesta_Koncesionar");
            });

            modelBuilder.Entity<CestovniObjekt>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipObjekta)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zanimljivost)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Dionica)
                    .WithMany(p => p.CestovniObjekt)
                    .HasForeignKey(d => d.DionicaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CestovniObjekt_Dionica");
            });

            modelBuilder.Entity<Dionica>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Autocesta)
                    .WithMany(p => p.Dionica)
                    .HasForeignKey(d => d.AutocestaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dionica_Autocesta");

                entity.HasOne(d => d.IzlaznaPostaja)
                    .WithMany(p => p.DionicaIzlaznaPostaja)
                    .HasForeignKey(d => d.IzlaznaPostajaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dionica_NaplatnaPostaja1");

                entity.HasOne(d => d.UlaznaPostaja)
                    .WithMany(p => p.DionicaUlaznaPostaja)
                    .HasForeignKey(d => d.UlaznaPostajaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dionica_NaplatnaPostaja");
            });

            modelBuilder.Entity<Incident>(entity =>
            {
                entity.Property(e => e.Datum).HasColumnType("date");

                entity.Property(e => e.MeteoroloskiUvjeti)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Opis)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Prohodnost)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.StanjeNaCesti)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Dionica)
                    .WithMany(p => p.Incident)
                    .HasForeignKey(d => d.DionicaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Incident_Dionica");

                entity.HasOne(d => d.VrstaIncidenta)
                    .WithMany(p => p.Incident)
                    .HasForeignKey(d => d.VrstaIncidentaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Incident_VrstaIncidenta");
            });

            modelBuilder.Entity<Kamera>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ufunkciji).HasColumnName("UFunkciji");

                entity.HasOne(d => d.Dionica)
                    .WithMany(p => p.Kamera)
                    .HasForeignKey(d => d.DionicaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Kamera_Dionica");

                entity.HasOne(d => d.VrstaKamere)
                    .WithMany(p => p.Kamera)
                    .HasForeignKey(d => d.VrstaKamereId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Kamera_VrstaKamere");
            });

            modelBuilder.Entity<KategorijaVozila>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Koncesionar>(entity =>
            {
                entity.Property(e => e.Adresa)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EMail");

                entity.Property(e => e.KoncesijaDo).HasColumnType("date");

                entity.Property(e => e.KoncesijaOd).HasColumnType("date");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NaplatnaKucica>(entity =>
            {
                entity.HasOne(d => d.NaplatnaPostaja)
                    .WithMany(p => p.NaplatnaKucica)
                    .HasForeignKey(d => d.NaplatnaPostajaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NaplatnaKucica_NaplatnaPostaja");

                entity.HasOne(d => d.VrstaNaplate)
                    .WithMany(p => p.NaplatnaKucica)
                    .HasForeignKey(d => d.VrstaNaplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NaplatnaKucica_VrstaNaplate");
            });

            modelBuilder.Entity<NaplatnaPostaja>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Autocesta)
                    .WithMany(p => p.NaplatnaPostaja)
                    .HasForeignKey(d => d.AutocestaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NaplatnaPostaja_Autocesta");
            });

            modelBuilder.Entity<Odmoriste>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Dionica)
                    .WithMany(p => p.Odmoriste)
                    .HasForeignKey(d => d.DionicaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Odmoriste_Dionica");
            });

            modelBuilder.Entity<OdrzavanjeObjekta>(entity =>
            {
                entity.Property(e => e.ImeFirme)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RadnimDanomDo).HasColumnType("time(0)");

                entity.Property(e => e.RadnimDanomOd).HasColumnType("time(0)");

                entity.Property(e => e.VikendimaDo).HasColumnType("time(0)");

                entity.Property(e => e.VikendimaOd).HasColumnType("time(0)");

                entity.HasOne(d => d.CestovniObjekt)
                    .WithMany(p => p.OdrzavanjeObjekta)
                    .HasForeignKey(d => d.CestovniObjektId)
                    .HasConstraintName("FK_OdrzavanjeObjekta_CestovniObjekt");

                entity.HasOne(d => d.Vrsta)
                    .WithMany(p => p.OdrzavanjeObjekta)
                    .HasForeignKey(d => d.VrstaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OdrzavanjeObjekta_VrstaOdrzavanja");
            });

            modelBuilder.Entity<PopratniSadrzaj>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RadnimDanomOd).HasColumnType("time(0)");

                entity.Property(e => e.RadninDanomDo).HasColumnType("time(0)");

                entity.Property(e => e.VikendimaDo).HasColumnType("time(0)");

                entity.Property(e => e.VikendimaOd).HasColumnType("time(0)");

                entity.HasOne(d => d.Odmoriste)
                    .WithMany(p => p.PopratniSadrzaj)
                    .HasForeignKey(d => d.OdmoristeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PopratniSadrzaj_Odmoriste");

                entity.HasOne(d => d.VrstaSadrzaja)
                    .WithMany(p => p.PopratniSadrzaj)
                    .HasForeignKey(d => d.VrstaSadrzajaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PopratniSadrzaj_VrstaPopratnog");
            });

            modelBuilder.Entity<ProlazakVozila>(entity =>
            {
                entity.Property(e => e.RegistracijskaOznaka)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VrijemeProlaska)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.KategorijaVozila)
                    .WithMany(p => p.ProlazakVozila)
                    .HasForeignKey(d => d.KategorijaVozilaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProlazakVozila_KategorijaVozila");

                entity.HasOne(d => d.NaplatnaKucica)
                    .WithMany(p => p.ProlazakVozila)
                    .HasForeignKey(d => d.NaplatnaKucicaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProlazakVozila_NaplatnaKucica1");
            });

            modelBuilder.Entity<Reakcija>(entity =>
            {
                entity.Property(e => e.Datum).HasColumnType("date");

                entity.Property(e => e.Opis)
                    .HasMaxLength(200)
                    .IsUnicode(false);


                entity.HasOne(d => d.Incident)
                    .WithMany(p => p.Reakcija)
                    .HasForeignKey(d => d.IncidentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reakcija_Incident");

                entity.HasOne(d => d.VrstaReakcije)
                    .WithMany(p => p.Reakcija)
                    .HasForeignKey(d => d.VrstaReakcijeId)
                    .HasConstraintName("FK_Reakcija_VrstaReakcije");
            });

            modelBuilder.Entity<VrstaIncidenta>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OpisPravilaPonasanja)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VrstaKamere>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VrstaNaplate>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VrstaOdrzavanja>(entity =>
            {
                entity.Property(e => e.GodisnjeDoba)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Periodicnost)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VrstaPopratnog>(entity =>
            {
                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VrstaReakcije>(entity =>
            {
                entity.Property(e => e.BrojTelefona)
                    .HasMaxLength(10)
                    .HasColumnName("Broj telefona")
                    .IsFixedLength();

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}