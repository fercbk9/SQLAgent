using SQLAgent.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var connectionstring = ConfigurationManager.ConnectionStrings[1].ConnectionString;
            SQLContext.sqlSetting = new SQLAgent.SQLSetting(connectionstring);
        }
    }
}
