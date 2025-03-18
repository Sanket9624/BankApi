using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Entities
{
    public class OtpVerifications
    {
        [Key]
        public int OtpId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Otp { get; set; }

        public DateTime OtpExpiry { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; }
    }

}
