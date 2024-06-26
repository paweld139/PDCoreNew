// SPUri.cs
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
using System.Xml;

namespace Microsoft.Xades
{
    /// <summary>
    /// SPUri represents the URL where the copy of the Signature Policy may be
    /// obtained.  The class derives from SigPolicyQualifier.
    /// </summary>
    public class SPUri : SigPolicyQualifier
    {
        #region Private variables
        private string uri;
        #endregion

        #region Public properties
        /// <summary>
        /// Uri for the sig policy qualifier
        /// </summary>
        public string Uri
        {
            get
            {
                return this.uri;
            }
            set
            {
                this.uri = value;
            }
        }

        /// <summary>
        /// Inherited generic element, not used in the SPUri class
        /// </summary>
        public override XmlElement AnyXmlElement
        {
            get
            {
                return null; //This does not make sense for SPUri
            }
            set
            {
                throw new CryptographicException("Setting AnyXmlElement on a SPUri is not supported");
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public SPUri()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Check to see if something has changed in this instance and needs to be serialized
        /// </summary>
        /// <returns>Flag indicating if a member needs serialization</returns>
        public override bool HasChanged()
        {
            bool retVal = false;

            if (this.uri != null && this.uri != "")
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Load state from an XML element
        /// </summary>
        /// <param name="xmlElement">XML element containing new state</param>
        public override void LoadXml(System.Xml.XmlElement xmlElement)
        {
            XmlNamespaceManager xmlNamespaceManager;
            XmlNodeList xmlNodeList;

            if (xmlElement == null)
            {
                throw new ArgumentNullException("xmlElement");
            }

            xmlNamespaceManager = new XmlNamespaceManager(xmlElement.OwnerDocument.NameTable);
            xmlNamespaceManager.AddNamespace("xsd", XadesSignedXml.XadesNamespaceUri);

            xmlNodeList = xmlElement.SelectNodes("xsd:SPURI", xmlNamespaceManager);

            this.uri = ((XmlElement)xmlNodeList.Item(0)).InnerText;
        }

        /// <summary>
        /// Returns the XML representation of the this object
        /// </summary>
        /// <returns>XML element containing the state of this object</returns>
        public override XmlElement GetXml()
        {
            XmlDocument creationXmlDocument;
            XmlElement bufferXmlElement;
            XmlElement retVal;

            creationXmlDocument = new XmlDocument();
            retVal = creationXmlDocument.CreateElement("SigPolicyQualifier", XadesSignedXml.XadesNamespaceUri);

            bufferXmlElement = creationXmlDocument.CreateElement("SPURI", XadesSignedXml.XadesNamespaceUri);
            bufferXmlElement.InnerText = this.uri;
            retVal.AppendChild(creationXmlDocument.ImportNode(bufferXmlElement, true));

            return retVal;
        }
        #endregion
    }
}
