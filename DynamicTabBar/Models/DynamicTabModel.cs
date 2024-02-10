namespace DynamicTabBar.Models;

public class DynamicTabModel : NotifyPropertyChanged
{
	public int? TabRef { get; set; }  //creates relation between bottom and top tabs within the same view
	public int? TabPos { get; set; }
	public string? TabTitle { get; set; }
	public string? TabImage { get; set; }

	bool tabIsSelected;
	public bool TabIsSelected
	{
		get => tabIsSelected;
		set
		{
			if (tabIsSelected != value)
			{
				tabIsSelected = value;
				OnPropertyChanged();
			}
		}
	}
}

