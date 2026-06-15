using demoExam.ClassesForAll;
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

namespace demoExam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Навигация на страницу авторизации при запуске
        public MainWindow()
        {
            InitializeComponent();
            Manager.MainFrame = MainFrame;
            Manager.MainFrame.Navigate(new AuthPage());
        }

        // Обновление информации о пользователе и кнопке входа
        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (Manager.MainFrame.Content is AuthPage)
            {
                UserBlock.Text = "";
                BtnBack.Visibility = Visibility.Collapsed;
            }
            else
            {
                BtnBack.Visibility = Visibility.Visible;

                if (Manager.CurrentUser != null)
                {
                    UserBlock.Text = $"{Manager.CurrentUser.Фамилия} {Manager.CurrentUser.Имя} {Manager.CurrentUser.Отчество}";
                }
                else
                {
                    UserBlock.Text = "Гость";
                }
            }
        }

        //Выход из учетной записи 
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.CurrentUser = null;

            Manager.MainFrame.Navigate(new AuthPage());

            UserBlock.Text = "";
            BtnBack.Visibility = Visibility.Collapsed;
        }
    }
}