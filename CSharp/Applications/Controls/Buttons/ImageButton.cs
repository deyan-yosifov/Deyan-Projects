using Deyo.Controls.Common;
using Deyo.Controls.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Deyo.Controls.Buttons
{
    [TemplatePart(Name = ImagePartName, Type = typeof(Image))]
    public class ImageButton : Button
    {
        public const string ImagePartName = "buttonImage";
        private Image image;

        public ImageButton()
        {
            this.DefaultStyleKey = (typeof(ImageButton));
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
                    Uri uri = new Uri(resource, UriKind.RelativeOrAbsolute);

                    using (Stream stream = ResourceHelper.GetResourceStream(uri))
                    {
                        imageSource = ImageExtensions.CreateBitmapSource(stream);
                    }
                }

                return imageSource;
            }
        }

        public static readonly DependencyProperty ButtonImageSourceProperty = DependencyProperty.Register("ButtonImageSource",
                         typeof(string), typeof(ImageButton), new PropertyMetadata(null, new PropertyChangedCallback(ButtonImageSourceChanged)));

        private static void ButtonImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageButton button = (ImageButton)d;

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
