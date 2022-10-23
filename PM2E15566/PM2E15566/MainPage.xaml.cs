using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.IO;
using Plugin.Geolocator;
using Plugin.Media;
using PM2E15566.Config;
using PM2E15566.Models;

namespace PM2E15566
{
    public partial class MainPage : ContentPage
    {
        bool estado = true;
        double dlatitud, dlongitud;
        byte[] image;

        public MainPage()
        {
            InitializeComponent();
            locationGPS();
        }

        public async void locationGPS()
        {

            var location = CrossGeolocator.Current;
            location.DesiredAccuracy = 50;

            if (!location.IsGeolocationEnabled || !location.IsGeolocationAvailable)
            {

                await DisplayAlert("Warning", " GPS no esta activo", "ok");

            }
            else
            {
                if (!location.IsListening)
                {
                    await location.StartListeningAsync(TimeSpan.FromSeconds(10), 1);


                }
                location.PositionChanged += (posicion, args) =>
                {
                    var ubicacion = args.Position;
                    txtlatitud.Text = ubicacion.Latitude.ToString();
                    dlatitud = Convert.ToDouble(txtlatitud.Text);
                    txtlongitud.Text = ubicacion.Longitude.ToString();
                    dlongitud = Convert.ToDouble(txtlongitud.Text);
                };

            }

        }



        private void btn01_Clicked(object sender, EventArgs e)
        {
            //GetLocation();
        }

        private async void btn02_Clicked(object sender, EventArgs e)
        {
            // var ubicaciones = new Views.ubicacionesPage();
            // await NavigationPage.PushAsync(ubicaciones);
            await Navigation.PushAsync(new Views.ubicacionesPage());
        }

        private async void btnguardar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var ubicaciones = new Models.Ubicaciones
                {
                    latitud = Convert.ToDouble(this.txtlatitud.Text),
                    longitud = Convert.ToDouble(this.txtlongitud.Text),
                    descripcion = this.txtdescripcion.Text,
                    fotografia = image
                };

                var resultado = await App.Basedatos02.GrabarUbicacion(ubicaciones);


                if (resultado == 1)
                {
                    await DisplayAlert("Mensaje", "Ingresada Exitosamente", "OK");
                }
                else
                {
                    await DisplayAlert("Mensaje", "Error, No se logro guardar Ubicacion", "OK");

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Mensaje", ex.Message.ToString(), "OK");

            }
        }

        private void Camera_button_Clicked(object sender, EventArgs e)
        {

        }

        private async void buttoncamera_Clicked(object sender, EventArgs e)
        {
            var camera = new StoreCameraMediaOptions();
            camera.PhotoSize = PhotoSize.Full;
            camera.Name = "img";
            camera.Directory = "MiApp";

            var foto = await CrossMedia.Current.TakePhotoAsync(camera);
            if (foto != null)
            {

                imagefile.Source = ImageSource.FromStream(() => {

                    return foto.GetStream();
                });

                imagefile.IsVisible = true;
                using (MemoryStream memory = new MemoryStream())
                {

                    Stream stream = foto.GetStream();
                    stream.CopyTo(memory);
                    image = memory.ToArray();
                }
            }
        }
    }
}