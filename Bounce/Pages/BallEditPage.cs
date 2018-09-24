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
		private SKBitmap _bitmap = null;
		private string _ballFilename = null;
		private bool _doSave = false;
        private SKPoint _lastPanPt = SKPoint.Empty;

        public BallEditPage(string ballFilename)
		{
			_ballFilename = ballFilename;
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

        private async void HandlePaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();
            if (_doSave) {
                using (var hole = new SKPath()) {
                    hole.AddCircle(info.Width / 2, info.Height / 2, info.Width / 3);
                    canvas.ClipPath(hole, antialias: true);
                }
            }
            canvas.SetMatrix(_m);
            //Draw ball image
            SKSize imgSize = new SKSize(_bitmap.Width, _bitmap.Height);
            SKRect aspectRect = SKRect.Create(info.Width, info.Height).AspectFit(imgSize);
            canvas.DrawBitmap(_bitmap, aspectRect);
            if (!_doSave) {
                canvas.ResetMatrix();
                //Draw circle overlay
                using (var frame = new SKPath())
                using (var hole = new SKPath()) {
                    frame.AddRect(info.Rect);
                    hole.AddCircle(info.Width / 2, info.Height / 2, info.Width / 3);
                    SKPath frameHole = frame.Op(hole, SKPathOp.Difference);
                    using (var p = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = new SKColor(128, 128, 128, 200) }) {
                        canvas.DrawPath(frameHole, p);
                    }
                }
            } else {
                SKImage snapI = e.Surface.Snapshot();
                snapI = snapI.Subset(canvas.DeviceClipBounds);
                SKData pngImage = snapI.Encode();
                File.WriteAllBytes(_ballFilename, pngImage.ToArray());
                await Navigation.PopAsync();
                OnBallImageUpdated(_ballFilename);
            }
        }

        private void HandlePan(object sender, PanUpdatedEventArgs e)
		{
            SKPoint panPt = ToUntransformedCanvasPt((float)e.TotalX, (float)e.TotalY);
            switch (e.StatusType) {
            case GestureStatus.Started:
                _lastPanPt = panPt;
                break;
            case GestureStatus.Running:
                SKPoint deltaTran = panPt - _lastPanPt;
                _lastPanPt = panPt;
                SKMatrix deltaM = SKMatrix.MakeTranslation(deltaTran.X, deltaTran.Y);
                SKMatrix.PostConcat(ref _m, deltaM);
                _canvasV.InvalidateSurface();
                break;
            }
        }

        private void HandlePinch(object sender, PinchGestureUpdatedEventArgs e)
		{
            switch (e.Status) {
            case GestureStatus.Running:
                SKPoint pivotPt = ToUntransformedCanvasPt((float)(e.ScaleOrigin.X * _canvasV.Width), (float)(e.ScaleOrigin.Y * _canvasV.Height));
                SKMatrix deltaM = SKMatrix.MakeScale((float)e.Scale, (float)e.Scale, pivotPt.X, pivotPt.Y);
                SKMatrix.PostConcat(ref _m, deltaM);
                _canvasV.InvalidateSurface();
                break;
            }
        }

        private SKPoint ToUntransformedCanvasPt(float x, float y)
        {
            return (new SKPoint(x * _canvasV.CanvasSize.Width / (float)_canvasV.Width, y * _canvasV.CanvasSize.Height / (float)_canvasV.Height));
        }

        private void OnBallImageUpdated(string ballFilename)
		{
			if (BallImageUpdated != null)
				BallImageUpdated(this, ballFilename);
		}
	}
}
