namespace NP.Utilities.BasicInterfaces
{
    public interface IMatchable
    {
        bool IsMatching { get; }

        void CheckMatching(string str);
    }
}
