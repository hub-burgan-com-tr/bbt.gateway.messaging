using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.Pusula.Model.GetByPhone
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://intertech.com.tr/Pusula")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://intertech.com.tr/Pusula", IsNullable = false)]
    public partial class DataTable
    {

        private diffgram diffgramField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1")]
        public diffgram diffgram
        {
            get
            {
                return this.diffgramField;
            }
            set
            {
                this.diffgramField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1", IsNullable = false)]
    public partial class diffgram
    {

        private DocumentElementCustomerDetail[] documentElementField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "")]
        [System.Xml.Serialization.XmlArrayItemAttribute("CustomerDetail", IsNullable = false)]
        public DocumentElementCustomerDetail[] DocumentElement
        {
            get
            {
                return this.documentElementField;
            }
            set
            {
                this.documentElementField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentElementCustomerDetail
    {

        private ulong customerNumberField;

        private string telephoneTypeField;

        private byte countryCodeField;

        private ushort cityCodeField;

        private uint telephoneNumberField;

        private string customerNameField;

        private ulong tcknField;

        private object vknField;

        private string idField;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public ulong CustomerNumber
        {
            get
            {
                return this.customerNumberField;
            }
            set
            {
                this.customerNumberField = value;
            }
        }

        /// <remarks/>
        public string TelephoneType
        {
            get
            {
                return this.telephoneTypeField;
            }
            set
            {
                this.telephoneTypeField = value;
            }
        }

        /// <remarks/>
        public byte CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        public ushort CityCode
        {
            get
            {
                return this.cityCodeField;
            }
            set
            {
                this.cityCodeField = value;
            }
        }

        /// <remarks/>
        public uint TelephoneNumber
        {
            get
            {
                return this.telephoneNumberField;
            }
            set
            {
                this.telephoneNumberField = value;
            }
        }

        /// <remarks/>
        public string CustomerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
            }
        }

        /// <remarks/>
        public ulong Tckn
        {
            get
            {
                return this.tcknField;
            }
            set
            {
                this.tcknField = value;
            }
        }

        /// <remarks/>
        public object Vkn
        {
            get
            {
                return this.vknField;
            }
            set
            {
                this.vknField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:xml-msdata")]
        public byte rowOrder
        {
            get
            {
                return this.rowOrderField;
            }
            set
            {
                this.rowOrderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1")]
        public string hasChanges
        {
            get
            {
                return this.hasChangesField;
            }
            set
            {
                this.hasChangesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class DocumentElement
    {

        private DocumentElementCustomerDetail[] customerDetailField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CustomerDetail")]
        public DocumentElementCustomerDetail[] CustomerDetail
        {
            get
            {
                return this.customerDetailField;
            }
            set
            {
                this.customerDetailField = value;
            }
        }
    }



}