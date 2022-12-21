namespace OOO_Obuvi.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserFullName { get; set; }

        [Required]
        [StringLength(100)]
        public string UserLogin { get; set; }

        [Required]
        [StringLength(100)]
        public string UserPassword { get; set; }

        public virtual Role Role { get; set; }
    }
}
