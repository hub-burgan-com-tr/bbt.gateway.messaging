using System.ComponentModel.DataAnnotations;

namespace bbt.gateway.common.Models.v2
{
    public class Phone
    {
        [Required(ErrorMessage = "This Field is Mandatory")]
        public int CountryCode { get; set; }
        [Required(ErrorMessage = "This Field is Mandatory")]
        public int Prefix { get; set; }
        [Required(ErrorMessage = "This Field is Mandatory")]
        public int Number { get; set; }

        public override string ToString()
        {
            return $"+{CountryCode}{Prefix}{Number.ToString().PadLeft(7, '0')}";
        }

    }
}
