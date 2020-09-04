namespace WorkOrders.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Part
    {
        [Key]
        public int Partsid { get; set; }

        public int? partsquantity { get; set; }

        public int? partsnumber { get; set; }

        [StringLength(50)]
        public string partsname { get; set; }

        public decimal? partcost { get; set; }

        public int? OrderID { get; set; }

        public virtual Order Order { get; set; }
    }
}
