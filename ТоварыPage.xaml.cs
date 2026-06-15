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
    /// Логика взаимодействия для ТоварыPage.xaml
    /// </summary>
    public partial class ТоварыPage : Page
    {
        public ТоварыPage()
        {
            InitializeComponent();
        }

        private demo_examEntities _db = demo_examEntities.GetContext();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Кнопка Добавить только для администратора; поиск, фильтр и сортировка для менеджера и для администратора
            bool isAdmin = Manager.CurrentUser?.Роли_пользователей.Роль_сотрудника == "Администратор";
            bool isManager = Manager.CurrentUser?.Роли_пользователей.Роль_сотрудника == "Менеджер";

            BtnAdd.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            bool canWorkWithProducts = isAdmin || isManager;

            SearchBox.Visibility = canWorkWithProducts ? Visibility.Visible : Visibility.Collapsed;
            FilterCombo.Visibility = canWorkWithProducts ? Visibility.Visible : Visibility.Collapsed;
            SortCombo.Visibility = canWorkWithProducts ? Visibility.Visible : Visibility.Collapsed;

            // Загружаем фильтр по поставщику
            var suppliers = _db.Поставщики.ToList();
            suppliers.Insert(0, new Поставщики { id_поставщика = 0, Поставщик = "Все поставщики" });
            FilterCombo.ItemsSource = suppliers;
            FilterCombo.DisplayMemberPath = "Поставщик";
            FilterCombo.SelectedIndex = 0;
            SortCombo.SelectedIndex = 0;

            LoadProducts();
        }

        //Загрузка списка товаров с учетом поиска, фильтра и сортировки
        private void LoadProducts()
        {
            var query = _db.Товары
                .Include("Наименования_товаров").Include("Категории_товаров")
                .Include("Производители").Include("Поставщики")
                .AsQueryable();

            // Поиск по тексту
            string search = SearchBox.Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    (t.Наименования_товаров.Наименование_товара ?? "").ToLower().Contains(search) ||
                    (t.Категории_товаров.Категория_товара ?? "").ToLower().Contains(search) ||
                    (t.Описание_товара ?? "").ToLower().Contains(search) ||
                    (t.Единица_измерения ?? "").ToLower().Contains(search) ||                                       
                    (t.Производители.Производитель ?? "").ToLower().Contains(search) ||
                    (t.Поставщики.Поставщик ?? "").ToLower().Contains(search));
            }

            // Фильтр по поставщику
            var sup = FilterCombo.SelectedItem as Поставщики;
            if (sup != null && sup.id_поставщика != 0)
                query = query.Where(t => t.id_поставщика == sup.id_поставщика);

            // Сортировка по количеству на складе
            if (SortCombo.SelectedIndex == 1)
            {
                query = query.OrderBy(t => t.Кол_во_на_складе);
            }
            else if (SortCombo.SelectedIndex == 2)
            {
                query = query.OrderByDescending(t => t.Кол_во_на_складе);
            }
            ProductsList.ItemsSource = query.ToList();
        }

        // Поиск в реальном времени
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadProducts();
        }

        // Изменение фильтра
        private void FilterCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }

        // Изменение сортировки
        private void SortCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }

        // Редактирование двойным кликом
        private void Grid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            bool isAdmin = Manager.CurrentUser?.Роли_пользователей.Роль_сотрудника == "Администратор";
            if (!isAdmin)
                return;
            var item = ProductsList.SelectedItem as Товары;
            if (item != null)
                Manager.MainFrame.Navigate(new ТоварыEditPage(item));
        }

        // Добавление товара
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ТоварыEditPage());
        }

        // Удаление товара
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = ProductsList.SelectedItem as Товары;
            if (item == null)
            {
                MessageBox.Show("Выберите товар для удаления.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка: товар не должен участвовать в заказах
            if (_db.Корзина.Any(k => k.id_товара == item.id_товара))
            {
                MessageBox.Show("Нельзя удалить товар, так как он присутствует в заказе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Удалить выбранный товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                _db.Товары.Remove(item);
                _db.SaveChanges();
                LoadProducts();
                MessageBox.Show("Товар удалён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Не удалось удалить товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
