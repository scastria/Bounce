using System;
using System.Threading.Tasks;
using CoreGraphics;
using Bounce.Services;
using Foundation;
using UIKit;

namespace Bounce.iOS.Services
{
    public class iOSImageProcessor : IImageProcessor
	{
        public Task<byte[]> NormalizeAsync(byte[] imageData, float quality)
        {
            return (Task<byte[]>.Factory.StartNew(delegate {
                UIImage img = UIImage.LoadFromData(NSData.FromArray(imageData));
                if (img.Orientation != UIImageOrientation.Up) {
                    UIGraphics.BeginImageContextWithOptions(img.Size, false, 1);
                    img.Draw(new CGRect(0, 0, img.Size.Width, img.Size.Height));
                    img = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();
                }
                return (img.AsJPEG(quality).ToArray());
            }));
        }

        public Task<byte[]> ResizeAsync(byte[] imageData, int maxWidth, int maxHeight, float quality)
		{
			return (Task<byte[]>.Factory.StartNew(delegate {
				UIImage img = UIImage.LoadFromData(NSData.FromArray(imageData));
                float widthRatio = maxWidth / (float)img.Size.Width;
				float heightRatio = maxHeight / (float)img.Size.Height;
				float scaleRatio = (widthRatio < heightRatio) ? widthRatio : heightRatio;
				CGRect newBounds = new CGRect(0, 0, (int)Math.Round(img.Size.Width * scaleRatio), (int)Math.Round(img.Size.Height * scaleRatio));
				UIGraphics.BeginImageContextWithOptions(newBounds.Size, false, 1);
				img.Draw(newBounds);
				UIImage thumb = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
                return (thumb.AsJPEG(quality).ToArray());
			}));
		}
	}
}
