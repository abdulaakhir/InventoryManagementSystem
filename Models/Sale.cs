using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; }

        
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Required]
        
        public int TotalAmount { get; set; }


    }
}
