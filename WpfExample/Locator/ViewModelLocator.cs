using Autofac;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using WpfExample.Models;
using WpfExample.Services;
using WpfExample.Services.Interfaces;
using WpfExample.ViewModels;

namespace WpfExample.Locator
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IUserService,UserService>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<TestViewModel>();
        }
        public MainViewModel Main
        {
            get => ServiceLocator.Current.GetInstance<MainViewModel>();
        }
        public TestViewModel Test
        {
            get => ServiceLocator.Current.GetInstance<TestViewModel>();
        }
    }
}
