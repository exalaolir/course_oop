using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace course_oop.Presentation.ViewModels.Components
{
    internal sealed class MenuItemViewModel(string icon, string text, ICommand command) : Base.ViewModel
    {
        public string Icon { get; set; } = icon;
        public string Text { get; set; } = text;
        public ICommand Command { get; set; } = command;
    }
}