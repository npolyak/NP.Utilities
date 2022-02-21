using System.ComponentModel;

namespace NP.Utilities.BasicServices
{
    public interface IAuthenticationService : INotifyPropertyChanged
    {
        string? CurrentUserName { get; }

        bool IsAuthenticated { get; }

        bool Authenticate(string userName, string password);
    }
}
