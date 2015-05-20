using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace EnzoMailEngine
{
    public class EnzoMailEngine
    {
        private SmtpClient _smtpClient { get; set; }

        public EnzoMailEngine _mailEngine { get; set; }

        public EnzoMailEngine(string sectionName = "default)
        {
            SmtpClient = new SmtpClient()

        }

    }
}
