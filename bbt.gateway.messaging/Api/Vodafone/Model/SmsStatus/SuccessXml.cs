using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Vodafone.Model.SmsStatus
{
    public class SuccessXml
    {

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2003/05/soap-envelope", IsNullable = false)]
        public partial class Envelope
        {

            private EnvelopeBody bodyField;

            /// <remarks/>
            public EnvelopeBody Body
            {
                get
                {
                    return this.bodyField;
                }
                set
                {
                    this.bodyField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2003/05/soap-envelope")]
        public partial class EnvelopeBody
        {

            private queryPacketStatusResponse queryPacketStatusResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://messaging.packet.services.system.sdf.oksijen.com")]
            public queryPacketStatusResponse queryPacketStatusResponse
            {
                get
                {
                    return this.queryPacketStatusResponseField;
                }
                set
                {
                    this.queryPacketStatusResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://messaging.packet.services.system.sdf.oksijen.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://messaging.packet.services.system.sdf.oksijen.com", IsNullable = false)]
        public partial class queryPacketStatusResponse
        {

            private @return returnField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public @return @return
            {
                get
                {
                    return this.returnField;
                }
                set
                {
                    this.returnField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class @return
        {

            private returnDeliveryStatusList deliveryStatusListField;

            private object descriptionField;

            private byte errorCodeField;

            private string logIdField;

            private byte resultField;

            private object serviceParametersField;

            /// <remarks/>
            public returnDeliveryStatusList deliveryStatusList
            {
                get
                {
                    return this.deliveryStatusListField;
                }
                set
                {
                    this.deliveryStatusListField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }

            /// <remarks/>
            public byte errorCode
            {
                get
                {
                    return this.errorCodeField;
                }
                set
                {
                    this.errorCodeField = value;
                }
            }

            /// <remarks/>
            public string logId
            {
                get
                {
                    return this.logIdField;
                }
                set
                {
                    this.logIdField = value;
                }
            }

            /// <remarks/>
            public byte result
            {
                get
                {
                    return this.resultField;
                }
                set
                {
                    this.resultField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object serviceParameters
            {
                get
                {
                    return this.serviceParametersField;
                }
                set
                {
                    this.serviceParametersField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class returnDeliveryStatusList
        {

            private returnDeliveryStatusListDeliveryStatus deliveryStatusField;

            /// <remarks/>
            public returnDeliveryStatusListDeliveryStatus deliveryStatus
            {
                get
                {
                    return this.deliveryStatusField;
                }
                set
                {
                    this.deliveryStatusField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class returnDeliveryStatusListDeliveryStatus
        {

            private object cancelTimeField;

            private System.DateTime createTimeField;

            private System.DateTime deliveryTimeField;

            private object expiryTimeField;

            private string messageIdField;

            private string packetIdField;

            private object reasonField;

            private ushort statusField;

            private object submitTimeField;

            private string transactionIdField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object cancelTime
            {
                get
                {
                    return this.cancelTimeField;
                }
                set
                {
                    this.cancelTimeField = value;
                }
            }

            /// <remarks/>
            public System.DateTime createTime
            {
                get
                {
                    return this.createTimeField;
                }
                set
                {
                    this.createTimeField = value;
                }
            }

            /// <remarks/>
            public System.DateTime deliveryTime
            {
                get
                {
                    return this.deliveryTimeField;
                }
                set
                {
                    this.deliveryTimeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object expiryTime
            {
                get
                {
                    return this.expiryTimeField;
                }
                set
                {
                    this.expiryTimeField = value;
                }
            }

            /// <remarks/>
            public string messageId
            {
                get
                {
                    return this.messageIdField;
                }
                set
                {
                    this.messageIdField = value;
                }
            }

            /// <remarks/>
            public string packetId
            {
                get
                {
                    return this.packetIdField;
                }
                set
                {
                    this.packetIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object reason
            {
                get
                {
                    return this.reasonField;
                }
                set
                {
                    this.reasonField = value;
                }
            }

            /// <remarks/>
            public ushort status
            {
                get
                {
                    return this.statusField;
                }
                set
                {
                    this.statusField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object submitTime
            {
                get
                {
                    return this.submitTimeField;
                }
                set
                {
                    this.submitTimeField = value;
                }
            }

            /// <remarks/>
            public string transactionId
            {
                get
                {
                    return this.transactionIdField;
                }
                set
                {
                    this.transactionIdField = value;
                }
            }
        }


    }
}
