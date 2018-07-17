package za.co.technovolve.dlcserializerrsa;

public class CompressedImage
{
	private byte[] imageData;
	private boolean isTopDownImage;

	public CompressedImage(
		byte[] imageData,
		boolean isTopDownImage)	
	{
		this.imageData = imageData;
		this.isTopDownImage = isTopDownImage;
	}
	
	public byte[] getImageData()
	{
		return imageData;
	}
	
	public boolean getIsTopDownImage()
	{
		return isTopDownImage;
	}
}
