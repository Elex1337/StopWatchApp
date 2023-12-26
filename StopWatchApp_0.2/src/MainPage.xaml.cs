using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace StopWatchApp_0._2
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        // Переменная, указывающий, работает ли таймер в текущий момент.
        private bool isTimerRunning;

        // Общее прошедшее время с момента запуска таймера.
        private TimeSpan elapsedTime;

        // Оставшееся время на таймере в секундах.
        private int remainingTimeInSeconds;

        // Задача, выполняющая отсчет времени в фоновом режиме.
        private Task timerTask;

        // 3 наших основных команд
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand PauseResumeCommand { get; }

        public string TimerText => elapsedTime.ToString(@"hh\:mm\:ss");

        public MainPage()
        {
            //инициализирует xaml 
            InitializeComponent();
            //Наши команды
            StartCommand = new Command(OnStartButtonClicked);
            StopCommand = new Command(OnStopButtonClicked);
            PauseResumeCommand = new Command(OnPauseResumeButtonClicked);

            BindingContext = this;
        }

        
        private async void OnStartButtonClicked()
        {
            // Проверяем, не запущен ли уже таймер.
            if (!isTimerRunning)
            {
                // Отображаем диалоговое окно с запросом ввода времени в секундах от пользователя
                string input = await DisplayPromptAsync("Таймер", "Введите время в секундах:", maxLength: 10, keyboard: Keyboard.Numeric);

                // Проверяем, удалось ли корректно преобразовать введенное значение в целое число
                if (ErrorHandler.TryParseInput(input, out int timeInSeconds))
                {
                    // Устанавливаем введенное время в переменную remainingTimeInSeconds
                    remainingTimeInSeconds = timeInSeconds;

                    // Устанавливаем флаг isTimerRunning в true, т.е., таймер запущен
                    isTimerRunning = true;

                    // Запускаем задачу timerTask, которая обновляет таймер.
                    timerTask = Task.Run(async () =>
                    {
                        // Пока таймер работает и оставшееся время больше нуля
                        while (isTimerRunning && remainingTimeInSeconds > -1)
                        {
                            // Обновляем отображаемое время
                            elapsedTime = TimeSpan.FromSeconds(remainingTimeInSeconds);
                            OnPropertyChanged(nameof(TimerText));

                            await Task.Delay(1000);

                            // Уменьшаем оставшееся время.
                            remainingTimeInSeconds--;
                        }

                        // Когда время на таймере достигло нуля
                        if (remainingTimeInSeconds == -1)
                        {
                           
                            ErrorHandler.DisplayAlert("Таймер", "Время вышло!", "OK");

                            // таймер остановлен
                            isTimerRunning = false;
                        }
                    });
                }
            }
        }


        private void OnStopButtonClicked()
        {
            //логика такова что оно останавливает таймер и меняет значение на ноль и текст на 0
            isTimerRunning = false;
            if (remainingTimeInSeconds > 0)
            {
                remainingTimeInSeconds = 0;
                elapsedTime = TimeSpan.Zero;
                OnPropertyChanged(nameof(TimerText));
                ErrorHandler.DisplayAlert("Таймер", "Перезапущен", "OK");
            }
        }


        private void OnPauseResumeButtonClicked()
        {
            // Инвертируем состояние флага, указывающего, работает ли таймер
            isTimerRunning = !isTimerRunning;

            // Проверяем, если таймер запущен и задача таймера не выполняется или не создана.
            if (isTimerRunning && (timerTask == null || timerTask.Status != TaskStatus.Running))
            {
                // Создаем и запускаем новую задачу таймера в фоновом режиме
                timerTask = Task.Run(async () =>
                {
                    // Пока таймер работает и оставшееся время больше нуля
                    while (isTimerRunning && remainingTimeInSeconds > -1)
                    {
                        // Обновляем отображаемое время на интерфейсе.
                        elapsedTime = TimeSpan.FromSeconds(remainingTimeInSeconds);
                        OnPropertyChanged(nameof(TimerText));

                        // Задержка на 1 секунду для эмуляции секундомера
                        await Task.Delay(1000);

                        // Уменьшаем оставшееся время.
                        remainingTimeInSeconds--;
                    }

                    // Когда время на таймере исчерпано.
                    if (remainingTimeInSeconds == -1)
                    {
                        // Отображаем сообщение об окончании времени.
                        ErrorHandler.DisplayAlert("Таймер", "Время вышло!", "OK");

                        // Устанавливаем флаг, что таймер больше не работает.
                        isTimerRunning = false;
                    }
                });
            }
        }



    }
}
