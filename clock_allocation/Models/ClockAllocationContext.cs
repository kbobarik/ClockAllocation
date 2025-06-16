using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace clock_allocation.Models;

public partial class ClockAllocationContext : DbContext
{
    public ClockAllocationContext()
    {
    }

    public ClockAllocationContext(DbContextOptions<ClockAllocationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Curriculum> Curricula { get; set; }

    public virtual DbSet<FormOfIntermediateCertification> FormOfIntermediateCertifications { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeachingLoad> TeachingLoads { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=clock_allocation;Username=postgres;Password=12345");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicPlanRecord>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });
        modelBuilder.Entity<Curriculum>(entity =>
        {
            entity.HasKey(e => e.IdCurriculum).HasName("curriculum_pk");

            entity.ToTable("curriculum");

            entity.Property(e => e.IdCurriculum).HasColumnName("id_curriculum");
            entity.Property(e => e.IdSpecialization).HasColumnName("id_specialization");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.IdSpecializationNavigation).WithMany(p => p.Curricula)
                .HasForeignKey(d => d.IdSpecialization)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("curriculum_id_specialization_fkey");
        });

        modelBuilder.Entity<FormOfIntermediateCertification>(entity =>
        {
            entity.HasKey(e => e.IdFormOfIntermediateCertification).HasName("form_of_intermediate_certification_pkey");

            entity.ToTable("form_of_intermediate_certification");

            entity.Property(e => e.IdFormOfIntermediateCertification)
                .HasDefaultValueSql(
                    "nextval('form_of_intermediate_certific_id_form_of_intermediate_certi_seq'::regclass)")
                .HasColumnName("id_form_of_intermediate_certification");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("group_pkey");

            entity.ToTable("group");

            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.FormOfEducation)
                .HasDefaultValueSql("'Бюджет'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("form_of_education");
            entity.Property(e => e.IdSpecialization).HasColumnName("id_specialization");
            entity.Property(e => e.StreamNumber).HasColumnName("stream_number");
            entity.Property(e => e.YearOfAdmission).HasColumnName("year_of_admission");

            entity.HasOne(d => d.IdSpecializationNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.IdSpecialization)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_id_specialization_fkey");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.IdLanguage).HasName("languages_pk");

            entity.ToTable("languages");

            entity.Property(e => e.IdLanguage).HasColumnName("id_language");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.IdModule).HasName("module_pkey");

            entity.ToTable("module");

            entity.Property(e => e.IdModule).HasColumnName("id_module");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.IdCurriculum)
                .HasDefaultValue(1)
                .HasColumnName("id_curriculum");
            entity.Property(e => e.IdHeadModule).HasColumnName("id_head_module");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");

            entity.HasOne(d => d.IdCurriculumNavigation).WithMany(p => p.Modules)
                .HasForeignKey(d => d.IdCurriculum)
                .HasConstraintName("module_curriculum_fk");

            entity.HasOne(d => d.IdHeadModuleNavigation).WithMany(p => p.InverseIdHeadModuleNavigation)
                .HasForeignKey(d => d.IdHeadModule)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("module_module_fk");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.IdSemester).HasName("semester_pkey");

            entity.ToTable("semester");

            entity.Property(e => e.IdSemester).HasColumnName("id_semester");
            entity.Property(e => e.Consultations)
                .HasDefaultValue(0)
                .HasColumnName("consultations");
            entity.Property(e => e.IdFormOfIntermediateCertification)
                .HasDefaultValue(4)
                .HasColumnName("id_form_of_intermediate_certification");
            entity.Property(e => e.IdSubject).HasColumnName("id_subject");
            entity.Property(e => e.IndependentWork)
                .HasDefaultValue(0)
                .HasColumnName("independent_work");
            entity.Property(e => e.IntermediateCertification)
                .HasDefaultValue(0)
                .HasColumnName("intermediate_certification");
            entity.Property(e => e.Lessons)
                .HasDefaultValue(0)
                .HasColumnName("lessons");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Practice)
                .HasDefaultValue(0)
                .HasColumnName("practice");

            entity.HasOne(d => d.IdFormOfIntermediateCertificationNavigation).WithMany(p => p.Semesters)
                .HasForeignKey(d => d.IdFormOfIntermediateCertification)
                .HasConstraintName("semester_id_form_of_intermediate_certification_fkey");

            entity.HasOne(d => d.IdSubjectNavigation).WithMany(p => p.Semesters)
                .HasForeignKey(d => d.IdSubject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("semester_id_subject_fkey");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.IdSpecialization).HasName("specialization_pkey");

            entity.ToTable("specialization");

