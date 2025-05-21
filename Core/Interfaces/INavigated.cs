using System.ComponentModel;

namespace course_oop.Core.Interfaces
{
    internal interface INavigated
    {
        public INavigation? Navigator { get; set; }
    }
}