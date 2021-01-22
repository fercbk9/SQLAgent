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
            UserGroups = new SQLAgent.SQLManager().SelectDeep<Models.UserGroup>("Select * from [UserGroup]").ToList();
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
