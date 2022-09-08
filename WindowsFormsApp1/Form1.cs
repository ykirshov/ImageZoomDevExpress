using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		private List<string> _types = new List<string>
		{
			"Magnifier",
			"Rectangle"
		};

		private ImageZoomHelper _zoomHelper;
		private Bitmap _bmp, _cursorBmp;
		private SolidBrush _brush;
		private Pen _pen;

		private Image image = Properties.Resources.nature;
		private Image _cursorImage = Properties.Resources.search;
		private Cursor _cursorZoom;

		private IconType _type;

		public Form1()
		{
			InitializeComponent();
			comboType.Properties.Items.Clear();
			comboType.Properties.Items.AddRange(_types);
			comboType.SelectedIndex = 0;
			_zoomHelper = new ImageZoomHelper();
			_brush = new SolidBrush(Color.FromArgb(150, 255, 255, 255));
			_pen = new Pen(Color.Black, 2)
			{
				Alignment = PenAlignment.Inset
			};
			_cursorBmp = new Bitmap(_cursorImage);
			_cursorZoom = CursorCreator.CreateCursor(_cursorBmp, 50, 50);

			ImageZoomConfigurations();
		}

		private void PictureEdit_MouseEnter(object sender, EventArgs e)
		{
			PictureEdit picture = sender as PictureEdit;
			flyoutPanel1.Width = flyoutPanel1.Height = Screen.PrimaryScreen.Bounds.Width / _zoomHelper.Scale;

			_zoomHelper.CalculateInputData(picture.Image, picture.Size);
			Size size = _zoomHelper.CalculateSize();
			float scale = (float)size.Width / _cursorBmp.Width * 2;

			Image cursorResized = ImageHelper.ShrinkImage(_cursorBmp, scale); 
			_cursorZoom = CursorCreator.CreateCursor((Bitmap)cursorResized, (int)(size.Width/1.4), (int)(size.Height/1.5));
			picture.Cursor = _cursorZoom;
		}

		private void PictureEdit_Paint(object sender, PaintEventArgs e)
		{
            e.Graphics.FillRectangle(_brush, _zoomHelper.Rectangle);
            e.Graphics.DrawRectangle(_pen, _zoomHelper.Rectangle);
        }

		private void PictureEdit_MouseLeave(object sender, EventArgs e)
		{
			PictureEdit picture = sender as PictureEdit;
			picture.Cursor = Cursors.Default;
			flyoutPanel1.HidePopup();
			_zoomHelper.LocationReset();
			picture.Invalidate();
		}

		private void PictureEdit_MouseMove(object sender, MouseEventArgs e)
		{
			PictureEdit picture = sender as PictureEdit;
			var tuple = _zoomHelper.CalculateLocation(e.Location);
			if (!tuple.rect.IsEmpty)
			{
                switch (_type)
                {
                    case IconType.Magnifier:
                        picture.Cursor = _cursorZoom;
                        break;
                    case IconType.Rectangle:
                        picture.Cursor = Cursors.SizeAll;
                        break;
                    default:
                        break;
                }

				flyoutPanel1.ShowPopup();

				if (Cursor.Position.Y < flyoutPanel1.Height + 50)
				{
					if (e.Location.X < flyoutPanel1.Width + 50)
					{
						if (Cursor.Position.X > Screen.PrimaryScreen.Bounds.Width - flyoutPanel1.Width - 75)
						{
							(flyoutPanel1.FindForm()).Location = picture.PointToScreen(new Point(tuple.flyLocation.X - flyoutPanel1.Width, tuple.flyLocation.Y - Cursor.Position.Y + 50));
						}
						else
						{
							(flyoutPanel1.FindForm()).Location = picture.PointToScreen(new Point(tuple.flyLocation.X + _zoomHelper.Rectangle.Width, tuple.flyLocation.Y - Cursor.Position.Y + 50));
						}
					}
					else
					{
						(flyoutPanel1.FindForm()).Location = picture.PointToScreen(new Point(tuple.flyLocation.X - flyoutPanel1.Width, tuple.flyLocation.Y - Cursor.Position.Y + 50));
					}
				}
				else
				{
					if (e.Location.X < flyoutPanel1.Width + 50)
					{
						if (Cursor.Position.X > Screen.PrimaryScreen.Bounds.Width - flyoutPanel1.Width - 75)
						{
							(flyoutPanel1.FindForm()).Location = picture.PointToScreen(new Point(tuple.flyLocation.X - flyoutPanel1.Width, tuple.flyLocation.Y - flyoutPanel1.Height));
						}
						else
						{
							(flyoutPanel1.FindForm()).Location = picture.PointToScreen(new Point(tuple.flyLocation.X + _zoomHelper.Rectangle.Width, tuple.flyLocation.Y - flyoutPanel1.Height));
						}
					}
					else
					{
						(flyoutPanel1.FindForm()).Location = picture.PointToScreen(new Point(tuple.flyLocation.X - flyoutPanel1.Width, tuple.flyLocation.Y - flyoutPanel1.Height));
					}
				}
				_bmp?.Dispose();
				_bmp = _zoomHelper.CropImage(tuple.rect);
				pictureEdit2.Image = _bmp;
				picture.Invalidate();
			}
			else
			{
				picture.Cursor = Cursors.Default;
				flyoutPanel1.HidePopup();
				picture.Invalidate();
			}
		}

		private void comboType_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxEdit combo = sender as ComboBoxEdit;
			switch (combo.SelectedIndex)
			{
				case (int)IconType.Magnifier:
					_type = IconType.Magnifier;
					pictureEdit1.Paint -= PictureEdit_Paint;
					break;
				case (int)IconType.Rectangle:
					_type = IconType.Rectangle;
					pictureEdit1.Paint += PictureEdit_Paint;
					break;
				default:
					break;
			}
		}

		public void ImageZoomConfigurations()
		{
			pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
			pictureEdit2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

			pictureEdit1.Image = image;
			pictureEdit1.MouseEnter += PictureEdit_MouseEnter;
			pictureEdit1.MouseMove += PictureEdit_MouseMove;
			pictureEdit1.MouseLeave += PictureEdit_MouseLeave;

			flyoutPanel1.OwnerControl = this;
			flyoutPanel1.Size = new Size(400, 400);
			flyoutPanel1.Options.Location = new Point(550, 22);
			flyoutPanel1.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
			flyoutPanel1.Options.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Fade;

			pictureEdit1.Properties.ContextMenuStrip = new ContextMenuStrip();
		}
	}
}
