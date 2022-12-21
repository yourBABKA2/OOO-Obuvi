namespace OOO_Obuvi.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [Key]
        [StringLength(100)]
        public string ProductArticle { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public int UnitId { get; set; }

        public double ProductCost { get; set; }

        public double ProductMaxSale { get; set; }

        public int ManufactureId { get; set; }

        public int ProviderId { get; set; }

        public int CategoryId { get; set; }

        public double? ProductSale { get; set; }

        public int ProductCount { get; set; }

        public string ProductDescription { get; set; }

        [StringLength(100)]
        public string ProductPhoto { get; set; }

        public virtual Category Category { get; set; }

        public virtual Manufacture Manufacture { get; set; }

        public virtual Provider Provider { get; set; }

        public virtual Unit Unit { get; set; }
    }
}
