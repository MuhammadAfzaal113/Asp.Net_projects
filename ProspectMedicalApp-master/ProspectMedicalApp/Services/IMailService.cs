using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProspectMedicalApp.Services
{
    public interface IMailService
    {
        void SendMessage(string to, string subject, string body);
    }
}
