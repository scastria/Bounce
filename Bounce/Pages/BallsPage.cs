using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
		private const string BALLS_DIR = "Balls";

		private App App { get { return ((App)Application.Current); } }

		private string _ballsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), BALLS_DIR);
		private ObservableCollection<BallItem> _ballItems = null;

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
					if (pickPhoto == null)
						return;
					Console.WriteLine(_ballsDir);
					Directory.CreateDirectory(_ballsDir);
					string ballFilename = Path.Combine(_ballsDir, Guid.NewGuid().ToString());
					using(Stream pickPhotoS = pickPhoto.GetStream()) {
						using (FileStream fs = new FileStream(ballFilename, FileMode.CreateNew)) {
							await pickPhotoS.CopyToAsync(fs);
						}
					}
					_ballItems.Add(new BallItem { Filename = ballFilename });
					BallEditPage bep = new BallEditPage(ballFilename);
					bep.BallImageUpdated += HandleBallImageUpdated;
					await Navigation.PushAsync(bep);
				}
			};
			ToolbarItems.Add(cameraTBI);
			BackgroundColor = AppStyle.Balls.BACKGROUND_COLOR;
			_ballItems = new ObservableCollection<BallItem>();
			_ballItems.Add(new BallItem { IsSelected = string.IsNullOrWhiteSpace(App.BallFilename) });
			Directory.CreateDirectory(_ballsDir);
			foreach (string ballFile in Directory.GetFiles(_ballsDir)) {
				string ballFilename = Path.Combine(_ballsDir, ballFile);
				_ballItems.Add(new BallItem { Filename = ballFilename, IsSelected = App.BallFilename == ballFilename });
			}
			DataTemplate it = new DataTemplate(typeof(BallItemCell));
			ListView listV = new ListView {
				BackgroundColor = AppStyle.Balls.LIST_BACKGROUND_COLOR,
				ItemsSource = _ballItems,
				ItemTemplate = it,
				RowHeight = AppStyle.Balls.LIST_ITEM_HEIGHT
			};
			listV.ItemTapped += (object sender, ItemTappedEventArgs e) => {
				BallItem selItem = (BallItem)e.Item;
				if (!selItem.IsSelected) {
					App.BallFilename = selItem.Filename;
					foreach (BallItem bi in _ballItems) {
						if (bi.IsSelected) {
							bi.IsSelected = false;
							break;
						}
					}
					selItem.IsSelected = true;
				}
				listV.SelectedItem = null;
				App.SetDetailPage(MenuItemType.Play);
			};
			Content = listV;
		}

		private void HandleBallImageUpdated(object sender, string ballFilename)
		{
			//Find ball item that matches ballFilename
			foreach (BallItem bi in _ballItems) {
				if (bi.Filename == ballFilename) {
					//Force an update by calling filename setter
					bi.Filename = ballFilename;
					break;
				}
			}
		}
	}

	public class BallItem : INotifyPropertyChanged
	{
		private string _filename = null;
		public string Filename {
			get { return (_filename); }
			set {
				_filename = value;
				OnPropertyChanged();
			}
		}
		private bool _isSelected = false;
		public bool IsSelected {
			get { return (_isSelected); }
			set {
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
