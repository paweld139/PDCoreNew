// UnsignedDataObjectProperties.cs
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
using System.Collections;
using System.Xml;

namespace Microsoft.Xades
{
    /// <summary>
    /// The UnsignedDataObjectProperties element may contain properties that
    /// qualify some of the signed data objects.
    /// </summary>
    public class UnsignedDataObjectProperties
    {
        #region Private variables
        private UnsignedDataObjectPropertyCollection unsignedDataObjectPropertyCollection;
        #endregion

        #region Public properties
        /// <summary>
        /// A collection of unsigned data object properties
        /// </summary>
        public UnsignedDataObjectPropertyCollection UnsignedDataObjectPropertyCollection
        {
            get
            {
                return this.unsignedDataObjectPropertyCollection;
            }
            set
            {
                this.unsignedDataObjectPropertyCollection = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public UnsignedDataObjectProperties()
        {
            this.unsignedDataObjectPropertyCollection = new UnsignedDataObjectPropertyCollection();
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

            if (this.unsignedDataObjectPropertyCollection.Count > 0)
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
            UnsignedDataObjectProperty newUnsignedDataObjectProperty;
            IEnumerator enumerator;
            XmlElement iterationXmlElement;

            if (xmlElement == null)
            {
                throw new ArgumentNullException("xmlElement");
            }

            xmlNamespaceManager = new XmlNamespaceManager(xmlElement.OwnerDocument.NameTable);
            xmlNamespaceManager.AddNamespace("xsd", XadesSignedXml.XadesNamespaceUri);

            this.unsignedDataObjectPropertyCollection.Clear();
            xmlNodeList = xmlElement.SelectNodes("xsd:UnsignedDataObjectProperty", xmlNamespaceManager);
            enumerator = xmlNodeList.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    iterationXmlElement = enumerator.Current as XmlElement;
                    if (iterationXmlElement != null)
                    {
                        newUnsignedDataObjectProperty = new UnsignedDataObjectProperty();
                        newUnsignedDataObjectProperty.LoadXml(iterationXmlElement);
                        this.unsignedDataObjectPropertyCollection.Add(newUnsignedDataObjectProperty);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
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
            retVal = creationXmlDocument.CreateElement("UnsignedDataObjectProperties", XadesSignedXml.XadesNamespaceUri);

            if (this.unsignedDataObjectPropertyCollection.Count > 0)
            {
                foreach (UnsignedDataObjectProperty unsignedDataObjectProperty in this.unsignedDataObjectPropertyCollection)
                {
                    if (unsignedDataObjectProperty.HasChanged())
                    {
                        retVal.AppendChild(creationXmlDocument.ImportNode(unsignedDataObjectProperty.GetXml(), true));
                    }
                }
            }

            return retVal;
        }
        #endregion
    }
}
