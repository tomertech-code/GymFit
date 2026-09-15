using System;
using System.Collections.Generic;
using System.Linq;
using GymFit.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Infrastructure.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<MemberBranchAccess> MemberBranchAccesses { get; set; }
        public DbSet<TrainerBranchAssignment> TrainerBranchAssignments { get; set; }
        public DbSet<DietPlan> DietPlans { get; set; }
        public DbSet<DietMeal> DietMeals { get; set; }
        public DbSet<ProgressRecord> ProgressRecords { get; set; }
        public DbSet<BodyMeasurement> BodyMeasurements { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<BranchEquipment> BranchEquipment { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Member Configuration
            builder.Entity<Member>(entity =>
            {
                entity.HasIndex(m => m.UserId).IsUnique();
                entity.HasOne(m => m.PrimaryBranch)
                    .WithMany(b => b.Members)
                    .HasForeignKey(m => m.PrimaryBranchId)
                    .OnDelete(DeleteBehavior.Restrict); // ✅ FIX
            });



            // Trainer Configuration
            builder.Entity<Trainer>()
                .HasIndex(t => t.UserId).IsUnique();

            builder.Entity<Trainer>()
                .HasOne(t => t.PrimaryBranch)
                .WithMany(b => b.Trainers)
                .HasForeignKey(t => t.PrimaryBranchId)
                .OnDelete(DeleteBehavior.Restrict);


            // Subscription Configuration
            builder.Entity<Subscription>(entity =>
            {
                entity.HasIndex(s => s.MemberId)
                    .HasFilter("[IsActive] = 1")
                    .IsUnique();

                entity.HasOne(s => s.Member)
                    .WithMany(m => m.Subscriptions)
                    .HasForeignKey(s => s.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.MembershipPlan)
                    .WithMany(mp => mp.Subscriptions)
                    .HasForeignKey(s => s.MembershipPlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment Configuration
            builder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Amount)
                    .HasPrecision(18, 2);
                entity.Property(p => p.TransactionId)
                    .HasMaxLength(64);
                entity.HasIndex(p => p.TransactionId)
                    .IsUnique()
                    .HasFilter("[TransactionId] IS NOT NULL");
                entity.HasIndex(p => new { p.MemberId, p.PaymentDate });
                entity.HasIndex(p => new { p.Status, p.PaymentDate });

                entity.HasOne(p => p.Member)
                    .WithMany(m => m.Payments)
                    .HasForeignKey(p => p.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Branch)
                    .WithMany()
                    .HasForeignKey(p => p.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MembershipPlan Configuration
            builder.Entity<MembershipPlan>(entity =>
            {
                entity.HasIndex(mp => mp.Name).IsUnique();
                entity.Property(mp => mp.Price)
                    .HasPrecision(18, 2);
            });

            builder.Entity<Branch>(entity =>
            {
                entity.HasIndex(b => b.Code).IsUnique();
                entity.Property(b => b.Code).HasMaxLength(30).IsRequired();
                entity.Property(b => b.Name).HasMaxLength(150).IsRequired();
            });

            builder.Entity<ContactMessage>(entity =>
            {
                entity.Property(m => m.Name).HasMaxLength(100).IsRequired();
                entity.Property(m => m.Email).HasMaxLength(256).IsRequired();
                entity.Property(m => m.Phone).HasMaxLength(30);
                entity.Property(m => m.Subject).HasMaxLength(200).IsRequired();
                entity.Property(m => m.Message).HasMaxLength(1000).IsRequired();
                entity.HasIndex(m => new { m.IsRead, m.CreatedAt });
            });

            // WorkoutPlan Configuration
            builder.Entity<WorkoutPlan>(entity =>
            {
                entity.HasOne(wp => wp.Member)
                    .WithMany(m => m.WorkoutPlans)
                    .HasForeignKey(wp => wp.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wp => wp.Trainer)
                    .WithMany(t => t.WorkoutPlans)
                    .HasForeignKey(wp => wp.TrainerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Exercise Configuration
            builder.Entity<Exercise>(entity =>
            {
                entity.HasOne(e => e.WorkoutPlan)
                    .WithMany(wp => wp.Exercises)
                    .HasForeignKey(e => e.WorkoutPlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Diet plan configuration
            builder.Entity<DietPlan>(entity =>
            {
                entity.Property(x => x.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Goal)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => new { x.MemberId, x.IsActive });

                entity.HasOne(x => x.Member)
                    .WithMany(m => m.DietPlans)
                    .HasForeignKey(x => x.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Trainer)
                    .WithMany(t => t.DietPlans)
                    .HasForeignKey(x => x.TrainerId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<DietMeal>(entity =>
            {
                entity.Property(x => x.MealType).HasMaxLength(60).IsRequired();
                entity.Property(x => x.FoodItems).HasMaxLength(1000).IsRequired();
                entity.HasIndex(x => new { x.DietPlanId, x.MealOrder });
                entity.HasOne(x => x.DietPlan).WithMany(p => p.Meals).HasForeignKey(x => x.DietPlanId).OnDelete(DeleteBehavior.Cascade);
            });

            // Progress and measurement configuration
            builder.Entity<ProgressRecord>(entity =>
            {
                entity.Property(x => x.WeightKg).HasPrecision(6, 2);
                entity.Property(x => x.BodyFatPercentage).HasPrecision(5, 2);
                entity.Property(x => x.MuscleMassKg).HasPrecision(6, 2);
                entity.Property(x => x.StrengthScore).HasPrecision(8, 2);
                entity.HasIndex(x => new { x.MemberId, x.RecordDate });
                entity.HasOne(x => x.Member).WithMany(m => m.ProgressRecords).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<BodyMeasurement>(entity =>
            {
                entity.Property(x => x.ChestCm).HasPrecision(6, 2);
                entity.Property(x => x.WaistCm).HasPrecision(6, 2);
                entity.Property(x => x.HipsCm).HasPrecision(6, 2);
                entity.Property(x => x.LeftArmCm).HasPrecision(6, 2);
                entity.Property(x => x.RightArmCm).HasPrecision(6, 2);
                entity.Property(x => x.LeftThighCm).HasPrecision(6, 2);
                entity.Property(x => x.RightThighCm).HasPrecision(6, 2);
                entity.HasIndex(x => new { x.MemberId, x.MeasurementDate });
                entity.HasOne(x => x.Member).WithMany(m => m.BodyMeasurements).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserNotification>(entity =>
            {
                entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
                entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Message).HasMaxLength(2000).IsRequired();
                entity.Property(x => x.Type).HasMaxLength(40).IsRequired();
                entity.Property(x => x.Url).HasMaxLength(500);
                entity.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
                entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // Attendance Configuration
            builder.Entity<Attendance>(entity =>
            {
                entity.HasIndex(a => a.MemberId)
                    .HasFilter("[CheckOutTime] IS NULL")
                    .IsUnique();

                entity.HasOne(a => a.Member)
                    .WithMany(m => m.Attendances)
                    .HasForeignKey(a => a.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
