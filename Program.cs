using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Создание и запуск главной формы
        Application.Run(new MainForm());
    }
}

public class MainForm : Form
{
    private TextBox outputTextBox;
    private Button runButton;
    private OpenFileDialog openFileDialog;

    public MainForm()
    {
        // Инициализация компонентов формы
        InitializeComponents();

        // Подготовка файла для записи журнала событий
        PrepareEventLogFile();
    }

    private void InitializeComponents()
    {
        outputTextBox = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            ReadOnly = true
        };

        runButton = new Button
        {
            Text = "Запустить",
            Dock = DockStyle.Bottom
        };

        runButton.Click += RunButton_Click;

        openFileDialog = new OpenFileDialog
        {
            Title = "Выберите файл t_help.txt",
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
        };

        Controls.Add(outputTextBox);
        Controls.Add(runButton);

        Text = "Утилита PowerShell";
        Size = new System.Drawing.Size(400, 300);
    }

    private void RunButton_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            // Получение выбранного файла
            string selectedFilePath = openFileDialog.FileName;

            // Вызов метода для выполнения скрипта PowerShell
            RunPowerShellScript(selectedFilePath);

            // Обновление содержимого файла event.txt
            UpdateEventLogFile();
        }
    }

    private void RunPowerShellScript(string scriptFilePath)
    {
        try
        {
            // Чтение содержимого файла
            string scriptContent = File.ReadAllText(scriptFilePath);

            // Создание процесса PowerShell
            using (Process PowerShellProcess = new Process())
            {
                // Настройка параметров процесса PowerShell
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "t_help.ps1",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                PowerShellProcess.StartInfo = startInfo;

                // Запуск процесса PowerShell
                PowerShellProcess.Start();

                // Передача скрипта в стандартный ввод PowerShell
                StreamWriter sw = PowerShellProcess.StandardInput;
                sw.WriteLine(scriptContent);
                sw.Close();

                // Чтение вывода и ошибок
                string output = PowerShellProcess.StandardOutput.ReadToEnd();
                string error = PowerShellProcess.StandardError.ReadToEnd();

                // Вывод результатов в текстовое поле
                outputTextBox.Text = "Вывод скрипта PowerShell:\r\n" + output;

                if (!string.IsNullOrEmpty(error))
                {
                    outputTextBox.Text += "\r\nОшибка выполнения скрипта PowerShell:\r\n" + error;
                }
            }
        }
        catch (Exception ex)
        {
            outputTextBox.Text = "Произошла ошибка: " + ex.Message;
        }
    }

    private void PrepareEventLogFile()
    {
        // Путь к файлу событий
        string eventLogFilePath = "event.txt";

        // Создание/пересоздание файла
        File.WriteAllText(eventLogFilePath, "");
    }

    private void UpdateEventLogFile()
    {
        // Путь к файлу событий
        string eventLogFilePath = "event.txt";

        try
        {
            // Запись в файл
            File.AppendAllText(eventLogFilePath, DateTime.Now.ToString() + ": Запуск скрипта PowerShell\r\n");
        }
        catch (Exception ex)
        {
            outputTextBox.Text += "\r\nОшибка записи в файл событий:\r\n" + ex.Message;
        }
    }
}
