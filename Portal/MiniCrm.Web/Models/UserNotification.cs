using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniCrm.Web.Models
{
    public class UserNotification
    {
        public List<int> Count { get; set; }
        public List<string> Describe { get; set; }

        public UserNotification(List<int> cou, List<string> des)
        {
            Count = cou;
            Describe=des;
        }
        public UserNotification()
        {

        }
    }
}