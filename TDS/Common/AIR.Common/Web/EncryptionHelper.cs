/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Text;
using System.Web;
using System.Web.Security;

namespace AIR.Common.Web
{
    /// <summary>
    /// Helpers methods for encryption and encoding
    /// </summary>
    /// <remarks>
    /// If you are going to use the encryption then you must have a pregenerated <machinekey> set in the <system.web> section of your web.config.
    /// </remarks>
    public static class EncryptionHelper
    {
        private readonly static EncryptionProvider encProv;

        static EncryptionHelper()
        {
            encProv = new EncryptionProvider();
        }

        /// <summary>
        /// Encode bytes to BASE64
        /// </summary>
        public static string EncodeToBase64(string s)
        {
            return HttpServerUtility.UrlTokenEncode(Encoding.Unicode.GetBytes(s));
        }

        /// <summary>
        /// Encrypt a string
        /// </summary>
        public static byte[] Encrypt(byte[] data)
        {
            return encProv.Encrypt(data);
        }

        /// <summary>
        /// Encrypt a string
        /// </summary>
        public static byte[] Encrypt(string data)
        {
            byte[] decryptedBytes = Encoding.Unicode.GetBytes(data);
            return Encrypt(decryptedBytes);
        }

        /// <summary>
        /// Encrypt a string and return a web safe base64 string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncryptToBase64(string data)
        {
            byte[] encryptedBytes = Encrypt(data);
            return HttpServerUtility.UrlTokenEncode(encryptedBytes);
        }

        /// <summary>
        /// Decode string from BASE64
        /// </summary>
        public static string DecodeFromBase64(string s)
        {
            return Encoding.Unicode.GetString(HttpServerUtility.UrlTokenDecode(s));
        }

        public static byte[] Decrypt(byte[] data)
        {
            return encProv.Decrypt(data);
        }

        public static byte[] Decrypt(string data)
        {
            byte[] encryptedBytes = Encoding.Unicode.GetBytes(data);
            return Decrypt(encryptedBytes);
        }

        public static string DecryptFromBase64(string data)
        {
            byte[] encryptedBytes = HttpServerUtility.UrlTokenDecode(data);
            byte[] decryptedBytes = Decrypt(encryptedBytes);

            return Encoding.Unicode.GetString(decryptedBytes);
        }

        #region ASP.NET provider

        /// <summary>
        /// This is only used for the membership providers password encryption. The encryption method that uses
        /// the machine key is internal to ASP.NET. So this is a workaround.
        /// </summary>
        internal class EncryptionProvider : MembershipProvider
        {
            public byte[] Encrypt(byte[] data)
            {
                return EncryptPassword(data);
            }

            public byte[] Decrypt(byte[] data)
            {
                return DecryptPassword(data);
            }

            public override MembershipUser CreateUser(string username, string password, string email,
                                                      string passwordQuestion, string passwordAnswer, bool isApproved,
                                                      object providerUserKey, out MembershipCreateStatus status)
            {
                throw new NotImplementedException();
            }

            public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                                 string newPasswordQuestion, string newPasswordAnswer)
            {
                throw new NotImplementedException();
            }

            public override string GetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override bool ChangePassword(string username, string oldPassword, string newPassword)
            {
                throw new NotImplementedException();
            }

            public override string ResetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override void UpdateUser(MembershipUser user)
            {
                throw new NotImplementedException();
            }

            public override bool ValidateUser(string username, string password)
            {
                throw new NotImplementedException();
            }

            public override bool UnlockUser(string userName)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(string username, bool userIsOnline)
            {
                throw new NotImplementedException();
            }

            public override string GetUserNameByEmail(string email)
            {
                throw new NotImplementedException();
            }

            public override bool DeleteUser(string username, bool deleteAllRelatedData)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override int GetNumberOfUsersOnline()
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                     out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                      out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override bool EnablePasswordRetrieval
            {
                get { throw new NotImplementedException(); }
            }

            public override bool EnablePasswordReset
            {
                get { throw new NotImplementedException(); }
            }

            public override bool RequiresQuestionAndAnswer
            {
                get { throw new NotImplementedException(); }
            }

            public override string ApplicationName
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public override int MaxInvalidPasswordAttempts
            {
                get { throw new NotImplementedException(); }
            }

            public override int PasswordAttemptWindow
            {
                get { throw new NotImplementedException(); }
            }

            public override bool RequiresUniqueEmail
            {
                get { throw new NotImplementedException(); }
            }

            public override MembershipPasswordFormat PasswordFormat
            {
                get { throw new NotImplementedException(); }
            }

            public override int MinRequiredPasswordLength
            {
                get { throw new NotImplementedException(); }
            }

            public override int MinRequiredNonAlphanumericCharacters
            {
                get { throw new NotImplementedException(); }
            }

            public override string PasswordStrengthRegularExpression
            {
                get { throw new NotImplementedException(); }
            }
        }

        #endregion

    }
}
