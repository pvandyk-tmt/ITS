package za.co.technovolve.dlcserializerrsa;

import java.util.Date;

public class Person
{
	private String surname;
	private String initials;
	private String[] driverRestrictions;
	private Date dateOfBirth;
	private String preferenceLanguage;
	private String gender;

	public Person(
		String surname,
		String initials,
		String[] driverRestrictions,
		String dateOfBirth,
		String preferenceLanguage,
		String gender)
	{
		this.surname = surname;
		this.initials = initials;
		this.driverRestrictions = driverRestrictions;
		this.dateOfBirth = DateFormatter.parse(dateOfBirth);
		this.preferenceLanguage = preferenceLanguage;
		this.gender = gender;
	}
	
	public String getSurname()
	{
		return surname;
	}

	public String getInitials()
	{
		return initials;
	}

	public String[] getDriverRestrictions()
	{
		return driverRestrictions;
	}

	public Date getDateOfBirth()
	{
		return dateOfBirth;
	}

	public String getPreferenceLanguage()
	{
		return preferenceLanguage;
	}

	public String getGender()
	{
		return gender;
	}
}
