using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace dndmapviewer
{
	public class OpenGLHandlers
	{

		private MainWindow _window;
		private OpenGLControl _GLControl;
		private OpenGL _gl;

		public List<Texture> mapTextures;
		//public List<Bitmap> mapBitmaps;
		public List<float[]> mapGrid = new List<float[]> { };
		public List<float[]> mapTexCoords = new List<float[]> { };

		private bool _leftDragging = false;
		private bool _rightDragging = false;
		private System.Windows.Point _lastPos; //In pixels from top left
		private System.Windows.Point _currPos;
		private double _windowwidth; //In metres
		private double _windowheight; //In metres
		public double[] LookAt; //In metres
		public double Ptodratio; //Pixels per metre
		public double AspectRatio;
		public double CrossHairRatio;
		public float PointSize;

		public OpenGLHandlers(MainWindow window, OpenGLControl GLControl )
		{
			_window = window;
			_GLControl = GLControl;
			PointSize = 10;
			LookAt = new double[] { 0, 0 };
			Ptodratio = 10000;
		}

		public void NewMap(Bitmap image)
		{
			AspectRatio = ((double)image.Size.Height) / ((double)image.Size.Width);
			//AspectRatio = (double)image.Size.Width / (double)image.Size.Height;
			//AspectRatio = 1;
			//LookAt = new double[2] { _window.Map.map_width / 2.0, _window.Map.map_width*AspectRatio / 2.0 };
			Ptodratio = _windowwidth / _window.Map.map_width;

			ImageFunctions.TileImage(_gl, image, out mapTextures, out mapGrid, out mapTexCoords);

			//foreach (float[] grid in mapGrid) {
			//	Console.Write(grid[0].ToString() + "-" + grid[2].ToString()+", ");
			//	Console.WriteLine(grid[1].ToString() + "-" + grid[3].ToString());
			//}

		}

		public void EDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{

			//  Clear the color and depth buffers.
			_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			if (_window.Map != null)
			{

				_gl.MatrixMode(OpenGL.GL_PROJECTION);
				_gl.LoadIdentity();
				_gl.Ortho(LookAt[0] - _gl.RenderContextProvider.Width / Ptodratio / 2.0,
					LookAt[0] + _gl.RenderContextProvider.Width / Ptodratio / 2.0,
					LookAt[1] - _gl.RenderContextProvider.Height / Ptodratio / 2.0,
					LookAt[1] + _gl.RenderContextProvider.Height / Ptodratio / 2.0,
					-100.0,
					100.0);

				//Console.WriteLine(_windowwidth);

				_gl.MatrixMode(OpenGL.GL_MODELVIEW);

				_gl.Enable(OpenGL.GL_TEXTURE_2D);
				_gl.Color(new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
				for (int i = 0; i< mapTextures.Count; i++)
				{
					mapTextures[i].Bind(_gl);

					_gl.LoadIdentity();
					_gl.Begin(OpenGL.GL_QUADS);
					_gl.TexCoord(mapTexCoords[i][0], mapTexCoords[i][2]);
					_gl.Vertex(mapGrid[i][0] * _window.Map.map_width,
						mapGrid[i][1] * _window.Map.map_width * AspectRatio, 0.0f);
					_gl.TexCoord(mapTexCoords[i][0], mapTexCoords[i][3]);
					_gl.Vertex(mapGrid[i][0] * _window.Map.map_width,
						mapGrid[i][3] * _window.Map.map_width * AspectRatio, 0.0f);
					_gl.TexCoord(mapTexCoords[i][1], mapTexCoords[i][3]);
					_gl.Vertex(mapGrid[i][2] * _window.Map.map_width,
						mapGrid[i][3] * _window.Map.map_width * AspectRatio, 0.0f);
					_gl.TexCoord(mapTexCoords[i][1], mapTexCoords[i][2]);
					_gl.Vertex(mapGrid[i][2] * _window.Map.map_width,
						mapGrid[i][1] * _window.Map.map_width*AspectRatio, 0.0f);
					_gl.End();

					mapTextures[i].Pop(_gl);
					//mapTextures[i].Destroy(gl);
				}
				_gl.PopMatrix();
				_gl.Disable(OpenGL.GL_TEXTURE_2D);

				_gl.LoadIdentity();
				_gl.PointSize(PointSize);
				_gl.Begin(OpenGL.GL_POINTS);
				foreach (Location loc in _window.Locations)
				{
					if (loc.visible)
					{
						_gl.Color(loc.color[0],loc.color[1],loc.color[2]);
						_gl.Vertex(loc.position[0]* _window.Map.map_width,
							loc.position[1]* _window.Map.map_width * AspectRatio, 0.0);
					}
				}
				foreach (Entity ent in _window.Entities)
				{
					_gl.Color(ent.color[0], ent.color[1], ent.color[2]);
					_gl.Vertex(ent.position[0] * _window.Map.map_width,
						ent.position[1] * _window.Map.map_width * AspectRatio, 0.0);
				}

				_gl.End();
				_gl.PopMatrix();

				foreach (Location loc in _window.Locations)
				{
					if (loc.label && loc.visible)
					{
						_gl.LoadIdentity();
						_gl.Translate(loc.position[0] * _window.Map.map_width + PointSize / Ptodratio,
							loc.position[1] * _window.Map.map_width * AspectRatio,
							0);
						_gl.Scale(PointSize*2 / Ptodratio, PointSize*2 / Ptodratio, 0);
						_gl.Color(loc.color[0], loc.color[1], loc.color[2]);
						_gl.DrawText3D("Arial Bold", 10, 0, 0, loc.name);
					}
				}

				foreach (Entity ent in _window.Entities)
				{
					if (ent.label)
					{
						_gl.LoadIdentity();
						_gl.Translate(ent.position[0] * _window.Map.map_width + PointSize / Ptodratio,
							ent.position[1] * _window.Map.map_width * AspectRatio,
							0);
						_gl.Scale(PointSize * 2 / Ptodratio, PointSize * 2 / Ptodratio, 0);
						_gl.Color(ent.color[0], ent.color[1], ent.color[2]);
						_gl.DrawText3D("Arial Bold", 10, 0, 0, ent.name);
					}
				}

				foreach (Entity ent in _window.Entities)
				{

					_gl.LoadIdentity();
					_gl.Begin(OpenGL.GL_LINE_LOOP);
					_gl.Color(ent.color[0], ent.color[1], ent.color[2]);
					for (float i = 0; i < 60;i+=1.0f)
					{
						_gl.Vertex(ent.position[0] * _window.Map.map_width + ent.speed*Math.Cos(i/60.0*Math.PI*2),
						ent.position[1] * _window.Map.map_width * AspectRatio + ent.speed * Math.Sin(i / 60.0 * Math.PI * 2),
						0.0);
					}
					_gl.End();

					_gl.LineStipple(4, 0xAAAA);
					_gl.Begin(OpenGL.GL_LINE_LOOP);
					for (float i = 0; i < 60; i += 1.0f)
					{
						_gl.Vertex(ent.position[0] * _window.Map.map_width + ent.rangeShort * Math.Cos(i / 60.0 * Math.PI * 2),
						ent.position[1] * _window.Map.map_width * AspectRatio + ent.rangeShort * Math.Sin(i / 60.0 * Math.PI * 2),
						0.0);
					}
					_gl.End();

					_gl.LineStipple(2, 0xAAAA);
					_gl.Begin(OpenGL.GL_LINE_LOOP);
					for (float i = 0; i < 60; i += 1.0f)
					{
						_gl.Vertex(ent.position[0] * _window.Map.map_width + ent.rangeLong * Math.Cos(i / 60.0 * Math.PI * 2),
						ent.position[1] * _window.Map.map_width * AspectRatio + ent.rangeLong * Math.Sin(i / 60.0 * Math.PI * 2),
						0.0);
					}
					_gl.End();

					_gl.LineStipple(2, 0xFFFF);
				}

				//Draw crosshairs
				_gl.LoadIdentity();
				_gl.Begin(OpenGL.GL_LINES);
				_gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
				_gl.Vertex(LookAt[0] - _windowwidth / Ptodratio / 2.0 * CrossHairRatio, LookAt[1], 0.0f);
				_gl.Vertex(LookAt[0] + _windowwidth / Ptodratio / 2.0 * CrossHairRatio, LookAt[1], 0.0f);
				_gl.Vertex(LookAt[0],LookAt[1] - _windowwidth / Ptodratio / 2.0 * CrossHairRatio, 0.0f);
				_gl.Vertex(LookAt[0], LookAt[1] + _windowwidth / Ptodratio / 2.0 * CrossHairRatio, 0.0f);
				_gl.End();
				_gl.PopMatrix();

				_gl.Flush();
			}

		}

		public void EInitialize(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			_gl = args.OpenGL;
			_windowwidth = _gl.RenderContextProvider.Width;
			_windowheight = _gl.RenderContextProvider.Height;
			//_windowwidth = _window.GLControl.Width;
			//_windowheight = _window.GLControl.Height;

			_gl.Disable(OpenGL.GL_DEPTH_TEST);
			_gl.Enable(OpenGL.GL_LINE_STIPPLE);
			_gl.Enable(OpenGL.GL_POINT_SIZE);
			_gl.Enable(OpenGL.GL_POINT_SMOOTH);
			_gl.Enable(OpenGL.GL_LINE_WIDTH);
			_gl.LineWidth(2.0f);

			CrossHairRatio = 0.1;
		}

		public void EResize(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			_windowwidth = _gl.RenderContextProvider.Width;
			_windowheight = _gl.RenderContextProvider.Height;
		}

		public void MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_leftDragging = false;
			_rightDragging = false;
		}

		public void MouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			_leftDragging = true;
			_lastPos = e.GetPosition(_GLControl);
		}

		public void MouseLeftUp(object sender, MouseButtonEventArgs e)
		{
			_leftDragging = false;
		}

		public void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (_leftDragging)
			{
				_currPos = e.GetPosition(_GLControl);
				LookAt[0] -= (_currPos.X - _lastPos.X) / Ptodratio;
				LookAt[1] += (_currPos.Y - _lastPos.Y) / Ptodratio;
				_lastPos = _currPos;
			}
		}

		public void MouseWheel(object sender, MouseWheelEventArgs e)
		{
			System.Windows.Point pos = e.GetPosition(_GLControl);

			double x = LookAt[0] + ((double)pos.X - _windowwidth / 2.0) / Ptodratio;
			double y = LookAt[1] + (_windowheight - (double)pos.Y - _windowheight / 2.0) / Ptodratio;

			if (e.Delta < 0) Ptodratio *= 0.92;
			else Ptodratio /= 0.92;

			LookAt[0] = x - ((double)pos.X - _windowwidth / 2.0) / Ptodratio;
			LookAt[1] = y - (_windowheight - (double)pos.Y - _windowheight / 2.0) / Ptodratio;

		}
	}
}
