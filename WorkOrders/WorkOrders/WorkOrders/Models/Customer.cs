namespace WorkOrders.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public int CustomerID { get; set; }

        [StringLength(50)]
        public string customername { get; set; }

        [StringLength(50)]
        public string address { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        public decimal? subtotal { get; set; }

        public decimal? taxamount { get; set; }

        public decimal? grandtotal { get; set; }
        
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
