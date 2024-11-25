using PrevisaoTempoSQLite.Models;
using System.Collections.ObjectModel;

namespace PrevisaoTempoSQLite.Views;

public partial class HistoryPage : ContentPage
{
	ObservableCollection<Weather> list = new ObservableCollection<Weather>();
	public HistoryPage()
	{
		InitializeComponent();

		lst_weather.ItemsSource = list;
	}

    protected async override void OnAppearing()
    {
        List<Weather> temp = await App.Database.GetAll();
        temp.ForEach(x => list.Add(x));
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = e.NewTextValue;

        list.Clear();

        List<Weather> temp = await App.Database.Search(query);

        temp.ForEach(x => list.Add(x));
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        var menuItem = (MenuItem)sender;
        var itemId = (int)menuItem.CommandParameter;

        await App.Database.Delete(itemId);
        await Navigation.PushAsync(new HistoryPage());
        Navigation.RemovePage(this);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            await App.Database.DeleteAll();

            DisplayAlert("Sucesso", "Previsões deletadas", "Ok");
            await Navigation.PushAsync(new HistoryPage());
            Navigation.RemovePage(this);
        }
        catch(Exception ex)
        {
            DisplayAlert("Erro:", ex.Message, "Ok");
        }
    }
}