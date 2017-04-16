using System;
using Xamarin.Forms;

namespace Bounce
{
	public class AppStyle
	{
		public class Default
		{
			public static Color ACCENT_COLOR = Color.FromRgb(147, 228, 234);
		}

		public class Menu
		{
			public static Color BACKGROUND_COLOR = Color.FromRgb(42, 120, 168);
			public static Color LIST_BACKGROUND_COLOR = Color.Transparent;
			public static Color ITEM_TEXT_COLOR = Color.White;
		}

		public class Balls
		{
			public static Color BACKGROUND_COLOR = Color.FromRgb(34, 34, 34);
			public static Color LIST_BACKGROUND_COLOR = Color.Transparent;
			public const int LIST_ITEM_PADDING = 10;
			public const int LIST_ITEM_HEIGHT = 88;
		}
	}
}
