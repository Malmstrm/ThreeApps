namespace Shared.Interfaces;

public interface INavigationService
{
    string NavigateWithArrows(string title, params string[] options);
}
