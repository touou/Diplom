using System.Security.Cryptography.X509Certificates;
using VerificationCenter.Model;

namespace VerificationCenter.VerificationCenterServices
{
    public interface IVerificationCenterService
    {
        X509Certificate2 GenerateSelfSignedCertificate(GenerateCertificateRequest request);

        X509Certificate2 GenerateIssueCertificate(X509Certificate2 issuerCertificate,
            GenerateCertificateRequest request);
    }
}
