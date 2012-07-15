using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Nokatak.Models
{
    public class Vote
    {
        public long ID { get; set; }
        public long NoktaID { get; set; }
        public virtual Nokta nokta { get; set; }
        public Boolean up { get; set; }
        public long userID { get; set; }
    }
}