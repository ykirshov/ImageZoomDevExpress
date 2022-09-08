using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	class ImageZoomHelper : IDisposable
	{
		private Image _image;
		private Bitmap _bmp;
		private Size _sizeEdit;
		private int _zoomFacet = 300;
		private int _scale;
		private double _ratio = 0;
		private int _delta = 0;
		private byte _type;

		public Rectangle Rectangle { get; set; }
		public int Scale { get => _scale; }

		public ImageZoomHelper(int scale = 4)
		{
			_scale = scale;
		}

		public (Rectangle rect, Point flyLocation) CalculateLocation(Point location)
		{
			Point rLocation = new Point();
			Point flyLocation = new Point();
			Size pSize = new Size();
			Rectangle rect = new Rectangle();
			if (_image != null)
			{
				if (_type == 0)
				{
					if (location.Y > _delta && location.Y < _sizeEdit.Height - _delta)
					{
						Point offsetLocation = location;
						offsetLocation.X -= (int)(_zoomFacet / _ratio / 2);
						offsetLocation.Y -= (int)(_zoomFacet / _ratio / 2);
						offsetLocation.Y -= _delta;
						offsetLocation.X = (int)(offsetLocation.X * _ratio);
						offsetLocation.Y = (int)(offsetLocation.Y * _ratio);

						offsetLocation.X = offsetLocation.X + _zoomFacet > _image.Width
							? _image.Width - _zoomFacet : offsetLocation.X;
						offsetLocation.X = offsetLocation.X < 0 ? 0 : offsetLocation.X;
						offsetLocation.Y = offsetLocation.Y + _zoomFacet > _image.Height
							? _image.Height - _zoomFacet : offsetLocation.Y;
						offsetLocation.Y = offsetLocation.Y < 0 ? 0 : offsetLocation.Y;

						rLocation = location;
						rLocation.X -= (int)(_zoomFacet / _ratio / 2);
						rLocation.Y -= (int)(_zoomFacet / _ratio / 2);

						rLocation.X = rLocation.X + (int)(_zoomFacet / _ratio) > _sizeEdit.Width
							? _sizeEdit.Width - (int)(_zoomFacet / _ratio) : rLocation.X;
						rLocation.X = rLocation.X < 0 ? 0 : rLocation.X;
						rLocation.Y = rLocation.Y + (int)(_zoomFacet / _ratio) > _sizeEdit.Height - _delta
							? _sizeEdit.Height - (int)(_zoomFacet / _ratio) - _delta : rLocation.Y;
						rLocation.Y = rLocation.Y < _delta ? _delta : rLocation.Y;

						pSize = new Size((int)(_zoomFacet / _ratio), (int)(_zoomFacet / _ratio));
						Rectangle = new Rectangle(rLocation, pSize);

						rect = new Rectangle(new Point(offsetLocation.X, offsetLocation.Y), new Size(_zoomFacet, _zoomFacet));
						flyLocation = new Point(location.X - (int)(_zoomFacet / _ratio / 2), location.Y - (int)(_zoomFacet / _ratio / 2));
					}
					else
					{
						LocationReset();
					}
				}
				else
				{
					if (location.X > _delta && location.X < _sizeEdit.Width - _delta)
					{
						Point offsetLocation = location;
						offsetLocation.X -= (int)(_zoomFacet / _ratio / 2);
						offsetLocation.Y -= (int)(_zoomFacet / _ratio / 2);
						offsetLocation.X -= _delta;
						offsetLocation.X = (int)(offsetLocation.X * _ratio);
						offsetLocation.Y = (int)(offsetLocation.Y * _ratio);

						offsetLocation.X = offsetLocation.X + _zoomFacet > _image.Width
							? _image.Width - _zoomFacet : offsetLocation.X;
						offsetLocation.X = offsetLocation.X < 0 ? 0 : offsetLocation.X;
						offsetLocation.Y = offsetLocation.Y + _zoomFacet > _image.Height
							? _image.Height - _zoomFacet : offsetLocation.Y;
						offsetLocation.Y = offsetLocation.Y < 0 ? 0 : offsetLocation.Y;

						rLocation = location;
						rLocation.X -= (int)(_zoomFacet / _ratio / 2);
						rLocation.Y -= (int)(_zoomFacet / _ratio / 2);

						rLocation.X = rLocation.X + (int)(_zoomFacet / _ratio) > _sizeEdit.Width - _delta
							? _sizeEdit.Width - (int)(_zoomFacet / _ratio) - _delta : rLocation.X;
						rLocation.X = rLocation.X < _delta ? _delta : rLocation.X;
						rLocation.Y = rLocation.Y + (int)(_zoomFacet / _ratio) > _sizeEdit.Height
							? _sizeEdit.Height - (int)(_zoomFacet / _ratio) : rLocation.Y;
						rLocation.Y = rLocation.Y < 0 ? 0 : rLocation.Y;

						pSize = new Size((int)(_zoomFacet / _ratio), (int)(_zoomFacet / _ratio));
						Rectangle = new Rectangle(rLocation, pSize);

						rect = new Rectangle(new Point(offsetLocation.X, offsetLocation.Y), new Size(_zoomFacet, _zoomFacet));
						flyLocation = new Point(location.X - (int)(_zoomFacet / _ratio / 2), location.Y - (int)(_zoomFacet / _ratio / 2));
					}
					else
					{
						LocationReset();
					}
				}
			}
			return (rect, flyLocation);
		}

		public Size CalculateSize()
		{
			return new Size((int)(_zoomFacet / _ratio), (int)(_zoomFacet / _ratio));
		}

		public Bitmap CropImage(Rectangle cropArea)
		{
			return _bmp.Clone(cropArea, _image.PixelFormat);
		}

		public void LocationReset()
		{
			Rectangle = new Rectangle();
		}

		public void CalculateInputData(Image image, Size size)
		{
			if (image != null)
			{
				_bmp?.Dispose();
				_bmp = new Bitmap(image);
				_image = image;
				_zoomFacet = image.Width / _scale;
				_sizeEdit = size;
				double ratio_x = image.Width / (double)size.Width;
				double ratio_y = image.Height / (double)size.Height;
				if (ratio_x >= ratio_y)
				{
					_ratio = ratio_x;
					_delta = (int)((size.Height - image.Height / _ratio) / 2);
					_type = 0;
				}
				else
				{
					_ratio = ratio_y;
					_delta = (int)((size.Width - image.Width / _ratio) / 2);
					_type = 1;
				}
			}
		}

		public void Dispose()
		{
			_image?.Dispose();
			_bmp?.Dispose();
		}
	}
}
