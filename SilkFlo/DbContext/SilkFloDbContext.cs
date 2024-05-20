using Microsoft.EntityFrameworkCore;
using SilkFlo.Models;
using System.Collections.Generic;
using System.Transactions;
using System.Xml.Linq;

namespace SilkFlo
{

    public class SilkFloDbContext : DbContext
    {
        public SilkFloDbContext(DbContextOptions<SilkFloDbContext> options) : base(options)
        {
        }
        public DbSet<Element> Elements { get; set; }
        public DbSet<FormBuilder> FormBuilder { get; set; }

    }
}
