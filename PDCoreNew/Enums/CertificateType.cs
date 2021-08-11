using System.ComponentModel;

namespace PDCoreNew.Enums
{
    /// <summary>
    /// Typ certyfikatu
    /// </summary>
    public enum CertificateType
    {
        /// <summary>
        /// Certyfikat WSS służący do podpisywania wiadomości
        /// </summary>
        [Description("Certyfikat WSS służący do podpisywania wiadomości")]
        WSS = 1,

        /// <summary>
        /// Certyfikat służący do zabezpieczenia komunikacji
        /// </summary>
        [Description("Certyfikat służący do zabezpieczenia komunikacji")]
        TLS = 2
    }
}
