using Hprose.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.IServices
{
    public interface IUserServ
    {
        [SimpleMode(true)]
        int EditUserEmail(string mobile, string email);
    }
}
