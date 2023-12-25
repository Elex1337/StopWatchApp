using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace StopWatchApp_0._2
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private bool isTimerRunning;
        private TimeSpan elapsedTime;
        private int remainingTimeInSeconds;
        private Task timerTask;

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand StoppCommand { get; }
        public ICommand PauseResumeCommand { get; }

        public string TimerText => elapsedTime.ToString(@"hh\:mm\:ss");

        public MainPage()
        {
            InitializeComponent();

            StartCommand = new Command(OnStartButtonClicked);
            StopCommand = new Command(OnStopButtonClicked);
            PauseResumeCommand = new Command(OnPauseResumeButtonClicked);

            BindingContext = this;
        }

        private async void OnStartButtonClicked()
        {
            if (!isTimerRunning)
            {
                string input = await DisplayPromptAsync("Таймер", "Введите время в секундах:", maxLength: 10, keyboard: Keyboard.Numeric);
                if (int.TryParse(input, out int timeInSeconds))
                {
                    remainingTimeInSeconds = timeInSeconds;
                    isTimerRunning = true;

                    timerTask = Task.Run(async () =>
                    {
                        while (isTimerRunning && remainingTimeInSeconds > -1)
                        {
                            elapsedTime = TimeSpan.FromSeconds(remainingTimeInSeconds);
                            OnPropertyChanged(nameof(TimerText));

                            await Task.Delay(1000);
                            remainingTimeInSeconds--;
                        }

                        if (remainingTimeInSeconds == -1)
                        {
                            MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Таймер", "Время вышло!", "OK"));
                            isTimerRunning = false;
                        }
                    });
                }
            }
        }

        private void OnStopButtonClicked()
        {
            isTimerRunning = false;
            if (remainingTimeInSeconds > 0)
            {
                remainingTimeInSeconds = 0;
                elapsedTime = TimeSpan.Zero;
                OnPropertyChanged(nameof(TimerText));
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Таймер", "", "OK"));
            }
        }

        private void OnPauseResumeButtonClicked()
        {
            isTimerRunning = !isTimerRunning;

            if (isTimerRunning && (timerTask == null || timerTask.Status != TaskStatus.Running))
            {
                timerTask = Task.Run(async () =>
                {
                    while (isTimerRunning && remainingTimeInSeconds > -1)
                    {
                        elapsedTime = TimeSpan.FromSeconds(remainingTimeInSeconds);
                        OnPropertyChanged(nameof(TimerText));

                        await Task.Delay(1000);
                        remainingTimeInSeconds--;
                    }

                    if (remainingTimeInSeconds == -1)
                    {
                        MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Таймер", "Время вышло!", "OK"));
                        isTimerRunning = false;
                    }
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
