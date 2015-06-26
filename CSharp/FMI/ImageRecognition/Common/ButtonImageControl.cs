using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageRecognition.Common
{
    [TemplatePart(Name = ImagePartName, Type = typeof(Image))]
    public class ButtonImageControl : Button
    {
        public const string ImagePartName = "buttonImage";
        private Image image;

        public ButtonImageControl()
        {
            this.DefaultStyleKey = (typeof(ButtonImageControl));
        }

        public string ButtonImageSource
        {
            get
            {
                return this.GetValue(ButtonImageSourceProperty) as string;
            }
            set
            {
                this.SetValue(ButtonImageSourceProperty, value);
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                ImageSource imageSource = null;
                string resource = this.ButtonImageSource;

                if (resource != null)
                {
                    AssemblyName assemblyName = new AssemblyName(typeof(ButtonImageControl).Assembly.FullName);
                    string resourcePath = "/" + assemblyName.Name + ";component/" + resource;
                    Uri resourceUri = new Uri(resourcePath, UriKind.Relative);

                    Stream stream = Application.GetResourceStream(resourceUri).Stream;
                    imageSource = ImageExtensions.CreateBitmapSource(stream);
                }

                return imageSource;
            }
        }

        public static readonly DependencyProperty ButtonImageSourceProperty = DependencyProperty.Register("ButtonImageSource",
                         typeof(string), typeof(ButtonImageControl), new PropertyMetadata(null, new PropertyChangedCallback(ButtonImageSourceChanged)));

        private static void ButtonImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonImageControl button = (ButtonImageControl)d;

            if (button.image != null)
            {
                button.image.Source = button.ImageSource;
            }            
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.image = (Image)this.GetTemplateChild(ImagePartName);
            this.image.Source = this.ImageSource;
        }
    }
}
