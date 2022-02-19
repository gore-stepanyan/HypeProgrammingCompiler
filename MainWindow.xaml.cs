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
using System.Reflection;
using ICSharpCode.AvalonEdit;
using System.IO;


namespace HypeProgrammingCompiler
{
    public partial class MainWindow : Window
    {
        // Флаг, указывающий является ли документ сохранённым
        List<bool> isSaved = new List<bool>();
        List<bool> isExist = new List<bool>();
        List<string> filePath = new List<string>();
        

        public MainWindow()
        {
            InitializeComponent();
            AddPage();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void AddPage()
        {
            //Добавление иконки для конпки закрытия вкладки
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(@"/Resources/close.png", UriKind.RelativeOrAbsolute));

            //Кнопка закрытия вкладки
            Button closeDocumentButton = new Button { Content = image, BorderThickness = new Thickness(0), Background = Brushes.Transparent};
            //button.Click += Button_Click;
            closeDocumentButton.Click += CloseDocumentButton_Click;

            //Контейнер хранения заголовка вкладки и кнопки закрытия
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(new TextBlock { Text = "Новый документ " + (InputTabControl.Items.Count + 1).ToString(), VerticalAlignment = VerticalAlignment.Center});
            stackPanel.Children.Add(closeDocumentButton);

            //Область ввода текста
            FastColoredTextBox fastColoredTextBox = new FastColoredTextBox();
            fastColoredTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            fastColoredTextBox.Language = FastColoredTextBoxNS.Language.CSharp;
            fastColoredTextBox.TextChanged += FastColoredTextBox_TextChanged;
            fastColoredTextBox.Font = new System.Drawing.Font("Consolas", 12);
            fastColoredTextBox.Zoom = 100;
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
            windowsFormsHost.Child = fastColoredTextBox;


            //TextEditor textEditor = new TextEditor();
            //textEditor.ShowLineNumbers = true;
            //textEditor.FontFamily = new FontFamily("Consolas");
            //textEditor.FontSize = 12;

            //Добавление новой вкладки
            InputTabControl.Items.Add(new TabItem
            {
                Header = stackPanel, //Заголовок
                Content = windowsFormsHost, //Текстовое поле
                IsSelected = true
            });;

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
            TabItem tabIntem = stackPanel.Parent as TabItem;

            //Получаем индекс вкладки которую необходимо закрыть
            int tabIndexToClose = InputTabControl.Items.IndexOf(tabIntem);

            //Если изменения сохранены
            if (isSaved[tabIndexToClose])
            {
                InputTabControl.Items.Remove(tabIntem); //Закрыть
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
                        InputTabControl.Items.Remove(tabIntem); //Закрыть
                        isSaved.RemoveAt(tabIndexToClose); //Перестать отслеживать изменения
                        isExist.RemoveAt(tabIndexToClose);
                        filePath.RemoveAt(tabIndexToClose);
                        break;

                }
                
            }


        }

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
            AddPage();
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
                catch (ArgumentException exp) { ErrorPrint("Данный путь недопустим или содержит недопустимые символы"); }
                catch (PathTooLongException exp) { ErrorPrint("Путь или имя файла превышают допустимую длину"); }
                catch (DirectoryNotFoundException exp) { ErrorPrint("Указан недопустимый путь (например, он ведет на несопоставленный диск)"); }
                catch (IOException exp) { ErrorPrint("При открытии файла произошла ошибка ввода-вывода"); }
                catch (UnauthorizedAccessException exp) { ErrorPrint(""); }
                catch (NotSupportedException exp) { ErrorPrint("Неверный формат файла"); }
                catch (SecurityException exp) { ErrorPrint("Неверный формат файла"); }
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
                //TextEditor tb = tab.Content as TextEditor;
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

        private void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "hpl files (*.hpl)|*.hpl|txt files (*.txt)|*.txt|cs files (*.cs)|*.cs|cpp files (*.cpp)|*.cpp|h files (*.h)|*.h|py files (*.py)|*.py|html files (*.html)|*.html|js files (*.js)|*.js|php files (*.php)|*.php";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    foreach (string f in filePath)
                    {
                        if (f == openFileDialog.FileName)
                        {
                            throw new IOException();
                        }
                    }

                    if (InputTabControl.Items.Count < 2 && !isExist[0])
                    {
                        InputTabControl.Items.Clear();
                    }

                    AddPage();
                    TabItem tabItem = InputTabControl.SelectedItem as TabItem;
                    WindowsFormsHost windowsFormsHost = tabItem.Content as WindowsFormsHost;
                    FastColoredTextBox fastColoredTextBox = windowsFormsHost.Child as FastColoredTextBox;
                    fastColoredTextBox.Text = File.ReadAllText(openFileDialog.FileName);

                    StackPanel stackPanel = tabItem.Header as StackPanel;
                    (stackPanel.Children[0] as TextBlock).Text = openFileDialog.SafeFileName;

                    isSaved[InputTabControl.SelectedIndex] = true;
                    isExist[InputTabControl.SelectedIndex] = true;
                    filePath[InputTabControl.SelectedIndex] = openFileDialog.FileName;
                }
                catch (FileFormatException exc) { ErrorPrint("Неверный формат файла"); }
                catch (FileLoadException exc) { ErrorPrint("Файл не может быть загружен"); }
                catch (FileNotFoundException exc) { ErrorPrint("Файл не найден"); }
                catch (IOException exc) { ErrorPrint(String.Format("Файл {0} уже загружен", openFileDialog.SafeFileName)); }
            }
        }

        private void ErrorPrint(string message)
        {
            ErrorTextBlock.Text = message;
            (OutputTabControl.Items[1] as TabItem).IsSelected = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
