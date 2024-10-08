﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Image = System.Drawing.Image;
using TaskDialogButton = Ookii.Dialogs.WinForms.TaskDialogButton;

namespace Titanium
{
	/// <summary>
	/// Just my library of small functions that makes c# WinForms programming easier.
	/// <para> Despite of the program license, THIS file is <see href="https://creativecommons.org/licenses/by-nc-sa/4.0">CC BY-NC-SA</see></para>
	/// <list type="table">
	/// <item>
	/// <term>Author</term>
	/// <see href="https://github.com/TuTAH1">Титан</see>
	/// </item>
	/// </list>
	/// </summary>
	public static class Forms
	{
		///  <item>
		/// <term>source</term>
		/// <see href="https://www.cyberforum.ru/wpf-silverlight/thread961899.html">SmirnoFF.Oleg</see>
		/// </item>
		public static T Clone<T>(this T controlToClone) 
			where T : Control
		{
			PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			T instance = Activator.CreateInstance<T>();

			foreach (PropertyInfo propInfo in controlProperties)
			{
				if (propInfo.CanWrite)
				{
					if(propInfo.Name != "WindowTarget")
						propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
				}
			}

			return instance;
		}
		
		
		///  <item>
		/// <term>source</term>
		/// <see href="https://stackoverflow.com/questions/3473597/it-is-possible-to-copy-all-the-properties-of-a-certain-control-c-window-forms">Stuart Helwig</see>
		/// </item>
		public static void CopyPropertiesTo(this Control sourceControl, Control targetControl)
		{
			// make sure these are the same
			if (sourceControl.GetType() != targetControl.GetType())
			{
				throw new ArgumentException("Incorrect control types");
			}

			foreach (PropertyInfo sourceProperty in sourceControl.GetType().GetProperties())
			{
				object newValue = sourceProperty.GetValue(sourceControl, null);

				MethodInfo mi = sourceProperty.GetSetMethod(true);
				if (mi != null)
				{
					sourceProperty.SetValue(targetControl, newValue, null);
				}
			}
		}

		public static int GetHeaderHeight(this Form F)
		{
			Rectangle screenRectangle = F.RectangleToScreen(F.ClientRectangle);
			return screenRectangle.Top - F.Top;
		}

		public static int GetWindowPadding(this Form F, Orientation Side
			) => (int)((Side == Orientation.Horizontal? 6 : 5) * (F.DeviceDpi/96.0));

		public static Size MeasureText(Label l) => TextRenderer.MeasureText(l.Text,l.Font);

		#region Image

			public static Image? Resize(this Image i, int NewDimensionValue, TypesFuncs.Dimension FixedDimension = TypesFuncs.Dimension.Height)
			{
				return i == null? null : new Bitmap(i, i.Size.Resize(NewDimensionValue, FixedDimension));
			}

		#endregion

		/*public static async Task ForegroundColorAnimation(this Control c, Color EndColor, int TimeMs)
		{
			double R = EndColor.R, G = EndColor.G, B = EndColor.B, A = EndColor.A;
			double R_ = c.ForeColor.R, G_ = c.ForeColor.G, B_ = c.ForeColor.B, A_ = c.ForeColor.A;
			int TimeNs = (TimeMs * 1000) / 256;
			Timer timer = new Timer();
			timer.Interval = 
			timer.Tick+=Timer_Tick(null);
			}*/

		/// <summary>
		/// Class that makes some *Action* on all depedent controlls if one/all text of depedent controls is/not valid
		/// </summary>
		public class DependentControls
		{
			private readonly LogicType _logic;
			private List<DependentControl> _controls;
			public bool isValid { get; private set; }
			private delegate void ControlValidHandler(DependentControl sender, bool isValid);
			public delegate void ValidHandler(bool isValid);
			public event ValidHandler ValidChanged;
			public Action<Control>  DefaultValidFalseAction;
			public Action<Control>  DefaultValidTrueAction;
			public Func<string, bool> DefaultValidCheck;
			public enum LogicType
			{
				AND,
				OR
			}

