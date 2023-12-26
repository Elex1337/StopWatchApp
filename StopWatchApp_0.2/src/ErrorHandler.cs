using Microsoft.Maui.Controls;
using System;

namespace StopWatchApp_0._2
{
    public class ErrorHandler
    {
        public static bool TryParseInput(string input, out int result)
        {
            result = 0;

            switch (string.IsNullOrWhiteSpace(input))
            {
                case true:
                    DisplayAlert("Ошибка", "Введите значение.", "OK");
                    return false;

                case false when !int.TryParse(input, out result):
                    DisplayAlert("Ошибка", "Введите корректное число.", "OK");
                    return false;

                case false when result <= 0:
                    DisplayAlert("Ошибка", "Введите положительное число.", "OK");
                    return false;

                default:
                    return true;
            }
        }

        public static void DisplayAlert(string title, string message, string cancel)
        {
            MainThread.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert(title, message, cancel));
        }
    }
}

