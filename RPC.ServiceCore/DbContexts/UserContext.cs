using Base.ServiceCore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.ServiceCore.DbContexts
{
    public class UserContext : DbContext
    {
        //传入connection string name
        public UserContext()
            : base("UserContext")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }
        //映射数据库中Condition表的数据集
        public DbSet<User> Users { get; set; }

        //Code First - MySQL - error can't find table
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
