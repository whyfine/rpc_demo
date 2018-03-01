using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.ServiceCore.Models
{
    //Table实体映射数据库表名
    [Table("user")]
    public class User
    {
  			public long Id {get;set;}
  			public string UserName {get;set;}
  			public string Password {get;set;}
  			public string Mobile {get;set;}
    }
}


