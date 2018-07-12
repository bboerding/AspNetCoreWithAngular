using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWithAngular.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }

        //Hier werden alle relevanten Product Infos für diese View hinzugefügt
        //Wenn diese Felder die Prefix "Product" haben, werden diese Felder über den Mapper automatisch gefüllt
        //  weil jedes OrderItem eine Referenz auf das zugehörige Product hat
        [Required]
        public int ProductId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSize { get; set; }
        public string ProductTitle { get; set; }
        public string ProductArtist { get; set; }
        public string ProductArtId { get; set; }
    }
}
