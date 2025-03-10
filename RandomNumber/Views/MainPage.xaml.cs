using RandomNumber.Services;

namespace RandomNumber
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadClasses();
        }

        private async void LoadClasses()
        {
            var classes = await DataService.GetClassesAsync();
            ClassesList.ItemsSource = classes;
        }

        private async void OnAddClassClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Nowa Klasa", "Podaj nazwę klasy:");
            if (!string.IsNullOrEmpty(result))
            {
                await DataService.AddClassAsync(result);
                LoadClasses();
            }
        }

        private async void OnClassSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                string selectedClass = e.SelectedItem.ToString();
                await Navigation.PushAsync(new ClassDetailsPage(selectedClass));
            }
        }
    }
}