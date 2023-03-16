using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Turkcell.Model.SendSms
{
    public class BodyXml
    {

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class MSGIDRETURN
        {

            private string vERSIONField;

            private MSGIDRETURNMSGID_LIST mSGID_LISTField;

            /// <remarks/>
            public string VERSION
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
            public MSGIDRETURNMSGID_LIST MSGID_LIST
            {
                get
                {
                    return this.mSGID_LISTField;
                }
                set
                {
                    this.mSGID_LISTField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class MSGIDRETURNMSGID_LIST
        {

            private long mSGIDField;

            /// <remarks/>
            public long MSGID
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
        }


    }
}
