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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private demo_examEntities _db = demo_examEntities.GetContext();

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = _db.Пользователи
                .FirstOrDefault(u => u.Логин == login && u.Пароль == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Manager.CurrentUser = user;
            Manager.MainFrame.Navigate(new ТоварыPage());


        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            Manager.CurrentUser = null;
            Manager.MainFrame.Navigate(new ТоварыPage());
        }

    }
}
