using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XunleiHomeCloud
{
    /// <summary>
    /// MD5 Tools
    /// </summary>
    public class MD5Helper
    {
        private static MD5 md5 = MD5.Create();

        /// <summary> 
        /// Generate a MD5 string
        /// </summary> 
        /// <param name="strSource">Raw string</param> 
        /// <returns>MD5 string</returns> 
        public static string HashString(string strSource)
        {
            return HashString(Encoding.UTF8, strSource);
        }

        /// <summary> 
        /// Generate a MD5 string use custom encoding 
        /// </summary> 
        /// <param name="encode">Encoding</param> 
        /// <param name="strSource">Raw string</param> 
        /// <returns>MD5 string</returns> 
        public static string HashString(Encoding encode, string strSource)
        {
            return HashString(encode.GetBytes(strSource));
        }

        /// <summary> 
        /// Generate a MD5 string
        /// </summary> 
        /// <param name="bySource">byte[]</param> 
        /// <returns>MD5 string</returns> 
        public static string HashString(byte[] bySource)
        {
            byte[] source = md5.ComputeHash(bySource);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                sBuilder.Append(source[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
