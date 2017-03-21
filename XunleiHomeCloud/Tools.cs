using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace XunleiHomeCloud
{
    public class Tools
    {
        /// <summary>
        /// Return TimeStamp as string
        /// </summary>
        /// <returns>TimeStamp</returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Return TimeStamp as long use DateTime
        /// </summary>
        /// <param name="dateTime">DataTime</param>
        /// <returns>TimeStamp</returns>
        public static long GetTimeStamp(DateTime dateTime)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }

        /// <summary>
        /// Return Milliseconds TimeStamp as long use DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetLongTimeStamp(DateTime dateTime)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalMilliseconds);
        }

        /// <summary>
        /// Encode contents to URL format
        /// </summary>
        /// <param name="content"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string URLEncoding(string content, Encoding e)
        {
            return System.Web.HttpUtility.UrlEncode(content, e);
        }

        /// <summary>
        /// Decode URL format contents to normal string
        /// </summary>
        /// <param name="content"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string URLDecoding(string content, Encoding e)
        {
            return System.Web.HttpUtility.UrlDecode(content, e);
        }
    }
}