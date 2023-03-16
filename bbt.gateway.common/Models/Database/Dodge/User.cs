using System;
using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public ICollection<UserDevice> Devices { get; set; }
    }
}
