using System;
using System.IO;
using System.Text;
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
                    StringBuilder SB = new StringBuilder();
                    while (!SR.EndOfStream)
                    {
                        string[] param = SR.ReadLine().Split('$');
                        SB.Append(string.Format("{0}={1}; ", param[1], param[2]));
                    }
                    _Cookies = SB.ToString();
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
