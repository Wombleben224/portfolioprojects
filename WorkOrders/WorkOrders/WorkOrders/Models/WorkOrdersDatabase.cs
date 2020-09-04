namespace WorkOrders.Models
{
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class WorkOrdersDatabase : DbContext
  {
    public WorkOrdersDatabase()
        : base("name=WorkOrdersDatabase1")
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Part> Parts { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Customer>()
          .Property(e => e.subtotal)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Customer>()
          .Property(e => e.taxamount)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Customer>()
          .Property(e => e.grandtotal)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Order>()
          .Property(e => e.ordernumber);

      modelBuilder.Entity<Order>()
          .Property(e => e.vehicleyear);

      modelBuilder.Entity<Order>()
          .Property(e => e.vehiclemileage)
          .HasPrecision(8, 1);

      modelBuilder.Entity<Order>()
          .Property(e => e.orderestimate)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Order>()
          .Property(e => e.laborhours)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Order>()
          .Property(e => e.laborcost)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Order>()
          .Property(e => e.labortotals)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Order>()
          .Property(e => e.GrandTotal)
          .HasPrecision(9, 2);

      modelBuilder.Entity<Part>()
          .Property(e => e.partcost)
          .HasPrecision(9, 2);
    }
  }
}
