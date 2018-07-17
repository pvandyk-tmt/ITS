using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Kapsch.Core.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // http://stackoverflow.com/questions/7924758/entity-framework-creates-a-plural-table-name-but-the-view-expects-a-singular-ta
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public IDbSet<Session> Sessions { get; set; }
        public IDbSet<Credential> Credentials { get; set; }
        public IDbSet<CredentialResetToken> CredentialResetTokens { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<VehiclePropeller> VehiclePropellers { get; set; }
        public IDbSet<VehicleFuelType> VehicleFuelType { get; set; }
        public IDbSet<Vehicle> Vehicles { get; set; }
        public IDbSet<VehicleMake> VehicleMakes { get; set; }
        public IDbSet<VehicleModel> VehicleModels { get; set; }
        public IDbSet<VehicleModelNumber> VehicleModelNumbers { get; set; }
        public IDbSet<VehicleCategory> VehicleCategories { get; set; }
        public IDbSet<VehicleType> VehicleTypes { get; set; }
        public IDbSet<VehicleColor> VehicleColors { get; set; }
        public IDbSet<VehicleTestBooking> VehicleTestBookings { get; set; }
        public IDbSet<VehicleTestResult> VehicleTestResults { get; set; }
        public IDbSet<VehicleTestQuestion> VehicleTestQuestions { get; set; }
        public IDbSet<VehicleTestQuestionAnswer> VehicleTestQuestionAnswers { get; set; }
        public IDbSet<TestType> TestTypes { get; set; }
        public IDbSet<TestCategory> TestCategories { get; set; }
        public IDbSet<VehicleCategoryTestType> VehicleCategoryTestTypes { get; set; }
        public IDbSet<IdentificationType> IdentificationTypes { get; set; }
        public IDbSet<CredentialSystemFunction> CredentialSystemFunctions { get; set; }
        public IDbSet<Application> Applications { get; set; }
        public IDbSet<Camera> Cameras { get; set; }
        public IDbSet<CameraStatus> DeviceStatuses { get; set; }
        public IDbSet<InfringementLocation> InfringementLocations { get; set; }
        public IDbSet<Region> Regions { get; set; }
        public IDbSet<District> Districts { get; set; }
        public IDbSet<Site> Sites { get; set; }
        public IDbSet<CameraLastStatistics> CameraLastStatistics { get; set; }
        public IDbSet<GatewayUsageLog> GatewayUsageLogs { get; set; }
        public IDbSet<Court> Courts { get; set; }
        public IDbSet<CourtRoom> CourtRooms { get; set; }
        public IDbSet<CourtDate> CourtDates { get; set; }
        public IDbSet<AddressInfo> AddressInfos { get; set; }
        public IDbSet<OffenceCode> OffenceCodes { get; set; }
        public IDbSet<OffenceSet> OffenceSets { get; set; }
        public IDbSet<OffenceRegister> OffenceRegister { get; set; }
        public IDbSet<Person> Persons { get; set; }
        public IDbSet<InfringementEvidence> InfringementEvidences { get; set; }
        public IDbSet<DistrictOffenceSet> DistrictOffenceSets { get; set; }
        public IDbSet<MobileDevice> MobileDevices { get; set; }
        public IDbSet<MobileDeviceItem> MobileDeviceItems { get; set; }
        public IDbSet<MobileDeviceConfigItem> MobileDeviceConfigItems { get; set; }
        public IDbSet<PublicHoliday> PublicHolidays { get; set; }
        public IDbSet<MobileDeviceDbScript> MobileDeviceDbScripts { get; set; }
        public IDbSet<MobileDeviceLocation> MobileDeviceLocations { get; set; }
        public IDbSet<SystemFunction> SystemFunctions { get; set; }
        public IDbSet<SystemRole> SystemRoles { get; set; }
        public IDbSet<SystemRoleFunction> SystemRoleFunctions { get; set; }
        public IDbSet<UserMobileDeviceActivity> UserMobileDeviceActivities { get; set; }
        public IDbSet<UserDistrict> UserDistricts { get; set; }
        public IDbSet<MobileDeviceApplication> MobileDeviceApplications { get; set; }
        public IDbSet<OffenceDescription> OffenceDescriptions { get; set; }
        public IDbSet<OffenceCodeOffenceDescription> OffenceCodeOffenceDescriptions { get; set; }
        public IDbSet<VosiAction> VosiActions { get; set; }
        public IDbSet<PaymentTerminal> PaymentTerminals { get; set; }
        public IDbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public IDbSet<PaymentTransactionItem> PaymentTransactionItems { get; set; }
        public IDbSet<EvidenceLog> EvidenceLogs { get; set; }
        public IDbSet<SpeedLog> SpeedLogs { get; set; }
        public IDbSet<ChargeInfo> ChargeInfos { get; set; }
        public IDbSet<HandWrittenCaptureLog> HandWrittenCaptures { get; set; }
        public IDbSet<Computer> Computers { get; set; }
        public IDbSet<ComputerConfigSetting> ComputerConfigSettings { get; set; }
        public IDbSet<GeneratedReferenceNumber> GeneratedReferenceNumbers { get; set; }
        public IDbSet<VosiActionCapture> VosiActionCaptures { get; set; }
        public IDbSet<Register> Registers { get; set; }
        public IDbSet<Account> Accounts { get; set; }
        public IDbSet<AccountTransaction> AccountTransactions { get; set; }
        public IDbSet<PaymentProviderRequest> PaymentProviderRequests { get; set; }
        public IDbSet<PaymentProviderQueueItem> PaymentProviderQueueItems { get; set; }
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<RollingRegister> RollingRegisters { get; set; }
        public IDbSet<RepresentationTransaction> RepresentationTransactions { get; set; }
        public IDbSet<Country> Countries { get; set; }
        public IDbSet<CorrespondenceItem> CorrespondenceItems { get; set; }
        public IDbSet<CorrespondenceRoute> CorrespondenceRoutes { get; set; }
        public IDbSet<CorrespondenceTemplate> CorrespondenceTemplates { get; set; }
        public IDbSet<CorrespondenceSmsPayload> CorrespondenceSmsPayloads { get; set; }
        public IDbSet<TISData> TISData { get; set; }
        public IDbSet<CorrespondenceEmailPayload> CorrespondenceEmailPayloads { get; set; }
        public IDbSet<NatisExport> NatisExports { get; set; }
        public IDbSet<ReferenceVehicle> ReferenceVehicles { get; set; }
        public IDbSet<OffenceRegulation> OffenceRegulations { get; set; }
        public IDbSet<OffenceCodeOffenceRegulation> OffenceCodeOffenceRegulations { get; set; }
    }
}
