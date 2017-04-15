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

		private Command<MenuItem> _itemTappedCommand = null;
		public ICommand ItemTappedCommand {
			get {
				_itemTappedCommand = _itemTappedCommand ?? new Command<MenuItem>(DoItemTappedCommand);
				return (_itemTappedCommand);
			}
		}

		public MenuPage()
		{
			BindingContext = this;
			Title = "Menu";
			BackgroundColor = AppStyle.Menu.BACKGROUND_COLOR;
			List<MenuItem> menuItems = new List<MenuItem> {
				new MenuItem { Type = MenuItemType.Play },
				new MenuItem { Type = MenuItemType.Balls }
			};
			DataTemplate it = new DataTemplate(typeof(TextCell));
			it.SetBinding(TextCell.TextProperty, "Title");
			it.SetValue(TextCell.TextColorProperty, AppStyle.Menu.ITEM_TEXT_COLOR);
			ItemTapListView listV = new ItemTapListView {
				BackgroundColor = AppStyle.Menu.LIST_BACKGROUND_COLOR,
				ItemsSource = menuItems,
				ItemTemplate = it,
				Footer = new BoxView {
					BackgroundColor = AppStyle.Menu.LIST_BACKGROUND_COLOR
				}
			};
			Content = listV;
			//Bindings
			listV.SetBinding(ItemTapListView.ItemTapCommandProperty, nameof(MenuPage.ItemTappedCommand));
		}

		private void DoItemTappedCommand(MenuItem selectedItem)
		{
			switch (selectedItem.Type) {
			default:
				App.SetDetailPage(selectedItem.Type);
				break;
			}
		}
	}

	class MenuItem
	{
		public MenuItemType Type { get; set; }
		public string Title { get { return (Type.ToString()); } }
	}
}
