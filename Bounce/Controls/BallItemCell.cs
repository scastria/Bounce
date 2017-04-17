using System;
using Bounce.Converters;
using Bounce.Pages;
using Xamarin.Forms;

namespace Bounce.Controls
{
	public class BallItemCell : ViewCell
	{
		public BallItemCell()
		{
			Image ballI = new Image {
				Aspect = Aspect.AspectFit,
				VerticalOptions = LayoutOptions.Center
			};
			Image checkI = new Image {
				VerticalOptions = LayoutOptions.Center
			};
			Grid mainG = new Grid {
				Padding = new Thickness(AppStyle.Balls.LIST_ITEM_PADDING),
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Star },
					new ColumnDefinition { Width = GridLength.Auto }
				}
			};
			mainG.Children.Add(ballI, 0, 0);
			mainG.Children.Add(checkI, 1, 0);
			View = mainG;
			//Bindings
			ballI.SetBinding(Image.SourceProperty, nameof(BallItem.Filename), converter: new FilenameImageValueConverter());
			checkI.SetBinding(Image.SourceProperty, nameof(BallItem.IsSelected), converter: new CheckImageValueConverter());
		}
	}
}
