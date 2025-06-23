namespace DontLetMeExpire;

public partial class AppWindow : Window
{


	public AppWindow(): base()
	{
		InitializeComponent();
	}

  public AppWindow(Page page) : base(page)
  {
    InitializeComponent();
  }
}