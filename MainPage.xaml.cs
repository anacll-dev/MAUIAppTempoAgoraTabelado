using MAUIAppTempoAgora.Models;
using MAUIAppTempoAgora.Service;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MAUIAppTempoAgora
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            list_tempos.ItemsSource = listaTempos;
        }

        CancellationTokenSource _cancelTokenSource;
        bool _isCheckingLocation;

        string? cidade;

        ObservableCollection<Tempo> listaTempos = new ObservableCollection<Tempo>();

        


        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                List<Tempo> ddsfs = await App.Db.GetAll();

                foreach(Tempo tempo in ddsfs)
                {
                    Debug.WriteLine(tempo.Humidity);
                }

                _cancelTokenSource = new CancellationTokenSource();

                GeolocationRequest request =
                    new GeolocationRequest(GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10));

                Location? location = await Geolocation.Default.GetLocationAsync(
                    request, _cancelTokenSource.Token);


                if (location != null)
                {
                    lbl_latitude.Text = location.Latitude.ToString();
                    lbl_longitude.Text = location.Longitude.ToString();


                    Debug.WriteLine("---------------------------------------");
                    Debug.WriteLine(location);
                    Debug.WriteLine("---------------------------------------");
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não Suporta",
                    fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização Desabilitada",
                    fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: ", ex.Message, "OK");
            }
        }
        private async Task<string> GetGeocodeReverseData(
            double latitude = 47.673988, double longitude = -122.121513)
        {
            IEnumerable<Placemark> placemarks =
                await Geocoding.Default.GetPlacemarksAsync(
                    latitude, longitude);

            Placemark? placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                cidade = placemark.Locality;

                return
                    $"AdminArea:       {placemark.AdminArea}\n" +
                    $"CountryCode:     {placemark.CountryCode}\n" +
                    $"CountryName:     {placemark.CountryName}\n" +
                    $"FeatureName:     {placemark.FeatureName}\n" +
                    $"Locality:        {placemark.Locality}\n" +
                    $"PostalCode:      {placemark.PostalCode}\n" +
                    $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                    $"SubLocality:     {placemark.SubLocality}\n" +
                    $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                    $"Thoroughfare:    {placemark.Thoroughfare}\n";
            }

            return "Nada";
        }

        protected async override void OnAppearing()
        {
            List<Tempo> tempos = await App.Db.GetAll();

            foreach (Tempo tempo in tempos)
            {
                listaTempos.Add(tempo);
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {

            

            double latitude = Convert.ToDouble(lbl_latitude.Text);
            double longitude = Convert.ToDouble(lbl_longitude.Text);

            lbl_reverso.Text = await GetGeocodeReverseData(latitude, longitude);
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(cidade))
                {
                    Tempo? previsao = await DataService.GetPrevisaoDoTempo(cidade);

                    string dados_previsao = "";

                    if (previsao != null)
                    {
                        dados_previsao = $"Humidade: {previsao.Humidity} \n" +
                                         $"Nascer do Sol: {previsao.Sunrise} \n " +
                                         $"Pôr do Sol: {previsao.Sunset} \n" +
                                         $"Temperatura: {previsao.Temperature} \n" +
                                         $"Titulo: {previsao.Title} \n" +
                                         $"Visibilidade: {previsao.Visibility} \n" +
                                         $"Vento: {previsao.Wind} \n" +
                                         $"Previsão: {previsao.Weather} \n" +
                                         $"Descrição: {previsao.WeatherDescription}";

                   

                        await App.Db.Insert(new Tempo
                        {
                            Title = previsao.Title,
                            Temperature = previsao.Temperature,
                            Wind = previsao.Wind,
                            Humidity = previsao.Humidity,
                            Visibility = previsao.Visibility,
                            Sunrise = previsao.Sunrise,
                            Sunset = previsao.Sunset,
                            Weather = previsao.Weather,
                            WeatherDescription = previsao.WeatherDescription,
                            Created = DateTime.Now
                        });

                        


                    }
                    else
                    {
                        dados_previsao = $"Sem dados, previsão nula.";
                    }

                    lbl_previsao.Text = dados_previsao;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro ", ex.Message, "OK");
            }
        }

    }

}
