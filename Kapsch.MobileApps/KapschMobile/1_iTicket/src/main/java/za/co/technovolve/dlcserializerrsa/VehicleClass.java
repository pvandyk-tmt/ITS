package za.co.technovolve.dlcserializerrsa;

import java.util.Date;

public class VehicleClass
{
	private String code;
	private String vehicleRestriction;
	private Date firstIssueDate;

	public VehicleClass(
		String code,
		String vehicleRestriction,
		String firstIssueDate)
	{
		this.code = code;
		this.vehicleRestriction = vehicleRestriction;
		this.firstIssueDate = DateFormatter.parse(firstIssueDate);
	}
	
	public String getCode()
	{
		return code;
	}

	public String getVehicleRestriction()
	{
		return vehicleRestriction;
	}

	public Date getFirstIssueDate()
	{
		return firstIssueDate;
	}
}
