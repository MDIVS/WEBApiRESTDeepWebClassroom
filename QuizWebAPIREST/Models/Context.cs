using System;
using Microsoft.EntityFrameworkCore;

namespace QuizWebAPIREST.Models {
    public class Context : DbContext {
        public DbSet<GameData> GameDatas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PostQuiz> QuizPosts { get; set; }
        public DbSet<Room> Rooms { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseNpgsql(@"Host=postgres.evilsummit.com.br;Port=5432;Username=usr077;Password=W9rKMUvPJE;Database=db077;");
            }
        }

        //OnModelCreating não obrigatório
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            Console.WriteLine("Modelo Criado");
        }
    }
}