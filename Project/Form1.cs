using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project 
{
    public partial class Form1 : Form
    {
        private string _lastCreatedFileName; // Глобальная переменная для хранения имени последнего созданного файла
        public Form1()
        {
            InitializeComponent();
            CheckAndCreateDataFolder(); //Инициализируем процесс создания папки Windows
            LoadProfilesToListBox(); //Инициализирует процессор проверки профилей йоу
        }
        private void LoadProfilesToListBox()
        {
            // Очищаем ListBox2 перед обновлением
            listBox2.Items.Clear();

            // Путь к папке Profiles
            string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");

            try
            {
                if (Directory.Exists(profilesFolderPath))
                {
                    // Получаем все папки в Profiles
                    string[] directories = Directory.GetDirectories(profilesFolderPath);

                    // Если папки найдены, добавляем их в ListBox2
                    foreach (string directory in directories)
                    {
                        listBox2.Items.Add(Path.GetFileName(directory));
                    }

                    
                }
                else
                {
                    listBox2.Items.Add("Папка Profiles отсутствует");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке папок из Profiles:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CheckAndCreateDataFolder()
        {
            // Определяем путь к папке Data в корневой папке приложения
            string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            // Проверяем существование папки
            if (!Directory.Exists(dataFolderPath))
            {
                try
                {
                    // Создаем папку, если её нет
                    Directory.CreateDirectory(dataFolderPath);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании папки Data:\n{ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            string profilesFolderPath = Path.Combine(dataFolderPath, "Profiles");
            
            // Проверяем и создаем папку Profiles
            if (!Directory.Exists(profilesFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(profilesFolderPath);
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании папки Profiles:\n{ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        
        private void button6_Click(object sender, EventArgs e) // КНОПКА СОЗДАТЬ ПРОФИЛЬ

        {

            string profileNameInput = PromptForProfileName();

            if (profileNameInput != null) // Проверяем на null
            {
                // Нормализуем строку, убирая лишние пробелы
                string profileName = String.Join(" ", profileNameInput.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

                if (!string.IsNullOrEmpty(profileName)) // Проверяем на пустоту после нормализации
                {
                    // Путь к папке Profiles
                    string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                    string newProfilePath = Path.Combine(profilesFolderPath, profileName);

                    try
                    {
                        // Проверяем, существует ли уже папка с таким именем
                        if (Directory.Exists(newProfilePath))
                        {
                            MessageBox.Show("Профиль с таким именем уже существует!",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            // Создаем новую папку
                            Directory.CreateDirectory(newProfilePath);
                            MessageBox.Show($"Профиль \"{profileName}\" успешно создан!",
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Создаём файл config.txt
                            string configFilePath = Path.Combine(newProfilePath, "config.txt");
                            using (StreamWriter writer = new StreamWriter(configFilePath))
                            {
                                // Сохраняем состояния чекбоксов
                                for (int i = 1; i <= 11; i++)
                                {
                                    CheckBox checkBox = Controls.Find($"checkBox{i}", true).FirstOrDefault() as CheckBox;
                                    if (checkBox != null)
                                    {
                                        writer.WriteLine($"{checkBox.Name}: {(checkBox.Checked ? "true" : "false")}");
                                    }
                                }
                            }

                            // Обновляем ListBox2
                            LoadProfilesToListBox();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при создании профиля:\n{ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Имя профиля не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private string PromptForProfileName()
        {
            // Открываем диалог для ввода имени профиля
            Form prompt = new Form
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Введите имя профиля",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label { Left = 10, Top = 20, Text = "Имя профиля:", AutoSize = true };
            TextBox inputBox = new TextBox { Left = 10, Top = 50, Width = 260 };
            Button confirmation = new Button { Text = "OK", Left = 190, Width = 80, Top = 80, DialogResult = DialogResult.OK };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : null;
        }
        private void button5_Click(object sender, EventArgs e) //удалить профиль
        {
            // Проверяем, есть ли выбранный элемент в ListBox2
            if (listBox2.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите профиль для удаления.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Получаем имя выбранного профиля
            string selectedProfile = listBox2.SelectedItem.ToString();

            // Путь к папке Profiles
            string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");

            // Полный путь к папке профиля
            string profilePath = Path.Combine(profilesFolderPath, selectedProfile);

            try
            {
                // Проверяем, существует ли папка профиля
                if (Directory.Exists(profilePath))
                {
                    // Подтверждение удаления
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить профиль \"{selectedProfile}\"?\nЭто действие нельзя отменить.",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Удаляем папку профиля
                        Directory.Delete(profilePath, true);

                        // Очищаем richTextBox1
                        richTextBox1.Clear();

                        // Обновляем ListBox2
                        LoadProfilesToListBox();

                        MessageBox.Show($"Профиль \"{selectedProfile}\" успешно удален.",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"Папка профиля \"{selectedProfile}\" не найдена.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // 6. Обновляем ListBox1 с файлами из папки "PC"
                LoadFilesIntoListBox(selectedProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении профиля:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Метод для копирования содержимого папки
        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            // Копируем файлы
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(destDir, fileName);
                File.Copy(filePath, destFilePath);
            }

            // Рекурсивно копируем подкаталоги
            foreach (string directoryPath in Directory.GetDirectories(sourceDir))
            {
                string directoryName = Path.GetFileName(directoryPath);
                string destSubDir = Path.Combine(destDir, directoryName);
                CopyDirectory(directoryPath, destSubDir);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                // Открываем диалоговое окно для выбора папки
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Выберите папку для импорта в Profiles";

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Путь к выбранной папке
                        string sourceFolderPath = folderDialog.SelectedPath;

                        // Получаем только имя папки
                        string folderName = new DirectoryInfo(sourceFolderPath).Name;

                        // Путь к папке Profiles
                        string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");

                        // Путь к целевой папке в Profiles
                        string destinationFolderPath = Path.Combine(profilesFolderPath, folderName);

                        // Проверяем, существует ли уже папка с таким именем
                        if (Directory.Exists(destinationFolderPath))
                        {
                            MessageBox.Show($"Папка с именем \"{folderName}\" уже существует в Profiles.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Копируем папку и её содержимое
                        CopyDirectory(sourceFolderPath, destinationFolderPath);

                        // Обновляем ListBox2
                        LoadProfilesToListBox();

                        MessageBox.Show($"Папка \"{folderName}\" успешно импортирована в Profiles.",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Операция импорта отменена.",
                            "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте папки:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, выбран ли профиль в ListBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите профиль для экспорта.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем имя выбранного профиля
                string selectedProfile = listBox2.SelectedItem.ToString();

                // Путь к папке профиля
                string profileFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles", selectedProfile);

                // Проверяем, существует ли папка профиля
                if (!Directory.Exists(profileFolderPath))
                {
                    MessageBox.Show($"Папка профиля \"{selectedProfile}\" не найдена.",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Открытие диалога для выбора папки назначения
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Выберите папку для экспорта профиля";

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Путь назначения
                        string destinationFolderPath = Path.Combine(folderDialog.SelectedPath, selectedProfile);

                        // Проверяем, существует ли уже папка с таким именем в месте назначения
                        if (Directory.Exists(destinationFolderPath))
                        {
                            MessageBox.Show($"В выбранной директории уже существует папка с именем \"{selectedProfile}\".",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Копируем папку профиля
                        CopyDirectory(profileFolderPath, destinationFolderPath);

                        MessageBox.Show($"Профиль \"{selectedProfile}\" успешно экспортирован в {destinationFolderPath}.",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Операция экспорта отменена.",
                            "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте профиля:\n{ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Проверка, выбран ли профиль
                if (listBox2.SelectedItem == null)
                {
                    return;
                }

                // Получаем имя выбранного профиля
                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string configFilePath = Path.Combine(profilesFolderPath, selectedProfile, "config.txt");

                // Проверяем, существует ли конфигурационный файл
                if (File.Exists(configFilePath))
                {
                    // Читаем конфигурационный файл
                    string[] configLines = File.ReadAllLines(configFilePath);

                    // Устанавливаем состояние чекбоксов в зависимости от содержимого файла
                    foreach (string line in configLines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length == 2)
                        {
                            string checkBoxName = parts[0].Trim();
                            string state = parts[1].Trim();

                            CheckBox checkBox = Controls.Find(checkBoxName, true).FirstOrDefault() as CheckBox;
                            if (checkBox != null)
                            {
                                // Устанавливаем состояние чекбокса в зависимости от значения в файле
                                checkBox.Checked = state == "true";
                            }
                        }
                    }
                }

                else
                {
                    MessageBox.Show($"Конфигурационный файл не найден для профиля \"{selectedProfile}\".",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке конфигурации: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshFileList();
            richTextBox1.Clear();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверка, выбран ли профиль в ListBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Выберите профиль для сохранения настроек.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем имя выбранного профиля
                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string configFilePath = Path.Combine(profilesFolderPath, selectedProfile, "config.txt");

                // Проверка и создание директории профиля
                if (!Directory.Exists(profilesFolderPath))
                {
                    Directory.CreateDirectory(profilesFolderPath);
                }
                string profileDirectory = Path.GetDirectoryName(configFilePath);
                if (!Directory.Exists(profileDirectory))
                {
                    Directory.CreateDirectory(profileDirectory);
                }

                // Сохранение конфигурации
                SaveConfig(configFilePath);

                // Уведомление об успешном сохранении
                MessageBox.Show($"Конфигурация профиля \"{selectedProfile}\" успешно сохранена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении конфигурации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Функция сохранения конфигурации (вызывает рекурсивный поиск)
        private void SaveConfig(string configFilePath)
        {
            List<string> configLines = new List<string>();
            CollectCheckBoxStates(this, configLines); // Начинаем поиск с главной формы
            File.WriteAllLines(configFilePath, configLines);
        }

        // Рекурсивная функция для сбора состояний чекбоксов
        private void CollectCheckBoxStates(Control parent, List<string> configLines)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is CheckBox checkBox)
                {
                    configLines.Add($"{checkBox.Name}: {checkBox.Checked.ToString().ToLower()}");
                }
                else if (control.HasChildren) // Проверяем наличие дочерних элементов
                {
                    CollectCheckBoxStates(control, configLines); // Рекурсивный вызов для дочерних элементов
                }
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Запрашиваем имя файла у пользователя
                // string fileName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя файла (без расширения):", "Создание файла", "NewFile");
                string fileName = String.Join(" ", Microsoft.VisualBasic.Interaction.InputBox("Введите имя файла (без расширения):", "Создание файла", "NewFile").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    MessageBox.Show("Имя файла не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Добавляем расширение .txt к имени файла
                string fileNameWithExtension = $"{fileName}.txt";

                // 3. Проверяем, выбран ли профиль в listBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Выберите профиль для создания файла.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 4. Формируем путь к файлу
                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");
                string pcFilePath = Path.Combine(pcFolderPath, fileNameWithExtension);

                // 5. Проверяем и создаем директории профиля и "PC", если нужно
                Directory.CreateDirectory(pcFolderPath); // Автоматически создаст и родительские директории

                // 6. Создаем файл
                File.Create(pcFilePath).Close();
                MessageBox.Show($"Файл \"{fileName}\" успешно создан в папке \"PC\" профиля \"{selectedProfile}\".", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 7. Добавляем имя файла в listBox1 (без расширения)
                listBox1.Items.Add(fileName);

                // 8. Сохраняем имя файла для использования в checkBox2_CheckedChanged
                _lastCreatedFileName = fileNameWithExtension;

                // 9. Дописываем информацию в файл, если соответствующий checkBox установлен
                if (checkBox2.Checked)
                {
                    AppendToFile(pcFilePath, "Процессор:", GetProcessorInfo());
                }
                if (checkBox3.Checked)
                {
                    AppendToFile(pcFilePath, "Видеокарта:", GetGPUInfo());
                }
                if (checkBox4.Checked)
                {
                    AppendToFile(pcFilePath, "Оперативная память:", GetRAMInfo());
                }
                if (checkBox5.Checked)
                {
                    AppendToFile(pcFilePath, "Диски:", GetDrivesInfo());
                }
                if (checkBox6.Checked)
                {
                    AppendToFile(pcFilePath, "Блок питания:", GetPowerSupplyInfo());
                }
                if (checkBox7.Checked)
                {
                    AppendToFile(pcFilePath, "Материнская плата:", GetMotherboardInfo());
                }
                if (checkBox8.Checked)
                {
                    AppendToFile(pcFilePath, "Операционная система:", GetOSInfo());
                }
                if (checkBox9.Checked)
                {
                    AppendToFile(pcFilePath, "BIOS:", GetBIOSInfo());
                }
                if (checkBox10.Checked)
                {
                    AppendToFile(pcFilePath, "IPv4:", GetIPv4Address());
                }
                if (checkBox11.Checked)
                {
                    AppendToFile(pcFilePath, "MAC:", GetMACAddress());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private string GetMACAddress()
        {
            try
            {
                StringBuilder macInfo = new StringBuilder();
                macInfo.Append(""); // Добавляем заголовок "Network:"

                bool macFound = false; // Флаг для проверки, найден ли хоть один MAC-адрес

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        macInfo.AppendLine(); // Добавляем перевод строки перед каждым новым интерфейсом
                        macInfo.AppendLine($"  Interface: {ni.Name}");
                        macInfo.AppendLine($"  MAC Address: {ni.GetPhysicalAddress().ToString()}");
                        macFound = true;
                    }
                }

                // Удаляем последний перевод строки, если он есть
                if (macInfo.ToString().EndsWith(Environment.NewLine))
                {
                    macInfo.Remove(macInfo.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                }

                if (!macFound)
                {
                    macInfo.AppendLine("Информация о сетевых подключениях не найдена.");
                }

                return macInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка получения информации о сети: {ex.Message}" + Environment.NewLine;
            }
        }
        private string GetIPv4Address()
        {
            try
            {
                StringBuilder ipInfo = new StringBuilder();
                ipInfo.Append(""); // Добавляем заголовок "Network:"

                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipInfo.AppendLine(); // Добавляем перевод строки перед каждым новым IP
                                ipInfo.AppendLine($"  Interface: {ni.Name}");
                                ipInfo.AppendLine($"  IPv4 Address: {ip.Address.ToString()}");
                            }
                        }
                    }
                }

                // Удаляем последний перевод строки, если он есть (чтобы не было двух подряд)
                if (ipInfo.ToString().EndsWith(Environment.NewLine))
                {
                    ipInfo.Remove(ipInfo.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                }

                // Добавляем перевод строки (отступ) после информации о IPv4
                ipInfo.AppendLine();

                // Проверяем длину после удаления перевода строки
                if (ipInfo.Length == 8) // 8, а не 7, т.к. удалили один символ перевода строки
                {
                    ipInfo.AppendLine("Информация о сетевых подключениях не найдена.");
                }

                return ipInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка получения информации о сети: {ex.Message}" + Environment.NewLine;
            }
        }
        private string GetBIOSInfo()
        {
            try
            {
                StringBuilder biosInfo = new StringBuilder();
                biosInfo.Append(""); // Добавляем заголовок "BIOS:"

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                var bios = searcher.Get();

                if (bios.Count > 0)
                {
                    biosInfo.AppendLine(); // Добавляем перевод строки после заголовка
                    foreach (ManagementObject b in bios)
                    {
                        biosInfo.AppendLine($"  Производитель: {b["Manufacturer"]}");
                        biosInfo.AppendLine($"  Версия: {b["SMBIOSBIOSVersion"]}");

                        // Форматируем дату, чтобы выводить только дату без времени
                        if (b["ReleaseDate"] != null)
                        {
                            biosInfo.AppendLine($"  Релиз: {ManagementDateTimeConverter.ToDateTime(b["ReleaseDate"].ToString()).ToShortDateString()}");
                        }
                        else
                        {
                            biosInfo.AppendLine($"  Релиз: Неизвестная дата");
                        }
                    }

                    // Удаляем последний перевод строки, добавленный в цикле foreach
                    if (biosInfo.ToString().EndsWith(Environment.NewLine))
                    {
                        biosInfo.Remove(biosInfo.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                    }

                    // Добавляем ОДИН перевод строки после информации о BIOS
                    biosInfo.AppendLine();
                }
                else
                {
                    biosInfo.AppendLine("Информация о BIOS не найдена.");
                }

                return biosInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка получения информации о BIOS: {ex.Message}" + Environment.NewLine;
            }
        }
        private string GetOSInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    // Формируем информацию об ОС
                    string osName = obj["Caption"]?.ToString() ?? "Неизвестная ОС";
                    string osVersion = obj["Version"]?.ToString() ?? "Неизвестная версия";
                    string architecture = obj["OSArchitecture"]?.ToString() ?? "Неизвестная архитектура";

                    // Возвращаем информацию с отступами и переводом строки в конце
                    return $"{osName}\n  Версия: {osVersion}\n  Архитектура: {architecture}\n"; // Добавлен \n в конце
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении информации об ОС: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Не удалось получить информацию об операционной системе." + Environment.NewLine; // Добавлен перевод строки
        }

        private string GetMotherboardInfo()
        {
            try
            {
                // Создаем объект для формирования строки с информацией
                StringBuilder motherboardInfo = new StringBuilder();
                motherboardInfo.Append(""); // Добавляем заголовок "Motherboard:"

                // Используем WMI для получения информации о материнской плате
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                var motherboards = searcher.Get();

                // Проверяем, найдены ли материнские платы
                if (motherboards.Count > 0)
                {
                    motherboardInfo.AppendLine(); // Добавляем перевод строки после заголовка
                    foreach (ManagementObject motherboard in motherboards)
                    {
                        // Добавляем информацию о каждой найденной материнской плате
                        motherboardInfo.AppendLine($"  Модель: {motherboard["Product"]}"); // Отступ для визуального отделения
                        motherboardInfo.AppendLine($"  Производитель: {motherboard["Manufacturer"]}");
                        motherboardInfo.AppendLine($"");
                    }

                    // Удаляем последний перевод строки
                    motherboardInfo.Remove(motherboardInfo.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                }
                else
                {
                    motherboardInfo.AppendLine("Информация о материнской плате не найдена."); // Добавляем перевод строки
                }


                return motherboardInfo.ToString();
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем сообщение с описанием ошибки
                return $"Ошибка получения информации о материнской плате: {ex.Message}" + Environment.NewLine;
            }
        }

        private string GetPowerSupplyInfo()
        {
            try
            {
                // Создаем объект для формирования строки с информацией
                StringBuilder powerSupplyInfo = new StringBuilder();

                // Используем WMI для получения информации о батареях
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");
                var batteries = searcher.Get();

                // Проверяем, найдены ли батареи
                if (batteries.Count > 0)
                {
                    foreach (ManagementObject battery in batteries)
                    {
                        // Добавляем информацию о каждой найденной батарее
                        powerSupplyInfo.AppendLine($"Имя батареи: {battery["Name"]}");
                        powerSupplyInfo.AppendLine($"  Статус: {battery["Status"]}");
                        powerSupplyInfo.AppendLine($"  Остаточный заряд: {battery["EstimatedChargeRemaining"]}%");
                        powerSupplyInfo.AppendLine($"  Оставшееся время работы: {battery["EstimatedRunTime"]} минут");
                        powerSupplyInfo.AppendLine();
                    }
                }
                else
                {
                    // Если батареи не найдены, предполагается, что используется стандартный блок питания
                    powerSupplyInfo.AppendLine("Батарея не обнаружена. Устройство, вероятно, использует стандартный блок питания.");
                }

                return powerSupplyInfo.ToString();
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем сообщение с описанием ошибки
                return $"Ошибка получения информации о блоке питания: {ex.Message}";
            }
        }

        private string GetDrivesInfo()
        {
            try
            {
                StringBuilder drivesInfo = new StringBuilder();
                drivesInfo.Append("");

                DriveInfo[] allDrives = DriveInfo.GetDrives();

                if (allDrives.Length > 0)
                {
                    drivesInfo.AppendLine();
                }
                else
                {
                    drivesInfo.AppendLine("Информация о дисках не найдена");
                    return drivesInfo.ToString();
                }

                foreach (DriveInfo drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        drivesInfo.AppendLine($"  Диск: {drive.Name}");

                        if (!string.IsNullOrEmpty(drive.VolumeLabel))
                        {
                            drivesInfo.AppendLine($"    Имя диска: {drive.VolumeLabel}");
                        }
                        else
                        {
                            drivesInfo.AppendLine($"    Имя диска: (not set)");
                        }

                        drivesInfo.AppendLine($"    Файловая система: {drive.DriveFormat}");
                        drivesInfo.AppendLine($"    Свободное пространство: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB");
                        drivesInfo.AppendLine($"    Пространство: {drive.TotalSize / (1024 * 1024 * 1024)} GB");
                        drivesInfo.AppendLine($"    Тип диска: {drive.DriveType}");
                        drivesInfo.AppendLine(); // Пустая строка между дисками
                    }
                }

                // Удаляем последний перевод строки, если он есть
                if (drivesInfo.ToString().EndsWith(Environment.NewLine))
                {
                    drivesInfo.Remove(drivesInfo.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                }

                return drivesInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Ошибка получения информации о дисках: {ex.Message}" + Environment.NewLine;
            }
        }

        private string GetRAMInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                string ramInfo = "";
                double totalCapacityGB = 0;
                int maxSpeed = 0;

                foreach (ManagementObject mo in searcher.Get())
                {
                    // Получаем объем каждого модуля в байтах
                    ulong capacity = (ulong)mo["Capacity"];
                    totalCapacityGB += capacity / (1024.0 * 1024.0 * 1024.0);

                    // Получаем максимальную частоту из всех модулей
                    int speed = Convert.ToInt32(mo["Speed"]);
                    if (speed > maxSpeed)
                    {
                        maxSpeed = speed;
                    }
                }

                ramInfo = $"{Math.Round(totalCapacityGB)} GB {maxSpeed} MHz";

                // Добавляем отступ вниз (перевод строки)
                ramInfo += Environment.NewLine;

                return ramInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении информации об оперативной памяти: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Не удалось получить информацию об оперативной памяти" + Environment.NewLine; // Добавляем отступ и здесь тоже
            }
        }
        private string GetGPUInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                string gpuInfo = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    gpuInfo += mo["Name"].ToString() + "; ";
                }

                // Удаляем последний разделитель, если он есть
                if (gpuInfo.EndsWith("; "))
                {
                    gpuInfo = gpuInfo.Substring(0, gpuInfo.Length - 2);
                }

                // Добавляем отступ вниз (перевод строки)
                gpuInfo += Environment.NewLine;

                return gpuInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении информации о видеокарте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Не удалось получить информацию о видеокарте" + Environment.NewLine; // Добавляем отступ и здесь тоже
            }
        }
        private string GetProcessorInfo()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return $"{obj["Name"]}{Environment.NewLine}";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении информации о процессоре: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "Не удалось получить информацию о процессоре.";
        }
        private void AppendToFile(string filePath, string key, string value)
        {
            // Убедимся, что директория существует
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Формируем строку для записи
            string lineToAppend = $"{key} {value}";

            // Дописываем строку в файл с указанием кодировки
            File.AppendAllText(filePath, lineToAppend + Environment.NewLine, Encoding.UTF8);
        }
        private void UpdateListBoxWithFiles(string selectedProfile)
        {
            try
            {
                // Очищаем ListBox1 перед обновлением
                listBox1.Items.Clear();

                // Путь к папке "PC" выбранного профиля
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string profileFolderPath = Path.Combine(profilesFolderPath, selectedProfile);
                string pcFolderPath = Path.Combine(profileFolderPath, "PC");

                // Проверяем, существует ли папка "PC"
                if (Directory.Exists(pcFolderPath))
                {
                    // Получаем все файлы в папке "PC"
                    string[] files = Directory.GetFiles(pcFolderPath);

                    // Добавляем имена файлов в ListBox1
                    foreach (string file in files)
                    {
                        listBox1.Items.Add(Path.GetFileName(file)); // Добавляем имя файла, без пути
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, выбран ли файл в ListBox1
                if (listBox1.SelectedItem == null)
                {
                    MessageBox.Show("Выберите файл для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем имя выбранного файла
                string selectedFileName = listBox1.SelectedItem.ToString();

                // Проверяем, выбран ли профиль в listBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Выберите профиль для удаления файла.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Формируем путь к файлу, который нужно удалить
                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");
                string filePath = Path.Combine(pcFolderPath, selectedFileName + ".txt"); // Добавляем расширение .txt

                // Проверяем, существует ли файл
                if (File.Exists(filePath))
                {
                    // Запрашиваем подтверждение на удаление файла
                    DialogResult dialogResult = MessageBox.Show($"Вы уверены, что хотите удалить файл \"{selectedFileName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        // Удаляем файл
                        File.Delete(filePath);
                        MessageBox.Show($"Файл \"{selectedFileName}\" успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Обновляем ListBox1
                        LoadFilesIntoListBox(selectedProfile);

                        // Очищаем richTextBox1
                        richTextBox1.Clear();
                    }
                }
                else
                {
                    MessageBox.Show($"Файл \"{selectedFileName}\" не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadFilesIntoListBox(string selectedProfile)
        {
            try
            {
                // Формируем путь к папке "PC" выбранного профиля
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");

                // Очищаем ListBox перед добавлением новых данных
                listBox1.Items.Clear();

                // Проверяем, существует ли папка "PC"
                if (Directory.Exists(pcFolderPath))
                {
                    // Получаем все файлы из папки "PC"
                    string[] files = Directory.GetFiles(pcFolderPath);

                    // Добавляем файлы в ListBox1 без расширения
                    foreach (string file in files)
                    {
                        listBox1.Items.Add(Path.GetFileNameWithoutExtension(file)); // ИСПРАВЛЕНО: удаляем расширение
                    }
                }
                else
                {
                    MessageBox.Show("Папка для выбранного профиля не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RefreshFileList()
        {
            try
            {
                // Проверяем, выбран ли профиль
                if (listBox2.SelectedItem == null)
                {
                    return; // Ничего не делаем, если профиль не выбран
                }

                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");

                // Очищаем listBox1
                listBox1.Items.Clear();

                // Получаем список файлов из папки "PC" выбранного профиля
                if (Directory.Exists(pcFolderPath))
                {
                    string[] files = Directory.GetFiles(pcFolderPath);
                    foreach (string file in files)
                    {
                        // Добавляем имена файлов в listBox1 без расширения
                        listBox1.Items.Add(Path.GetFileNameWithoutExtension(file)); // Обрезаем расширение .txt
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вызываем RefreshFileList при загрузке формы
        private void YourFormName_Load(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, выбран ли файл в listBox1
                if (listBox1.SelectedItem == null)
                {
                    MessageBox.Show("Выберите файл для открытия его директории.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверяем, выбран ли профиль в listBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Выберите профиль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");
                string selectedFileName = listBox1.SelectedItem.ToString();
                string filePath = Path.Combine(pcFolderPath, selectedFileName + ".txt"); // Добавляем расширение .txt

                // Проверяем, существует ли файл
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Выбранный файл не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Открываем директорию файла
                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии директории: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, выбран ли файл в listBox1
                if (listBox1.SelectedItem == null)
                {
                    return; // Ничего не делаем, если файл не выбран
                }

                // Проверяем, выбран ли профиль в listBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Выберите профиль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");
                string selectedFileName = listBox1.SelectedItem.ToString();

                // Ищем файл с нужным именем и любым расширением
                string filePath = Directory.GetFiles(pcFolderPath, $"{selectedFileName}*").FirstOrDefault();

                // Проверяем, существует ли файл
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Выбранный файл не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Читаем содержимое файла и отображаем в richTextBox1
                string fileContent = File.ReadAllText(filePath);
                richTextBox1.Text = fileContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ШИФРОВАНИЕ 









        
            













        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Проверяем, выбран ли файл в listBox1
                if (listBox1.SelectedItem == null)
                {
                    MessageBox.Show("Выберите файл для шифрования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Проверяем, выбран ли профиль в listBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Выберите профиль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");
                string selectedFileName = listBox1.SelectedItem.ToString();

                // Определение расширения файла
                string fileExtension = Path.GetExtension(Directory.GetFiles(pcFolderPath, $"{selectedFileName}.*").FirstOrDefault());

                string originalFilePath = Path.Combine(pcFolderPath, selectedFileName + fileExtension);

                // 3. Запрашиваем ключ шифрования у пользователя
                string encryptionKey = Microsoft.VisualBasic.Interaction.InputBox("Введите ключ шифрования:", "Шифрование файла", "");
                if (string.IsNullOrWhiteSpace(encryptionKey))
                {
                    MessageBox.Show("Ключ шифрования не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 4. Шифруем содержимое файла с помощью AES
                string encryptedFilePath = Path.Combine(pcFolderPath, selectedFileName + "_encrypted" + fileExtension);
                EncryptFileAes(originalFilePath, encryptedFilePath, encryptionKey);

                // 5. Заменяем оригинальный файл зашифрованным
                File.Delete(originalFilePath);
                File.Move(encryptedFilePath, originalFilePath);

                // 6. Шифруем имя файла (теперь с Base64)
                string encryptedFileName = EncryptFileName(selectedFileName, encryptionKey);

                // Добавляем расширение к зашифрованному имени файла
                string newFilePath = Path.Combine(pcFolderPath, encryptedFileName + fileExtension);

                File.Move(originalFilePath, newFilePath);

                // 7. Обновляем список файлов в listBox1
                RefreshFileList();
                richTextBox1.Clear();

                MessageBox.Show($"Файл \"{selectedFileName}\" успешно зашифрован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при шифровании файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        } 

        private void EncryptFileAes(string inputFile, string outputFile, string password)
        {
            // Generate a random salt
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a key from the password and salt using PBKDF2
            var key = new Rfc2898DeriveBytes(password, salt, 600000, HashAlgorithmName.SHA256);

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (var fsCrypt = new FileStream(outputFile, FileMode.Create))
                {
                    // Write the salt to the beginning of the file
                    fsCrypt.Write(salt, 0, salt.Length);

                    // Calculate hash of the original file
                    byte[] fileHash;
                    using (var sha256 = SHA256.Create())
                    using (var fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        fileHash = sha256.ComputeHash(fsIn);
                    }

                    // Write hash length and hash
                    fsCrypt.Write(BitConverter.GetBytes(fileHash.Length), 0, sizeof(int));
                    fsCrypt.Write(fileHash, 0, fileHash.Length);

                    using (var cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (var fsIn = new FileStream(inputFile, FileMode.Open))
                        {
                            byte[] buffer = new byte[1048576];
                            int read;
                            while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cs.Write(buffer, 0, read);
                            }
                        }
                    }
                }
            }
        }

        // Простой пример функции для шифрования имени файла с помощью XOR
        private string EncryptFileName(string fileName, string key)
        {
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encryptedBytes = new byte[fileNameBytes.Length];

            for (int i = 0; i < fileNameBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(fileNameBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }











        //ДЕШИФРОВАНИЕЕЕЕЕ






        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Проверяем, выбран ли файл в listBox1
                if (listBox1.SelectedItem == null)
                {
                    MessageBox.Show("Выберите файл для расшифровки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Проверяем, выбран ли профиль в listBox2
                if (listBox2.SelectedItem == null)
                {
                    MessageBox.Show("Выберите профиль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedProfile = listBox2.SelectedItem.ToString();
                string profilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Profiles");
                string pcFolderPath = Path.Combine(profilesFolderPath, selectedProfile, "PC");
                string selectedFileName = listBox1.SelectedItem.ToString();

                // Определение расширения файла
                string fileExtension = Path.GetExtension(Directory.GetFiles(pcFolderPath, $"{selectedFileName}.*").FirstOrDefault());

                string encryptedFilePath = Path.Combine(pcFolderPath, selectedFileName + fileExtension);

                // 3. Запрашиваем ключ шифрования у пользователя
                string encryptionKey = Microsoft.VisualBasic.Interaction.InputBox("Введите ключ шифрования:", "Расшифровка файла", "");
                if (string.IsNullOrWhiteSpace(encryptionKey))
                {
                    MessageBox.Show("Ключ шифрования не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 4. Расшифровываем имя файла (сначала Base64)
                string decryptedFileName = DecryptFileName(selectedFileName, encryptionKey);

                // Добавляем расширение к расшифрованному имени файла
                string decryptedFilePath = Path.Combine(pcFolderPath, decryptedFileName + fileExtension);

                File.Move(encryptedFilePath, decryptedFilePath);

                // 5. Расшифровываем содержимое файла
                string tempDecryptedFilePath = Path.Combine(pcFolderPath, decryptedFileName + "_temp" + fileExtension);

                try
                {
                    DecryptFileAes(decryptedFilePath, tempDecryptedFilePath, encryptionKey);
                }
                catch (CryptographicException cex)
                {
                    // Восстанавливаем оригинальное имя файла при ошибке дешифровки
                    File.Move(decryptedFilePath, encryptedFilePath);
                    MessageBox.Show($"Ошибка: {cex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (Exception ex)
                {
                    // Восстанавливаем оригинальное имя файла при ошибке дешифровки
                    File.Move(decryptedFilePath, encryptedFilePath);
                    MessageBox.Show($"Ошибка при расшифровке файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 6. Заменяем зашифрованный файл расшифрованным
                File.Delete(decryptedFilePath);
                File.Move(tempDecryptedFilePath, decryptedFilePath);

                // 7. Обновляем список файлов
                RefreshFileList();
                richTextBox1.Clear();

                MessageBox.Show($"Файл \"{decryptedFileName}\" успешно расшифрован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расшифровке файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }








        }
        private void DecryptFileAes(string inputFile, string outputFile, string password)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var fsCrypt = new FileStream(inputFile, FileMode.Open))
                    {
                        // Read the salt from the beginning of the file
                        byte[] salt = new byte[32];
                        fsCrypt.Read(salt, 0, salt.Length);

                        // Derive the key from the password and salt using PBKDF2
                        var key = new Rfc2898DeriveBytes(password, salt, 600000, HashAlgorithmName.SHA256);
                        aes.Key = key.GetBytes(aes.KeySize / 8);
                        aes.IV = key.GetBytes(aes.BlockSize / 8);

                        // Read hash length and hash
                        byte[] hashLengthBytes = new byte[sizeof(int)];
                        fsCrypt.Read(hashLengthBytes, 0, hashLengthBytes.Length);
                        int hashLength = BitConverter.ToInt32(hashLengthBytes, 0);

                        byte[] originalHash = new byte[hashLength];
                        fsCrypt.Read(originalHash, 0, originalHash.Length);

                        using (var cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (var fsOut = new FileStream(outputFile, FileMode.Create))
                            {
                                byte[] buffer = new byte[1048576];
                                int read;
                                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fsOut.Write(buffer, 0, read);
                                }
                            }
                        }

                        // Verify the decrypted file's hash matches the original
                        byte[] decryptedHash;
                        using (var sha256 = SHA256.Create())
                        using (var fsOut = new FileStream(outputFile, FileMode.Open))
                        {
                            decryptedHash = sha256.ComputeHash(fsOut);
                        }

                        if (!decryptedHash.SequenceEqual(originalHash))
                        {
                            File.Delete(outputFile);
                            throw new CryptographicException("Неверный ключ дешифрования или повреждённый файл.");
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                throw; // Перебрасываем исключение дальше
            }
        }
        private string DecryptFileName(string encryptedFileName, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedFileName);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] decryptedBytes = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }

    }

    
}
