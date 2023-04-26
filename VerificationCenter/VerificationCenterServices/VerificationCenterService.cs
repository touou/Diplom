using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Math;
using VerificationCenter.Model;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Operators;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using VerificationCenter.CertificateServices;

namespace VerificationCenter.VerificationCenterServices
{
    public class VerificationCenterService : IVerificationCenterService
    {
        private readonly ICertificateService _certificateService;

        public VerificationCenterService(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        public X509Certificate2 GenerateSelfSignedCertificate(GenerateCertificateRequest request)
        {
            var random = _certificateService.GetSecureRandom();

            var subjectKeyPair = _certificateService.GenerateKeyPair(random, 2048);

            var csr = _certificateService.GenerateCsr(request, CryptographyAlgorithm.SHA256withRSA, subjectKeyPair);

            var csrInfo = csr.GetCertificationRequestInfo();

            var issuerKeyPair = subjectKeyPair;

            var serialNumber = _certificateService.GenerateSerialNumber(random);
            var issuerSerialNumber = serialNumber;

            const bool isCertificateAuthority = true;
            var certificate = _certificateService.GenerateCertificate(random, csrInfo.Subject, subjectKeyPair, serialNumber,
                csrInfo.Subject, issuerKeyPair,
                issuerSerialNumber, isCertificateAuthority, DateTime.Now.AddYears(3));

            var convertedCertificate = _certificateService.ConvertCertificate(certificate, subjectKeyPair, random, request.Password);

            _certificateService.SaveCertificateToStorage(convertedCertificate);

            return convertedCertificate;
        }

        public X509Certificate2 GenerateIssueCertificate(X509Certificate2 issuerCertificate, GenerateCertificateRequest request)
        {
            var random = _certificateService.GetSecureRandom();

            var subjectKeyPair = _certificateService.GenerateKeyPair(random, 2048);

            var csr = _certificateService.GenerateCsr(request, CryptographyAlgorithm.SHA256withRSA, subjectKeyPair);

            var csrInfo = csr.GetCertificationRequestInfo();

            var issuerKeyPair = DotNetUtilities.GetKeyPair(issuerCertificate.GetRSAPrivateKey());

            var serialNumber = _certificateService.GenerateSerialNumber(random);

            var issuerSerialNumber = new BigInteger(issuerCertificate.GetSerialNumber());

            var pfxBouncyCastleCertificate = DotNetUtilities.FromX509Certificate(issuerCertificate);

            const bool isCertificateAuthority = false;
            var certificate = _certificateService.GenerateCertificate(random,
                csrInfo.Subject, 
                subjectKeyPair,
                serialNumber,
                pfxBouncyCastleCertificate.SubjectDN, 
                issuerKeyPair,
                issuerSerialNumber, 
                isCertificateAuthority, DateTime.Now.AddYears(1));

            var convertedCertificate = _certificateService.ConvertCertificate(certificate, subjectKeyPair, random, request.Password);

            _certificateService.SaveCertificateToStorage(convertedCertificate);

            return convertedCertificate;
        }
    }
}
