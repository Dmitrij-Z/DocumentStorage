using System.Collections.Generic;
using System.Linq;

namespace DocumentStorage.HelpClasses
{
    class Calculate
    {
        /// <summary>
        /// Фильтрация при условии строгого совпадения текста поиска
        /// </summary>
        /// <param name="txt">значение переменной документа</param>
        /// <param name="searchTxt">искомый текст</param>
        /// <param name="caseSensitive">должен ли учитываться регистр</param>
        public bool ContainsAllInStrictOrder(string txt, string searchTxt, bool caseSensitive)
        {
            txt = IsCaseSensitive(txt, caseSensitive);
            searchTxt = IsCaseSensitive(searchTxt, caseSensitive);
            return txt.Contains(searchTxt);
        }

        /// <summary>
        /// Фильтрация при условии совпадения всех слов поска в произвольном порядке
        /// </summary>
        /// <param name="txt">значение переменной документа</param>
        /// <param name="searchTxt">искомый текст</param>
        /// <param name="caseSensitive">должен ли учитываться регистр</param>
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

        /// <summary>
        /// Фильтрация при условии совпадения хотя бы одного слова из поиска
        /// </summary>
        /// <param name="txt">значение переменной документа</param>
        /// <param name="searchTxt">искомый текст</param>
        /// <param name="caseSensitive">должен ли учитываться регистр</param>
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

        /// <summary>
        /// Преобразование регистра текста согласно условию
        /// </summary>
        /// <param name="str">текст</param>
        /// <param name="caseSensitive">должен ли учитываться регистр</param>
        /// <returns>Если регист не учитывается, возвращает текст без изменений. Иначе переводит в нижний регистр</returns>
        private string IsCaseSensitive(string str, bool caseSensitive)
        {
            str = string.IsNullOrEmpty(str) ? string.Empty : str;
            if (caseSensitive)
            {
                return str;
            }
            return str.ToLower();
        }

        /// <summary>
        /// Сравнение двух массивов байт
        /// </summary>
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
