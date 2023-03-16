using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Helpers
{
    public interface IFakeSmtpHelper
    {
        public void SendFakeMail(string fromMail, string fromName, string to, string subject, string content, List<Attachment>? attachments);
    }
}
