using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ValidadorXml.Models.DB;

namespace ValidadorXml.Models.DB;

public partial class ValidadorXmlContext : DbContext
{
    public ValidadorXmlContext()
    {
    }

    public ValidadorXmlContext(DbContextOptions<ValidadorXmlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estatus> Estatuses { get; set; }

    public virtual DbSet<Informacion> Informacions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //optionsBuilder.UseSqlServer("server=localhost; database=ValidadorXml; user id=sa; password=1203; TrustServerCertificate=True;");
        }
    }


//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estatus>(entity =>
        {
            entity.HasKey(e => e.EstatusId).HasName("PK__Estatus__FE5015C388FA32FB");

            entity.ToTable("Estatus");

            entity.Property(e => e.EstatusId).HasColumnName("ESTATUS_ID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .HasColumnName("DESCRIPCION");
        });

        modelBuilder.Entity<LogError>(entity =>
        {
            
            entity.HasKey(e => e.Id).HasName("PK_LogErrores");
            entity.ToTable("LogErrores");
            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd(); 
            entity.Property(e => e.Mensaje)
                .HasColumnName("MENSAJE")
                .HasColumnType("NVARCHAR(MAX)"); 
            entity.Property(e => e.Fecha)
                .HasColumnName("FECHA")
                .IsRequired(); 
            entity.Property(e => e.Archivo)
                .HasColumnName("ARCHIVO")
                .HasMaxLength(255); });


        modelBuilder.Entity<Informacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Informac__3214EC27F1A791E1");

            entity.ToTable("Informacion");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EstatusId).HasColumnName("ESTATUS_ID");
            entity.Property(e => e.FechaEmision)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_EMISION");
            entity.Property(e => e.FolioFiscal)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("FOLIO_FISCAL");
            entity.Property(e => e.RfcEmisor)
                .HasMaxLength(13)
                .HasColumnName("RFC_EMISOR");
            entity.Property(e => e.RfcReceptor)
                .HasMaxLength(13)
                .HasColumnName("RFC_RECEPTOR");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TOTAL");

            entity.HasOne(d => d.Estatus).WithMany(p => p.Informacions)
                .HasForeignKey(d => d.EstatusId)
                .HasConstraintName("FK_CFDI_ESTATUS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<ValidadorXml.Models.DB.LogError> LogError { get; set; } = default!;
}
