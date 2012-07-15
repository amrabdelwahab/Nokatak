using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace Nokatak.Models
{
    public class NokatDB : DbContext
    {
        public DbSet<Nokta> Noktas { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}