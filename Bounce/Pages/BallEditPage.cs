using System;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Bounce.Pages
{
	public class BallEditPage : ContentPage
	{
		public event EventHandler<string> BallImageUpdated;

		private SKCanvasView _canvasV = null;
		private SKMatrix _m = SKMatrix.MakeIdentity();
		private SKMatrix _startPanM = SKMatrix.MakeIdentity();
		private SKMatrix _startPinchM = SKMatrix.MakeIdentity();
		private Point _startPinchAnchor = Point.Zero;
		private float _totalPinchScale = 1f;
		private float _screenScale;
		private SKBitmap _bitmap = null;
		private string _ballFilename = null;
		private bool _doSave = false;

		public BallEditPage(string ballFilename)
		{
			_ballFilename = ballFilename;
#if __ANDROID__
            _screenScale = ((Android.App.Activity)Forms.Context).Resources.DisplayMetrics.Density;
#else
			_screenScale = (float)UIKit.UIScreen.MainScreen.Scale;
#endif
			Title = "Ball Edit";
			ToolbarItem saveTBI = new ToolbarItem {
				Text = "Save"
			};
			saveTBI.Clicked += (object sender, EventArgs e) => {
				//Save part of ball image inside circle
				_doSave = true;
				_canvasV.InvalidateSurface();
			};
			ToolbarItems.Add(saveTBI);
			_canvasV = new SKCanvasView();
			_canvasV.PaintSurface += HandlePaintCanvas;
			Content = _canvasV;
			//Load assets
			_bitmap = SKBitmap.Decode(_ballFilename);
			//Interaction
			PanGestureRecognizer pgr = new PanGestureRecognizer();
			pgr.PanUpdated += HandlePan;
			_canvasV.GestureRecognizers.Add(pgr);
			PinchGestureRecognizer pngr = new PinchGestureRecognizer();
			pngr.PinchUpdated += HandlePinch;
			_canvasV.GestureRecognizers.Add(pngr);
		}

		private void HandlePan(object sender, PanUpdatedEventArgs puea)
		{
			switch (puea.StatusType) {
			case GestureStatus.Started:
				_startPanM = _m;
				break;
			case GestureStatus.Running:
				float canvasTotalX = (float)puea.TotalX * _screenScale;
				float canvasTotalY = (float)puea.TotalY * _screenScale;
				SKMatrix canvasTranslation = SKMatrix.MakeTranslation(canvasTotalX, canvasTotalY);
				SKMatrix.Concat(ref _m, ref canvasTranslation, ref _startPanM);
				_canvasV.InvalidateSurface();
				break;
			default:
				_startPanM = SKMatrix.MakeIdentity();
				break;
			}
		}

		private void HandlePinch(object sender, PinchGestureUpdatedEventArgs puea)
		{
			Point canvasAnchor = new Point(puea.ScaleOrigin.X * _canvasV.Width * _screenScale,
										   puea.ScaleOrigin.Y * _canvasV.Height * _screenScale);
			switch (puea.Status) {
			case GestureStatus.Started:
				_startPinchM = _m;
				_startPinchAnchor = canvasAnchor;
				_totalPinchScale = 1f;
				break;
			case GestureStatus.Running:
				_totalPinchScale *= (float)puea.Scale;
				SKMatrix canvasScaling = SKMatrix.MakeScale(_totalPinchScale, _totalPinchScale, (float)_startPinchAnchor.X, (float)_startPinchAnchor.Y);
				SKMatrix.Concat(ref _m, ref canvasScaling, ref _startPinchM);
				_canvasV.InvalidateSurface();
				break;
			default:
				_startPinchM = SKMatrix.MakeIdentity();
				_startPinchAnchor = Point.Zero;
				_totalPinchScale = 1f;
				break;
			}
		}

		private async void HandlePaintCanvas(object sender, SKPaintSurfaceEventArgs e)
		{
			e.Surface.Canvas.Clear();
			if (_doSave) {
				using (var hole = new SKPath()) {
					hole.AddCircle(e.Info.Width / 2, e.Info.Height / 2, e.Info.Width / 3);
					e.Surface.Canvas.ClipPath(hole, SKClipOperation.Intersect, true);
				}
			}
			e.Surface.Canvas.SetMatrix(_m);
			//Draw ball image
			SKSize imgSize = new SKSize(_bitmap.Width, _bitmap.Height);
			SKRect aspectRect = SKRect.Create(e.Info.Width, e.Info.Height).AspectFit(imgSize);
			e.Surface.Canvas.DrawBitmap(_bitmap, aspectRect);
			if (!_doSave) {
				e.Surface.Canvas.ResetMatrix();
				//Draw circle overlay
				using (var frame = new SKPath())
				using (var hole = new SKPath()) {
					frame.AddRect(e.Info.Rect);
					hole.AddCircle(e.Info.Width / 2, e.Info.Height / 2, e.Info.Width / 3);
					SKPath frameHole = frame.Op(hole, SKPathOp.Difference);
					using (var paint = new SKPaint()) {
						paint.IsAntialias = true;
						paint.Style = SKPaintStyle.Fill;
						paint.Color = new SKColor(128, 128, 128, 200);
						e.Surface.Canvas.DrawPath(frameHole, paint);
					}
				}
			} else {
				SKImage snapI = e.Surface.Snapshot();
				snapI = snapI.Subset(e.Surface.Canvas.ClipDeviceBounds);
				SKData pngImage = snapI.Encode();
				File.WriteAllBytes(_ballFilename, pngImage.ToArray());
				await Navigation.PopAsync();
				OnBallImageUpdated(_ballFilename);
			}
		}

		private void OnBallImageUpdated(string ballFilename)
		{
			if (BallImageUpdated != null)
				BallImageUpdated(this, ballFilename);
		}
	}
}
