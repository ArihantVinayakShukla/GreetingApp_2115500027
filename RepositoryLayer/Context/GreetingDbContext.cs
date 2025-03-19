﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class GreetingDbContext : DbContext
    {
        public GreetingDbContext(DbContextOptions<GreetingDbContext> options) : base(options)
        {

        }

        public DbSet<GreetingEntity> Greetings { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreetingEntity>()
                .HasOne(g => g.User)
                .WithMany(u => u.Greetings)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
