using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace dndmapviewer
{
	/// <summary>
	/// Interaction logic for CloneWindow.xaml
	/// </summary>
	public partial class CloneWindow : Window
	{
		private OpenGLHandlers _GLHandlers;

		public CloneWindow(OpenGLHandlers inGLHandlers)
		{
			InitializeComponent();

			_GLHandlers = inGLHandlers;
		}

		#region OpenGLControl Handler Assignments

		public void EDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			_GLHandlers.EDraw(sender, args);
		}

		public void EInitialize(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			_GLHandlers.EInitialize(sender, args);
		}

		public void EResize(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
		{
			_GLHandlers.EResize(sender, args);
		}

		public new void MouseLeave(object sender, System.Windows.Input.MouseEventArgs args)
		{
			_GLHandlers.MouseLeave(sender, args);
		}

		public void MouseLeftDown(object sender, MouseButtonEventArgs args)
		{
			_GLHandlers.MouseLeftDown(sender, args);
		}

		public void MouseLeftUp(object sender, MouseButtonEventArgs args)
		{
			_GLHandlers.MouseLeftUp(sender, args);
		}

		public new void MouseMove(object sender, System.Windows.Input.MouseEventArgs args)
		{
			_GLHandlers.MouseMove(sender, args);
		}

		public new void MouseWheel(object sender, MouseWheelEventArgs args)
		{
			_GLHandlers.MouseWheel(sender, args);
		}

		#endregion
	}
}
