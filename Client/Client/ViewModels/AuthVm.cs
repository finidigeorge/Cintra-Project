using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Commands;
using Client.Security;
using Common;
using Common.DtoMapping;
using Mapping;
using Shared;
using Shared.Dto;
using Shared.Interfaces;

namespace Client.ViewModels
{
    public enum AuthVmProperties
    {
        UserName,
        Status,
        AuthenticatedUser,
        IsAuthenticated,
        IsAdmin
    }

    public class AuthVm : BaseVm
    {
        private readonly IAuthController _authController;
        private readonly IUserRolesController _userRolesController;

        private string _username;
        private string _status;

        public IAsyncCommand LoginCommand { get; }

        public IAsyncCommand LogoutCommand { get; }


        public AuthVm(IAuthController authController, IUserRolesController userRolesController)
        {
            _authController = authController;
            _userRolesController = userRolesController;
            LoginCommand = new AsyncCommand<object>(async (x) =>
            {
                await Login(x);
            }, CanLogin);

            LogoutCommand = new AsyncCommand<object>(async (x) =>
            {
                await Logout(x);
            }, CanLogout);            
        }

        #region Properties
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(AuthVmProperties.UserName)); }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return
                        $"Signed in as {Thread.CurrentPrincipal.Identity.Name}. {(IsAdmin ? "You are member of the administrators group" : "You are regular user")}";

                return "Not authenticated!";
            }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(AuthVmProperties.Status)); }
        }

        public bool IsAuthenticated => Thread.CurrentPrincipal.Identity.IsAuthenticated;

        public bool IsAdmin => IsAuthenticated && Thread.CurrentPrincipal.IsInRole(nameof(UserRolesEnum.Administrator));

        #endregion

        private async Task Login(object parameter)
        {            
            string clearTextPassword = parameter.ToString();
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                //Validate credentials through the authentication service
                var token = await _authController.Login(new UserDtoUi {Login = Username, Password = clearTextPassword });
                AuthProvider.SetToken(token);
                var roles = (await _userRolesController.GetByUser(Username)).ToList<UserRoleDto, UserRoleDtoUi>();

                //Get the current principal object
                var principal = Thread.CurrentPrincipal as UserPrincipal;
                if (principal == null)
                    throw new ArgumentException("The application's default thread principal must be set on startup.");

                //Authenticate the user
                principal.Identity = new UserIdentity(Username, roles);

                //Update UI
                OnPropertyChanged(nameof(AuthVmProperties.AuthenticatedUser));
                OnPropertyChanged(nameof(AuthVmProperties.IsAuthenticated));
                OnPropertyChanged(nameof(AuthVmProperties.IsAdmin));
            }            
            catch (Exception ex)
            {
                Status = $"ERROR: {ex.Message}";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated;
        }

        private async Task Logout(object parameter)
        {
            await Task.Run(() =>
            {
                var principal = Thread.CurrentPrincipal as UserPrincipal;
                if (principal != null)
                {
                    principal.Identity = new AnonymousIdentity();

                    //update UI
                    OnPropertyChanged(nameof(AuthVmProperties.AuthenticatedUser));
                    OnPropertyChanged(nameof(AuthVmProperties.IsAuthenticated));

                    Status = string.Empty;
                }
            });
        }

        private bool CanLogout(object parameter)
        {
            return IsAuthenticated;
        }
        
    }
}

