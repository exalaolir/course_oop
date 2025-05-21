using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.Base;

namespace course_oop.Presentation.ViewModels.UsersPart
{
    internal sealed class SettingsViewModel : ViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }
    }
}