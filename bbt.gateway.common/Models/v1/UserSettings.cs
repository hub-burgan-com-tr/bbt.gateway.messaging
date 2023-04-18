using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.v1
{
    public class UserSettings
    {
        public UserSettingsModel[] Users { get; set; }
    }
    public class UserSettingsModel
    {
        public string UserName { get; set; }
        public Dictionary<string,string> Pages { get; set; }
    }
}
