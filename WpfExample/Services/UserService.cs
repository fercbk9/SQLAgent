using SQLAgent;
using SQLAgent.Criteria;
using System;
using System.Collections.Generic;
using System.Text;
using WpfExample.Models;
using WpfExample.Services.Interfaces;

namespace WpfExample.Services
{
    public class UserService : BaseService<User>,IUserService
    {
        public UserService()
        {
        }
    }
}
