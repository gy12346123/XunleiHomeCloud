using System;

namespace XunleiHomeCloud
{
    public class CommonException
    {
        /// <summary>
        /// Match the error code which xunlei returned and throw a exception
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="from">From which method or cs</param>
        public static void ErrorCode(int code, string from)
        {
            switch (code)
            {
                case 103:
                    throw new XunleiActivityKeyNotFoundException(string.Format("{0}: Activity key not found.", from));
                case 104:
                    throw new XunleiActivityKeyInvalidException(string.Format("{0}: Activity key invalid.", from));
                case 202:
                    throw new XunleiRepeatTaskException(string.Format("{0}: Repeat task, skip.", from));
                case 403:
                    throw new XunleiUserSessionNoneException(string.Format("{0}: User id or Session id is none.", from));
                case 420:
                    throw new XunleiConnectionException(string.Format("{0}: Device client connection error.", from));
                case 1004:
                    throw new XunleiUserNotLoginException(string.Format("{0}: User not login.", from));
                default:
                    throw new XunleiErrorCodeNotHandleException(string.Format("{0}: Error code not handle.", from));
            }
        }
    }

    #region CustomException
    public class XunleiNoDeviceException : ApplicationException
    {
        public XunleiNoDeviceException() { }
        public XunleiNoDeviceException(string message) : base(message) { }
        public XunleiNoDeviceException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiNoCookieException : ApplicationException
    {
        public XunleiNoCookieException () { }
        public XunleiNoCookieException(string message) : base(message) { }
        public XunleiNoCookieException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiRepeatTaskException : ApplicationException
    {
        public XunleiRepeatTaskException() { }
        public XunleiRepeatTaskException(string message) : base(message) { }
        public XunleiRepeatTaskException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiDevicePathException : ApplicationException
    {
        public XunleiDevicePathException() { }
        public XunleiDevicePathException(string message) : base(message) { }
        public XunleiDevicePathException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiUserNotLoginException : ApplicationException
    {
        public XunleiUserNotLoginException() { }
        public XunleiUserNotLoginException(string message) : base(message) { }
        public XunleiUserNotLoginException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiUserSessionNoneException : ApplicationException
    {
        public XunleiUserSessionNoneException() { }
        public XunleiUserSessionNoneException(string message) : base(message) { }
        public XunleiUserSessionNoneException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiTaskListException : ApplicationException
    {
        public XunleiTaskListException() { }
        public XunleiTaskListException(string message) : base(message) { }
        public XunleiTaskListException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiConnectionException : ApplicationException
    {
        public XunleiConnectionException() { }
        public XunleiConnectionException(string message) : base(message) { }
        public XunleiConnectionException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiSettingException : ApplicationException
    {
        public XunleiSettingException() { }
        public XunleiSettingException(string message) : base(message) { }
        public XunleiSettingException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiLoginDeviceIdException : ApplicationException
    {
        public XunleiLoginDeviceIdException() { }
        public XunleiLoginDeviceIdException(string message) : base(message) { }
        public XunleiLoginDeviceIdException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiCookiesParamException : ApplicationException
    {
        public XunleiCookiesParamException() { }
        public XunleiCookiesParamException(string message) : base(message) { }
        public XunleiCookiesParamException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiDeviceSpaceException : ApplicationException
    {
        public XunleiDeviceSpaceException() { }
        public XunleiDeviceSpaceException(string message) : base(message) { }
        public XunleiDeviceSpaceException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiDeviceNoSavePathException : ApplicationException
    {
        public XunleiDeviceNoSavePathException() { }
        public XunleiDeviceNoSavePathException(string message) : base(message) { }
        public XunleiDeviceNoSavePathException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiActivityKeyNotFoundException : ApplicationException
    {
        public XunleiActivityKeyNotFoundException() { }
        public XunleiActivityKeyNotFoundException(string message) : base(message) { }
        public XunleiActivityKeyNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiActivityKeyInvalidException : ApplicationException
    {
        public XunleiActivityKeyInvalidException() { }
        public XunleiActivityKeyInvalidException(string message) : base(message) { }
        public XunleiActivityKeyInvalidException(string message, Exception inner) : base(message, inner) { }
    }

    public class XunleiErrorCodeNotHandleException : ApplicationException
    {
        public XunleiErrorCodeNotHandleException() { }
        public XunleiErrorCodeNotHandleException(string message) : base(message) { }
        public XunleiErrorCodeNotHandleException(string message, Exception inner) : base(message, inner) { }
    }
    #endregion
}
