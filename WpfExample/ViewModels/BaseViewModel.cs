using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WpfExample.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject
    {
        #region Constructor
        public BaseViewModel()
        {
            InitCommands();
            InitVariables();
            InitMethods();
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Metodo para Cargar los Commandos del ViewModels.
        /// </summary>
        protected virtual void InitCommands()
        {

        }

        protected virtual void InitMethods()
        {

        }

        protected virtual void InitVariables()
        {

        }

        #endregion
    }
}
