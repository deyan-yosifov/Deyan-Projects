using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Deyo.Core.Media.Imaging
{
    public class JpegDecoder
    {
        public static BitmapSource GetBitmapSource(Stream jpegStream)
        {
            MemoryStream memory = new MemoryStream();

            using (jpegStream)
            {
                jpegStream.CopyTo(memory);
            }

            memory.Seek(0, SeekOrigin.Begin);
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(memory, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];

            return bitmapSource;
        }
    }
}
