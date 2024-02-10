namespace DynamicTabBar.ViewModels;

public class BaseViewModel : NotifyPropertyChanged
{
	bool isBusy = false;
	public bool IsBusy
	{
		get => isBusy;
		set
		{
			if (isBusy == value) { return; }
			isBusy = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(IsNotBusy));
		}
	}
	public bool IsNotBusy => !IsBusy;

	public static string CurrentPage { get; set; } = string.Empty;
	public Command GoToPageCommand => new Command<string>(async (page) =>
	{
		try
		{
			IsBusy = true;
			if (CurrentPage != page)
			{
				var pageType = Type.GetType(GlobalVariables.pageFolder + page);
				if (pageType != null)
				{
					await Shell.Current.GoToAsync(page, true);
					CurrentPage = page;
				}
				else
				{
					await Shell.Current.DisplayAlert("Error:", $"{page} - Not available", "OK");
				}
			}
		}
		catch (Exception ex) { await Shell.Current.DisplayAlert("Error:", ex.Message, "OK"); }
		finally
		{
			IsBusy = false;
		}
	}, (page) => IsNotBusy);

	public Command GoBackToPageCommand => new Command<string>(async (page) =>
	{
		try
		{
			IsBusy = true;
			await Shell.Current.GoToAsync(page, true);
			CurrentPage = string.Empty;
		}
		catch (Exception ex) { await Shell.Current.DisplayAlert("Error:", ex.Message, "OK"); }
		finally
		{
			IsBusy = false;
		}
	}, (page) => IsNotBusy);

	#region Create Bottom/Top Tabs
	public ObservableCollection<DynamicTabModel>? BottomTabList { get; set; } = [];
	public ObservableCollection<DynamicTabModel>? TopTabListOrig { get; set; } = [];
	public ObservableCollection<DynamicTabModel>? TopTabList { get; set; } = [];

	string? bottomTabTitle = string.Empty;
	public string? BottomTabTitle
	{
		get => bottomTabTitle;
		set
		{
			if (bottomTabTitle != value)
			{
				bottomTabTitle = value;
				OnPropertyChanged();
			}
		}
	}

	public Command GetTabDataCommand => new Command<string>(async (filename) =>
	{
		try
		{
			IsBusy = true;
			BottomTabList?.Clear(); TopTabListOrig?.Clear(); TopTabList?.Clear();
			using Stream stream = await FileSystem.OpenAppPackageFileAsync(filename);
			using StreamReader reader = new(stream);
			string jsonResponse = await reader.ReadToEndAsync();

			if (!string.IsNullOrEmpty(jsonResponse))
			{
				var jsonDeserialized = JsonSerializer.Deserialize<ObservableCollection<DynamicTabModel>>(jsonResponse)!;
				if (jsonDeserialized != null || jsonDeserialized?.Count > 0) //check if tab exist
				{
					foreach (var item in jsonDeserialized!)
					{
						if (!string.IsNullOrEmpty(item.TabTitle) || item.TabPos is >= 1 and <= 5)  //check if tab exist, max 5 tabs
						{
							//build bottom tab list
							if (item.TabRef + 0 < 1)  //bottom tab should have zero reference
							{
								if (!BottomTabList!.Any(b => b.TabPos == item.TabPos))  //check no tab duplication
								{
									DynamicTabModel bt = new()
									{
										TabPos = item.TabPos,
										TabTitle = item.TabTitle,
										TabImage = item.TabImage,
										TabIsSelected = item.TabIsSelected
									};
									BottomTabList?.Add(bt);
								}
							}

							//build top tab list
							if (item.TabRef + 0 is >= 1 and <= 5)
							{
								if (!TopTabList!.Any(b => b.TabRef == item.TabRef && b.TabPos == item.TabPos))  //check no tab duplication
								{
									DynamicTabModel tt = new()
									{
										TabRef = item.TabRef,
										TabPos = item.TabPos,
										TabTitle = item.TabTitle,
										TabImage = item.TabImage,
										TabIsSelected = item.TabIsSelected
									};
									TopTabListOrig?.Add(tt);
								}
							}

						}
					}
				}

				int? bottomTabPos = BottomTabList?.FirstOrDefault(b => b.TabIsSelected)?.TabPos ?? 1;
				int? topTabPos = TopTabListOrig?.FirstOrDefault(t => t.TabRef == bottomTabPos && t.TabIsSelected)?.TabPos ?? 1;
				SwitchTabCommand.Execute($"{bottomTabPos},{topTabPos}");
			}
		}
		catch (Exception ex) { await Shell.Current.DisplayAlert("Error:", ex.Message, "OK"); }
		finally
		{
			IsBusy = false;
		}
	}, (filename) => IsNotBusy);

	public Command SwitchTabCommand => new Command<string>((str) =>
	{
		try
		{
			IsBusy = true;
			string[]? substrings = str?.Split(',')!;
			if (!string.IsNullOrEmpty(substrings[0]))
			{
				int bottomTabPos = int.Parse(substrings[0]);
				if (bottomTabPos >= 1)  //bottom tab switched
				{
					for (int i = 0; i < BottomTabList?.Count; i++)
					{
						BottomTabList[i].TabIsSelected = false;
						if (BottomTabList[i].TabPos == bottomTabPos)
						{
							BottomTabList[i].TabIsSelected = true;
						}
					}
					BottomTabTitle = BottomTabList?[bottomTabPos - 1].TabTitle;

					TopTabList?.Clear();
					foreach (var tt in TopTabListOrig!.Where(b => b.TabRef == bottomTabPos))  //build top tab list based on bottom tab
					{
						TopTabList?.Add(tt);
					}
				}

				if (!string.IsNullOrEmpty(substrings[1]))
				{
					int topTabPos = int.Parse(substrings[1]);
					if (topTabPos >= 1)  //top tab switched
					{
						for (int i = 0; i < TopTabList?.Count; i++)
						{
							TopTabList[i].TabIsSelected = false;
							if (TopTabList[i].TabRef == bottomTabPos && TopTabList[i].TabPos == topTabPos)
							{
								TopTabList[i].TabIsSelected = true;
							}
						}
					}
				}

				//update UI
				OnPropertyChanged(nameof(BottomTabList));
				OnPropertyChanged(nameof(TopTabList));
			}
		}
		catch (Exception ex) { Shell.Current.DisplayAlert("Error:", ex.Message, "OK"); }
		finally
		{
			IsBusy = false;
		}
	}, (str) => IsNotBusy);
	#endregion
}
