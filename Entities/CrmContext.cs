using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Sales_CRM.Entities;

public partial class CrmContext : DbContext
{
    public CrmContext()
    {
    }

    public CrmContext(DbContextOptions<CrmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<Timesheet> Timesheets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserNote> UserNotes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=CRM;Username=postgres;Password=Jai@2005");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.HasIndex(e => e.Name, "departments_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("leads_pkey");

            entity.ToTable("leads");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(150)
                .HasColumnName("address");
            entity.Property(e => e.Assigned)
                .HasMaxLength(100)
                .HasColumnName("assigned");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Comments)
                .HasMaxLength(255)
                .HasColumnName("comments");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Deposits)
                .HasPrecision(12, 2)
                .HasColumnName("deposits");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LastContact)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_contact");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .HasColumnName("source");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Tags)
                .HasMaxLength(100)
                .HasColumnName("tags");
            entity.Property(e => e.Website)
                .HasMaxLength(150)
                .HasColumnName("website");
            entity.Property(e => e.WhatsappEnable)
                .HasDefaultValue(false)
                .HasColumnName("whatsapp_enable");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(20)
                .HasColumnName("zipcode");
        });

        modelBuilder.Entity<Timesheet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("timesheets_pkey");

            entity.ToTable("timesheets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");
            entity.Property(e => e.Task)
                .HasMaxLength(255)
                .HasColumnName("task");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WorkedMinutes).HasColumnName("worked_minutes");

            entity.HasOne(d => d.User).WithMany(p => p.Timesheets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("timesheets_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Direction)
                .HasMaxLength(50)
                .HasColumnName("direction");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EmailSignature).HasColumnName("email_signature");
            entity.Property(e => e.Facebook).HasColumnName("facebook");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.HourlyRate)
                .HasPrecision(10, 2)
                .HasColumnName("hourly_rate");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("is_admin");
            entity.Property(e => e.IsStaff)
                .HasDefaultValue(true)
                .HasColumnName("is_staff");
            entity.Property(e => e.LastActiveAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_active_at");
            entity.Property(e => e.LastMonthLoggedTime).HasColumnName("last_month_logged_time");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.LastWeekLoggedTime).HasColumnName("last_week_logged_time");
            entity.Property(e => e.Linkedin).HasColumnName("linkedin");
            entity.Property(e => e.PasswordChangedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("password_changed_at");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ProfileImage).HasColumnName("profile_image");
            entity.Property(e => e.SalesTarget)
                .HasPrecision(12, 2)
                .HasColumnName("sales_target");
            entity.Property(e => e.Skype).HasColumnName("skype");
            entity.Property(e => e.ThisMonthLoggedTime).HasColumnName("this_month_logged_time");
            entity.Property(e => e.ThisWeekLoggedTime).HasColumnName("this_week_logged_time");
            entity.Property(e => e.TotalLoggedTime).HasColumnName("total_logged_time");
            entity.Property(e => e.TwilioPhone)
                .HasMaxLength(20)
                .HasColumnName("twilio_phone");
            entity.Property(e => e.TwilioWhatsappEnabled)
                .HasDefaultValue(false)
                .HasColumnName("twilio_whatsapp_enabled");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Departments).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserDepartment",
                    r => r.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .HasConstraintName("fk_user_departments_department"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_departments_user"),
                    j =>
                    {
                        j.HasKey("UserId", "DepartmentId").HasName("user_departments_pkey");
                        j.ToTable("user_departments");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("DepartmentId").HasColumnName("department_id");
                    });
        });

        modelBuilder.Entity<UserNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_notes_pkey");

            entity.ToTable("user_notes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddedFrom)
                .HasMaxLength(100)
                .HasColumnName("added_from");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_notes_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
