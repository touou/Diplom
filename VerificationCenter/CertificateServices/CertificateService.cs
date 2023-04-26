using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Math;
using VerificationCenter.Extensions;
using VerificationCenter.Model;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace VerificationCenter.CertificateServices
{
    public class CertificateService : ICertificateService
    {
        public X509Certificate GenerateCertificate(SecureRandom random,
            X509Name subjectDn,
            AsymmetricCipherKeyPair subjectKeyPair,
            BigInteger subjectSerialNumber,
            X509Name issuerDn,
            AsymmetricCipherKeyPair issuerKeyPair,
            BigInteger issuerSerialNumber,
            bool isCertificateAuthority,
            DateTime endDate)
        {
            const string signatureAlgorithm = "SHA256WithRSA";

            var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerKeyPair.Private, random);

            var certificateGenerator = new X509V3CertificateGenerator();

            certificateGenerator.SetSerialNumber(subjectSerialNumber);

            certificateGenerator.SetIssuerDN(issuerDn);

            certificateGenerator.SetSubjectDN(subjectDn);

            var notBefore = DateTime.UtcNow.Date;

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(endDate);

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            certificateGenerator.AddAuthorityKeyIdentifier(issuerDn, issuerKeyPair, issuerSerialNumber);
            certificateGenerator.AddSubjectKeyIdentifier(subjectKeyPair);
            certificateGenerator.AddBasicConstraints(isCertificateAuthority);

            var certificate = certificateGenerator.Generate(signatureFactory);

            return certificate;
        }

        public X509Certificate2 ConvertCertificate(X509Certificate certificate,
            AsymmetricCipherKeyPair subjectKeyPair,
            SecureRandom random,
            string password)
        {
            var store = new Pkcs12StoreBuilder().Build();

            string friendlyName = certificate.SubjectDN.ToString();

            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);

            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });

            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), random);

            var convertedCertificate =
                new X509Certificate2(stream.ToArray(),
                    password,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return convertedCertificate;
        }

        public AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, int strength)
        {
            var keyGenerationParameters = new KeyGenerationParameters(random, strength);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            return subjectKeyPair;
        }

        public SecureRandom GetSecureRandom()
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            return random;
        }

        public BigInteger GenerateSerialNumber(SecureRandom random)
        {
            var serialNumber =
                BigIntegers.CreateRandomInRange(
                    BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
            return serialNumber;
        }

        public Pkcs10CertificationRequest GenerateCsr(GenerateCertificateRequest request,
            CryptographyAlgorithm algorithm,
            AsymmetricCipherKeyPair keyPair)
        {
            var subject = new X509Name($"C={request.Country}, L={request.locality}, O={request.Organization}, UID={request.Inn} OU={request.OrganizationalUnit}, CN={request.CommonName}");

            var algorithmName = algorithm.ToString();

            var csr = keyPair.Private switch
            {
                RsaPrivateCrtKeyParameters => new Pkcs10CertificationRequest(algorithmName, subject, keyPair.Public, null, keyPair.Private),
                _ => throw new Exception("Unknown key pair type")
            };

            return csr;
        }

        public void SaveCertificateToStorage(X509Certificate2 certificate)
        {
            var store = new X509Store(StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadWrite);

            var crt = store.Certificates.First();

            store.Add(certificate);

            store.Close();
        }
    }
}
