namespace NP.Utilities.BasicInterfaces
{
    public interface IErrorContainer
    {
        string ErrorMsg { get; }

        bool HasError => ErrorMsg != null;
    }

    public interface IErrorSetter
    {
        protected void SetErrorMsg(string errorMsg);
    }
}
