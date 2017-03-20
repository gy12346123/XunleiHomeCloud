using System;
using System.IO;
using System.Threading.Tasks;

namespace XunleiHomeCloud
{
    public class Cookie
    {
        private static string _Cookies;

        /// <summary>
        /// Xunlei cookies
        /// </summary>
        public static string Cookies {
            get
            {
                return _Cookies;
            }
        }


        /// <summary>
        /// Load cookies from local file
        /// </summary>
        /// <param name="path">Local path</param>
        /// <param name="overwrite">Need overwrite</param>
        /// <returns>True:succeed, flase:failed</returns>
        public static bool LoadCookie(string path, bool overwrite = false)
        {
            if (Cookies == null || Cookies.Equals("") || overwrite)
            {
                using (StreamReader SR = new StreamReader(new FileStream(path, FileMode.Open)))
                {
                    _Cookies = SR.ReadToEnd();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Load cookies from local file
        /// </summary>
        /// <param name="path">Local path</param>
        /// <param name="overwrite">Need overwrite</param>
        /// <returns>Task<bool>True:succeed, flase:failed</returns>
        public static Task<bool> LoadCookieAsync(string path, bool overwrite = false)
        {
            return Task.Factory.StartNew(()=> {
                return LoadCookie(path, overwrite);
            });
        }

        /// <summary>
        /// Check cookie
        /// </summary>
        /// <returns>True:cookie existed, false:cookie not found</returns>
        public static bool CheckCookie()
        {
            if (Cookies == null || Cookies.Equals(""))
            {
                return false;
            }
            return true;
        }

    }
}
