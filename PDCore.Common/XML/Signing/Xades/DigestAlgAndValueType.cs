// DigestAlgAndValueType.cs
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
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Microsoft.Xades
{
    /// <summary>
    /// This class indicates the algortithm used to calculate the digest and
    /// the digest value itself
    /// </summary>
    public class DigestAlgAndValueType
    {
        #region Private variables
        private string tagName;
        private DigestMethod digestMethod;
        private byte[] digestValue;
        #endregion

        #region Public properties
        /// <summary>
        /// The name of the element when serializing
        /// </summary>
        public string TagName
        {
            get
            {
                return this.tagName;
            }
            set
            {
                this.tagName = value;
            }
        }

        /// <summary>
        /// Indicates the digest algorithm
        /// </summary>
        public DigestMethod DigestMethod
        {
            get
            {
                return this.digestMethod;
            }
            set
            {
                this.digestMethod = value;
            }
        }

        /// <summary>
        /// Contains the value of the digest
        /// </summary>
        public byte[] DigestValue
        {
            get
            {
                return this.digestValue;
            }
            set
            {
                this.digestValue = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public DigestAlgAndValueType()
        {
            this.digestMethod = new DigestMethod();
            this.digestValue = null;
        }

        /// <summary>
        /// Constructor with TagName
        /// </summary>
        /// <param name="tagName">Name of the tag when serializing with GetXml</param>
        public DigestAlgAndValueType(string tagName) : this()
        {
            this.tagName = tagName;
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

            if (this.digestMethod != null && this.digestMethod.HasChanged())
            {
                retVal = true;
            }

            if (this.digestValue != null && this.digestValue.Length > 0)
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

            xmlNamespaceManager = new XmlNamespaceManager(xmlElement.OwnerDocument.NameTable);
            xmlNamespaceManager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);


            xmlNodeList = xmlElement.SelectNodes("ds:DigestMethod", xmlNamespaceManager);
            if (xmlNodeList.Count == 0)
            {
                throw new CryptographicException("DigestMethod missing");
            }
            this.digestMethod = new DigestMethod();
            this.digestMethod.LoadXml((XmlElement)xmlNodeList.Item(0));

            xmlNodeList = xmlElement.SelectNodes("ds:DigestValue", xmlNamespaceManager);
            if (xmlNodeList.Count == 0)
            {
                throw new CryptographicException("DigestValue missing");
            }
            this.digestValue = Convert.FromBase64String(xmlNodeList.Item(0).InnerText);
        }

        /// <summary>
        /// Returns the XML representation of the this object
        /// </summary>
        /// <returns>XML element containing the state of this object</returns>
        public XmlElement GetXml()
        {
            XmlDocument creationXmlDocument;
            XmlElement retVal;
            XmlElement bufferXmlElement;

            creationXmlDocument = new XmlDocument();
            retVal = creationXmlDocument.CreateElement(XadesSignedXml.XmlXadesPrefix, this.tagName, XadesSignedXml.XadesNamespaceUri);
            retVal.SetAttribute("xmlns:ds", SignedXml.XmlDsigNamespaceUrl);

            if (this.digestMethod != null && this.digestMethod.HasChanged())
            {
                retVal.AppendChild(creationXmlDocument.ImportNode(this.digestMethod.GetXml(), true));
            }
            else
            {
                throw new CryptographicException("DigestMethod element missing in DigestAlgAndValueType");
            }

            if (this.digestValue != null && this.digestValue.Length > 0)
            {
                //bufferXmlElement = creationXmlDocument.CreateElement("DigestValue", XadesSignedXml.XadesNamespaceUri);
                bufferXmlElement = creationXmlDocument.CreateElement(XadesSignedXml.XmlDSigPrefix, "DigestValue", SignedXml.XmlDsigNamespaceUrl);
                bufferXmlElement.SetAttribute("xmlns:xades", XadesSignedXml.XadesNamespaceUri);

                bufferXmlElement.InnerText = Convert.ToBase64String(this.digestValue);
                retVal.AppendChild(bufferXmlElement);
            }
            else
            {
                throw new CryptographicException("DigestValue element missing in DigestAlgAndValueType");
            }

            return retVal;
        }
        #endregion
    }
}