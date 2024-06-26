// SignatureProductionPlace.cs
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
using System.Xml;

namespace Microsoft.Xades
{
    /// <summary>
    /// In some transactions the purported place where the signer was at the time
    /// of signature creation may need to be indicated. In order to provide this
    /// information a new property may be included in the signature.
    /// This property specifies an address associated with the signer at a
    /// particular geographical (e.g. city) location.
    /// This is a signed property that qualifies the signer.
    /// An XML electronic signature aligned with the present document MAY contain
    /// at most one SignatureProductionPlace element.
    /// </summary>
    public class SignatureProductionPlace
    {
        #region Private variables
        private string city;
        private string stateOrProvince;
        private string postalCode;
        private string countryName;
        #endregion

        #region Public properties
        /// <summary>
        /// City where signature was produced
        /// </summary>
        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                this.city = value;
            }
        }

        /// <summary>
        /// State or province where signature was produced
        /// </summary>
        public string StateOrProvince
        {
            get
            {
                return this.stateOrProvince;
            }
            set
            {
                this.stateOrProvince = value;
            }
        }

        /// <summary>
        /// Postal code of place where signature was produced
        /// </summary>
        public string PostalCode
        {
            get
            {
                return this.postalCode;
            }
            set
            {
                this.postalCode = value;
            }
        }

        /// <summary>
        /// Country where signature was produced
        /// </summary>
        public string CountryName
        {
            get
            {
                return this.countryName;
            }
            set
            {
                this.countryName = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public SignatureProductionPlace()
        {
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

            if (!String.IsNullOrEmpty(this.city))
            {
                retVal = true;
            }

            if (!String.IsNullOrEmpty(this.stateOrProvince))
            {
                retVal = true;
            }

            if (!String.IsNullOrEmpty(this.postalCode))
            {
                retVal = true;
            }

            if (!String.IsNullOrEmpty(this.countryName))
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
            xmlNamespaceManager.AddNamespace("xsd", XadesSignedXml.XadesNamespaceUri);

            xmlNodeList = xmlElement.SelectNodes("xsd:City", xmlNamespaceManager);
            if (xmlNodeList.Count != 0)
            {
                this.city = xmlNodeList.Item(0).InnerText;
            }

            xmlNodeList = xmlElement.SelectNodes("xsd:PostalCode", xmlNamespaceManager);
            if (xmlNodeList.Count != 0)
            {
                this.postalCode = xmlNodeList.Item(0).InnerText;
            }

            xmlNodeList = xmlElement.SelectNodes("xsd:StateOrProvince", xmlNamespaceManager);
            if (xmlNodeList.Count != 0)
            {
                this.stateOrProvince = xmlNodeList.Item(0).InnerText;
            }

            xmlNodeList = xmlElement.SelectNodes("xsd:CountryName", xmlNamespaceManager);
            if (xmlNodeList.Count != 0)
            {
                this.countryName = xmlNodeList.Item(0).InnerText;
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
            XmlElement bufferXmlElement;

            creationXmlDocument = new XmlDocument();
            retVal = creationXmlDocument.CreateElement("SignatureProductionPlace", XadesSignedXml.XadesNamespaceUri);

            if (!String.IsNullOrEmpty(this.city))
            {
                bufferXmlElement = creationXmlDocument.CreateElement("City", XadesSignedXml.XadesNamespaceUri);
                bufferXmlElement.InnerText = this.city;
                retVal.AppendChild(bufferXmlElement);
            }

            if (!String.IsNullOrEmpty(this.stateOrProvince))
            {
                bufferXmlElement = creationXmlDocument.CreateElement("StateOrProvince", XadesSignedXml.XadesNamespaceUri);
                bufferXmlElement.InnerText = this.stateOrProvince;
                retVal.AppendChild(bufferXmlElement);
            }

            if (!String.IsNullOrEmpty(this.postalCode))
            {
                bufferXmlElement = creationXmlDocument.CreateElement("PostalCode", XadesSignedXml.XadesNamespaceUri);
                bufferXmlElement.InnerText = this.postalCode;
                retVal.AppendChild(bufferXmlElement);
            }

            if (this.countryName != null && this.countryName != "")
            {
                bufferXmlElement = creationXmlDocument.CreateElement("CountryName", XadesSignedXml.XadesNamespaceUri);
                bufferXmlElement.InnerText = this.countryName;
                retVal.AppendChild(bufferXmlElement);
            }

            return retVal;
        }
        #endregion
    }
}
