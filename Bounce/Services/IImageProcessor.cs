using System;
using System.Threading.Tasks;

namespace Bounce.Services
{
	public interface IImageProcessor
	{
        Task<byte[]> NormalizeAsync(byte[] imageData, float quality);
		Task<byte[]> ResizeAsync(byte[] imageData, int maxWidth, int maxHeight, float quality);
	}
}
