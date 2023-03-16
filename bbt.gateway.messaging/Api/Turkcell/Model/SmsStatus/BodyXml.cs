using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Turkcell.Model.SmsStatus
{
    public class BodyXml
    {

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class STATUSRETURN
        {

            private decimal vERSIONField;

            private STATUSRETURNSTATUS_LIST sTATUS_LISTField;

            /// <remarks/>
            public decimal VERSION
            {
                get
                {
                    return this.vERSIONField;
                }
                set
                {
                    this.vERSIONField = value;
                }
            }

            /// <remarks/>
            public STATUSRETURNSTATUS_LIST STATUS_LIST
            {
                get
                {
                    return this.sTATUS_LISTField;
                }
                set
                {
                    this.sTATUS_LISTField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class STATUSRETURNSTATUS_LIST
        {

            private STATUSRETURNSTATUS_LISTSTATUS sTATUSField;

            /// <remarks/>
            public STATUSRETURNSTATUS_LISTSTATUS STATUS
            {
                get
                {
                    return this.sTATUSField;
                }
                set
                {
                    this.sTATUSField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class STATUSRETURNSTATUS_LISTSTATUS
        {

            private ulong mSGIDField;

            private byte mSGSTATField;

            private ulong dTIMEField;

            private object rEASONField;

            /// <remarks/>
            public ulong MSGID
            {
                get
                {
                    return this.mSGIDField;
                }
                set
                {
                    this.mSGIDField = value;
                }
            }

            /// <remarks/>
            public byte MSGSTAT
            {
                get
                {
                    return this.mSGSTATField;
                }
                set
                {
                    this.mSGSTATField = value;
                }
            }

            /// <remarks/>
            public ulong DTIME
            {
                get
                {
                    return this.dTIMEField;
                }
                set
                {
                    this.dTIMEField = value;
                }
            }

            /// <remarks/>
            public object REASON
            {
                get
                {
                    return this.rEASONField;
                }
                set
                {
                    this.rEASONField = value;
                }
            }
        }


    }
}
