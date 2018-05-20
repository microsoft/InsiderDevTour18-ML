using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinMLDemo
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }     
        
        // snippet 1
        CNTKGraphModel model;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///FER-Emotion-Recognition.onnx"));
            model = await CNTKGraphModel.CreateCNTKGraphModel(file);

            await Camera.StartAsync();
            Camera.CameraHelper.FrameArrived += CameraHelper_FrameArrived; ;
        }

        // snippet 2
        private async void CameraHelper_FrameArrived(object sender, Microsoft.Toolkit.Uwp.Helpers.FrameEventArgs e)
        {
            if (e.VideoFrame.SoftwareBitmap == null) return;

            var input = new CNTKGraphModelInput() { Input338 = e.VideoFrame };
            var output = await model.EvaluateAsync(input);
        }
    }
}
