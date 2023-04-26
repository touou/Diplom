using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using Org.BouncyCastle.Pkcs;
using VerificationCenter.Model;

namespace VerificationCenter.CertificateServices
{
    public interface ICertificateService
    {
        X509Certificate GenerateCertificate(SecureRandom random,
            X509Name subjectDn,
            AsymmetricCipherKeyPair subjectKeyPair,
            BigInteger subjectSerialNumber,
            X509Name issuerDn,
            AsymmetricCipherKeyPair issuerKeyPair,
            BigInteger issuerSerialNumber,
            bool isCertificateAuthority,
            DateTime endDate);

        X509Certificate2 ConvertCertificate(X509Certificate certificate,
            AsymmetricCipherKeyPair subjectKeyPair,
            SecureRandom random,
            string password);

        AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, int strength);

        SecureRandom GetSecureRandom();

        BigInteger GenerateSerialNumber(SecureRandom random);

        Pkcs10CertificationRequest GenerateCsr(GenerateCertificateRequest request,
            CryptographyAlgorithm algorithm,
            AsymmetricCipherKeyPair keyPair);

        void SaveCertificateToStorage(X509Certificate2 certificate);
    }
}
