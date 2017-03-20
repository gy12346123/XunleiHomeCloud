using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XunleiHomeCloud
{
    public class CommonException
    {

    }

    public class XunleiNoDeviceException : ApplicationException
    {
        public XunleiNoDeviceException() { }
        public XunleiNoDeviceException(string message) : base(message) { }
        public XunleiNoDeviceException(string message, Exception inner) : base(message, inner) { }
    }

    public class NoCookieException : ApplicationException
    {
        public NoCookieException () { }
        public NoCookieException(string message) : base(message) { }
        public NoCookieException(string message, Exception inner) : base(message, inner) { }
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

    public class UserNotLoginException : ApplicationException
    {
        public UserNotLoginException() { }
        public UserNotLoginException(string message) : base(message) { }
        public UserNotLoginException(string message, Exception inner) : base(message, inner) { }
    }

    public class UserSessionNoneException : ApplicationException
    {
        public UserSessionNoneException() { }
        public UserSessionNoneException(string message) : base(message) { }
        public UserSessionNoneException(string message, Exception inner) : base(message, inner) { }
    }
}
