using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentStorage.HelpClasses
{
    class Calculate
    {
        public bool ContainsAllInStrictOrder(string txt, string searchTxt, bool caseSensitive)
        {
            txt = IsCaseSensitive(txt, caseSensitive);
            searchTxt = IsCaseSensitive(searchTxt, caseSensitive);
            return txt.Contains(searchTxt);
        }
        public bool ContainsAllInAnyOrder(string txt, string searchTxt, bool caseSensitive)
        {
            txt = IsCaseSensitive(txt, caseSensitive);
            searchTxt = IsCaseSensitive(searchTxt, caseSensitive);
            List<string> lst = searchTxt.Split(' ').ToList();
            for (int i = 0; i < lst.Count; i++)
            {
                if (!txt.Contains(lst[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public bool ContainsAtLeastOne(string txt, string searchTxt, bool caseSensitive)
        {
            txt = IsCaseSensitive(txt, caseSensitive);
            searchTxt = IsCaseSensitive(searchTxt, caseSensitive);
            List<string> lst = searchTxt.Split(' ').ToList();
            for (int i = 0; i < lst.Count; i++)
            {
                if (txt.Contains(lst[i]))
                {
                    return true;
                }
            }
            return false;
        }
        private string IsCaseSensitive(string str, bool caseSensitive)
        {
            str = string.IsNullOrEmpty(str) ? string.Empty : str;
            if (caseSensitive)
            {
                return str;
            }
            return str.ToLower();
        }
        public bool IsEqualByteArrays(byte[] bArr1, byte[] bArr2)
        {
            if (bArr1 == null && bArr2 == null)
            {
                return true;
            }
            if (bArr1 != null && bArr2 != null)
            {
                return bArr1.SequenceEqual(bArr2);
            }
            return false;
        }
    }
}
