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
using System.Security.Cryptography;
using System.IO;

namespace TDS.Shared.Web
{
    public class Encryption
    {
        private static byte[] key = { };
        private static byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
        private static string _encryptionkey = "!#$a54?3";

        public static string ScrambleText(string rtskey, string strVal)
        {
            if (rtskey == "")
                rtskey = _encryptionkey;
            return Encrypt(strVal, MakeUniformLength(rtskey, 8));
        }

        public static string UnScrambleText(string rtskey, string strVal)
        {
            if (rtskey == "")
                rtskey = _encryptionkey;
            return Decrypt(strVal, MakeUniformLength(rtskey, 8));
        }

        private static string MakeUniformLength(string Text, int TextLength)
        {
            if (Text.Length > TextLength)
            {
                Text = Text.Substring(0, 8);
            }
            else
            {
                Text += _encryptionkey.Substring(0, TextLength - Text.Length);
            }
            return Text;
        }

        private static string Encrypt(string stringToEncrypt, string sEncryptionKey)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace(" ", "+"));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

}
