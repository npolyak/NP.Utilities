using System;
using System.Collections.Generic;

namespace NP.Utilities
{
    public class DoubleParamMap<OuterKeyType, InnerKeyType, InfoType>
    {
        Dictionary<OuterKeyType, Dictionary<InnerKeyType, InfoType>> _dictionary =
            new Dictionary<OuterKeyType, Dictionary<InnerKeyType, InfoType>>();

        public void AddKeyValue
        (
            OuterKeyType outerKey,
            InnerKeyType innerKey,
            InfoType infoData
        )
        {
            Dictionary<InnerKeyType, InfoType> innerDictionary;

            if (!_dictionary.TryGetValue(outerKey, out innerDictionary))
            {
                innerDictionary = new Dictionary<InnerKeyType, InfoType>();

                _dictionary[outerKey] = innerDictionary;
            }

            innerDictionary[innerKey] = infoData;
        }

        public bool TryGetValue
        (
            OuterKeyType outerKey,
            InnerKeyType innerKey,
            out InfoType infoData
        )
        {
            infoData = default(InfoType);
            Dictionary<InnerKeyType, InfoType> innerDictionary;

            if (
                (!_dictionary.TryGetValue(outerKey, out innerDictionary)) ||
                (innerDictionary == null)
               )
            {
                return false;
            }

            return
                innerDictionary.TryGetValue(innerKey, out infoData);
        }

        public bool ContainsKeys
        (
            OuterKeyType outerKey,
            InnerKeyType innerKey
        )
        {
            InfoType infoData;

            return this.TryGetValue(outerKey, innerKey, out infoData);
        }

        public InfoType GetValue
        (
            OuterKeyType outerKey, 
            InnerKeyType innerKey
        )
        {
            InfoType result = default(InfoType);
            if (!TryGetValue(outerKey, innerKey, out result))
                throw new Exception("There is no value in dictionary");

            return result;
        }
    }
}
