using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Tesseract;

namespace ScreenText.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        // Коллекция изображений, добавленных пользователем
        public ObservableCollection<ImageSource> Images { get; } = new ObservableCollection<ImageSource>();

        // Переменная для хранения извлеченного текста
        private string _extractedText;

        public string ExtractedText
        {
            get => _extractedText;
            set => SetProperty(ref _extractedText, value);
        }

        // Команды для добавления изображения и извлечения текста
        public ICommand AddImageCommand { get; }
        public ICommand ExtractTextCommand { get; }

        public MainViewModel()
        {
            AddImageCommand = new AsyncRelayCommand(AddImage);
            ExtractTextCommand = new AsyncRelayCommand(ExtractText);
        }

        // Метод для добавления изображения с использованием MediaPicker
        private async Task AddImage()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var result = await MediaPicker.PickPhotoAsync();

                    if (result != null)
                    {
                        var imageSource = ImageSource.FromFile(result.FullPath);
                        Images.Add(imageSource);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Функция выбора изображения не поддерживается на этом устройстве.", "ОК");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось выбрать изображение: {ex.Message}", "ОК");
            }
        }

        // Метод для извлечения текста из всех добавленных изображений
        private async Task ExtractText()
        {
            var extractedTextBuilder = new StringBuilder();

            foreach (var image in Images)
            {
                // Извлечение текста из изображения
                var text = await ExtractTextFromImageAsync(image);
                extractedTextBuilder.AppendLine(text);
            }

            ExtractedText = extractedTextBuilder.ToString();
        }


        // Метод для извлечения текста из одного изображения с помощью Tesseract OCR
        private async Task<string> ExtractTextFromImageAsync(ImageSource imageSource)
        {
            try
            {
                if (imageSource is FileImageSource fileImageSource)
                {
                    using var pix = Pix.LoadFromFile(fileImageSource.File);
                    // Укажите полный путь к файлу `rus.traineddata` или настройте его для использования в вашей конфигурации
                    var tessEngine = new TesseractEngine(@"C:\Users\kuno_\source\repos\ScreenText\ScreenText\Resources\Raw", "rus", EngineMode.Default);


                    using var page = tessEngine.Process(pix);
                    var extractedText = page.GetText();

                    return extractedText;
                }
                return "Не удалось обработать изображение.";
            }
            catch (Exception ex)
            {
                return $"Ошибка OCR: {ex.Message}";
            }
        }
    }
}
