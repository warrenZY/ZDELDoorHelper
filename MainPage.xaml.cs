using Camera.MAUI;
using Camera.MAUI.ZXing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ZDELDoorHelper.Models;
using ZDELDoorHelper.Services;
namespace ZDELDoorHelper
{

    public partial class MainPage : ContentPage
    {
        bool isPlaying = true;
        int selectCameraIndex = 0;
        string qRCodeData = "";
        string mobileNum = "";

        ZDELDoorHelperTools _doorTools = new();

        ObservableCollection<Configuration> _configurations = new ObservableCollection<Configuration>();
        public ObservableCollection<Configuration> observableConfig { get { return _configurations; } }

        List<Configuration> configList = new();

        public MainPage()
        {
            InitializeComponent();
            cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            cameraView.BarCodeOptions = new BarcodeDecodeOptions()
            {
                PossibleFormats = { BarcodeFormat.QR_CODE },
                AutoRotate = true,
                ReadMultipleCodes = false,
                TryHarder = true,
                TryInverted = true
            };

            cameraView.BarCodeDetectionFrameRate = 10;
            cameraView.BarCodeDetectionMaxThreads = 5;
            cameraView.ControlBarcodeResultDuplicate = true;
            cameraView.BarCodeDetectionEnabled = true;

            
            ConfigView.ItemsSource = _configurations;
            
            ConfigInitializeAsync();
        }

        private void CameraView_CamerasLoaded(object sender, EventArgs e)
        {
            if (cameraView.NumCamerasDetected > 0)
            {
                Task.Delay(300);
                cameraView.Camera = cameraView.Cameras.First();
            }
        }

        private void OnZoomSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            ZoomPercentLabel.Text = args.NewValue.ToString("F1");
            cameraView.ZoomFactor = ((float)Math.Round(args.NewValue, 1));
        }


        private void OnCounterClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (isPlaying == false)
                {
                    await cameraView.StopCameraAsync();
                    CounterBtn.Text = "Start";
                    isPlaying = true;
                }
                else
                {
                    await cameraView.StartCameraAsync();
                    CounterBtn.Text = "Pause";
                    isPlaying = false;
                }
                SemanticScreenReader.Announce(CounterBtn.Text);

            });

        }



        private void CameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                barcodeResult.Text = $"{_doorTools.GetZDELDoorParameter(args.Result[selectCameraIndex].Text)}";
                qRCodeData = $"{args.Result[selectCameraIndex].Text}";
            });
        }

        private void OnTorchClicked(object sender, EventArgs e) => cameraView.TorchEnabled = !cameraView.TorchEnabled;

        private void OnCamSwitchDownClicked(object sender, EventArgs e)
        {
            if (cameraView.NumCamerasDetected > 0 && selectCameraIndex > 0)
            {
                selectCameraIndex--;
                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    await cameraView.StopCameraAsync();
                    cameraView.Camera = cameraView.Cameras[selectCameraIndex];
                    await cameraView.StartCameraAsync();

                });

            }

        }

        private void OnCamSwitchUpClicked(object sender, EventArgs e)
        {
            if (cameraView.NumCamerasDetected > 0 && selectCameraIndex < cameraView.NumCamerasDetected - 1)
            {
                selectCameraIndex++;
                MainThread.BeginInvokeOnMainThread(async () =>
                {

                    await cameraView.StopCameraAsync();
                    cameraView.Camera = cameraView.Cameras[selectCameraIndex];
                    await cameraView.StartCameraAsync();

                });
            }
        }

        private async void OnOpenDoorClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(mobileNum))
            { 
                await DisplayAlert("Info", "Please apply a valid cookie first!", "Understand");
                return;
            }
            if (string.IsNullOrWhiteSpace(qRCodeData))
            {
                await DisplayAlert("Info", "Please scan a QR code first!", "Understand");
                return;
            }
            try
            {
                //_doorTools.GetZDELDoorParameter(qRCodeData)
                await _doorTools.SendOpenDoorHttpGetRequestAync(qRCodeData, mobileNum);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }


        private void OnSettingsClicked(object sender, TouchEventArgs e)
        {
            SettingsContext.IsVisible = !SettingsContext.IsVisible;
            CurrentCookie.Text = $"Current Cookie is: {mobileNum}";

        }


        private async void ConfigInitializeAsync()
        {
            _configurations.Clear();
            try { configList = ConfigurationStorageService.GetListsFromXml(); }
            catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
            foreach (Configuration config in configList)
            {
                _configurations.Add(config);
            }
        }

        private async void OnConfigViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            if(e.Item is Configuration tappedConfig)
            {
                mobileNum = EntryCookie.Text = tappedConfig.MobileNumber;
                CurrentCookie.Text = $"Current Cookie is: {mobileNum}";
                await DisplayAlert("Item Tapped", $"Change phone number to {tappedConfig.MobileNumber}","OK");
            }
        }

        private async void OnCookieSubmitClicked(object sender, EventArgs e)
        {
            //matches phone number of 11 character
            if (string.IsNullOrWhiteSpace(EntryCookie.Text)|| EntryCookie.Text.Length!=11)
            {
                await DisplayAlert("Info", "Please input a valid phone number (length equals 11) first!", "Understand");
                return;
            }

            if (configList.Count(p => p.MobileNumber.Equals(EntryCookie.Text, StringComparison.Ordinal))>0)
            {
                await DisplayAlert("Info", "phone number already exist", "Understand");
                return;
            }

            CurrentCookie.Text = $"Current Cookie is: {mobileNum}";
            Configuration _configuration = new Configuration()
            {
                MobileNumber = EntryCookie.Text,
                Description = ""
            };
            
            _configurations.Add(_configuration);
            configList.Add(_configuration);
            try { ConfigurationStorageService.WriteListToXml(configList); }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void OnCookieRemoveClicked(object sender, TouchEventArgs e)
        {
            configList.RemoveAll(p=>p.MobileNumber.Equals(EntryCookie.Text));
            ConfigurationStorageService.WriteListToXml(configList);
            ConfigInitializeAsync();
        }
    }

}
