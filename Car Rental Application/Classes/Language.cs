﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Rental_Application.Classes
{
    public class Language
    {
        Dictionary<string, string> dictionary;

        public Language(Dictionary<string, string> dictionary)
        {
            this.dictionary = dictionary;
        }

        /// <summary>
        /// If there's a translation available for the provided text, returns the translation. Returns the provided text otherwise.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Translate(string text)
        {
            if (dictionary.ContainsKey(text) && dictionary[text] != null)
            {
                return dictionary[text];
            }

            return text;
        }
    }
}
