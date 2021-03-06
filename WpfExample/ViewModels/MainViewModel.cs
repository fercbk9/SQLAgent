﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows.Forms;
using WpfExample.Services.Interfaces;

namespace WpfExample.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private ReactiveCommand<object,object> _command;
        public ReactiveCommand<object, object> Command
        {
            get => _command;
            set => this.RaiseAndSetIfChanged(ref _command, value);
        }
        private List<Models.UserGroup> userGroups;
        public List<Models.UserGroup> UserGroups
        {
            get => userGroups;
            set => this.RaiseAndSetIfChanged(ref userGroups, value);
        }

        public MainViewModel(IUserService userService) : base()
        {
            _userService = userService;
            //var Users = new SQLAgent.SQLManager().SelectDeep<Models.User>("Select * from [User]");
            UserGroups = new SQLAgent.Criteria.CriteriaSet<Models.UserGroup>()
                .GetEntitiesDeep().ToList();
            var x = UserGroups.FirstOrDefault();
            var users = _userService.SelectAll();
            //new SQLAgent.SQLManager().Insert(x.User);
            /*var result = new SQLAgent.DataAccessObject.BaseDAO<Models.User>(Models.User.TableName).Delete(usergroup.User);
            var result2 = new SQLAgent.DataAccessObject.BaseDAO<Models.UserGroup>(Models.UserGroup.TableName).Delete(usergroup);*/

        }

        protected override void InitCommands()
        {
            base.InitCommands();
            Command = ReactiveCommand.Create<object,object>(ExecuteCommand);
        }


        private object ExecuteCommand(object obj)
        {
            Console.WriteLine(obj);
            return obj;
        }

        
    }
}
