package za.co.technovolve.dlcserializerrsa;

import java.nio.charset.Charset;
import java.security.InvalidParameterException;

import android.util.Xml.Encoding;

public class VehicleLicenseDisk
{
	private final String[] fields;
	
	private VehicleLicenseDisk(String[] fields)
	{
		this.fields = fields;
	}
	
	public static VehicleLicenseDisk parse(byte[] barcodeData)
	{
		try
		{
			String text = new String(
				barcodeData,
				Charset.forName("US-ASCII"));
			
			String[] fields = text.split("%");
			
			if (fields.length != 15) throw new InvalidParameterException();
			
			return new VehicleLicenseDisk(fields);	
		}
		catch (Exception e)
		{
			return null;
		}
	}
	
	public String getControlNumber()
	{
		return fields[5];
	}
	
	public String getLicenseNumber()
	{
		return fields[6];
	}
	
	public String getVehicleRegisterNumber()
	{
		return fields[7];
	}
	
	public String getVehicleDescription()
	{
		return fields[8];
	}
	
	public String getMake()
	{
		return fields[9];
	}
	
	public String getSeriesName()
	{
		return fields[10];
	}
	
	public String getColour()
	{
		return fields[11];
	}
	
	public String getVIN()
	{
		return fields[12];
	}
	
	public String getEngineNumber()
	{
		return fields[13];
	}
	
	public String getExpiryDate()
	{
		return fields[14];
	}
}
