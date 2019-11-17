namespace NP.Utilities.BasicInterfaces
{
    public interface IStrSaveable
    { 
        bool CanSave { get; }

        // save to string
        string Save();
    }
}
