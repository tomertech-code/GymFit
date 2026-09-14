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

                entity.HasOne(p => p.Member)
                    .WithMany(m => m.Payments)
                    .HasForeignKey(p => p.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            // Attendance Configuration
            builder.Entity<Attendance>(entity =>
            {
                entity.HasOne(a => a.Member)
                    .WithMany(m => m.Attendances)
                    .HasForeignKey(a => a.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
