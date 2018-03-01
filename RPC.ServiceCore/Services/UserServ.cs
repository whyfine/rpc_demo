using Base.ServiceCore.DbContexts;
using Base.ServiceCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Base.ServiceCore.Services
{
    /// <summary>
    /// public会发布为服务，不需要发布为服务则private
    /// </summary>
    public class MemberServ
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="memberid"></param>
        /// <returns></returns>
        public User GetUser(string mobile)
        {
            using (var context = new UserContext())
            {
                var r = context.Users.FirstOrDefault(v => v.Mobile.Equals(mobile));
                if (r == null)
                    return null;
                return r;
            }
        }
    }
}
