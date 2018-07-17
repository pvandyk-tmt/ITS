package za.co.technovolve.dlcserializerrsa;

import java.nio.IntBuffer;

import android.graphics.Bitmap;
import android.graphics.Bitmap.Config;

public class RawGreyscaleImage
{
	private int width;
	private int height;
	private byte[] pixels;
	
	public RawGreyscaleImage(
		int width,
		int height,
		byte[] pixels)
	{
		this.width = width;
		this.height = height;
		this.pixels = pixels;
	}

	public Bitmap toBitmap()
	{
		int[] array = new int[pixels.length];
		int alpha = (0xFF << 24);
		
		for (int i = 0; i < array.length; i++)
		{
			int pixel = pixels[i] & 0xFF;
			
			array[i] = alpha | (pixel << 16) | (pixel << 8) | pixel;
		}
		
		Bitmap bitmap = Bitmap.createBitmap(width, height, Config.ARGB_8888);
		
		bitmap.copyPixelsFromBuffer(IntBuffer.wrap(array));
		
		return bitmap;
	}
	
	public int getWidth()
	{
		return width;
	}

	public int getHeight()
	{
		return height;
	}

	public byte[] getPixels()
	{
		return pixels;
	}
}
