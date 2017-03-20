using System;

namespace XunleiHomeCloud
{
    public class CommonException
    {
        public static void ErrorCode(int code, string from)
        {
            switch (code)
            {
                case 202:
                    throw new XunleiRepeatTaskException(string.Format("{0}: Repeat task, skip.", from));
                case 403:
                    throw new XunleiUserSessionNoneException(string.Format("{0}: User id or Session id is none.", from));
                case 420:
                    throw new XunleiConnectionException(string.Format("{0}: Device client connection error.", from));
                case 1004:
                    throw new XunleiUserNotLoginException(string.Format("{0}: User not login.", from));
            }
        }
    }

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
}
