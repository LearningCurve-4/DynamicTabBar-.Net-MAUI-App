namespace DynamicTabBar.Views.Pages;

public partial class DynamicTabPage : ContentPage
{
	public DynamicTabPage()
	{
		InitializeComponent();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}