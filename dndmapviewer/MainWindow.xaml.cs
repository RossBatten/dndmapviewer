using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace dndmapviewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window , INotifyPropertyChanged
	{
		public Texture mapTexture = new Texture();
		//List<Texture> entityTextures;

		private string _mapFilename = "";
		private string _locationsFilename = "";
		private string _entitiesFilename = "";

		public Map Map = null;
		public ObservableCollection<Location> Locations { get; set; }
		public ObservableCollection<Entity> Entities { get; set; }
		public int SelectionL { get; set; }
		public int SelectionE { get; set; }
		public int SelectionTab { get; set; }

		private OpenGLHandlers _GLHandlers;
		private OpenGLHandlers _GLHandlersClone;
		

		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = this;
			
			_GLHandlers = new OpenGLHandlers(this, GLControl);

			Locations = new ObservableCollection<Location> { };
			Entities = new ObservableCollection<Entity> { };

			SelectionTab = 0;
			SelectionL = -1;
			SelectionE = -1;

			_GLHandlersClone = new OpenGLHandlers(this, GLControl);
			CloneWindow cloneWindow = new CloneWindow(_GLHandlersClone);
			cloneWindow.Show();
		}



		private void Color_Click(object sender, RoutedEventArgs e)
		{
			if (SelectionTab == 0 && SelectionL > -1)
			{
				ColorDialog MyDialog = new ColorDialog();
				MyDialog.FullOpen = true;
				if (MyDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Locations[SelectionL].color = new byte[] { MyDialog.Color.R, MyDialog.Color.G, MyDialog.Color.B };
				}
			}
			else if (SelectionE > -1)
			{
				ColorDialog MyDialog = new ColorDialog();
				MyDialog.FullOpen = true;
				if (MyDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Entities[SelectionE].color = new byte[] { MyDialog.Color.R, MyDialog.Color.G, MyDialog.Color.B };
				}
			}
		}

		private void Reset_Click(object sender, RoutedEventArgs e)
		{
			if (SelectionTab == 0 && SelectionL > -1)
			{
				Locations[SelectionL].position[0] = _GLHandlers.LookAt[0] / Map.map_width;
				Locations[SelectionL].position[1] = _GLHandlers.LookAt[1] / Map.map_width / _GLHandlers.AspectRatio;
			}
			else if (SelectionE > -1)
			{
				Entities[SelectionE].position[0] = _GLHandlers.LookAt[0] / Map.map_width;
				Entities[SelectionE].position[1] = _GLHandlers.LookAt[1] / Map.map_width / _GLHandlers.AspectRatio;
			}
		}

		private void TakeMe_Click(object sender, RoutedEventArgs e)
		{
			if (SelectionTab == 0 && SelectionL > -1)
			{
				_GLHandlers.LookAt[0] = Locations[SelectionL].position[0] * Map.map_width;
				_GLHandlers.LookAt[1] = Locations[SelectionL].position[1] * Map.map_width * _GLHandlers.AspectRatio;
				_GLHandlersClone.LookAt[0] = _GLHandlers.LookAt[0];
				_GLHandlersClone.LookAt[1] = _GLHandlers.LookAt[1];
			}
			else if (SelectionE > -1)
			{
				_GLHandlers.LookAt[0] = Entities[SelectionE].position[0] * Map.map_width;
				_GLHandlers.LookAt[1] = Entities[SelectionE].position[1] * Map.map_width * _GLHandlers.AspectRatio;
				_GLHandlersClone.LookAt[0] = _GLHandlers.LookAt[0];
				_GLHandlersClone.LookAt[1] = _GLHandlers.LookAt[1];
			}
		}

		private void Open_Map_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "map files|*.map.json";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_mapFilename = openFileDialog.FileName;

				string jsondata = File.ReadAllText(openFileDialog.FileName);
				Map = JsonHelper.ToClass<Map>(jsondata);

				//mapTexture.Create(GLControl.OpenGL, Map.image_filename);

				using (Bitmap image = new Bitmap(Map.image_filename))
				{
					if (image != null)
					{
						_GLHandlers.NewMap(image);
						_GLHandlersClone.NewMap(image);
					}
				}
			}
		}

		private void Open_Loc_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "location files|*.loc.json";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_locationsFilename = openFileDialog.FileName;

				string jsondata = File.ReadAllText(openFileDialog.FileName);
				Locations = JsonHelper.ToClass<ObservableCollection<Location>>(jsondata);
				OnPropertyChanged(nameof(Locations));
			}
		}

		private void Open_Ent_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "entity files|*.ent.json";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_entitiesFilename = openFileDialog.FileName;

				string jsondata = File.ReadAllText(openFileDialog.FileName);
				Entities = JsonHelper.ToClass<ObservableCollection<Entity>>(jsondata);
				OnPropertyChanged(nameof(Entities));
			}
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
			_GLHandlersClone.SourcePos = _GLHandlers.SourcePos;
		}

		public void MouseLeftDown(object sender, MouseButtonEventArgs args)
		{
			_GLHandlers.MouseLeftDown(sender, args);
		}

		public void MouseLeftUp(object sender, MouseButtonEventArgs args)
		{
			_GLHandlers.MouseLeftUp(sender, args);
		}

		public void MouseRightDown(object sender, MouseButtonEventArgs args)
		{
			_GLHandlers.MouseRightDown(sender, args);
			_GLHandlersClone.SourcePos = _GLHandlers.SourcePos;
		}

		public void MouseRightUp(object sender, MouseButtonEventArgs args)
		{
			_GLHandlers.MouseRightUp(sender, args);
			_GLHandlersClone.SourcePos = _GLHandlers.SourcePos;
			OnPropertyChanged(nameof(Locations));
			OnPropertyChanged(nameof(Entities));
		}

		public new void MouseMove(object sender, System.Windows.Input.MouseEventArgs args)
		{
			_GLHandlers.MouseMove(sender, args);
			_GLHandlersClone.LookAt[0] = _GLHandlers.LookAt[0];
			_GLHandlersClone.LookAt[1] = _GLHandlers.LookAt[1];
			_GLHandlersClone.TargetPos[0] = _GLHandlers.TargetPos[0];
			_GLHandlersClone.TargetPos[1] = _GLHandlers.TargetPos[1];
		}

		public new void MouseWheel(object sender, MouseWheelEventArgs args)
		{
			_GLHandlers.MouseWheel(sender, args);
			_GLHandlersClone.Ptodratio = _GLHandlers.Ptodratio;
			_GLHandlersClone.LookAt[0] = _GLHandlers.LookAt[0];
			_GLHandlersClone.LookAt[1] = _GLHandlers.LookAt[1];
		}

		#endregion


		#region Notification Changed Handler
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		private void New_Image_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "image files|*.png;*.tif";
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Map.image_filename = openFileDialog.FileName;

				//mapTexture.Create(GLControl.OpenGL, Map.image_filename);

				using (Bitmap image = new Bitmap(Map.image_filename))
				{
					
					if (image != null)
					{
						_GLHandlers.NewMap(image);
						_GLHandlersClone.NewMap(image);
					}
				}
			}
		}

		private void Save_Map_Click(object sender, RoutedEventArgs e)
		{
			string jsondata = JsonHelper.FromClass(Map);
			File.WriteAllText(_mapFilename, jsondata);
		}

		private void Save_Map_As_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "map files|*.map.json";
			if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_mapFilename = saveFileDialog.FileName;
				string jsondata = JsonHelper.FromClass(Map);
				File.WriteAllText(_mapFilename, jsondata);
			}
		}

		private void Save_Loc_Click(object sender, RoutedEventArgs e)
		{
			string jsondata = JsonHelper.FromClass(Locations);
			File.WriteAllText(_locationsFilename, jsondata);
		}

		private void Save_Ent_Click(object sender, RoutedEventArgs e)
		{
			string jsondata = JsonHelper.FromClass(Entities);
			File.WriteAllText(_entitiesFilename, jsondata);
		}

		private void Add_New_Loc_Click(object sender, RoutedEventArgs e)
		{
			double[] new_pos = new double[2];
			new_pos[0] = _GLHandlers.LookAt[0] / Map.map_width;
			new_pos[1] = _GLHandlers.LookAt[1] / Map.map_width / _GLHandlers.AspectRatio;
			Locations.Add(new Location( new_pos ) { name = "New location" });
			SelectionL = Locations.Count-1;
			OnPropertyChanged(nameof(Locations));
			OnPropertyChanged(nameof(SelectionL));
		}

		private void Add_New_Ent_Click(object sender, RoutedEventArgs e)
		{
			double[] new_pos = new double[2];
			new_pos[0] = _GLHandlers.LookAt[0] / Map.map_width;
			new_pos[1] = _GLHandlers.LookAt[1] / Map.map_width / _GLHandlers.AspectRatio;
			Entities.Add(new Entity(new_pos) { name = "New entity" });
			SelectionE = Entities.Count - 1;
			OnPropertyChanged(nameof(Entities));
			OnPropertyChanged(nameof(SelectionE));
		}

		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void Remove_Loc_Click(object sender, RoutedEventArgs e)
		{
			if (SelectionTab == 0 && SelectionL>= 0)
			{
				
				SelectionL--;
				OnPropertyChanged(nameof(SelectionL));
				Locations.RemoveAt(SelectionL+1);
				OnPropertyChanged(nameof(Locations));
				
			}
		}

		private void Remove_Ent_Click(object sender, RoutedEventArgs e)
		{
			if (SelectionTab == 1 && SelectionE >= 0)
			{
				Entities.RemoveAt(SelectionE);
				SelectionE--;
				OnPropertyChanged(nameof(SelectionE));
				OnPropertyChanged(nameof(Entities));
			}
		}
	}
}
