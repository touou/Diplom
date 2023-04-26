using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using VerificationCenter.CertificateServices;
using VerificationCenter.Model;
using VerificationCenter.VerificationCenterServices;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void WriteCertificate(X509Certificate2 certificate, string outputFileName, string password)
        {
            var bytes = certificate.Export(X509ContentType.Pfx, password);
            File.WriteAllBytes(outputFileName, bytes);
        }

        private static X509Certificate2 LoadCertificate(string issuerFileName, string password)
        {
            var issuerCertificate = new X509Certificate2(issuerFileName, password, X509KeyStorageFlags.Exportable);
            return issuerCertificate;
        }

        static void Main(string[] args)
        {
            var services = new VerificationCenterService(new CertificateService());

            var SelfSignedRequest = new GenerateCertificateRequest()
            {
                Country = "2",
                Inn = "12345678",
                CommonName = "Жопный Уцышка",
                locality = "4",
                Organization = "5",
                OrganizationalUnit = "6",
                Password = "password"
            };

            var issueCertificateRequestRequest = new GenerateCertificateRequest()
            {
                Country = "2",
                Inn = "12345678",
                CommonName = "Сабирзянов Данил Азатович",
                locality = "4",
                Organization = "5",
                OrganizationalUnit = "6",
                Password = "password"
            };

            //var cert = services.GenerateSelfSignedCertificate(SelfSignedRequest);

            //WriteCertificate(cert, "ca-certificate.pfx", SelfSignedRequest.Password);

            //var loadCert = LoadCertificate("ca-certificate.pfx", SelfSignedRequest.Password);

            //var issuerCert = services.GenerateIssueCertificate(loadCert, issueCertificateRequestRequest);

            //WriteCertificate(issuerCert, "issue-certificate.pfx", issueCertificateRequestRequest.Password);

            X509Certificate2 issuerCert = LoadCertificate("issue-certificate.pfx", SelfSignedRequest.Password);

            char[] password = issueCertificateRequestRequest.Password.ToCharArray();

            Pkcs12Store pk12;
            byte[] rawdata;
            rawdata = issuerCert.Export(X509ContentType.Pfx, issueCertificateRequestRequest.Password);
            var memStream = new MemoryStream(rawdata);
            pk12 = new Pkcs12Store(memStream, password);

            string alias = null;
            foreach (object a in pk12.Aliases)
            {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                {
                    break;
                }
            }
            ICipherParameters pk = pk12.GetKey(alias).Key;

            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            X509Certificate[] chain = new X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
            {
                chain[k] = ce[k].Certificate;
            }

            string DEST = @"C:\Users\FATCOCKPC\Downloads\SignedPdf.pdf";
            string SRC = @"C:\Users\FATCOCKPC\Desktop\sample-contract.pdf";

            PdfReader reader = new PdfReader(SRC);
            PdfSigner signer = new PdfSigner(reader,
                new FileStream(DEST, FileMode.Create),
                new StampingProperties());

            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);

            signer.SignDetached(pks, chain, null, null, null, 0,
                PdfSigner.CryptoStandard.CMS);
        }
    }
}