			/// <summary>
			/// All parametrs describes a default behavior for new added control
			/// </summary>
			/// <param name="ValidFalseAction">Action that executes when control.text is NOT valid</param>
			/// <param name="ValidTrueAction">Action that executes when control.text IS valid</param>
			/// <param name="ValidCheck">Function that checks is control.text valid</param>
			public DependentControls(LogicType Logic, Action<Control> ValidFalseAction = null, Action<Control> ValidTrueAction = null, Func<string,bool> ValidCheck = null)
			{
				_controls = new List<DependentControl>();
				_logic = Logic;
				DefaultValidFalseAction = ValidFalseAction;
				DefaultValidTrueAction = ValidTrueAction;
				DefaultValidCheck = ValidCheck;

			}


			public void Add(Control Control, Action<Control> ValidFalseAction= null, Action<Control> ValidTrueAction= null, Func<string,bool> ValidCheck = null)
			{
				ValidFalseAction ??= DefaultValidFalseAction ?? (_ => {}); 
				ValidTrueAction ??= DefaultValidTrueAction;
				ValidCheck ??= DefaultValidCheck;
				var newControl = new DependentControl(_logic, Control, ValidFalseAction, ValidTrueAction, ValidCheck);
				_controls.Add(newControl);
				newControl.ValidChanged += ValidChangeReaction;

				//newControl.CheckValidity();
			}

			public void AddRange(List<Control> Controls, Action<Control> ValidFalseAction = null, Action<Control> ValidTrueAction = null, Func<string, bool> ValidCheck = null) => 
				Controls.ForEach(Control => this.Add(Control, ValidFalseAction, ValidTrueAction, ValidCheck));

			private void ValidChangeReaction(object _sender, bool value)
			{
				var sender = _sender as DependentControl;
				switch (_logic)
				{
					case LogicType.AND:
					{
						if (value)
						{
							sender.ExecuteValidTrueAction();
							bool temp = _controls.All(x => x.IsValid);
							if(temp!= isValid) ValidChanged?.Invoke(temp);
							isValid = temp;


						}
						else
						{
							sender.ExecuteValidFalseAction();
							if(isValid) ValidChanged?.Invoke(false);
							isValid = false;

						}
					} break;
						
					case LogicType.OR:
					{
						if (value)
						{
							if (!isValid) ValidChanged?.Invoke(true);
							isValid = true;
							foreach (var c in _controls)
								c.ExecuteValidTrueAction();
						}
						else 
						{
							bool temp = false;
							foreach (var c in _controls)
							{
								if (c.IsValid) {temp = true; break;}
							}
							if(!temp) foreach (var c in _controls)
								c.ExecuteValidFalseAction();
							if(temp!=isValid) ValidChanged?.Invoke(temp);
							isValid = temp;
						}
					} break;
				}
			}

			class DependentControl
			{
				private LogicType _logic;
				internal Control Control;
				private readonly Control InitialControl;

				public bool IsValid { get; private set; }
				private event ControlValidHandler _validChanged;

				public event ControlValidHandler ValidChanged
				{
					add
					{
						_validChanged += value; 
						CheckValidity();
						_validChanged?.Invoke(this, IsValid);
					}
					remove => _validChanged -= value;
				}

				private Func<string, bool> _validCheck;
				private Action<Control> _validFalseAction;
				private Action<Control> _validTrueAction;

				public DependentControl(LogicType logic, Control Control, Action<Control> ValidFalseAction, Action<Control> ValidTrueAction = null, Func<string, bool> ValidCheck = null)
				{
					_logic = logic;
					this.Control = Control;
					InitialControl = Control.Clone();
					_validCheck = ValidCheck ?? (x => x != "");
					IsValid = _validCheck(Control.Text);
					_validFalseAction = ValidFalseAction;
					_validTrueAction = ValidTrueAction?? ((_control) =>  InitialControl.CopyPropertiesTo(_control));

					Control.TextChanged +=Control_TextChanged;
				}

				private void Control_TextChanged(object sender, EventArgs e) //TODO: можно сделать с таймером для оптимизации
				{
					CheckValidity();
				}

				internal void ExecuteValidTrueAction() => _validTrueAction(Control);
				internal void ExecuteValidFalseAction() => _validFalseAction(Control);

				public bool CheckValidity()
				{
					bool old = IsValid;
					IsValid = _validCheck(Control.Text);
					if (old != IsValid)
					{
						_validChanged?.Invoke(this, IsValid);
					}

					return (bool)IsValid;
				}
			}
		}

