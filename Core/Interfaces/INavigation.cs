using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace course_oop.Core.Interfaces
{
    public interface INavigation
    {
        public void Navigate<T>(T page) where T : Page;

        public void Navigate(string pageName);
    }
}