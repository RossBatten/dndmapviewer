using SharpGL.SceneGraph.Assets;
using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dndmapviewer
{
	public static class ImageFunctions
	{

		public static Bitmap CropImage(Bitmap source, Rectangle section)
		{
			// An empty bitmap which will hold the cropped image
			//Bitmap bmp = new Bitmap(section.Width, section.Height);

			//Graphics g = Graphics.FromImage(bmp);

			//// Draw the given area (section) of the source image
			//// at location 0,0 on the empty bitmap (bmp)
			//g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

			Bitmap CroppedImage = source.Clone(section, source.PixelFormat);

			return CroppedImage;
		}

		public static void TileImage(OpenGLControl GLControl,Bitmap image, out List<Texture> textures, out List<float[]> mapGrid, out List<float[]> mapTexCoords)
		{
			textures = new List<Texture> { };
			mapGrid = new List<float[]> { };
			mapTexCoords = new List<float[]> { };

			int maxpix = 500;

			for (int i = 0; i <= image.Size.Width / maxpix; i++)
			{
				for (int j = 0; j <= image.Size.Height / maxpix; j++)
				{
					int edge = 2;

					Point origin = new Point();
					Size size = new Size();
					float[] grid = new float[4];
					float[] texCoords = new float[4] { 0.0f, 1.0f, 0.0f, 1.0f };
					origin.X = i * maxpix;
					origin.Y = j * maxpix;
					grid[0] = ((float)i) / ((float)image.Size.Width) * maxpix;
					grid[1] = ((float)j) / ((float)image.Size.Height) * maxpix;
					if (i < image.Size.Width / maxpix)
					{
						size.Width = maxpix;
						grid[2] = (((float)i + 1) * maxpix) / ((float)image.Size.Width);
					}
					else
					{
						size.Width = image.Size.Width - i * maxpix;
						grid[2] = 1.0f;
					}
					if (j < image.Size.Height / maxpix)
					{
						size.Height = maxpix;
						grid[3] = (((float)j + 1) * maxpix) / ((float)image.Size.Height);
					}
					else
					{
						size.Height = image.Size.Height - j * maxpix;
						grid[3] = 1.0f;
					}

					//Modify x-texture coordinates
					if (origin.X >= edge && image.Size.Width - origin.X - size.Width > edge)
					{
						origin.X -= edge;
						size.Width += 2 * edge;
						texCoords[0] += (float)edge / (float)size.Width;
						texCoords[1] -= (float)edge / (float)size.Width;
					}
					else if (origin.X >= edge)
					{
						origin.X -= edge;
						size.Width += edge;
						texCoords[0] += (float)edge / (float)size.Width;
					}
					else if (image.Size.Width - origin.X - size.Width > edge)
					{
						size.Width += edge;
						texCoords[1] -= (float)edge / (float)size.Width;
					}

					//Modify y-texture coordinates
					if (origin.Y >= edge && image.Size.Height - origin.Y - size.Height > edge)
					{
						origin.Y -= edge;
						size.Height += 2 * edge;
						texCoords[2] += (float)edge / (float)size.Height;
						texCoords[3] -= (float)edge / (float)size.Height;
					}
					else if (origin.Y >= edge)
					{
						origin.Y -= edge;
						size.Height += edge;
						texCoords[2] += (float)edge / (float)size.Height;
					}
					else if (image.Size.Height - origin.Y - size.Height > edge)
					{
						size.Height += edge;
						texCoords[3] -= (float)edge / (float)size.Height;
					}

					grid[1] = 1 - grid[1];
					grid[3] = 1 - grid[3];

					Console.WriteLine(origin.X.ToString() + "," + (origin.X+size.Width).ToString());

					Bitmap croppedImage = CropImage(image, new Rectangle(origin, size));

					Texture texture = new Texture();
					texture.Create(GLControl.OpenGL, croppedImage);
					textures.Add(texture);
					mapGrid.Add(grid);
					mapTexCoords.Add(texCoords);

					
					
				}
			}
		}
	}
}