		/// <summary>
		/// Just Picture and text to the right to the picture. Not a standalone control because I don't really want to mess up with learning to how to render it, phew
		/// </summary>
		internal class PictureText : Control
		{
			private readonly PictureBox Picture;
			private readonly Label Label;

			public PictureText(Point Location, Bitmap Image,  int ImageHeight, string Text, int Spacing, VerticalAlignment TextAlignment = VerticalAlignment.Center)
			{
				this.Location = Location;

				Picture = new PictureBox();
				Picture.Location = Location;
				Picture.BackgroundImage = Image;
				Picture.Size = Image.Size.Resize(ImageHeight);
				Picture.BackgroundImageLayout = ImageLayout.Zoom;

				Label = new Label();
				Label.Text = Text;
				Label.Padding = Padding.Empty;
				Label.Margin = Padding.Empty;
				Label.Size = MeasureText(Label)+ new Size(4,0);
				Label.Location = new Point(
					Location.X + Spacing + Picture.Width,
					TextAlignment switch
					{
						VerticalAlignment.Top => Location.Y,
						VerticalAlignment.Center => Location.Y + ImageHeight/2 - Label.Size.Height/2,
						VerticalAlignment.Bottom => Location.Y + ImageHeight - Label.Size.Height,
						_ => throw new ArgumentOutOfRangeException(nameof(TextAlignment), TextAlignment, null)
					});

				Size = new(Picture.Width + Spacing + Label.Width, Math.Max(Picture.Height, Label.Height));
			}

			public void AddTo(Control control)
			{
				control.Controls.AddRange(new Control[]{Picture, Label});
			}
		}

		public class PictureTextCollection
		{
			// ReSharper disable once FieldCanBeMadeReadOnly.Local
			private List<PictureText> _items;

			//public List<PictureText> Items => _items;
			public Point Location { get; }
			public int Count => _items.Count;
			public int ImageHeight;
			public int ImageToTextDistance;
			public int ImageToImageDistance;
			public VerticalAlignment TextAlignment;
			public Size Size = Size.Empty;
			public int Width => Size.Width;
			public int Height => Size.Height;
			public Panel ParentPanel;

			public PictureTextCollection(Point Location, int ImageHeight, int ImageToTextDistance, int ImageToImageDistance, Panel ParentPanel, VerticalAlignment TextAlignment = VerticalAlignment.Center)
			{
				this.ImageHeight = ImageHeight;
				this.ImageToTextDistance = ImageToTextDistance;
				this.Location = Location;
				this.ImageToImageDistance = ImageToImageDistance;
				this.ParentPanel = ParentPanel;
				this.TextAlignment = TextAlignment;

				_items = new List<PictureText>();
			}

			public void Add(Bitmap Image, string Text)
			{
				Point newLocation = Count == 0 ? Location : new Point(Location.X, _items.Last().Location.Y + _items.Last().Size.Height + ImageToImageDistance);
				var item = new PictureText(newLocation, Image, ImageHeight, Text, ImageToTextDistance);
				_items.Add(item);
				item.AddTo(ParentPanel);
				Size = new Size(
					Math.Max(Width, item.Width),
					Height == 0? (item.Height) : (Height + ImageToImageDistance + item.Height));
			}

			public void AddRange(Bitmap[] Images, string[] Texts)
			{
				if (Images.Length != Texts.Length) throw new ArgumentException("Arrays should have same length");
				for (int i = 0; i < Images.Length; i++)
				{
					Add(Images[i], Texts[i]);	
				}
			}
		}

		//add function to static class Ookii.Dialogs.WinForms.TaskDialog TaskDialog
		
		public static TaskDialogButton Show(this Ookii.Dialogs.WinForms.TaskDialog TaskDialog, string Content = null, string Title = null, string Footer = null, params string[] ButtonTexts)
		{
			TaskDialog.WindowTitle = Title ?? "";
			TaskDialog.Content = Content;
			if (Footer != null) TaskDialog.Footer = Footer;
			//extract "close" localisation from resource

			foreach (var ButtonText in ButtonTexts)
			{
				TaskDialog.Buttons.Add(new Ookii.Dialogs.WinForms.TaskDialogButton(ButtonText));
			}


			return TaskDialog.ShowDialog();
		}
	}
}


