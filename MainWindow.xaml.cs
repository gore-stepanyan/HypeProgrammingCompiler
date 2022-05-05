using System;
using Microsoft.Win32;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FastColoredTextBoxNS;
using System.Windows.Forms.Integration;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace HypeProgrammingCompiler
{
    public partial class MainWindow : Window
    {
        List<bool> isSaved = new List<bool>(); //Флаги сохранений
        List<bool> isExist = new List<bool>(); //Флаги существований в файловой системе
        List<string> filePath = new List<string>(); //Полные имена в файловой системе
        
        public MainWindow()
        {
            InitializeComponent();
            AddKeys();
            AddPage(null, null);
        }

        // Горячие калвиши
        private void AddKeys()
        {
            RoutedCommand openCommand = new RoutedCommand();
            openCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(openCommand, Open));

            RoutedCommand addCommand = new RoutedCommand();
            addCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(addCommand, AddPage));

            RoutedCommand saveCommand = new RoutedCommand();
            saveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(saveCommand, Save));
        }

        private void AddPage(object sender, RoutedEventArgs e)
        {
            //Добавление иконки для конпки закрытия вкладки
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(@"/Resources/close.png", UriKind.RelativeOrAbsolute));

            //Кнопка закрытия вкладки
            Button closeDocumentButton = new Button { Content = image, BorderThickness = new Thickness(0), Background = Brushes.Transparent};
            closeDocumentButton.Click += CloseDocumentButton_Click;

            //Контейнер хранения заголовка вкладки и кнопки закрытия
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(new TextBlock { Text = "Новый документ " + (InputTabControl.Items.Count + 1).ToString(), VerticalAlignment = VerticalAlignment.Center});
            stackPanel.Children.Add(closeDocumentButton);

            //Область ввода текста
            FastColoredTextBox fastColoredTextBox = new FastColoredTextBox();
            fastColoredTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            fastColoredTextBox.TextChanged += FastColoredTextBox_TextChanged;
            //fastColoredTextBox.TextChanged += AnalyzeChangedText;
            fastColoredTextBox.Font = new System.Drawing.Font("Consolas", 12);
            fastColoredTextBox.Zoom = 100;
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
            windowsFormsHost.Child = fastColoredTextBox;

            //Добавление новой вкладки
            TabItem tabItem = new TabItem()
            {
                Header = stackPanel, //Заголовок
                Content = windowsFormsHost, //Текстовое поле
                IsSelected = true
            };
            InputTabControl.Items.Add(tabItem);

            //Добавление список для отслеживания изменений
            isSaved.Add(true);
            isExist.Add(false);
            filePath.Add("");
        }

        private void CloseDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //Получение вкладки которую необходимо закрыть
            Button button = sender as Button;
            StackPanel stackPanel = button.Parent as StackPanel;
            TabItem tabItem = stackPanel.Parent as TabItem;

            //Получаем индекс вкладки которую необходимо закрыть
            int tabIndexToClose = InputTabControl.Items.IndexOf(tabItem);

            //Если изменения сохранены
            if (isSaved[tabIndexToClose])
            {
                InputTabControl.Items.Remove(tabItem); //Закрыть
                isSaved.RemoveAt(tabIndexToClose); //Перестать отслеживать изменения
                isExist.RemoveAt(tabIndexToClose);
                filePath.RemoveAt(tabIndexToClose);
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("В документе были сделаны изменения. Сохранить их?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (messageBoxResult)
                {
                    case MessageBoxResult.No:
                        InputTabControl.Items.Remove(tabItem); //Закрыть
                        isSaved.RemoveAt(tabIndexToClose); //Перестать отслеживать изменения
                        isExist.RemoveAt(tabIndexToClose);
                        filePath.RemoveAt(tabIndexToClose);
                        break;

                    case MessageBoxResult.Yes:
                        Save(sender, e);
                        break;
                }
            }
        }

        // Звёздочки
        private void FastColoredTextBox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            if (isSaved[InputTabControl.SelectedIndex])
            {
                isSaved[InputTabControl.SelectedIndex] = false;
                TabItem tabItem = InputTabControl.SelectedItem as TabItem;
                StackPanel stackPanel = tabItem.Header as StackPanel;
                (stackPanel.Children[0] as TextBlock).Text += "*"; ;
            }
        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            AddPage(null, null);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            //Если файл существует, сохраняем
            if (isExist[InputTabControl.SelectedIndex]) 
            { 
                try
                {
                    TabItem tabItem = InputTabControl.SelectedItem as TabItem;
                    WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
                    FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;

                    File.WriteAllText(filePath[InputTabControl.SelectedIndex], fastColoredTextBox.Text);
                    
                    StackPanel stackPanel = tabItem.Header as StackPanel;
                    (stackPanel.Children[0] as TextBlock).Text = (stackPanel.Children[0] as TextBlock).Text.Trim('*');
                }
                catch (ArgumentException exception) { ErrorPrint("Данный путь недопустим или содержит недопустимые символы"); }
                catch (PathTooLongException exception) { ErrorPrint("Путь или имя файла превышают допустимую длину"); }
                catch (DirectoryNotFoundException exception) { ErrorPrint("Указан недопустимый путь (например, он ведет на несопоставленный диск)"); }
                catch (IOException exception) { ErrorPrint("При открытии файла произошла ошибка ввода-вывода"); }
                catch (UnauthorizedAccessException exception) { ErrorPrint(""); }
                catch (NotSupportedException exception) { ErrorPrint("Неверный формат файла"); }
                catch (SecurityException exception) { ErrorPrint("Неверный формат файла"); }
            }
            else //Иначе - сохраняем как...
            {
                SaveAs(sender, e);
            }
            isSaved[InputTabControl.SelectedIndex] = true;
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "hpl files (*.hpl)|*.hpl|txt files (*.txt)|*.txt|cs files (*.cs)|*.cs|cpp files (*.cpp)|*.cpp|h files (*.h)|*.h|py files (*.py)|*.py|html files (*.html)|*.html|js files (*.js)|*.js|php files (*.php)|*.php";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                TabItem tabItem = InputTabControl.SelectedItem as TabItem;
                WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
                FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
                
                File.WriteAllText(saveFileDialog.FileName, fastColoredTextBox.Text);
                StackPanel stackPanel = tabItem.Header as StackPanel;

                (stackPanel.Children[0] as TextBlock).Text = saveFileDialog.SafeFileName;

                isExist[InputTabControl.SelectedIndex] = true;
                filePath[InputTabControl.SelectedIndex] = saveFileDialog.FileName;
            }
        }

        private void OpenFile(string fileName, string safeFileName)
        {
            foreach (string f in filePath)
            {
                if (f == fileName)
                {
                    throw new IOException();
                }
            }

            AddPage(null, null);
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.Text = File.ReadAllText(fileName);

            StackPanel stackPanel = tabItem.Header as StackPanel;
            (stackPanel.Children[0] as TextBlock).Text = safeFileName;

            isSaved[InputTabControl.SelectedIndex] = true;
            isExist[InputTabControl.SelectedIndex] = true;
            filePath[InputTabControl.SelectedIndex] = fileName;
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "hpl files (*.hpl)|*.hpl|txt files (*.txt)|*.txt|cs files (*.cs)|*.cs|cpp files (*.cpp)|*.cpp|h files (*.h)|*.h|py files (*.py)|*.py|html files (*.html)|*.html|js files (*.js)|*.js|php files (*.php)|*.php";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    OpenFile(openFileDialog.FileName, openFileDialog.SafeFileName);
                }
                catch (FileFormatException exception) { ErrorPrint("Неверный формат файла"); }
                catch (FileLoadException exception) { ErrorPrint("Файл не может быть загружен"); }
                catch (FileNotFoundException exception) { ErrorPrint("Файл не найден"); }
                catch (IOException exception) { ErrorPrint(String.Format("Файл {0} уже загружен", openFileDialog.SafeFileName)); }
            }
        }

        private void ErrorPrint(string message)
        {
            ErrorTextBlock.Text = message;
            (OutputTabControl.Items[1] as TabItem).IsSelected = true;
        }

        private void RemoveTab(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;

            //Получаем индекс вкладки которую необходимо закрыть
            int tabIndexToClose = InputTabControl.Items.IndexOf(tabItem);

            //Если изменения сохранены
            if (isSaved[tabIndexToClose])
            {
                InputTabControl.Items.Remove(tabItem); //Закрыть
                isSaved.RemoveAt(tabIndexToClose); //Перестать отслеживать изменения
                isExist.RemoveAt(tabIndexToClose);
                filePath.RemoveAt(tabIndexToClose);
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("В документе были сделаны изменения. Сохранить их?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (messageBoxResult)
                {
                    case MessageBoxResult.No:
                        InputTabControl.Items.Remove(tabItem); //Закрыть
                        isSaved.RemoveAt(tabIndexToClose); //Перестать отслеживать изменения
                        isExist.RemoveAt(tabIndexToClose);
                        filePath.RemoveAt(tabIndexToClose);
                        break;

                    case MessageBoxResult.Yes:
                        Save(sender, e);
                        break;
                }
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.Undo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.Redo();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.Cut();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.Copy();
        }

        private void Insert(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.InsertText(Clipboard.GetText());
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.ClearSelected();
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
            fastColoredTextBox.SelectAll();
        }

        private void ShowInfo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("HypeProgrammingCompiler\n\n(с) 2022 HPS Андрей Мазуров АВТ-912 АВТФ\ngithub.com/gore-stepanyan/HypeProgrammingCompiler", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (TabItem tabItem in InputTabControl.Items)
            {
                if (!isSaved[InputTabControl.SelectedIndex])
                {
                    tabItem.IsSelected = true;
                    MessageBoxResult messageBoxResult = MessageBox.Show("В документе были сделаны изменения. Сохранить их?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                    switch (messageBoxResult)
                    {
                        case MessageBoxResult.Yes: { Save(null, null); break; }
                        case MessageBoxResult.Cancel: { e.Cancel = true; return; }
                    }
                }
            }
        }

        private void ShowManual(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://gore-stepanyan.github.io/manual/Readme.pdf"));
        }

        private void RunIPV6(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;

            MyRegExp.MatchIPV6(fastColoredTextBox.Text);
            
            OutputListView.Items.Clear();
            foreach (MatchedSubstring substring in MyRegExp.MatchedSubstrings)
            {
                OutputListView.Items.Add(substring);
            }
            FastColoredTextBoxNS.Style BlueStyle = new TextStyle(System.Drawing.Brushes.Blue, null, System.Drawing.FontStyle.Regular);
            fastColoredTextBox.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fastColoredTextBox.Range.SetStyle(BlueStyle, MyRegExp.RegexIPV6);
        }

        private void Run2010(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;

            MyRegExp.Match2010(fastColoredTextBox.Text);

            OutputListView.Items.Clear();
            foreach (MatchedSubstring substring in MyRegExp.MatchedSubstrings)
            {
                OutputListView.Items.Add(substring);
            }
            FastColoredTextBoxNS.Style BlueStyle = new TextStyle(System.Drawing.Brushes.Blue, null, System.Drawing.FontStyle.Regular);
            fastColoredTextBox.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fastColoredTextBox.Range.SetStyle(BlueStyle, MyRegExp.Regex2010);
        }
        private void Run10(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;

            MyRegExp.Match10(fastColoredTextBox.Text);

            OutputListView.Items.Clear();
            foreach (MatchedSubstring substring in MyRegExp.MatchedSubstrings)
            {
                OutputListView.Items.Add(substring);
            }
            FastColoredTextBoxNS.Style BlueStyle = new TextStyle(System.Drawing.Brushes.Blue, null, System.Drawing.FontStyle.Regular);
            fastColoredTextBox.ClearStyle(FastColoredTextBoxNS.StyleIndex.All);
            fastColoredTextBox.Range.SetStyle(BlueStyle, @"0*10*10*10*");
        }

        private void AnalyzeChangedText(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;

            Parser parser = new Parser(fastColoredTextBox.Text);
            parser.Parse();

            OutputListView.Items.Clear();
            foreach (Parser.Error error in parser.errorList)
            {
                OutputListView.Items.Add(error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = InputTabControl.SelectedItem as TabItem;
            WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
            FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;

            fastColoredTextBox.Focus();
        }
    }
}