package za.co.technovolve.dlcserializerrsa;

import java.text.SimpleDateFormat;
import java.util.Date;

public final class DateFormatter
{
	private static final SimpleDateFormat formatter =
		new SimpleDateFormat("dd'/'MM'/'yyyy");
	
	public static Date parse(String value)
	{
		try
		{
			return formatter.parse(value);
		}
		catch (Exception e)
		{
			return null;
		}
	}
}
