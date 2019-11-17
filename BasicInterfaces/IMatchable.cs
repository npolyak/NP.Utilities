namespace NP.Utilities.BasicInterfaces
{
    public interface IMatchable
    {
        bool IsMatching { get; }

        public void CheckMatching(string str);
    }
}
