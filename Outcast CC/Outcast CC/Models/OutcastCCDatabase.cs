namespace Outcast_CC.Models
{
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;

  public partial class OutcastCCDatabase : IdentityDbContext<AppUser>
  {
    public OutcastCCDatabase()
        : base("name=OutcastCCDatabase")
    {
    }

    static OutcastCCDatabase()
      {
      Database.SetInitializer<OutcastCCDatabase>(new AppDbInit());
      }

    public static OutcastCCDatabase Create()
    {
      return new OutcastCCDatabase();
    }

    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<UserEvents> UserEvents { get; set; }
    public virtual DbSet<Subscriber> Subscribers { get; set; }

    //protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //{
    //  modelBuilder.Entity<Blog>()
    //      .Property(e => e.PdfFileType)
    //      .IsUnicode(false);

    //  modelBuilder.Entity<Blog>()
    //      .Property(e => e.PdfFileName)
    //      .IsUnicode(false);

    //  modelBuilder.Entity<Event>()
    //      .Property(e => e.PdfFileType)
    //      .IsUnicode(false);

    //  modelBuilder.Entity<Event>()
    //      .Property(e => e.PdfFileName)
    //      .IsUnicode(false);

    //  modelBuilder.Entity<Member>()
    //      .Property(e => e.ProfileImageType)
    //      .IsUnicode(false);

    //  modelBuilder.Entity<Member>()
    //      .Property(e => e.ProfileImageName)
    //      .IsUnicode(false);
    //}
    public class AppDbInit : CreateDatabaseIfNotExists<OutcastCCDatabase>
    {

    }

  }
}
