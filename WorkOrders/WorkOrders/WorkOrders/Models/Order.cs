namespace WorkOrders.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            Parts = new HashSet<Part>();
        }

        public int OrderID { get; set; }

        public int ordernumber { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime repairdate { get; set; }

        public int vehicleyear { get; set; }

        [Required]
        [StringLength(50)]
        public string vehiclemake { get; set; }

        [Required]
        [StringLength(50)]
        public string vehiclemodel { get; set; }

        [Required]
        [StringLength(50)]
        public string vehicleliscenseplate { get; set; }

        public decimal vehiclemileage { get; set; }

        public decimal orderestimate { get; set; }

        [Required]
        [StringLength(50)]
        public string techname { get; set; }

        public decimal laborhours { get; set; }

        public decimal laborcost { get; set; }

        public decimal labortotals { get; set; }

        public int? CustomerID { get; set; }

        public decimal? GrandTotal { get; set; }

        public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Part> Parts { get; set; }
    }
}
