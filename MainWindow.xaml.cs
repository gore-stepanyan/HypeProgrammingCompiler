using System;
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


namespace HypeProgrammingCompiler
{
    public partial class MainWindow : Window
    {
        // Флаг, указывающий является ли документ сохранённым
        List <bool> isSaved = new List<bool>();

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
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("В документе были сделаны изменения. Сохранить их?", "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (messageBoxResult)
                {
                    case MessageBoxResult.No:
                        InputTabControl.Items.Remove(tabIntem); //Закрыть
                        isSaved.RemoveAt(tabIndexToClose); //Перестать отслеживать изменения
                        break;

                }
                
            }


        }

        private void FastColoredTextBox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            isSaved[InputTabControl.SelectedIndex] = false;
        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            AddPage();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
