using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ULTIMATE_MIND.Arquitetura.Model.UltimateMind
{
    public partial class ultimate_mindContext : DbContext
    {
        public ultimate_mindContext()
        {
        }

        public ultimate_mindContext(DbContextOptions<ultimate_mindContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Atendimento> Atendimento { get; set; }
        public virtual DbSet<Cargo> Cargo { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Configuracao> Configuracao { get; set; }
        public virtual DbSet<Empresa> Empresa { get; set; }
        public virtual DbSet<EmpresaUsuario> EmpresaUsuario { get; set; }
        public virtual DbSet<GrupoPermissao> GrupoPermissao { get; set; }
        public virtual DbSet<Obra> Obra { get; set; }
        public virtual DbSet<ObraUsuario> ObraUsuario { get; set; }
        public virtual DbSet<Ponto> Ponto { get; set; }
        public virtual DbSet<Tela> Tela { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<ValidacaoContraCheque> ValidacaoContraCheque { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=186.250.240.35\\MSSQLSERVER2022,14337;Database=ultimate_mind;User Id=utimid;Password=Uy1m4j&90;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "utimid");

            modelBuilder.Entity<Atendimento>(entity =>
            {
                entity.HasKey(e => e.Idatendimento);

                entity.ToTable("Atendimento", "dbo");

                entity.Property(e => e.Idatendimento).HasColumnName("IDAtendimento");

                entity.Property(e => e.DataAtendimento).HasColumnType("date");

                entity.Property(e => e.FimAtendimento).HasColumnType("datetime");

                entity.Property(e => e.Idcliente).HasColumnName("IDCliente");

                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.Property(e => e.InicioAtendimento).HasColumnType("datetime");

                entity.Property(e => e.Observacao).HasColumnType("text");

                entity.HasOne(d => d.IdclienteNavigation)
                    .WithMany(p => p.Atendimento)
                    .HasForeignKey(d => d.Idcliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Atendimento_Cliente");

                entity.HasOne(d => d.IdusuarioNavigation)
                    .WithMany(p => p.Atendimento)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Atendimento_Usuario");
            });

            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.HasKey(e => e.Idcargo);

                entity.ToTable("Cargo", "dbo");

                entity.Property(e => e.Idcargo).HasColumnName("IDCargo");

                entity.Property(e => e.NomeCargo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Idcliente);

                entity.ToTable("Cliente", "dbo");

                entity.Property(e => e.Idcliente).HasColumnName("IDCliente");

                entity.Property(e => e.Cnpj)
                    .HasColumnName("CNPJ")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Cpf)
                    .HasColumnName("CPF")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Email).HasColumnType("text");

                entity.Property(e => e.Endereco).HasColumnType("text");

                entity.Property(e => e.Idempresa).HasColumnName("IDEmpresa");

                entity.Property(e => e.NomeCliente)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Telefone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.Cliente)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cliente_Empresa");
            });

            modelBuilder.Entity<Configuracao>(entity =>
            {
                entity.HasKey(e => e.Idconfiguracao);

                entity.ToTable("Configuracao", "dbo");

                entity.Property(e => e.Idconfiguracao).HasColumnName("IDConfiguracao");

                entity.Property(e => e.Idempresa).HasColumnName("IDEmpresa");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Valor)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.Configuracao)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Configuracao_Empresa");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasKey(e => e.Idempresa);

                entity.ToTable("Empresa", "dbo");

                entity.Property(e => e.Idempresa).HasColumnName("IDEmpresa");

                entity.Property(e => e.Apelido)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Cnpj)
                    .HasColumnName("CNPJ")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Endereco).HasColumnType("text");

                entity.Property(e => e.NomeFantasia).HasColumnType("text");

                entity.Property(e => e.RazaoSocial).HasColumnType("text");

                entity.Property(e => e.Telefone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EmpresaUsuario>(entity =>
            {
                entity.HasKey(e => e.IdempresaUsuario);

                entity.ToTable("EmpresaUsuario", "dbo");

                entity.Property(e => e.IdempresaUsuario).HasColumnName("IDEmpresaUsuario");

                entity.Property(e => e.Idempresa).HasColumnName("IDEmpresa");

                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.EmpresaUsuario)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmpresaUsuario_Empresa");

                entity.HasOne(d => d.IdusuarioNavigation)
                    .WithMany(p => p.EmpresaUsuario)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmpresaUsuario_Usuario");
            });

            modelBuilder.Entity<GrupoPermissao>(entity =>
            {
                entity.HasKey(e => e.IdgrupoPermissao);

                entity.ToTable("GrupoPermissao", "dbo");

                entity.Property(e => e.IdgrupoPermissao).HasColumnName("IDGrupoPermissao");

                entity.Property(e => e.Idempresa).HasColumnName("IDEmpresa");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.GrupoPermissao)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GrupoPermissao_Empresa");
            });

            modelBuilder.Entity<Obra>(entity =>
            {
                entity.HasKey(e => e.Idobra);

                entity.Property(e => e.Idobra).HasColumnName("IDObra");

                entity.Property(e => e.Endereco)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Idcliente).HasColumnName("IDCliente");

                entity.Property(e => e.NomeObra)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdclienteNavigation)
                    .WithMany(p => p.Obra)
                    .HasForeignKey(d => d.Idcliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Obra_Cliente");
            });

            modelBuilder.Entity<ObraUsuario>(entity =>
            {
                entity.HasKey(e => e.IdobraUsuario);

                entity.Property(e => e.IdobraUsuario).HasColumnName("IDObraUsuario");

                entity.Property(e => e.Idobra).HasColumnName("IDObra");

                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.HasOne(d => d.IdobraNavigation)
                    .WithMany(p => p.ObraUsuario)
                    .HasForeignKey(d => d.Idobra)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ObraUsuario_Obra");

                entity.HasOne(d => d.IdusuarioNavigation)
                    .WithMany(p => p.ObraUsuario)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ObraUsuario_Usuario");
            });

            modelBuilder.Entity<Ponto>(entity =>
            {
                entity.HasKey(e => e.Idponto);

                entity.ToTable("Ponto", "dbo");

                entity.Property(e => e.Idponto).HasColumnName("IDPonto");

                entity.Property(e => e.DataPonto).HasColumnType("date");

                entity.Property(e => e.FimAlmoco).HasColumnType("datetime");

                entity.Property(e => e.FimDia).HasColumnType("datetime");

                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.Property(e => e.InicioAlmoco).HasColumnType("datetime");

                entity.Property(e => e.InicioDia).HasColumnType("datetime");

                entity.HasOne(d => d.IdusuarioNavigation)
                    .WithMany(p => p.Ponto)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ponto_Usuario");
            });

            modelBuilder.Entity<Tela>(entity =>
            {
                entity.HasKey(e => e.Idtela);

                entity.ToTable("Tela", "dbo");

                entity.Property(e => e.Idtela).HasColumnName("IDTela");

                entity.Property(e => e.IdgrupoPermissao).HasColumnName("IDGrupoPermissao");

                entity.Property(e => e.NomeTela)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdgrupoPermissaoNavigation)
                    .WithMany(p => p.Tela)
                    .HasForeignKey(d => d.IdgrupoPermissao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tela_GrupoPermissao");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Idusuario);

                entity.ToTable("Usuario", "dbo");

                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.Property(e => e.ChaveSignatario).HasColumnType("text");

                entity.Property(e => e.Cpf)
                    .IsRequired()
                    .HasColumnName("CPF")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.DataAdmissao).HasColumnType("date");

                entity.Property(e => e.DataDemissao).HasColumnType("date");

                entity.Property(e => e.DataNascimento).HasColumnType("date");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.HoraEntrada)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HoraSaida)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Idcargo).HasColumnName("IDCargo");

                entity.Property(e => e.Idempresa).HasColumnName("IDEmpresa");

                entity.Property(e => e.IdgrupoPermissao).HasColumnName("IDGrupoPermissao");

                entity.Property(e => e.Matricula)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NomeFoto).HasColumnType("text");

                entity.Property(e => e.Rg)
                    .HasColumnName("RG")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Senha)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Telefone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdcargoNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.Idcargo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Cargo");

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Empresa");

                entity.HasOne(d => d.IdgrupoPermissaoNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.IdgrupoPermissao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_GrupoPermissao");
            });

            modelBuilder.Entity<ValidacaoContraCheque>(entity =>
            {
                entity.HasKey(e => e.IdvalidacaoContraCheque);

                entity.ToTable("ValidacaoContraCheque", "dbo");

                entity.Property(e => e.IdvalidacaoContraCheque).HasColumnName("IDValidacaoContraCheque");

                entity.Property(e => e.DataAssinado).HasColumnType("datetime");

                entity.Property(e => e.DataInserido).HasColumnType("datetime");

                entity.Property(e => e.Idempresa).HasColumnName("IDEmpresa");

                entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");

                entity.Property(e => e.NomeArquivo)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Referencia).HasColumnType("date");

                entity.HasOne(d => d.IdempresaNavigation)
                    .WithMany(p => p.ValidacaoContraCheque)
                    .HasForeignKey(d => d.Idempresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ValidacaoContraCheque_Empresa");

                entity.HasOne(d => d.IdusuarioNavigation)
                    .WithMany(p => p.ValidacaoContraCheque)
                    .HasForeignKey(d => d.Idusuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ValidacaoContraCheque_Usuario");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
