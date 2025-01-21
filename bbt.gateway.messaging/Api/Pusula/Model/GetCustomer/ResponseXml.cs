using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Pusula.Model.GetCustomer
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://intertech.com.tr/Pusula")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://intertech.com.tr/Pusula", IsNullable = false)]
    public partial class DataSet
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

        private CusInfo cusInfoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
        public CusInfo CusInfo
        {
            get
            {
                return this.cusInfoField;
            }
            set
            {
                this.cusInfoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class CusInfo
    {

        private CusInfoCustomerIndividual customerIndividualField;

        private CusInfoTelephones telephonesField;

        private CusInfoEmails[] emailsField;

        private CusInfoIntegrations integrationsField;

        private CusInfoAddressesNew[] addressesNewField;

        private CusInfoCorroboration corroborationField;

        private CusInfoCusInfoChannels cusInfoChannelsField;

        /// <remarks/>
        public CusInfoCustomerIndividual CustomerIndividual
        {
            get
            {
                return this.customerIndividualField;
            }
            set
            {
                this.customerIndividualField = value;
            }
        }

        /// <remarks/>
        public CusInfoTelephones Telephones
        {
            get
            {
                return this.telephonesField;
            }
            set
            {
                this.telephonesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Emails")]
        public CusInfoEmails[] Emails
        {
            get
            {
                return this.emailsField;
            }
            set
            {
                this.emailsField = value;
            }
        }

        /// <remarks/>
        public CusInfoIntegrations Integrations
        {
            get
            {
                return this.integrationsField;
            }
            set
            {
                this.integrationsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AddressesNew")]
        public CusInfoAddressesNew[] AddressesNew
        {
            get
            {
                return this.addressesNewField;
            }
            set
            {
                this.addressesNewField = value;
            }
        }

        /// <remarks/>
        public CusInfoCorroboration Corroboration
        {
            get
            {
                return this.corroborationField;
            }
            set
            {
                this.corroborationField = value;
            }
        }

        /// <remarks/>
        public CusInfoCusInfoChannels CusInfoChannels
        {
            get
            {
                return this.cusInfoChannelsField;
            }
            set
            {
                this.cusInfoChannelsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CusInfoCustomerIndividual
    {

        private ulong idField;

        private uint externalClientNoField;

        private string customerNameField;

        private string middleNameField;

        private string surnameField;

        private string shortNameField;

        private ushort channelNoField;

        private object taxOfficeField;

        private object taxOfficeNameField;

        private string birthDateField;

        private ulong citizenshipNumberField;

        private object drivingLicenceNoField;

        private string fatherNameField;

        private object iBSeriesField;

        private object iBSerialNoField;

        private byte identityTypeField;

        private string motherNameField;

        private object taxNoField;

        private string birthPlaceField;

        private byte birthPlaceCityCodeField;

        private string birthPlaceCityNameField;

        private object birthPlaceTownCodeField;

        private object birthPlaceTownNameField;

        private string genderField;

        private string maritalStatusField;

        private string motherMaidenSurnameField;

        private byte nationalityField;

        private string nationalityNameField;

        private object referencePersonnelField;

        private object referencePersonnelNameField;

        private sbyte referenceCustomerField;

        private object referenceCustomerNameField;

        private byte residenceCountryField;

        private string residenceCountryNameField;

        private string hasResidenceLicenceField;

        private string portfolioCodeField;

        private object investmentRepresentativeCodeField;

        private object investmentRepresentativeNameField;

        private string identityNoField;

        private string firstRelationDateField;

        private string customerProfileField;

        private string hasDrivingLicenceField;

        private byte creditCardExtreAddressField;

        private byte creditCardDeliveryAddressField;

        private byte connectAddressField;

        private byte aTMCardFlagField;

        private string accountFlagField;

        private sbyte aTMCardGroupField;

        private byte aTMCustomerGroupField;

        private string aTMCustomerGroupNameField;

        private sbyte creditCardGroupField;

        private object creditCardGroupNameField;

        private byte iBRegisteredCityField;

        private string iBRegisteredCityNameField;

        private string iBRegisteredTownField;

        private string iBQuarterField;

        private byte iBVolumeField;

        private byte iBFamilyOrderNoField;

        private byte iBOrderNoField;

        private object iBPlaceGivenField;

        private string givingReasonField;

        private byte iBRegisterNoField;

        private string dateGivenField;

        private string netSalaryField;

        private string totalIncomeField;

        private string totalExpenseField;

        private object familyNetSalaryField;

        private string familyTotalIncomeField;

        private string familyTotalExpenseField;

        private byte employeeCountField;

        private string personalCommercialField;

        private byte unitStatusField;

        private string isCompanyField;

        private object companyTitleField;

        private string companyDateField;

        private string workingCompanyField;

        private byte workStatusField;

        private object firmNameField;

        private ushort portfolioManagerField;

        private sbyte educationStatusField;

        private object primarySchoolNameField;

        private byte quantisExtreAddressField;

        private object marriageDateField;

        private byte customerTypeField;

        private ushort mainBranchCodeField;

        private string isTarisYatirimCustomerField;

        private sbyte segmentCodeField;

        private sbyte residenceStatusField;

        private object warningMessageField;

        private object errorMessageField;

        private string recordStatusField;

        private object subSegmentField;

        private byte workTitleField;

        private byte workAreaField;

        private byte workingStatusField;

        private byte farmerField;

        private string coreBankingServiceAgreementField;

        private ushort coreBankingServiceAgreementNoField;

        private sbyte sectorField;

        private object sectorNameField;

        private CusInfoCustomerIndividualMainSectorCode mainSectorCodeField;

        private object mainSectorCodeNameField;

        private CusInfoCustomerIndividualSubSectorCode subSectorCodeField;

        private object subSectorCodeNameField;

        private CusInfoCustomerIndividualSectorActivity sectorActivityField;

        private object sectorActivityNameField;

        private object balanceSystemField;

        private sbyte creditGroupCodeField;

        private object creditGroupNameField;

        private object creditAllocationDepartmentField;

        private string businessLineField;

        private object salaryFirmCodeField;

        private object payrollAccountField;

        private object retiredSalaryAccCheckDateField;

        private string iBTCKKSeriesNoField;

        private object iBTransientNoField;

        private System.DateTime iBExpireDateField;

        private string isHandicapField;

        private object handicapRateField;

        private object handicapTypeField;

        private string updatedByField;

        private string isPrivateBankingField;

        private string actualSegmentField;

        private string idField1;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public ulong Id
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
        public uint ExternalClientNo
        {
            get
            {
                return this.externalClientNoField;
            }
            set
            {
                this.externalClientNoField = value;
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
        public string MiddleName
        {
            get
            {
                return this.middleNameField;
            }
            set
            {
                this.middleNameField = value;
            }
        }

        /// <remarks/>
        public string Surname
        {
            get
            {
                return this.surnameField;
            }
            set
            {
                this.surnameField = value;
            }
        }

        /// <remarks/>
        public string ShortName
        {
            get
            {
                return this.shortNameField;
            }
            set
            {
                this.shortNameField = value;
            }
        }

        /// <remarks/>
        public ushort ChannelNo
        {
            get
            {
                return this.channelNoField;
            }
            set
            {
                this.channelNoField = value;
            }
        }

        /// <remarks/>
        public object TaxOffice
        {
            get
            {
                return this.taxOfficeField;
            }
            set
            {
                this.taxOfficeField = value;
            }
        }

        /// <remarks/>
        public object TaxOfficeName
        {
            get
            {
                return this.taxOfficeNameField;
            }
            set
            {
                this.taxOfficeNameField = value;
            }
        }

        /// <remarks/>
        public string BirthDate
        {
            get
            {
                return this.birthDateField;
            }
            set
            {
                this.birthDateField = value;
            }
        }

        /// <remarks/>
        public ulong CitizenshipNumber
        {
            get
            {
                return this.citizenshipNumberField;
            }
            set
            {
                this.citizenshipNumberField = value;
            }
        }

        /// <remarks/>
        public object DrivingLicenceNo
        {
            get
            {
                return this.drivingLicenceNoField;
            }
            set
            {
                this.drivingLicenceNoField = value;
            }
        }

        /// <remarks/>
        public string FatherName
        {
            get
            {
                return this.fatherNameField;
            }
            set
            {
                this.fatherNameField = value;
            }
        }

        /// <remarks/>
        public object IBSeries
        {
            get
            {
                return this.iBSeriesField;
            }
            set
            {
                this.iBSeriesField = value;
            }
        }

        /// <remarks/>
        public object IBSerialNo
        {
            get
            {
                return this.iBSerialNoField;
            }
            set
            {
                this.iBSerialNoField = value;
            }
        }

        /// <remarks/>
        public byte IdentityType
        {
            get
            {
                return this.identityTypeField;
            }
            set
            {
                this.identityTypeField = value;
            }
        }

        /// <remarks/>
        public string MotherName
        {
            get
            {
                return this.motherNameField;
            }
            set
            {
                this.motherNameField = value;
            }
        }

        /// <remarks/>
        public object TaxNo
        {
            get
            {
                return this.taxNoField;
            }
            set
            {
                this.taxNoField = value;
            }
        }

        /// <remarks/>
        public string BirthPlace
        {
            get
            {
                return this.birthPlaceField;
            }
            set
            {
                this.birthPlaceField = value;
            }
        }

        /// <remarks/>
        public byte BirthPlaceCityCode
        {
            get
            {
                return this.birthPlaceCityCodeField;
            }
            set
            {
                this.birthPlaceCityCodeField = value;
            }
        }

        /// <remarks/>
        public string BirthPlaceCityName
        {
            get
            {
                return this.birthPlaceCityNameField;
            }
            set
            {
                this.birthPlaceCityNameField = value;
            }
        }

        /// <remarks/>
        public object BirthPlaceTownCode
        {
            get
            {
                return this.birthPlaceTownCodeField;
            }
            set
            {
                this.birthPlaceTownCodeField = value;
            }
        }

        /// <remarks/>
        public object BirthPlaceTownName
        {
            get
            {
                return this.birthPlaceTownNameField;
            }
            set
            {
                this.birthPlaceTownNameField = value;
            }
        }

        /// <remarks/>
        public string Gender
        {
            get
            {
                return this.genderField;
            }
            set
            {
                this.genderField = value;
            }
        }

        /// <remarks/>
        public string MaritalStatus
        {
            get
            {
                return this.maritalStatusField;
            }
            set
            {
                this.maritalStatusField = value;
            }
        }

        /// <remarks/>
        public string MotherMaidenSurname
        {
            get
            {
                return this.motherMaidenSurnameField;
            }
            set
            {
                this.motherMaidenSurnameField = value;
            }
        }

        /// <remarks/>
        public byte Nationality
        {
            get
            {
                return this.nationalityField;
            }
            set
            {
                this.nationalityField = value;
            }
        }

        /// <remarks/>
        public string NationalityName
        {
            get
            {
                return this.nationalityNameField;
            }
            set
            {
                this.nationalityNameField = value;
            }
        }

        /// <remarks/>
        public object ReferencePersonnel
        {
            get
            {
                return this.referencePersonnelField;
            }
            set
            {
                this.referencePersonnelField = value;
            }
        }

        /// <remarks/>
        public object ReferencePersonnelName
        {
            get
            {
                return this.referencePersonnelNameField;
            }
            set
            {
                this.referencePersonnelNameField = value;
            }
        }

        /// <remarks/>
        public sbyte ReferenceCustomer
        {
            get
            {
                return this.referenceCustomerField;
            }
            set
            {
                this.referenceCustomerField = value;
            }
        }

        /// <remarks/>
        public object ReferenceCustomerName
        {
            get
            {
                return this.referenceCustomerNameField;
            }
            set
            {
                this.referenceCustomerNameField = value;
            }
        }

        /// <remarks/>
        public byte ResidenceCountry
        {
            get
            {
                return this.residenceCountryField;
            }
            set
            {
                this.residenceCountryField = value;
            }
        }

        /// <remarks/>
        public string ResidenceCountryName
        {
            get
            {
                return this.residenceCountryNameField;
            }
            set
            {
                this.residenceCountryNameField = value;
            }
        }

        /// <remarks/>
        public string HasResidenceLicence
        {
            get
            {
                return this.hasResidenceLicenceField;
            }
            set
            {
                this.hasResidenceLicenceField = value;
            }
        }

        /// <remarks/>
        public string PortfolioCode
        {
            get
            {
                return this.portfolioCodeField;
            }
            set
            {
                this.portfolioCodeField = value;
            }
        }

        /// <remarks/>
        public object InvestmentRepresentativeCode
        {
            get
            {
                return this.investmentRepresentativeCodeField;
            }
            set
            {
                this.investmentRepresentativeCodeField = value;
            }
        }

        /// <remarks/>
        public object InvestmentRepresentativeName
        {
            get
            {
                return this.investmentRepresentativeNameField;
            }
            set
            {
                this.investmentRepresentativeNameField = value;
            }
        }

        /// <remarks/>
        public string IdentityNo
        {
            get
            {
                return this.identityNoField;
            }
            set
            {
                this.identityNoField = value;
            }
        }

        /// <remarks/>
        public string FirstRelationDate
        {
            get
            {
                return this.firstRelationDateField;
            }
            set
            {
                this.firstRelationDateField = value;
            }
        }

        /// <remarks/>
        public string CustomerProfile
        {
            get
            {
                return this.customerProfileField;
            }
            set
            {
                this.customerProfileField = value;
            }
        }

        /// <remarks/>
        public string HasDrivingLicence
        {
            get
            {
                return this.hasDrivingLicenceField;
            }
            set
            {
                this.hasDrivingLicenceField = value;
            }
        }

        /// <remarks/>
        public byte CreditCardExtreAddress
        {
            get
            {
                return this.creditCardExtreAddressField;
            }
            set
            {
                this.creditCardExtreAddressField = value;
            }
        }

        /// <remarks/>
        public byte CreditCardDeliveryAddress
        {
            get
            {
                return this.creditCardDeliveryAddressField;
            }
            set
            {
                this.creditCardDeliveryAddressField = value;
            }
        }

        /// <remarks/>
        public byte ConnectAddress
        {
            get
            {
                return this.connectAddressField;
            }
            set
            {
                this.connectAddressField = value;
            }
        }

        /// <remarks/>
        public byte ATMCardFlag
        {
            get
            {
                return this.aTMCardFlagField;
            }
            set
            {
                this.aTMCardFlagField = value;
            }
        }

        /// <remarks/>
        public string AccountFlag
        {
            get
            {
                return this.accountFlagField;
            }
            set
            {
                this.accountFlagField = value;
            }
        }

        /// <remarks/>
        public sbyte ATMCardGroup
        {
            get
            {
                return this.aTMCardGroupField;
            }
            set
            {
                this.aTMCardGroupField = value;
            }
        }

        /// <remarks/>
        public byte ATMCustomerGroup
        {
            get
            {
                return this.aTMCustomerGroupField;
            }
            set
            {
                this.aTMCustomerGroupField = value;
            }
        }

        /// <remarks/>
        public string ATMCustomerGroupName
        {
            get
            {
                return this.aTMCustomerGroupNameField;
            }
            set
            {
                this.aTMCustomerGroupNameField = value;
            }
        }

        /// <remarks/>
        public sbyte CreditCardGroup
        {
            get
            {
                return this.creditCardGroupField;
            }
            set
            {
                this.creditCardGroupField = value;
            }
        }

        /// <remarks/>
        public object CreditCardGroupName
        {
            get
            {
                return this.creditCardGroupNameField;
            }
            set
            {
                this.creditCardGroupNameField = value;
            }
        }

        /// <remarks/>
        public byte IBRegisteredCity
        {
            get
            {
                return this.iBRegisteredCityField;
            }
            set
            {
                this.iBRegisteredCityField = value;
            }
        }

        /// <remarks/>
        public string IBRegisteredCityName
        {
            get
            {
                return this.iBRegisteredCityNameField;
            }
            set
            {
                this.iBRegisteredCityNameField = value;
            }
        }

        /// <remarks/>
        public string IBRegisteredTown
        {
            get
            {
                return this.iBRegisteredTownField;
            }
            set
            {
                this.iBRegisteredTownField = value;
            }
        }

        /// <remarks/>
        public string IBQuarter
        {
            get
            {
                return this.iBQuarterField;
            }
            set
            {
                this.iBQuarterField = value;
            }
        }

        /// <remarks/>
        public byte IBVolume
        {
            get
            {
                return this.iBVolumeField;
            }
            set
            {
                this.iBVolumeField = value;
            }
        }

        /// <remarks/>
        public byte IBFamilyOrderNo
        {
            get
            {
                return this.iBFamilyOrderNoField;
            }
            set
            {
                this.iBFamilyOrderNoField = value;
            }
        }

        /// <remarks/>
        public byte IBOrderNo
        {
            get
            {
                return this.iBOrderNoField;
            }
            set
            {
                this.iBOrderNoField = value;
            }
        }

        /// <remarks/>
        public object IBPlaceGiven
        {
            get
            {
                return this.iBPlaceGivenField;
            }
            set
            {
                this.iBPlaceGivenField = value;
            }
        }

        /// <remarks/>
        public string GivingReason
        {
            get
            {
                return this.givingReasonField;
            }
            set
            {
                this.givingReasonField = value;
            }
        }

        /// <remarks/>
        public byte IBRegisterNo
        {
            get
            {
                return this.iBRegisterNoField;
            }
            set
            {
                this.iBRegisterNoField = value;
            }
        }

        /// <remarks/>
        public string DateGiven
        {
            get
            {
                return this.dateGivenField;
            }
            set
            {
                this.dateGivenField = value;
            }
        }

        /// <remarks/>
        public string NetSalary
        {
            get
            {
                return this.netSalaryField;
            }
            set
            {
                this.netSalaryField = value;
            }
        }

        /// <remarks/>
        public string TotalIncome
        {
            get
            {
                return this.totalIncomeField;
            }
            set
            {
                this.totalIncomeField = value;
            }
        }

        /// <remarks/>
        public string TotalExpense
        {
            get
            {
                return this.totalExpenseField;
            }
            set
            {
                this.totalExpenseField = value;
            }
        }

        /// <remarks/>
        public object FamilyNetSalary
        {
            get
            {
                return this.familyNetSalaryField;
            }
            set
            {
                this.familyNetSalaryField = value;
            }
        }

        /// <remarks/>
        public string FamilyTotalIncome
        {
            get
            {
                return this.familyTotalIncomeField;
            }
            set
            {
                this.familyTotalIncomeField = value;
            }
        }

        /// <remarks/>
        public string FamilyTotalExpense
        {
            get
            {
                return this.familyTotalExpenseField;
            }
            set
            {
                this.familyTotalExpenseField = value;
            }
        }

        /// <remarks/>
        public byte EmployeeCount
        {
            get
            {
                return this.employeeCountField;
            }
            set
            {
                this.employeeCountField = value;
            }
        }

        /// <remarks/>
        public string PersonalCommercial
        {
            get
            {
                return this.personalCommercialField;
            }
            set
            {
                this.personalCommercialField = value;
            }
        }

        /// <remarks/>
        public byte UnitStatus
        {
            get
            {
                return this.unitStatusField;
            }
            set
            {
                this.unitStatusField = value;
            }
        }

        /// <remarks/>
        public string IsCompany
        {
            get
            {
                return this.isCompanyField;
            }
            set
            {
                this.isCompanyField = value;
            }
        }

        /// <remarks/>
        public object CompanyTitle
        {
            get
            {
                return this.companyTitleField;
            }
            set
            {
                this.companyTitleField = value;
            }
        }

        /// <remarks/>
        public string CompanyDate
        {
            get
            {
                return this.companyDateField;
            }
            set
            {
                this.companyDateField = value;
            }
        }

        /// <remarks/>
        public string WorkingCompany
        {
            get
            {
                return this.workingCompanyField;
            }
            set
            {
                this.workingCompanyField = value;
            }
        }

        /// <remarks/>
        public byte WorkStatus
        {
            get
            {
                return this.workStatusField;
            }
            set
            {
                this.workStatusField = value;
            }
        }

        /// <remarks/>
        public object FirmName
        {
            get
            {
                return this.firmNameField;
            }
            set
            {
                this.firmNameField = value;
            }
        }

        /// <remarks/>
        public ushort PortfolioManager
        {
            get
            {
                return this.portfolioManagerField;
            }
            set
            {
                this.portfolioManagerField = value;
            }
        }

        /// <remarks/>
        public sbyte EducationStatus
        {
            get
            {
                return this.educationStatusField;
            }
            set
            {
                this.educationStatusField = value;
            }
        }

        /// <remarks/>
        public object PrimarySchoolName
        {
            get
            {
                return this.primarySchoolNameField;
            }
            set
            {
                this.primarySchoolNameField = value;
            }
        }

        /// <remarks/>
        public byte QuantisExtreAddress
        {
            get
            {
                return this.quantisExtreAddressField;
            }
            set
            {
                this.quantisExtreAddressField = value;
            }
        }

        /// <remarks/>
        public object MarriageDate
        {
            get
            {
                return this.marriageDateField;
            }
            set
            {
                this.marriageDateField = value;
            }
        }

        /// <remarks/>
        public byte CustomerType
        {
            get
            {
                return this.customerTypeField;
            }
            set
            {
                this.customerTypeField = value;
            }
        }

        /// <remarks/>
        public ushort MainBranchCode
        {
            get
            {
                return this.mainBranchCodeField;
            }
            set
            {
                this.mainBranchCodeField = value;
            }
        }

        /// <remarks/>
        public string IsTarisYatirimCustomer
        {
            get
            {
                return this.isTarisYatirimCustomerField;
            }
            set
            {
                this.isTarisYatirimCustomerField = value;
            }
        }

        /// <remarks/>
        public sbyte SegmentCode
        {
            get
            {
                return this.segmentCodeField;
            }
            set
            {
                this.segmentCodeField = value;
            }
        }

        /// <remarks/>
        public sbyte ResidenceStatus
        {
            get
            {
                return this.residenceStatusField;
            }
            set
            {
                this.residenceStatusField = value;
            }
        }

        /// <remarks/>
        public object WarningMessage
        {
            get
            {
                return this.warningMessageField;
            }
            set
            {
                this.warningMessageField = value;
            }
        }

        /// <remarks/>
        public object ErrorMessage
        {
            get
            {
                return this.errorMessageField;
            }
            set
            {
                this.errorMessageField = value;
            }
        }

        /// <remarks/>
        public string RecordStatus
        {
            get
            {
                return this.recordStatusField;
            }
            set
            {
                this.recordStatusField = value;
            }
        }

        /// <remarks/>
        public object SubSegment
        {
            get
            {
                return this.subSegmentField;
            }
            set
            {
                this.subSegmentField = value;
            }
        }

        /// <remarks/>
        public byte WorkTitle
        {
            get
            {
                return this.workTitleField;
            }
            set
            {
                this.workTitleField = value;
            }
        }

        /// <remarks/>
        public byte WorkArea
        {
            get
            {
                return this.workAreaField;
            }
            set
            {
                this.workAreaField = value;
            }
        }

        /// <remarks/>
        public byte WorkingStatus
        {
            get
            {
                return this.workingStatusField;
            }
            set
            {
                this.workingStatusField = value;
            }
        }

        /// <remarks/>
        public byte Farmer
        {
            get
            {
                return this.farmerField;
            }
            set
            {
                this.farmerField = value;
            }
        }

        /// <remarks/>
        public string CoreBankingServiceAgreement
        {
            get
            {
                return this.coreBankingServiceAgreementField;
            }
            set
            {
                this.coreBankingServiceAgreementField = value;
            }
        }

        /// <remarks/>
        public ushort CoreBankingServiceAgreementNo
        {
            get
            {
                return this.coreBankingServiceAgreementNoField;
            }
            set
            {
                this.coreBankingServiceAgreementNoField = value;
            }
        }

        /// <remarks/>
        public sbyte Sector
        {
            get
            {
                return this.sectorField;
            }
            set
            {
                this.sectorField = value;
            }
        }

        /// <remarks/>
        public object SectorName
        {
            get
            {
                return this.sectorNameField;
            }
            set
            {
                this.sectorNameField = value;
            }
        }

        /// <remarks/>
        public CusInfoCustomerIndividualMainSectorCode MainSectorCode
        {
            get
            {
                return this.mainSectorCodeField;
            }
            set
            {
                this.mainSectorCodeField = value;
            }
        }

        /// <remarks/>
        public object MainSectorCodeName
        {
            get
            {
                return this.mainSectorCodeNameField;
            }
            set
            {
                this.mainSectorCodeNameField = value;
            }
        }

        /// <remarks/>
        public CusInfoCustomerIndividualSubSectorCode SubSectorCode
        {
            get
            {
                return this.subSectorCodeField;
            }
            set
            {
                this.subSectorCodeField = value;
            }
        }

        /// <remarks/>
        public object SubSectorCodeName
        {
            get
            {
                return this.subSectorCodeNameField;
            }
            set
            {
                this.subSectorCodeNameField = value;
            }
        }

        /// <remarks/>
        public CusInfoCustomerIndividualSectorActivity SectorActivity
        {
            get
            {
                return this.sectorActivityField;
            }
            set
            {
                this.sectorActivityField = value;
            }
        }

        /// <remarks/>
        public object SectorActivityName
        {
            get
            {
                return this.sectorActivityNameField;
            }
            set
            {
                this.sectorActivityNameField = value;
            }
        }

        /// <remarks/>
        public object BalanceSystem
        {
            get
            {
                return this.balanceSystemField;
            }
            set
            {
                this.balanceSystemField = value;
            }
        }

        /// <remarks/>
        public sbyte CreditGroupCode
        {
            get
            {
                return this.creditGroupCodeField;
            }
            set
            {
                this.creditGroupCodeField = value;
            }
        }

        /// <remarks/>
        public object CreditGroupName
        {
            get
            {
                return this.creditGroupNameField;
            }
            set
            {
                this.creditGroupNameField = value;
            }
        }

        /// <remarks/>
        public object CreditAllocationDepartment
        {
            get
            {
                return this.creditAllocationDepartmentField;
            }
            set
            {
                this.creditAllocationDepartmentField = value;
            }
        }

        /// <remarks/>
        public string BusinessLine
        {
            get
            {
                return this.businessLineField;
            }
            set
            {
                this.businessLineField = value;
            }
        }

        /// <remarks/>
        public object SalaryFirmCode
        {
            get
            {
                return this.salaryFirmCodeField;
            }
            set
            {
                this.salaryFirmCodeField = value;
            }
        }

        /// <remarks/>
        public object PayrollAccount
        {
            get
            {
                return this.payrollAccountField;
            }
            set
            {
                this.payrollAccountField = value;
            }
        }

        /// <remarks/>
        public object RetiredSalaryAccCheckDate
        {
            get
            {
                return this.retiredSalaryAccCheckDateField;
            }
            set
            {
                this.retiredSalaryAccCheckDateField = value;
            }
        }

        /// <remarks/>
        public string IBTCKKSeriesNo
        {
            get
            {
                return this.iBTCKKSeriesNoField;
            }
            set
            {
                this.iBTCKKSeriesNoField = value;
            }
        }

        /// <remarks/>
        public object IBTransientNo
        {
            get
            {
                return this.iBTransientNoField;
            }
            set
            {
                this.iBTransientNoField = value;
            }
        }

        /// <remarks/>
        public System.DateTime IBExpireDate
        {
            get
            {
                return this.iBExpireDateField;
            }
            set
            {
                this.iBExpireDateField = value;
            }
        }

        /// <remarks/>
        public string IsHandicap
        {
            get
            {
                return this.isHandicapField;
            }
            set
            {
                this.isHandicapField = value;
            }
        }

        /// <remarks/>
        public object HandicapRate
        {
            get
            {
                return this.handicapRateField;
            }
            set
            {
                this.handicapRateField = value;
            }
        }

        /// <remarks/>
        public object HandicapType
        {
            get
            {
                return this.handicapTypeField;
            }
            set
            {
                this.handicapTypeField = value;
            }
        }

        /// <remarks/>
        public string UpdatedBy
        {
            get
            {
                return this.updatedByField;
            }
            set
            {
                this.updatedByField = value;
            }
        }

        /// <remarks/>
        public string IsPrivateBanking
        {
            get
            {
                return this.isPrivateBankingField;
            }
            set
            {
                this.isPrivateBankingField = value;
            }
        }

        /// <remarks/>
        public string ActualSegment
        {
            get
            {
                return this.actualSegmentField;
            }
            set
            {
                this.actualSegmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1")]
        public string id
        {
            get
            {
                return this.idField1;
            }
            set
            {
                this.idField1 = value;
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
    public partial class CusInfoCustomerIndividualMainSectorCode
    {

        private string spaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CusInfoCustomerIndividualSubSectorCode
    {

        private string spaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CusInfoCustomerIndividualSectorActivity
    {

        private string spaceField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string space
        {
            get
            {
                return this.spaceField;
            }
            set
            {
                this.spaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class CusInfoTelephones
    {

        private byte telephoneTypeField;

        private string telefonTipiField;

        private byte countryCodeField;

        private string ülkeField;

        private ushort areaCodeField;

        private uint telephoneNumberField;

        private object extensionField;

        private string idField;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public byte TelephoneType
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
        [System.Xml.Serialization.XmlElementAttribute("Telefon Tipi")]
        public string TelefonTipi
        {
            get
            {
                return this.telefonTipiField;
            }
            set
            {
                this.telefonTipiField = value;
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
        public string Ülke
        {
            get
            {
                return this.ülkeField;
            }
            set
            {
                this.ülkeField = value;
            }
        }

        /// <remarks/>
        public ushort AreaCode
        {
            get
            {
                return this.areaCodeField;
            }
            set
            {
                this.areaCodeField = value;
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
        public object Extension
        {
            get
            {
                return this.extensionField;
            }
            set
            {
                this.extensionField = value;
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
    public partial class CusInfoEmails
    {

        private byte emailTypeField;

        private string emailTipiField;

        private string emailField;

        private string sendEkstreField;

        private string isVerifiedField;

        private string verifyDateField;

        private string idField;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public byte EmailType
        {
            get
            {
                return this.emailTypeField;
            }
            set
            {
                this.emailTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Email Tipi")]
        public string EmailTipi
        {
            get
            {
                return this.emailTipiField;
            }
            set
            {
                this.emailTipiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("E-mail")]
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public string SendEkstre
        {
            get
            {
                return this.sendEkstreField;
            }
            set
            {
                this.sendEkstreField = value;
            }
        }

        /// <remarks/>
        public string IsVerified
        {
            get
            {
                return this.isVerifiedField;
            }
            set
            {
                this.isVerifiedField = value;
            }
        }

        /// <remarks/>
        public string VerifyDate
        {
            get
            {
                return this.verifyDateField;
            }
            set
            {
                this.verifyDateField = value;
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
    public partial class CusInfoIntegrations
    {

        private ulong idField;

        private byte integrationTypeField;

        private ushort branchCodeField;

        private string userNameField;

        private System.DateTime recordTimeField;

        private string idField1;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public ulong Id
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
        public byte IntegrationType
        {
            get
            {
                return this.integrationTypeField;
            }
            set
            {
                this.integrationTypeField = value;
            }
        }

        /// <remarks/>
        public ushort BranchCode
        {
            get
            {
                return this.branchCodeField;
            }
            set
            {
                this.branchCodeField = value;
            }
        }

        /// <remarks/>
        public string UserName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime RecordTime
        {
            get
            {
                return this.recordTimeField;
            }
            set
            {
                this.recordTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1")]
        public string id
        {
            get
            {
                return this.idField1;
            }
            set
            {
                this.idField1 = value;
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
    public partial class CusInfoAddressesNew
    {

        private string addressTypeDescriptionField;

        private string cityNameField;

        private string countryNameField;

        private ulong idField;

        private byte addressTypeField;

        private byte countryCodeField;

        private byte cityCodeField;

        private string cityField;

        private string townField;

        private string districtField;

        private string streetField;

        private string addressDetailField;

        private sbyte longtitudeField;

        private sbyte latitudeField;

        private ushort postalCodeField;

        private byte validationLevelField;

        private System.DateTime validityStartDateField;

        private System.DateTime validityEndDateField;

        private System.DateTime recordTimeField;

        private ushort iLCE_IDField;

        private string idField1;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public string AddressTypeDescription
        {
            get
            {
                return this.addressTypeDescriptionField;
            }
            set
            {
                this.addressTypeDescriptionField = value;
            }
        }

        /// <remarks/>
        public string CityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
            }
        }

        /// <remarks/>
        public string CountryName
        {
            get
            {
                return this.countryNameField;
            }
            set
            {
                this.countryNameField = value;
            }
        }

        /// <remarks/>
        public ulong Id
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
        public byte AddressType
        {
            get
            {
                return this.addressTypeField;
            }
            set
            {
                this.addressTypeField = value;
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
        public byte CityCode
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
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string Town
        {
            get
            {
                return this.townField;
            }
            set
            {
                this.townField = value;
            }
        }

        /// <remarks/>
        public string District
        {
            get
            {
                return this.districtField;
            }
            set
            {
                this.districtField = value;
            }
        }

        /// <remarks/>
        public string Street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }

        /// <remarks/>
        public string AddressDetail
        {
            get
            {
                return this.addressDetailField;
            }
            set
            {
                this.addressDetailField = value;
            }
        }

        /// <remarks/>
        public sbyte Longtitude
        {
            get
            {
                return this.longtitudeField;
            }
            set
            {
                this.longtitudeField = value;
            }
        }

        /// <remarks/>
        public sbyte Latitude
        {
            get
            {
                return this.latitudeField;
            }
            set
            {
                this.latitudeField = value;
            }
        }

        /// <remarks/>
        public ushort PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public byte ValidationLevel
        {
            get
            {
                return this.validationLevelField;
            }
            set
            {
                this.validationLevelField = value;
            }
        }

        /// <remarks/>
        public System.DateTime ValidityStartDate
        {
            get
            {
                return this.validityStartDateField;
            }
            set
            {
                this.validityStartDateField = value;
            }
        }

        /// <remarks/>
        public System.DateTime ValidityEndDate
        {
            get
            {
                return this.validityEndDateField;
            }
            set
            {
                this.validityEndDateField = value;
            }
        }

        /// <remarks/>
        public System.DateTime RecordTime
        {
            get
            {
                return this.recordTimeField;
            }
            set
            {
                this.recordTimeField = value;
            }
        }

        /// <remarks/>
        public ushort ILCE_ID
        {
            get
            {
                return this.iLCE_IDField;
            }
            set
            {
                this.iLCE_IDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1")]
        public string id
        {
            get
            {
                return this.idField1;
            }
            set
            {
                this.idField1 = value;
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
    public partial class CusInfoCorroboration
    {

        private bool addressCheckField;

        private bool telephoneCheckField;

        private bool eMailCheckField;

        private bool registerRecordCheckField;

        private bool corroboratorsCheckField;

        private bool identityCheckField;

        private bool authorizedSignaturesCheckField;

        private bool casebookCheckField;

        private bool corroboratorsAuthorizationCheckField;

        private bool addressADNKSCheckField;

        private sbyte footballTeamField;

        private string idField;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public bool AddressCheck
        {
            get
            {
                return this.addressCheckField;
            }
            set
            {
                this.addressCheckField = value;
            }
        }

        /// <remarks/>
        public bool TelephoneCheck
        {
            get
            {
                return this.telephoneCheckField;
            }
            set
            {
                this.telephoneCheckField = value;
            }
        }

        /// <remarks/>
        public bool EMailCheck
        {
            get
            {
                return this.eMailCheckField;
            }
            set
            {
                this.eMailCheckField = value;
            }
        }

        /// <remarks/>
        public bool RegisterRecordCheck
        {
            get
            {
                return this.registerRecordCheckField;
            }
            set
            {
                this.registerRecordCheckField = value;
            }
        }

        /// <remarks/>
        public bool CorroboratorsCheck
        {
            get
            {
                return this.corroboratorsCheckField;
            }
            set
            {
                this.corroboratorsCheckField = value;
            }
        }

        /// <remarks/>
        public bool IdentityCheck
        {
            get
            {
                return this.identityCheckField;
            }
            set
            {
                this.identityCheckField = value;
            }
        }

        /// <remarks/>
        public bool AuthorizedSignaturesCheck
        {
            get
            {
                return this.authorizedSignaturesCheckField;
            }
            set
            {
                this.authorizedSignaturesCheckField = value;
            }
        }

        /// <remarks/>
        public bool CasebookCheck
        {
            get
            {
                return this.casebookCheckField;
            }
            set
            {
                this.casebookCheckField = value;
            }
        }

        /// <remarks/>
        public bool CorroboratorsAuthorizationCheck
        {
            get
            {
                return this.corroboratorsAuthorizationCheckField;
            }
            set
            {
                this.corroboratorsAuthorizationCheckField = value;
            }
        }

        /// <remarks/>
        public bool AddressADNKSCheck
        {
            get
            {
                return this.addressADNKSCheckField;
            }
            set
            {
                this.addressADNKSCheckField = value;
            }
        }

        /// <remarks/>
        public sbyte FootballTeam
        {
            get
            {
                return this.footballTeamField;
            }
            set
            {
                this.footballTeamField = value;
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
    public partial class CusInfoCusInfoChannels
    {

        private sbyte customerNoField;

        private string isRetiredCustomerField;

        private string retiredSalaryAccCheckDateField;

        private string hasExpireAttentionField;

        private int expireDayField;

        private string canLoginChannelField;

        private string expireDateField;

        private byte identityTypeField;

        private ulong citizenshipNumberField;

        private object iBSeriesField;

        private object iBSerialNoField;

        private string iBTCKKSeriesNoField;

        private object mersisNoField;

        private string gsmNoWaitingApproveField;

        private object iBTransientNoField;

        private System.DateTime iBExpireDateField;

        private string isHandicapField;

        private object handicapRateField;

        private object handicapTypeField;

        private string contacttoCustomerDateField;

        private string coreBankingServiceAgreementField;

        private ushort coreBankingServiceAgreementNoField;

        private string idField;

        private byte rowOrderField;

        private string hasChangesField;

        /// <remarks/>
        public sbyte CustomerNo
        {
            get
            {
                return this.customerNoField;
            }
            set
            {
                this.customerNoField = value;
            }
        }

        /// <remarks/>
        public string IsRetiredCustomer
        {
            get
            {
                return this.isRetiredCustomerField;
            }
            set
            {
                this.isRetiredCustomerField = value;
            }
        }

        /// <remarks/>
        public string RetiredSalaryAccCheckDate
        {
            get
            {
                return this.retiredSalaryAccCheckDateField;
            }
            set
            {
                this.retiredSalaryAccCheckDateField = value;
            }
        }

        /// <remarks/>
        public string HasExpireAttention
        {
            get
            {
                return this.hasExpireAttentionField;
            }
            set
            {
                this.hasExpireAttentionField = value;
            }
        }

        /// <remarks/>
        public int ExpireDay
        {
            get
            {
                return this.expireDayField;
            }
            set
            {
                this.expireDayField = value;
            }
        }

        /// <remarks/>
        public string CanLoginChannel
        {
            get
            {
                return this.canLoginChannelField;
            }
            set
            {
                this.canLoginChannelField = value;
            }
        }

        /// <remarks/>
        public string ExpireDate
        {
            get
            {
                return this.expireDateField;
            }
            set
            {
                this.expireDateField = value;
            }
        }

        /// <remarks/>
        public byte IdentityType
        {
            get
            {
                return this.identityTypeField;
            }
            set
            {
                this.identityTypeField = value;
            }
        }

        /// <remarks/>
        public ulong CitizenshipNumber
        {
            get
            {
                return this.citizenshipNumberField;
            }
            set
            {
                this.citizenshipNumberField = value;
            }
        }

        /// <remarks/>
        public object IBSeries
        {
            get
            {
                return this.iBSeriesField;
            }
            set
            {
                this.iBSeriesField = value;
            }
        }

        /// <remarks/>
        public object IBSerialNo
        {
            get
            {
                return this.iBSerialNoField;
            }
            set
            {
                this.iBSerialNoField = value;
            }
        }

        /// <remarks/>
        public string IBTCKKSeriesNo
        {
            get
            {
                return this.iBTCKKSeriesNoField;
            }
            set
            {
                this.iBTCKKSeriesNoField = value;
            }
        }

        /// <remarks/>
        public object MersisNo
        {
            get
            {
                return this.mersisNoField;
            }
            set
            {
                this.mersisNoField = value;
            }
        }

        /// <remarks/>
        public string GsmNoWaitingApprove
        {
            get
            {
                return this.gsmNoWaitingApproveField;
            }
            set
            {
                this.gsmNoWaitingApproveField = value;
            }
        }

        /// <remarks/>
        public object IBTransientNo
        {
            get
            {
                return this.iBTransientNoField;
            }
            set
            {
                this.iBTransientNoField = value;
            }
        }

        /// <remarks/>
        public System.DateTime IBExpireDate
        {
            get
            {
                return this.iBExpireDateField;
            }
            set
            {
                this.iBExpireDateField = value;
            }
        }

        /// <remarks/>
        public string IsHandicap
        {
            get
            {
                return this.isHandicapField;
            }
            set
            {
                this.isHandicapField = value;
            }
        }

        /// <remarks/>
        public object HandicapRate
        {
            get
            {
                return this.handicapRateField;
            }
            set
            {
                this.handicapRateField = value;
            }
        }

        /// <remarks/>
        public object HandicapType
        {
            get
            {
                return this.handicapTypeField;
            }
            set
            {
                this.handicapTypeField = value;
            }
        }

        /// <remarks/>
        public string ContacttoCustomerDate
        {
            get
            {
                return this.contacttoCustomerDateField;
            }
            set
            {
                this.contacttoCustomerDateField = value;
            }
        }

        /// <remarks/>
        public string CoreBankingServiceAgreement
        {
            get
            {
                return this.coreBankingServiceAgreementField;
            }
            set
            {
                this.coreBankingServiceAgreementField = value;
            }
        }

        /// <remarks/>
        public ushort CoreBankingServiceAgreementNo
        {
            get
            {
                return this.coreBankingServiceAgreementNoField;
            }
            set
            {
                this.coreBankingServiceAgreementNoField = value;
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

    public class PusulaCustomerRoot
    { 
        public PusulaCustomerInfo root { get; set; }
    }
    public class PusulaCustomerInfo
    {
        public string BusinessLine { get; set; }
        public int MainBranchCode { get; set; }
        public string CitizenshipNumber { get; set; }
        public string PortfolioCode { get; set; }
        public string CustomerProfile { get; set; }
    }

    public class PusulaPhoneRoot
    {
        public PusulaPhoneInfo root { get; set; }
    }
    public class PusulaPhoneInfo
    {
        public int TelephoneType { get; set; }
        public int CountryCode { get; set; }
        public int AreaCode { get; set; }
        public int TelephoneNumber { get; set; }
    }

    public class PusulaMailRoot
    {
        public PusulaMailInfo root { get; set; }
    }
    public class PusulaMailInfo
    {
        public int EmailType { get; set; }
        [JsonProperty(PropertyName = "E-mail")]
        public string Email { get; set; }
        public string IsVerified { get; set; }
    }

}
