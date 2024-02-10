namespace DynamicTabBar.ViewModels;

public class DynamicTabViewModel : BaseViewModel
{
	public DynamicTabViewModel()
	{
		GetTabDataCommand.Execute("tabdata.json");  //json file saved in Resources > Raw folder
	}
}
