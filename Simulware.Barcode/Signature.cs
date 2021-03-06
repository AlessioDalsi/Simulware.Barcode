﻿using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Simulware.Barcode
{
    public class Signature
    {
        public byte[] CreateSignature(byte[] data)
        {
            byte[] ret = null;
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            //System.Diagnostics.Debugger.Break();              //utilizzato per il debug
            RSA provider = null;
            foreach (X509Certificate2 crt in store.Certificates)
            {
                string sn = ConfigurationManager.AppSettings["SerialNumberCert"];
                if (crt.SerialNumber == sn)
                {
                    provider = (RSA)crt.PrivateKey;
                    if (provider != null) { break; }
                }
            }

            if (provider == null)
            {
                throw new Exception("Cannot find crt");
            }

            SHA1Managed sha1 = new SHA1Managed();
            byte[] bytes = data;
            byte[] hash = sha1.ComputeHash(bytes);
            ret = provider.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            return ret;
        }
    }
}