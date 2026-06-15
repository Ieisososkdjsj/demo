using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace demoExam.ClassesForAll
{
    class Manager
    {
        // Главный Frame для навигации между страницами
        public static Frame MainFrame { get; set; }

        // Текущий авторизованный пользователь
        public static Пользователи CurrentUser { get; set; }

    }
}
