using System;
using System.Collections.Generic;
using System.Windows.Input;
using Bounce.Controls;
using Xamarin.Forms;

namespace Bounce.Pages
{
	public class MenuPage : ContentPage
	{
		private App App { get { return ((App)Application.Current); } }

		public MenuPage()
		{
			BindingContext = this;
			Title = "Menu";
			BackgroundColor = AppStyle.Menu.BACKGROUND_COLOR;
			List<MenuItem> menuItems = new List<MenuItem> {
				new MenuItem { Type = MenuItemType.Play },
				new MenuItem { Type = MenuItemType.Balls },
				new MenuItem { Type = MenuItemType.Crash }
			};
			DataTemplate it = new DataTemplate(typeof(TextCell));
			it.SetBinding(TextCell.TextProperty, "Title");
			it.SetValue(TextCell.TextColorProperty, AppStyle.Menu.ITEM_TEXT_COLOR);
			ListView listV = new ListView {
				BackgroundColor = AppStyle.Menu.LIST_BACKGROUND_COLOR,
				ItemsSource = menuItems,
				ItemTemplate = it,
				Footer = new BoxView {
					BackgroundColor = AppStyle.Menu.LIST_BACKGROUND_COLOR
				}
			};
			listV.ItemTapped += (object sender, ItemTappedEventArgs e) => {
				MenuItem selItem = (MenuItem)e.Item;
                if(selItem.Type == MenuItemType.Crash) {
                    string crash = null;
                    Console.WriteLine(crash.Length);
                } else
    				App.SetDetailPage(selItem.Type);
				listV.SelectedItem = null;
			};
			Content = listV;
		}
	}

	class MenuItem
	{
		public MenuItemType Type { get; set; }
		public string Title { get { return (Type.ToString()); } }
	}
}
