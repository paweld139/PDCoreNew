// CompleteCertificateRefs.cs
//
// XAdES Starter Kit for Microsoft .NET 3.5 (and above)
// 2010 Microsoft France
//
// Originally published under the CECILL-B Free Software license agreement,
// modified by Dpto. de Nuevas Tecnologías de la Dirección General de Urbanismo del Ayto. de Cartagena
// and published under the GNU General Public License version 3.
// 
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 

using System;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Microsoft.Xades
{
    /// <summary>
    /// This clause defines the XML element containing the sequence of
    /// references to the full set of CA certificates that have been used
    /// to validate the electronic signature up to (but not including) the
    /// signer's certificate. This is an unsigned property that qualifies
    /// the signature.
    /// An XML electronic signature aligned with the XAdES standard may
    /// contain at most one CompleteCertificateRefs element.
    /// </summary>
    public class CompleteCertificateRefs
    {
        #region Private variables
        private string id;
        private CertRefs certRefs;
        #endregion

        #region Public properties
        /// <summary>
        /// The optional Id attribute can be used to make a reference to the CompleteCertificateRefs element
        /// </summary>
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// The CertRefs element contains a sequence of Cert elements, incorporating the
        /// digest of each certificate and optionally the issuer and serial number identifier.
        /// </summary>
        public CertRefs CertRefs
        {
            get
            {
                return this.certRefs;
            }
            set
            {
                this.certRefs = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public CompleteCertificateRefs()
        {
            this.certRefs = new CertRefs();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Check to see if something has changed in this instance and needs to be serialized
        /// </summary>
        /// <returns>Flag indicating if a member needs serialization</returns>
        public bool HasChanged()
        {
            bool retVal = false;

            if (!String.IsNullOrEmpty(this.id))
            {
                retVal = true;
            }
            if (this.certRefs != null && this.certRefs.HasChanged())
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Load state from an XML element
        /// </summary>
        /// <param name="xmlElement">XML element containing new state</param>
        public void LoadXml(System.Xml.XmlElement xmlElement)
        {
            XmlNamespaceManager xmlNamespaceManager;
            XmlNodeList xmlNodeList;

            if (xmlElement == null)
            {
                throw new ArgumentNullException("xmlElement");
            }
            if (xmlElement.HasAttribute("Id"))
            {
                this.id = xmlElement.GetAttribute("Id");
            }
            else
            {
                this.id = "";
            }

            xmlNamespaceManager = new XmlNamespaceManager(xmlElement.OwnerDocument.NameTable);
            xmlNamespaceManager.AddNamespace("xsd", XadesSignedXml.XadesNamespaceUri);

            xmlNodeList = xmlElement.SelectNodes("xsd:CertRefs", xmlNamespaceManager);
            if (xmlNodeList.Count != 0)
            {
                this.certRefs = new CertRefs();
                this.certRefs.LoadXml((XmlElement)xmlNodeList.Item(0));
            }
        }

        /// <summary>
        /// Returns the XML representation of the this object
        /// </summary>
        /// <returns>XML element containing the state of this object</returns>
        public XmlElement GetXml()
        {
            XmlDocument creationXmlDocument;
            XmlElement retVal;

            creationXmlDocument = new XmlDocument();
            retVal = creationXmlDocument.CreateElement(XadesSignedXml.XmlXadesPrefix, "CompleteCertificateRefs", XadesSignedXml.XadesNamespaceUri);
            retVal.SetAttribute("xmlns:ds", SignedXml.XmlDsigNamespaceUrl);

            if (!String.IsNullOrEmpty(this.id))
            {
                retVal.SetAttribute("Id", this.id);
            }

            if (this.certRefs != null && this.certRefs.HasChanged())
            {
                retVal.AppendChild(creationXmlDocument.ImportNode(this.certRefs.GetXml(), true));
            }

            return retVal;
        }
        #endregion
    }
}
