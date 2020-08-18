using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebHotel.Models
{
    public class Customer
    {
        
        [Key, Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; }
        [Required]
        [MinLength(2), MaxLength(20)]
        [RegularExpression(@"[A-Z][a-z'-]{1,19}", ErrorMessage = "First letter to be capital")]
        public string FamilyName { get; set; }
        [Required]
        [MinLength(2), MaxLength(20)]
        [RegularExpression(@"[A-Z][a-z'-]{1,19}", ErrorMessage = "First letter to be capital")]
        public string GivenName { get; set; }
        [Required]
        [StringLength(4)]
        [RegularExpression(@"^[0-8]{1}[0-9]{3}$", ErrorMessage = "Enter a valid postal code")]
        public string Postalcode { get; set; }
        public ICollection<Booking> TheBookings { get; set; }
    }
}
