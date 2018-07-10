using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWithAngular.Services
{
    public interface IMailService
    {
        void SendMessage(string to, string subject, string body);
    }
}
