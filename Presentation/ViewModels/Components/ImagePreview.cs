using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.Commands;

namespace course_oop.Presentation.ViewModels.Components
{
    class ImagePreview<T>(T image, Action<T> delete) where T : IImage
    {
        private T _image = image;

        internal T Image
        {
            get => _image;
        }

        public ICommand DeleteCommand { get; } = new Command(() => delete.Invoke(image));

        public string Path
        {
            get => _image.Path;
        }
    }
}