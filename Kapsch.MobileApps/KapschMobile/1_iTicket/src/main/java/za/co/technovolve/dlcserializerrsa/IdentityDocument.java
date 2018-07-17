package za.co.technovolve.dlcserializerrsa;

public class IdentityDocument
{
	private String number;
	private String type;
	private String countryOfIssue;

	public IdentityDocument(
		String number,
		String type,
		String countryOfIssue)
	{
		this.number = number;
		this.type = type;
		this.countryOfIssue = countryOfIssue;
	}
	
	public String getNumber()
	{
		return number;
	}

	public String getType()
	{
		return type;
	}

	public String getCountryOfIssue()
	{
		return countryOfIssue;
	}
}