            entity.Property(e => e.IdSpecialization).HasColumnName("id_specialization");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.Direction)
                .HasColumnType("character varying")
                .HasColumnName("direction");
            entity.Property(e => e.DurationOfTraining)
                .HasColumnType("character varying")
                .HasColumnName("duration_of_training");
            entity.Property(e => e.EducationLevel)
                .HasColumnType("character varying")
                .HasColumnName("education_level");
            entity.Property(e => e.FormOfEducation)
                .HasColumnType("character varying")
                .HasColumnName("form_of_education");
            entity.Property(e => e.IdLanguage).HasColumnName("id_language");
            entity.Property(e => e.Qualification)
                .HasColumnType("character varying")
                .HasColumnName("qualification");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");

            entity.HasOne(d => d.IdLanguageNavigation).WithMany(p => p.Specializations)
                .HasForeignKey(d => d.IdLanguage)
                .HasConstraintName("specialization_languages_fk");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.IdSubject).HasName("subject_pkey");

            entity.ToTable("subject");

            entity.Property(e => e.IdSubject).HasColumnName("id_subject");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.CourseDesign)
                .HasDefaultValue(0)
                .HasColumnName("course_design");
            entity.Property(e => e.IdModule).HasColumnName("id_module");
            entity.Property(e => e.LaboratoryAndPracticalExercises)
                .HasDefaultValue(0)
                .HasColumnName("laboratory_and_practical_exercises");
            entity.Property(e => e.Lessons)
                .HasDefaultValue(0)
                .HasColumnName("lessons");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");

            entity.HasOne(d => d.IdModuleNavigation).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.IdModule)
                .HasConstraintName("subject_id_module_fkey");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.IdTeacher).HasName("teacher_pkey");

            entity.ToTable("teacher");

            entity.Property(e => e.IdTeacher).HasColumnName("id_teacher");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasColumnType("character varying")
                .HasColumnName("patronymic");
            entity.Property(e => e.Surname)
                .HasColumnType("character varying")
                .HasColumnName("surname");
        });

        modelBuilder.Entity<TeachingLoad>(entity =>
        {
            entity.HasKey(e => e.IdTeachingLoad).HasName("teaching_load_pkey");

            entity.ToTable("teaching_load");

            entity.Property(e => e.IdTeachingLoad).HasColumnName("id_teaching_load");
            entity.Property(e => e.Billing)
                .HasDefaultValue(0)
                .HasColumnName("billing");
            entity.Property(e => e.CheckingCoursework)
                .HasDefaultValue(0)
                .HasColumnName("checking_coursework");
            entity.Property(e => e.Consultations)
                .HasDefaultValue(0)
                .HasColumnName("consultations");
            entity.Property(e => e.CourseDesign)
                .HasDefaultValue(0)
                .HasColumnName("course_design");
            entity.Property(e => e.Exam)
                .HasDefaultValue(0)
                .HasColumnName("exam");
            entity.Property(e => e.HoursPerWeek)
                .HasDefaultValue(0)
                .HasColumnName("hours_per_ week");
            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.IdSemester).HasColumnName("id_semester");
            entity.Property(e => e.IdSubject).HasColumnName("id_subject");
            entity.Property(e => e.IdTeacher).HasColumnName("id_teacher");
            entity.Property(e => e.IndependentWork)
                .HasDefaultValue(0)
                .HasColumnName("independent_work");
            entity.Property(e => e.Laboratory)
                .HasDefaultValue(0)
                .HasColumnName("laboratory");
            entity.Property(e => e.Lectures)
                .HasDefaultValue(0)
                .HasColumnName("lectures");
            entity.Property(e => e.MaximumWorkload)
                .HasDefaultValue(0)
                .HasColumnName("maximum_workload");
            entity.Property(e => e.ReviewAndLecture)
                .HasDefaultValue(0)
                .HasColumnName("review_and_lecture");
            entity.Property(e => e.Split)
                .HasDefaultValue(false)
                .HasColumnName("split");
            entity.Property(e => e.TotalYear)
                .HasDefaultValue(0)
                .HasColumnName("total_year");
            entity.Property(e => e.AcademicHoursPerYear)
                .HasDefaultValue(0)
                .HasColumnName("academic_hours_per_year");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.СonsultationsPayment)
                .HasDefaultValue(0)
                .HasColumnName("сonsultations_payment");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.TeachingLoads)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teaching_load_id_group_fkey");

            entity.HasOne(d => d.IdSemesterNavigation).WithMany(p => p.TeachingLoads)
                .HasForeignKey(d => d.IdSemester)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teaching_load_id_semester_fkey");

            entity.HasOne(d => d.IdSubjectNavigation).WithMany(p => p.TeachingLoads)
                .HasForeignKey(d => d.IdSubject)
                .HasConstraintName("teaching_load_subject_fk");

            entity.HasOne(d => d.IdTeacherNavigation).WithMany(p => p.TeachingLoads)
                .HasForeignKey(d => d.IdTeacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teaching_load_id_teacher_fkey");
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pk");

            entity.ToTable("users");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}