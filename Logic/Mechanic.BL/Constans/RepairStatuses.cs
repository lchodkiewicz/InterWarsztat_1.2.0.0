using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mechanic.BL.Constans
{
    public class DictionaryStatusesRepair
    {
        private Dictionary<int, string> dictionary = new Dictionary<int, string>();
        public const int PrzyjętyDoNaprawy = 1;
        public const int WTrakcieNaprawy = 2;
        public const int Naprawiony = 3;
        public const int WOczekiwaniuNaDecyzjeKlienta = 4;
        public const int WOczekiwaniuNaCzesci = 5;

        public DictionaryStatusesRepair()
        {
            dictionary.Add(1, "Przyjęty do naprawy");
            dictionary.Add(2, "W trakcie naprawy");
            dictionary.Add(3, "Naprawiony");
            dictionary.Add(4, "W oczekiwaniu na decyzje klienta");
            dictionary.Add(5, "W oczekiwaniu na częsci");

        }

        public bool HasKey(int key)
        {
            return dictionary.ContainsKey(key);
        }

        public string Value(int key)
        {
            string value;
            dictionary.TryGetValue(key, out value);
            return value;
        }
    }
}
