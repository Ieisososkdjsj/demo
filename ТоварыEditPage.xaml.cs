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
    /// Логика взаимодействия для ТоварыEditPage.xaml
    /// </summary>
    public partial class ТоварыEditPage : Page
    {
        public ТоварыEditPage()
        {
            InitializeComponent();
        }

        private demo_examEntities _db = demo_examEntities.GetContext();
        private Товары _product; // null = добавление, не null = редактирование

        // Передача товара для редактирования
        public ТоварыEditPage(Товары product = null)
        {
            InitializeComponent();
            _product = product;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Заполнение выпадающих списков
            CategoryCombo.ItemsSource = _db.Категории_товаров.ToList();
            ManufacturerCombo.ItemsSource = _db.Производители.ToList();
            SupplierCombo.ItemsSource = _db.Поставщики.ToList();
            NameCombo.ItemsSource = _db.Наименования_товаров.ToList();

            if (_product != null)
            {
                // Режим редактирования — заполняем поля
                TitleBlock.Text = "Редактирование товара";
                PriceBox.Text = _product.Цена.ToString();
                DiscountBox.Text = _product.Действующая_скидка.ToString();
                StockBox.Text = _product.Кол_во_на_складе.ToString();
                DescBox.Text = _product.Описание_товара;
                CategoryCombo.SelectedValue = _product.id_категории_товара;
                ManufacturerCombo.SelectedValue = _product.id_производителя;
                SupplierCombo.SelectedValue = _product.id_поставщика;
                NameCombo.SelectedValue = _product.id_наименования_товара;
                UnitBox.Text = _product.Единица_измерения;
                PhotoBox.Text = _product.Фото;
            }
        }

        // Сохранение товара
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка цены
            double price;
            if (!double.TryParse(PriceBox.Text, out price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка скидки
            int discount;
            if (!int.TryParse(DiscountBox.Text, out discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть числом от 0 до 100!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка обязательных полей
            if (CategoryCombo.SelectedItem == null || ManufacturerCombo.SelectedItem == null || SupplierCombo.SelectedItem == null || NameCombo.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление нового товара
            Товары product;
            if (_product == null)
            {
                product = new Товары();
                product.id_товара = _db.Товары.Max(t => t.id_товара) + 1;
                _db.Товары.Add(product);
            }
            else
            {
                product = _db.Товары.First(t => t.id_товара == _product.id_товара);
            }
            product.Цена = price;
            product.Действующая_скидка = discount;
            product.Единица_измерения = UnitBox.Text;
            _product.Фото = PhotoBox.Text;

            // Проверка количества на складе
            int stock;
            if (!int.TryParse(StockBox.Text, out stock) || stock < 0)
            {
                MessageBox.Show("Количество на складе должно быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            product.Кол_во_на_складе = stock;
            product.Описание_товара = DescBox.Text;
            product.id_категории_товара = (CategoryCombo.SelectedItem as Категории_товаров).id_категории_товара;
            product.id_производителя = (ManufacturerCombo.SelectedItem as Производители).id_производителя;
            product.id_поставщика = (SupplierCombo.SelectedItem as Поставщики).id_поставщика;
            product.id_наименования_товара = (NameCombo.SelectedItem as Наименования_товаров).id_наименования_товара;
            _db.SaveChanges();
            MessageBox.Show("Сохранено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Manager.MainFrame.Navigate(new ТоварыPage());
        }

        // Возрат на предыдущую страницу
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

    }
}
