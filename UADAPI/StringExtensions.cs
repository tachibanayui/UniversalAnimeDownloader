using System;
using System.Collections.Generic;
using System.Text;

namespace UADAPI
{
    public static class StringExtensions
    {
        public static string RemoveInvalidChar(this string originalString) => originalString.Replace('/', ' ').Replace('\\', ' ').Replace(':', ' ').Replace('*', ' ').Replace('?', ' ').Replace('"', ' ').Replace('>', ' ').Replace('<', ' ').Replace('|', ' ');
    }
}
