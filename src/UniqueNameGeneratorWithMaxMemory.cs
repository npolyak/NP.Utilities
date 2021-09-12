using System.Collections.Generic;

namespace NP.Utilities
{
    public class UniqueNameGeneratorWithMaxMemory
    {
        private int _max = 0;

        public string GetUniqueName
        (
            IEnumerable<string> names,
            string prefix)
        {
            string uniqueName;

            (uniqueName, _max) = names.GetUniqueNameAndMaxNumber(prefix, _max);

            return uniqueName;
        }
    }
}
