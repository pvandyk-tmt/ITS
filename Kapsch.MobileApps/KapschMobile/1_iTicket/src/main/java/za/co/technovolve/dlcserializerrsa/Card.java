package za.co.technovolve.dlcserializerrsa;

import java.util.Date;

public class Card
{
	private String issueNumber;
	private Date validFrom;
	private Date validUntil;

	public Card(
		String issueNumber,
		String validFrom,
		String validUntil)
	{
		this.issueNumber = issueNumber;
		this.validFrom = DateFormatter.parse(validFrom);
		this.validUntil = DateFormatter.parse(validUntil);
	}
	
	public String getIssueNumber()
	{
		return issueNumber;
	}

	public Date getValidFrom()
	{
		return validFrom;
	}

	public Date getValidUntil()
	{
		return validUntil;
	}
}
