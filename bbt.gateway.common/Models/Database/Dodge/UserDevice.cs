
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace bbt.gateway.common.Models
{
    public class UserDevice
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
    }
}
