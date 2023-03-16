using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Vodafone.Model.SendSms
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

            private sendSMSPacketResponse sendSMSPacketResponseField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://messaging.packet.services.system.sdf.oksijen.com")]
            public sendSMSPacketResponse sendSMSPacketResponse
            {
                get
                {
                    return this.sendSMSPacketResponseField;
                }
                set
                {
                    this.sendSMSPacketResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://messaging.packet.services.system.sdf.oksijen.com")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://messaging.packet.services.system.sdf.oksijen.com", IsNullable = false)]
        public partial class sendSMSPacketResponse
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

            private returnDeliveryResponseList deliveryResponseListField;

            private object descriptionField;

            private int errorCodeField;

            private string logIdField;

            private string packetIdField;

            private int resultField;

            private object serviceParametersField;

            /// <remarks/>
            public returnDeliveryResponseList deliveryResponseList
            {
                get
                {
                    return this.deliveryResponseListField;
                }
                set
                {
                    this.deliveryResponseListField = value;
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
            public int errorCode
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
            public int result
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
        public partial class returnDeliveryResponseList
        {

            private returnDeliveryResponseListDeliveryResponse deliveryResponseField;

            /// <remarks/>
            public returnDeliveryResponseListDeliveryResponse deliveryResponse
            {
                get
                {
                    return this.deliveryResponseField;
                }
                set
                {
                    this.deliveryResponseField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class returnDeliveryResponseListDeliveryResponse
        {

            private returnDeliveryResponseListDeliveryResponseDeliveryInfoList deliveryInfoListField;

            private string messageIdField;

            private object unifierField;

            /// <remarks/>
            public returnDeliveryResponseListDeliveryResponseDeliveryInfoList deliveryInfoList
            {
                get
                {
                    return this.deliveryInfoListField;
                }
                set
                {
                    this.deliveryInfoListField = value;
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
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object unifier
            {
                get
                {
                    return this.unifierField;
                }
                set
                {
                    this.unifierField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class returnDeliveryResponseListDeliveryResponseDeliveryInfoList
        {

            private returnDeliveryResponseListDeliveryResponseDeliveryInfoListDeliveryInfo deliveryInfoField;

            /// <remarks/>
            public returnDeliveryResponseListDeliveryResponseDeliveryInfoListDeliveryInfo deliveryInfo
            {
                get
                {
                    return this.deliveryInfoField;
                }
                set
                {
                    this.deliveryInfoField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class returnDeliveryResponseListDeliveryResponseDeliveryInfoListDeliveryInfo
        {

            private int errorCodeField;

            private ulong msisdnField;

            private returnDeliveryResponseListDeliveryResponseDeliveryInfoListDeliveryInfoSubscriberAttributes subscriberAttributesField;

            private string transactionIdField;

            /// <remarks/>
            public int errorCode
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
            public ulong msisdn
            {
                get
                {
                    return this.msisdnField;
                }
                set
                {
                    this.msisdnField = value;
                }
            }

            /// <remarks/>
            public returnDeliveryResponseListDeliveryResponseDeliveryInfoListDeliveryInfoSubscriberAttributes subscriberAttributes
            {
                get
                {
                    return this.subscriberAttributesField;
                }
                set
                {
                    this.subscriberAttributesField = value;
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

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class returnDeliveryResponseListDeliveryResponseDeliveryInfoListDeliveryInfoSubscriberAttributes
        {

            private object attributeField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
            public object attribute
            {
                get
                {
                    return this.attributeField;
                }
                set
                {
                    this.attributeField = value;
                }
            }
        }


    }
}
