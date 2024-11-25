using Newtonsoft.Json.Linq;
using PrevisaoTempoSQLite.Models;
using PrevisaoTempoSQLite.Services;
using System.Diagnostics;
using System.Globalization;

namespace PrevisaoTempoSQLite.Views;

public partial class MainPage : ContentPage
{
    CancellationTokenSource _cancelTokenSource;
    bool _isCheckingLocation;

    string? cidade;
    string? county;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            _cancelTokenSource = new CancellationTokenSource();

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            Location? location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {
                input_lat.Text = location.Latitude.ToString();
                input_long.Text = location.Longitude.ToString();
                Debug.WriteLine("------------------------");
                Debug.WriteLine(location);
                Debug.WriteLine("-------------------------");
            }
        }
        catch (FeatureNotSupportedException fnsex)
        {
            await DisplayAlert("Erro: Dispositivo não suporta", fnsex.Message, "Ok");
        }
        catch (FeatureNotEnabledException fneEx)
        {
            await DisplayAlert("Erro: Localização desabilitada", fneEx.Message, "Ok");
        }
        catch (PermissionException pEx)
        {
            await DisplayAlert("Erro: Permissão", pEx.Message, "Ok");
        }
        catch (Exception ex) {
            await DisplayAlert("Erro: ", ex.Message, "Ok");
        }

    }

    private async Task<string> GetGeocodeReverseData(string latitude, string longitude)
    {
        string apiKey = App.apiKey;
        string url = $"https://api.geoapify.com/v1/geocode/reverse?lat={latitude}&lon={longitude}&apiKey={apiKey}";
        Debug.WriteLine(url);

        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode) {
            var json = await response.Content.ReadAsStringAsync();
            var jsonObj = JObject.Parse(json);

            var featuresArray = jsonObj["features"] as JArray;
            var city = jsonObj["features"]?[0]?["properties"]?["city"]?.ToString();
            county = jsonObj["features"]?[0]?["properties"]?["county"]?.ToString();

            cidade = city;

        }

        return $"{cidade}, {county}";

    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(input_lat.Text) || !string.IsNullOrEmpty(input_long.Text))
        {
            string latitudeText = input_lat.Text.Replace(",", ".");
            string longitudeText = input_long.Text.Replace(",", ".");

            Debug.WriteLine(latitudeText, longitudeText);

            double latitude = double.Parse(latitudeText, CultureInfo.InvariantCulture);
            double longitude = double.Parse(longitudeText, CultureInfo.InvariantCulture);

            Debug.WriteLine($"Lat: {latitude} Long: {longitude}");

            lbl_Weather.Text = await GetGeocodeReverseData(latitudeText, longitudeText);
        }
        else
        {
            DisplayAlert("Erro: ", "Preencha todos os campos", "Ok");
        }
    }

    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        string lat = input_lat.Text;
        string longi = input_long.Text;
        

        if (!string.IsNullOrEmpty(lat) || !string.IsNullOrEmpty(longi))
        {
            try
            {
                Debug.WriteLine($"Cidade: {cidade}");
                if (!String.IsNullOrEmpty(cidade))
                {
                    Debug.WriteLine("CLICADO");
                    APImodel? previsao = await WeatherAPI.GetPrevisaoDoTempo(cidade);

                    string dados_previsao = "";

                    if (previsao != null)
                    {
                        dados_previsao = $"Humidade: {previsao.Humidity} \n" +
                            $"Nascer do Sol: {previsao.Sunrise} \n" +
                            $"Pôr do Sol: {previsao.Sunset} \n" +
                            $"Temperatura: {previsao.Temperature} \n" +
                            $"Visibilidade: {previsao.Visibility} \n" +
                            $"Vento: {previsao.Wind} \n" +
                            $"Previsão: {previsao.Weather} \n" +
                            $"Descrição: {previsao.WeatherDescription}";
                        try
                        {
                            Weather weather = new Weather
                            {
                                Latitude = Convert.ToDouble(input_lat.Text),
                                Longitude = Convert.ToDouble(input_long.Text),
                                City = cidade,
                                County = county,
                                Temperature = previsao.Temperature
                            };

                            await App.Database.Insert(weather);
                            DisplayAlert("Sucesso", "Previsão registrada", "Ok");
                        } catch (Exception ex) {
                            DisplayAlert("Erro ao registrar previsão: ", ex.Message, "Ok");
                        }
                    }
                    else
                    {
                        dados_previsao = $"Sem dados, previsão nula";
                    }
                    Debug.WriteLine("-------------------------------------------");
                    Debug.WriteLine(dados_previsao);
                    Debug.WriteLine("-------------------------------------------");

                    lbl_Weather.Text = dados_previsao;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: ", ex.Message, "Ok");
            }

        }
        else
        {
            DisplayAlert("Erro: ", "Preencha todos os campos", "Ok");
        }
    }

    private async void Button_Clicked_3(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.HistoryPage());
        }
        catch (Exception ex) {
            DisplayAlert("Erro: ", ex.Message, "Ok");
        }
    }
}