using FirmaXadesNet;
using FirmaXadesNet.Crypto;
using FirmaXadesNet.Signature.Parameters;
using Microsoft.Xades;
using PDCore.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace PDCore.Common.Utils
{
    public static class XMLUtils
    {
        public static string XadesSign(string xml, X509Certificate2 x509Certificate)
        {
            XadesService xadesService = new XadesService();

            SignatureParameters parametros = new SignatureParameters
            {
                SignaturePolicyInfo = new SignaturePolicyInfo(),
                SignaturePackaging = SignaturePackaging.ENVELOPED,
                InputMimeType = "text/xml"
            };


            using (parametros.Signer = new Signer(x509Certificate))
            {
                using (MemoryStream fs = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    var docFirmado = xadesService.Sign(fs, parametros);

                    return docFirmado.Document.ToXml();
                }
            }
        }

        public static string Sign(string xml, X509Certificate2 x509)
        {
            // Wczytaj.
            XmlDocument doc = new XmlDocument
            {
                PreserveWhitespace = true
            };
            doc.LoadXml(xml);

            // SignedXml object
            XadesSignedXml signedXml = new XadesSignedXml(doc);

            signedXml.Signature.Id = "ID-1234";
            signedXml.SigningKey = x509.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            // dodaj referencję na dokument
            Reference reference = new Reference("#Dokument");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);

            // dodaj KeyInfo
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(x509)); // ??? WholeChain ???
            signedXml.KeyInfo = keyInfo;

            //
            XadesObject xo = new XadesObject();
            {
                Cert cert = new Cert();

                cert.IssuerSerial.X509IssuerName = x509.IssuerName.Name;
                cert.IssuerSerial.X509SerialNumber = x509.SerialNumber;

                {
                    SHA1 cryptoServiceProvider = new SHA1CryptoServiceProvider();
                    cert.CertDigest.DigestValue = cryptoServiceProvider.ComputeHash(x509.RawData);
                    cert.CertDigest.DigestMethod.Algorithm = SignedXml.XmlDsigSHA1Url;
                }

                xo.QualifyingProperties.Target = "#" + signedXml.Signature.Id;
                xo.QualifyingProperties.SignedProperties.SignedSignatureProperties.SigningTime = DateTime.Now;
                xo.QualifyingProperties.SignedProperties.SignedSignatureProperties.SignaturePolicyIdentifier.SignaturePolicyImplied = true;

                xo.QualifyingProperties.SignedProperties.SignedSignatureProperties.SigningCertificate.CertCollection.Add(cert);

                DataObjectFormat dof = new DataObjectFormat
                {
                    ObjectReferenceAttribute = "#Dokument",
                    Description = "Dokument w formacie xml [XML]",
                    Encoding = SignedXml.XmlDsigBase64TransformUrl, // ...xmldsig/#base64
                    MimeType = "text/plain"
                };
                xo.QualifyingProperties.SignedProperties.SignedDataObjectProperties.DataObjectFormatCollection.Add(dof);
            }
            signedXml.AddXadesObject(xo);

            //// W dokumentacji 2.9.9.a, Id dla <ds:Object> ma mieć wartość "Dokument", ale nie ma tego w przykładach
            var data = new DataObject("Dokument", "text/xml", "", doc.DocumentElement);
            signedXml.AddObject(data);

            // Podpisz
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();


            return xmlDigitalSignature.OuterXml;
        }
    }
}
