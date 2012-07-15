using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nokatak.Models
{
    public class Nokta
    {
        public long ID { get; set; }
        public String Body { get; set; }
        public long userID { get; set; }
        public virtual ICollection<Vote> votes { get; set; }
    }

}