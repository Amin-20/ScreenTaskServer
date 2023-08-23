using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenRecorder_Server_Side.Helpers
{
    public class ImageHelper
    {
        public void CreateFolder(string path)
        {
            int number = 0;
            while (Directory.Exists(path))
            {
                if (number != 0)
                {
                    path.Replace((number - 1).ToString(), "");
                    path += number;
                }
                number++;
            }
            Directory.CreateDirectory(path);
        }

        public string GetImagePath(byte[] buffer, string path)
        {
            ImageConverter ic = new ImageConverter();
            var data = ic.ConvertFrom(buffer);
            Image img = data as Image;

            if (img != null)
            {
                Bitmap bitmap = new Bitmap(img);
                var strGuid = Guid.NewGuid().ToString();
                var imagePath = $@"{path}\image{strGuid}.png";
                bitmap.Save(imagePath);
                return imagePath;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}