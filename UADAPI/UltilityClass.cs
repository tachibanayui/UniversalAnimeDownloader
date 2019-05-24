using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace UADAPI
{
    internal class UltilityClass
    {
        public static int FindObservableCollectionIndex<T>(ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (predicate(collection[i]))
                    return i;
            }

            return -1;
        }

    }
}
