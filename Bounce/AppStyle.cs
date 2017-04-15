using System;
using Xamarin.Forms;

namespace Bounce
{
	public class AppStyle
	{
		public class Default
		{
			public static Color ACCENT_COLOR = Color.FromRgb(147,228,234);
		}

		public class Menu
		{
			public static Color BACKGROUND_COLOR = Color.FromRgb (42, 120, 168);
			public static Color LIST_BACKGROUND_COLOR = Color.Transparent;
			public static Color ITEM_TEXT_COLOR = Color.White;
			public static Color GROUP_BACKGROUND_COLOR = Color.Black;
			public static Color GROUP_BUTTON_BACKGROUND_COLOR = Color.Transparent;
			public const FontAttributes GROUP_TEXT_ATTRIBUTES = FontAttributes.Bold;
			public const int GROUP_INDENT = 5;
		}
	}
}
