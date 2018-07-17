package za.co.technovolve.dlcserializerrsa;

public class DrivingLicenseCard
{
	private IdentityDocument identityDocument;
	private Person person;
	private DrivingLicense drivingLicense;
	private Card card;
	private ProfessionalDrivingPermit professionalDrivingPermit;
	private VehicleClass[] vehicleClasses;
	private CompressedImage photoData;
	private RawGreyscaleImage photo;
	
	static
	{
		System.loadLibrary("dlcserializerrsa");
	}
	
	public static native void activate(
		String packageName,
		byte[] license) throws Exception;
	
	public static native boolean isActive();
	
	public static native String getUniqueDeviceId();
	
	public static native String getVersion();
	
	public static native DrivingLicenseCard deserialize(byte[] buffer) throws Exception;
	
	private static native RawGreyscaleImage uncompress(
		byte[] buffer,
		boolean isTopDown) throws Exception; 
	
	public DrivingLicenseCard(
		IdentityDocument identityDocument,
		Person person,
		DrivingLicense drivingLicense,
		Card card,
		ProfessionalDrivingPermit professionalDrivingPermit,
		VehicleClass[] vehicleClasses,
		CompressedImage photoData)
	{
		this.identityDocument = identityDocument;
		this.person = person;
		this.drivingLicense = drivingLicense;
		this.card = card;
		this.professionalDrivingPermit = professionalDrivingPermit;
		this.vehicleClasses = vehicleClasses;
		this.photoData = photoData;
	}
			
	public IdentityDocument getIdentityDocument()
	{
		return identityDocument;
	}
	
	public Person getPerson()
	{
		return person;
	}

	public DrivingLicense getDrivingLicense()
	{
		return drivingLicense;
	}
	
	public Card getCard()
	{
		return card;
	}
	
	public ProfessionalDrivingPermit getProfessionalDrivingPermit()
	{
		return professionalDrivingPermit;
	}
	
	public VehicleClass[] getVehicleClasses()
	{
		return vehicleClasses;
	}
	
	public RawGreyscaleImage getPhoto() throws Exception
	{
		return (photo == null) 
			? photo = uncompress(photoData.getImageData(), photoData.getIsTopDownImage())
			: photo;
	}
}
