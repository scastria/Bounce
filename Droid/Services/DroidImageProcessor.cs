using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;
using Bounce.Services;

namespace Bounce.Droid.Services
{
    public class DroidImageProcessor : IImageProcessor
	{
        public Task<byte[]> NormalizeAsync(byte[] imageData, float quality)
        {
            return (Task<byte[]>.Factory.StartNew(delegate {
                int orient;
                using(MemoryStream ms = new MemoryStream(imageData)) {
                    ExifInterface exif = new ExifInterface(ms);
                    orient = exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);
                }
                using(Bitmap bmp = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length)) {
                    Matrix m = new Matrix();
                    switch(orient) {
                    case (int)Orientation.Rotate90:
                        m.PostRotate(90);
                        break;
                    case (int)Orientation.Rotate180:
                        m.PostRotate(180);
                        break;
                    case (int)Orientation.Rotate270:
                        m.PostRotate(270);
                        break;
                    default:
                        m = null;
                        break;
                    }
                    byte[] retVal = null;
                    if (m != null) {
                        using(Bitmap normBmp = Bitmap.CreateBitmap(bmp, 0, 0, bmp.Width, bmp.Height, m, true)) {
                            using (MemoryStream ms = new MemoryStream()) {
                                normBmp.Compress(Bitmap.CompressFormat.Jpeg, (int)(quality * 100), ms);
                                retVal = ms.ToArray();
                            }
                        }
                    } else {
                        using (MemoryStream ms = new MemoryStream()) {
                            bmp.Compress(Bitmap.CompressFormat.Jpeg, (int)(quality * 100), ms);
                            retVal = ms.ToArray();
                        }
                    }
                    return (retVal);
                }
            }));
        }

        public Task<byte[]> ResizeAsync(byte[] imageData, int maxWidth, int maxHeight, float quality)
		{
			return (Task<byte[]>.Factory.StartNew(delegate {
				Bitmap bmp = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
				float widthRatio = maxWidth / (float)bmp.Width;
				float heightRatio = maxHeight / (float)bmp.Height;
				float scaleRatio = (widthRatio < heightRatio) ? widthRatio : heightRatio;
				int newWidth = (int)Math.Round(bmp.Width * scaleRatio);
				int newHeight = (int)Math.Round(bmp.Height * scaleRatio);
				Bitmap thumb = Bitmap.CreateScaledBitmap(bmp, newWidth, newHeight, false);
				byte[] retVal = null;
				using (MemoryStream ms = new MemoryStream()) {
                    thumb.Compress(Bitmap.CompressFormat.Jpeg, (int)(quality * 100), ms);
					retVal = ms.ToArray();
				}
				return (retVal);
			}));
		}
	}
}
