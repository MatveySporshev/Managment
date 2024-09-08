using ProjectManagementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managment.Models;
using Newtonsoft.Json;

namespace ProjectManagementSystem
{
    public interface IUserService
    {
        void RegisterUser(User user);
    }
}
