using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace SlxJobScheduler
{
    /// <summary>
    /// Specifies special behavior for this function. 
    /// This value can be a bitwise-OR combination of zero or more of the following values. 
    /// </summary>
    // For more information of these flags see:
    // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/secauthn/security/creduipromptforcredentials.asp
    // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnnetsec/html/dpapiusercredentials.asp?frame=true
    [Flags]
    public enum UserCredentialsDialogFlags
    {
        Default = GenericCredentials |
                  DoNotPersist |
                  AlwaysShowUI |
                  ExpectConfirmation,
        None = 0x0,
        IncorrectPassword = 0x1,
        DoNotPersist = 0x2,
        RequestAdministrator = 0x4,
        ExcludesCertificates = 0x8,
        RequireCertificate = 0x10,
        ShowSaveCheckbox = 0x40,
        AlwaysShowUI = 0x80,
        RequireSmartCard = 0x100,
        PasswordOnlyOk = 0x200,
        ValidateUsername = 0x400,
        CompleteUserName = 0x800,
        Persist = 0x1000,
        ServerCredential = 0x4000,
        ExpectConfirmation = 0x20000,
        GenericCredentials = 0x40000,
        UsernameTargetCredentials = 0x80000,
        KeepUsername = 0x100000
    }

    /// <summary>
    /// Displays a dialog box and prompts the user for login credentials.
    /// </summary>
    public class UserCredentialsDialog : CommonDialog
    {
        #region Fields

        private string _user;
        private SecureString _password;
        private string _domain;
        private string _target;
        private string _message;
        private string _caption;
        private Image _banner;
        private bool _saveChecked;
        private UserCredentialsDialogFlags _flags;

        #endregion

        #region Constructors

        public UserCredentialsDialog()
        {
            Reset();
        }

        #endregion

        #region Properties

        public string User
        {
            get { return _user; }
            set
            {
                if (value != null && value.Length > Win32Native.CREDUI_MAX_USERNAME_LENGTH)
                {
                    throw new ArgumentException(string.Format(
                        "The user name has a maximum length of {0} characters.",
                        Win32Native.CREDUI_MAX_USERNAME_LENGTH), "value");
                }
                _user = value;
            }
        }

        public SecureString Password
        {
            get { return _password; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_PASSWORD_LENGTH)
                    {
                        throw new ArgumentException(string.Format(
                            "The password has a maximum length of {0} characters.",
                            Win32Native.CREDUI_MAX_PASSWORD_LENGTH), "value");
                    }
                }
                _password = value;
            }
        }

        public string Domain
        {
            get { return _domain; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_DOMAIN_TARGET_LENGTH)
                    {
                        throw new ArgumentException(string.Format(
                            "The domain name has a maximum length of {0} characters.",
                            Win32Native.CREDUI_MAX_DOMAIN_TARGET_LENGTH), "value");
                    }
                }
                _domain = value;
            }
        }

        public string Target
        {
            get { return _target; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_GENERIC_TARGET_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The target has a maximum length of {0} characters.",
                                Win32Native.CREDUI_MAX_GENERIC_TARGET_LENGTH), "value");
                    }
                }
                _target = value;
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_MESSAGE_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The message has a maximum length of {0} characters.",
                                Win32Native.CREDUI_MAX_MESSAGE_LENGTH), "value");
                    }
                }
                _message = value;
            }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                if (value != null)
                {
                    if (value.Length > Win32Native.CREDUI_MAX_CAPTION_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The caption has a maximum length of {0} characters.",
                                Win32Native.CREDUI_MAX_CAPTION_LENGTH), "value");
                    }
                }
                _caption = value;
            }
        }

        public Image Banner
        {
            get { return _banner; }
            set
            {
                if (value != null)
                {
                    if (value.Width != Win32Native.CREDUI_BANNER_WIDTH)
                    {
                        throw new ArgumentException(
                            string.Format("The banner image width must be {0} pixels.",
                                Win32Native.CREDUI_BANNER_WIDTH), "value");
                    }
                    if (value.Height != Win32Native.CREDUI_BANNER_HEIGHT)
                    {
                        throw new ArgumentException(
                            string.Format("The banner image height must be {0} pixels.",
                                Win32Native.CREDUI_BANNER_HEIGHT), "value");
                    }
                }
                _banner = value;
            }
        }

        public bool SaveChecked
        {
            get { return _saveChecked; }
            set { _saveChecked = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// This method is for backward compatibility with APIs that does
        /// not provide the <see cref="SecureString"/> type.
        /// </summary>
        /// <returns></returns>
        public string PasswordToString()
        {
            var ptr = Marshal.SecureStringToGlobalAllocUnicode(_password);
            try
            {
                // Unsecure managed string
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        #endregion

        #region CommonDialog overrides

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            if (Environment.OSVersion.Version.Major < 5)
            {
                throw new PlatformNotSupportedException("The Credential Management API requires Windows XP / Windows Server 2003 or later.");
            }

            var credInfo = new Win32Native.CredUIInfo(hwndOwner,
                _caption, _message, _banner);
            var usr = new StringBuilder(Win32Native.CREDUI_MAX_USERNAME_LENGTH);
            var pwd = new StringBuilder(Win32Native.CREDUI_MAX_PASSWORD_LENGTH);

            if (!string.IsNullOrEmpty(User))
            {
                if (!string.IsNullOrEmpty(Domain))
                {
                    usr.Append(Domain + "\\");
                }
                usr.Append(User);
            }
            if (Password != null)
            {
                pwd.Append(PasswordToString());
            }

            try
            {
                var result = Win32Native.CredUIPromptForCredentials(
                    ref credInfo, _target,
                    IntPtr.Zero, 0,
                    usr, Win32Native.CREDUI_MAX_USERNAME_LENGTH,
                    pwd, Win32Native.CREDUI_MAX_PASSWORD_LENGTH,
                    ref _saveChecked, _flags);
                switch (result)
                {
                    case Win32Native.CredUIReturnCodes.NO_ERROR:
                        LoadUserDomainValues(usr);
                        LoadPasswordValue(pwd);
                        return true;
                    case Win32Native.CredUIReturnCodes.ERROR_CANCELLED:
                        User = null;
                        Password = null;
                        return false;
                    default:
                        throw new InvalidOperationException(TranslateReturnCode(result));
                }
            }
            finally
            {
                usr.Remove(0, usr.Length);
                pwd.Remove(0, pwd.Length);
                if (_banner != null)
                {
                    Win32Native.DeleteObject(credInfo.hbmBanner);
                }
            }
        }

        /// <summary>
        /// Set all properties to it's default values.
        /// </summary>
        public override void Reset()
        {
            _target = Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName;
            _user = null;
            _password = null;
            _domain = null;
            _caption = null; // target as caption;
            _message = null;
            _banner = null;
            _saveChecked = false;
            _flags = UserCredentialsDialogFlags.Default;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_password != null)
            {
                _password.Dispose();
                _password = null;
            }
        }

        #endregion

        #region Private methods

        private static string TranslateReturnCode(Win32Native.CredUIReturnCodes result)
        {
            return string.Format("Invalid operation: {0}", result);
        }

        private void LoadPasswordValue(StringBuilder password)
        {
            var pwd = new char[password.Length];
            var securePassword = new SecureString();
            try
            {
                password.CopyTo(0, pwd, 0, pwd.Length);
                foreach (var c in pwd)
                {
                    securePassword.AppendChar(c);
                }
                securePassword.MakeReadOnly();
                Password = securePassword.Copy();
            }
            finally
            {
                // discard the char array
                Array.Clear(pwd, 0, pwd.Length);
            }
        }

        private void LoadUserDomainValues(StringBuilder principalName)
        {
            var user = new StringBuilder(Win32Native.CREDUI_MAX_USERNAME_LENGTH);
            var domain = new StringBuilder(Win32Native.CREDUI_MAX_DOMAIN_TARGET_LENGTH);
            var result = Win32Native.CredUIParseUserNameW(principalName.ToString(),
                user, Win32Native.CREDUI_MAX_USERNAME_LENGTH, domain, Win32Native.CREDUI_MAX_DOMAIN_TARGET_LENGTH);

            if (result == Win32Native.CredUIReturnCodes.NO_ERROR)
            {
                User = user.ToString();
                Domain = domain.ToString();
            }
            else
            {
                User = principalName.ToString();
                Domain = Environment.MachineName;
            }
        }

        #endregion

        #region Unmanaged code

        [SuppressUnmanagedCodeSecurity]
        private static class Win32Native
        {
            internal const int CREDUI_MAX_MESSAGE_LENGTH = 100;
            internal const int CREDUI_MAX_CAPTION_LENGTH = 100;
            internal const int CREDUI_MAX_GENERIC_TARGET_LENGTH = 100;
            internal const int CREDUI_MAX_DOMAIN_TARGET_LENGTH = 100;
            internal const int CREDUI_MAX_USERNAME_LENGTH = 100;
            internal const int CREDUI_MAX_PASSWORD_LENGTH = 100;
            internal const int CREDUI_BANNER_HEIGHT = 60;
            internal const int CREDUI_BANNER_WIDTH = 320;

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            internal static extern bool DeleteObject(IntPtr hObject);

            [DllImport("credui.dll", EntryPoint = "CredUIPromptForCredentialsW", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern CredUIReturnCodes CredUIPromptForCredentials(
                ref CredUIInfo creditUR,
                string targetName,
                IntPtr reserved1,
                int iError,
                StringBuilder userName,
                int maxUserName,
                StringBuilder password,
                int maxPassword,
                ref bool iSave,
                UserCredentialsDialogFlags flags);

            [DllImport("credui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern CredUIReturnCodes CredUIParseUserNameW(
                string userName,
                StringBuilder user,
                int userMaxChars,
                StringBuilder domain,
                int domainMaxChars);

            internal enum CredUIReturnCodes
            {
                NO_ERROR = 0,
                ERROR_CANCELLED = 1223,
                ERROR_NO_SUCH_LOGON_SESSION = 1312,
                ERROR_NOT_FOUND = 1168,
                ERROR_INVALID_ACCOUNT_NAME = 1315,
                ERROR_INSUFFICIENT_BUFFER = 122,
                ERROR_INVALID_PARAMETER = 87,
                ERROR_INVALID_FLAGS = 1004
            }

            internal struct CredUIInfo
            {
                internal CredUIInfo(IntPtr owner, string caption, string message, Image banner)
                {
                    cbSize = Marshal.SizeOf(typeof (CredUIInfo));
                    hwndParent = owner;
                    pszCaptionText = caption;
                    pszMessageText = message;

                    if (banner != null)
                    {
                        hbmBanner = new Bitmap(banner,
                            CREDUI_BANNER_WIDTH, CREDUI_BANNER_HEIGHT).GetHbitmap();
                    }
                    else
                    {
                        hbmBanner = IntPtr.Zero;
                    }
                }

                internal int cbSize;
                internal IntPtr hwndParent;
                [MarshalAs(UnmanagedType.LPWStr)] internal string pszMessageText;
                [MarshalAs(UnmanagedType.LPWStr)] internal string pszCaptionText;
                internal readonly IntPtr hbmBanner;
            }
        }

        #endregion
    }
}