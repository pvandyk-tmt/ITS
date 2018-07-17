package za.co.technovolve.dlcserializerrsa;

import java.util.Date;

public class ProfessionalDrivingPermit
{
	private String category;
	private Date dateValidUntil;

	public ProfessionalDrivingPermit(
		String category,
		String dateValidUntil)
	{
		this.category = category;
		this.dateValidUntil = DateFormatter.parse(dateValidUntil);
	}
	
	public String getCategory()
	{
		return category;
	}

	public Date getDateValidUntil()
	{
		return dateValidUntil;
	}
}
