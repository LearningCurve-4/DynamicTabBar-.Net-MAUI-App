namespace DynamicTabBar.Views.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}