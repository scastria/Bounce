using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace Bounce.Controls
{
	public class ItemTapListView : ListView
	{
		public static readonly BindableProperty ItemTapCommandProperty = BindableProperty.Create (
			"ItemTapCommand",
			typeof(ICommand),
			typeof(ItemTapListView),
			null,
			BindingMode.OneWay
		);
		public ICommand ItemTapCommand
		{
			get { return (ICommand)GetValue(ItemTapCommandProperty); }
			set { SetValue (ItemTapCommandProperty, value); }
		}

		public ItemTapListView()
		{
			ItemTapped += delegate(object sender, ItemTappedEventArgs itea) {
				if((ItemTapCommand != null) && (ItemTapCommand.CanExecute(itea.Item)))
					ItemTapCommand.Execute(itea.Item);
				SelectedItem = null;
			};
		}
	}
}
