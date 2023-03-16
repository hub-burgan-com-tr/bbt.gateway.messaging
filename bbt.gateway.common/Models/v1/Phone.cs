using Microsoft.EntityFrameworkCore;

namespace bbt.gateway.common.Models
{
    [Index("CountryCode","Prefix","Number",IsUnique = false)]
    public class Phone
    {
        public int CountryCode { get; set; }
        public int Prefix { get; set; }
        public int Number { get; set; }

        public Phone()
        {

        }

        public Phone(string phone)
        {
            CountryCode = Convert.ToInt32(phone.Substring(0, 2));
            Prefix = Convert.ToInt32(phone.Substring(2, 3));
            Number = Convert.ToInt32(phone.Substring(5, 7));
        }

        public override string ToString()
        {
            return $"+{CountryCode}{Prefix}{Number.ToString().PadLeft(7, '0')}";
        }

        public string Concatenate()
        {
            if (CountryCode == 90)
            {
                return $"{CountryCode}{Prefix}{Number.ToString().PadLeft(7, '0')}";
            }
            else
            {
                return $"{CountryCode}{Prefix}{Number}";
            }
        }
    }
}
