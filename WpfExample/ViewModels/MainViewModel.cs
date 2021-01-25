using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows.Forms;

namespace WpfExample.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
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

        public MainViewModel() : base()
        {
            //var Users = new SQLAgent.SQLManager().SelectDeep<Models.User>("Select * from [User]");
            var usergroup = new SQLAgent.Criteria.CriteriaSet<Models.UserGroup>()
                .Compare(Models.UserGroup.Properties.CodUser,SQLAgent.Criteria.ComparisonOperators.Equals,9)
                .GetEntitiesDeep()
                .FirstOrDefault();
            var result = new SQLAgent.SQLManager().Delete(usergroup.User);
            var result2 = new SQLAgent.SQLManager().Delete(usergroup);
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
