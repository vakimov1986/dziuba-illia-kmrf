using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;

using System.Windows;
using System.Windows.Input;

using System.Threading.Tasks;
using CurConvApp.Models; // !!!

namespace CurConvApp.ViewModels
{
    public class RegistrationViewModel
    {
        public User User { get; set; }

        public ICommand SubmitCommand { get; }

        public RegistrationViewModel() 
        {
            User = new User()               // ініціалізація
            {
                Name = "Illia",
                Surname = "Dziuba"
            };

            SubmitCommand = new RelayCommand(() => { MessageBox.Show("This is a test app"); });
        }
    }
}
