namespace DynamicTabBar
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
			InitializeRouting();
		}

		public void InitializeRouting()
		{
			Routing.RegisterRoute(nameof(DynamicTabPage), typeof(DynamicTabPage));
		}
	}
}
