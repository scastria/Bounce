using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bounce.Controls;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Bounce.Pages
{
	public class BallsPage : ContentPage
	{
		private const string TAKE_PHOTO = "Take a photo";
		private const string CHOOSE_PHOTO = "Choose a photo";

		public BallsPage()
		{
			BindingContext = this;
			Title = "Balls";
			ToolbarItem cameraTBI = new ToolbarItem {
				Icon = "camera"
			};
			cameraTBI.Clicked += async (object sender, EventArgs e) => {
				string choice = await DisplayActionSheet("Photo Option", "Cancel", null, TAKE_PHOTO, CHOOSE_PHOTO);
				if (string.IsNullOrWhiteSpace(choice))
					return;
				await Task.Delay(TimeSpan.FromSeconds(0.5));
				await CrossMedia.Current.Initialize();
				if (choice == TAKE_PHOTO) {
					if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
						await DisplayAlert("Error", "Cannot access camera", "Close");
						return;
					}
				} else {
					if (!CrossMedia.Current.IsPickPhotoSupported) {
						await DisplayAlert("Error", "Cannot access photo library", "Close");
						return;
					}
					MediaFile pickPhoto = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { PhotoSize = PhotoSize.Medium });
					Console.WriteLine(pickPhoto.Path);
				}
			};
			ToolbarItems.Add(cameraTBI);
			BackgroundColor = AppStyle.Balls.BACKGROUND_COLOR;
			List<BallItem> ballItems = new List<BallItem> {
				new BallItem { IsSelected = true },
			};
			DataTemplate it = new DataTemplate(typeof(BallItemCell));
			ListView listV = new ListView {
				BackgroundColor = AppStyle.Balls.LIST_BACKGROUND_COLOR,
				ItemsSource = ballItems,
				ItemTemplate = it,
				RowHeight = AppStyle.Balls.LIST_ITEM_HEIGHT
			};
			listV.ItemTapped += (object sender, ItemTappedEventArgs e) => {
				listV.SelectedItem = null;
			};
			Content = listV;
		}
	}

	public class BallItem
	{
		public string Filename { get; set; }
		public bool IsSelected { get; set; }
	}
}
