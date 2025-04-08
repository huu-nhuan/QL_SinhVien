using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebQuanLySinhVien.Models;

public partial class QuanLySinhVienContext : DbContext
{
    public QuanLySinhVienContext()
    {
    }

    public QuanLySinhVienContext(DbContextOptions<QuanLySinhVienContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Diemhp> Diemhps { get; set; }

    public virtual DbSet<GiangVien> GiangViens { get; set; }

    public virtual DbSet<HoSoHocTap> HoSoHocTaps { get; set; }

    public virtual DbSet<Hocphan> Hocphans { get; set; }

    public virtual DbSet<Khoa> Khoas { get; set; }

    public virtual DbSet<Lop> Lops { get; set; }

    public virtual DbSet<Nganh> Nganhs { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=HuuNhuan;Initial Catalog=QuanLySinhVien;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Diemhp>(entity =>
        {
            entity.HasKey(e => new { e.MaSv, e.MaHp });

            entity.ToTable("DIEMHP");

            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .HasColumnName("MaSV");
            entity.Property(e => e.MaHp)
                .HasMaxLength(10)
                .HasColumnName("MaHP");
            entity.Property(e => e.DiemHp)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("DiemHP");

            entity.HasOne(d => d.MaHpNavigation).WithMany(p => p.Diemhps)
                .HasForeignKey(d => d.MaHp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DIEMHP_HOCPHAN");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.Diemhps)
                .HasForeignKey(d => d.MaSv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DIEMHP_SinhVien");
        });

        modelBuilder.Entity<GiangVien>(entity =>
        {
            entity.HasKey(e => e.MaGv);

            entity.ToTable("GiangVien");

            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .HasColumnName("MaGV");
            entity.Property(e => e.DiaChi).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .HasColumnName("SDT");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.GiangViens)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK_GiangVien_TAIKHOAN");
        });

        modelBuilder.Entity<HoSoHocTap>(entity =>
        {
            entity.ToTable("HoSoHocTap");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DiemTichLuy).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .HasColumnName("MaSV");
            entity.Property(e => e.SoTctichLuy).HasColumnName("SoTCTichLuy");
            entity.Property(e => e.TrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<Hocphan>(entity =>
        {
            entity.HasKey(e => e.MaHp);

            entity.ToTable("HOCPHAN");

            entity.Property(e => e.MaHp)
                .HasMaxLength(10)
                .HasColumnName("MaHP");
            entity.Property(e => e.HocKy).HasMaxLength(50);
            entity.Property(e => e.MaNganh).HasMaxLength(10);
            entity.Property(e => e.SoTc).HasColumnName("SoTC");
            entity.Property(e => e.TenHp)
                .HasMaxLength(50)
                .HasColumnName("TenHP");

            entity.HasOne(d => d.MaNganhNavigation).WithMany(p => p.Hocphans)
                .HasForeignKey(d => d.MaNganh)
                .HasConstraintName("FK_HOCPHAN_NGANH");
        });

        modelBuilder.Entity<Khoa>(entity =>
        {
            entity.HasKey(e => e.MaKhoa);

            entity.ToTable("KHOA");

            entity.Property(e => e.MaKhoa).HasMaxLength(10);
            entity.Property(e => e.TenKhoa).HasMaxLength(50);
        });

        modelBuilder.Entity<Lop>(entity =>
        {
            entity.HasKey(e => e.MaLop);

            entity.ToTable("LOP");

            entity.Property(e => e.MaLop).HasMaxLength(10);
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .HasColumnName("MaGV");
            entity.Property(e => e.NamNhapHoc).HasMaxLength(50);
            entity.Property(e => e.TenLop).HasMaxLength(50);

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.Lops)
                .HasForeignKey(d => d.MaGv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LOP_GiangVien");
        });

        modelBuilder.Entity<Nganh>(entity =>
        {
            entity.HasKey(e => e.MaNganh);

            entity.ToTable("NGANH");

            entity.Property(e => e.MaNganh).HasMaxLength(10);
            entity.Property(e => e.MaKhoa).HasMaxLength(10);
            entity.Property(e => e.TenNganh).HasMaxLength(50);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.Nganhs)
                .HasForeignKey(d => d.MaKhoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NGANH_KHOA");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv);

            entity.ToTable("SinhVien");

            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .HasColumnName("MaSV");
            entity.Property(e => e.DiaChi).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.MaLop).HasMaxLength(10);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .HasColumnName("SDT");

            entity.HasOne(d => d.HoSoNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.HoSo)
                .HasConstraintName("FK_SinhVien_HoSoHocTap");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK_SinhVien_TaiKhoan");

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaLop)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SinhVien_LOP");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.IdTk).HasName("PK__TAIKHOAN__8B63B1A9CC67867F");

            entity.ToTable("TAIKHOAN");

            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.MatKhau).HasMaxLength(50);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
