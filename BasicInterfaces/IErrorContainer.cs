namespace NP.Utilities.BasicInterfaces
{
    public interface IErrorContainer
    {
        string ErrorMsg { get; }

        bool HasError { get; }
    }
}
