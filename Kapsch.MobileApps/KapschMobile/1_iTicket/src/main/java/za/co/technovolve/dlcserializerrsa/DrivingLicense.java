package za.co.technovolve.dlcserializerrsa;

public class DrivingLicense
{
	private String certificateNumber;
	private String countryOfIssue;

	public DrivingLicense(
		String certificateNumber,
		String countryOfIssue)
	{
		this.certificateNumber = certificateNumber;
		this.countryOfIssue = countryOfIssue;
	}
	
	public String getCertificateNumber()
	{
		return certificateNumber;
	}

	public String getCountryOfIssue()
	{
		return countryOfIssue;
	}
}
