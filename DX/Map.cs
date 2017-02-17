using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX
{
    class Map
    {

        Bitmap bitmap;


        //Принимает путь к файлу карты
        public Map(string map_path)
        {
            bitmap = new Bitmap(map_path);
        }

        public int[,] MapArray()
        {

            int[,] brightnessArray = Map.GetBrightnessArray(bitmap);

            //Сюда строчку с цветами переписываю
            int[] colors = new int[bitmap.Width];

            //Тут нумерация цветов
            //int[] color_number = new int[25]; for (int i = 0; i < color_number.Length; i++, color_number[i] = i) ;

            int[,] result = new int[brightnessArray.GetUpperBound(1), brightnessArray.GetUpperBound(0)+1];

            for (int i = 0; i <= brightnessArray.GetUpperBound(0); i++)
            {
                //Console.Write(" " + brightnessArray[i, 0].ToString());
                colors[i] = brightnessArray[i, 0];
            }


            for (int j = 1; j < brightnessArray.GetUpperBound(1); j++)
            {
                for (int i = 0; i <= brightnessArray.GetUpperBound(0); i++)
                {
                    for (int q = 0; q < colors.Length; q++)
                    {
                        if (brightnessArray[i, j] == colors[q])
                        {
                            result[i, j - 1] = q;
                            //Console.Write(" " + result[i, j - 1].ToString());
                            break;
                        }
                        else {
                            result[i, j - 1] = 0;
                        }
                    }
                    //result[j,i] = 
                    //Console.Write(brightnessArray[i, j].ToString()+ "\t");

                }
                //Console.WriteLine();
            }
            return result;
        }


        public static int[,] GetBrightnessArray(Bitmap srcImage)
        {
            if (srcImage == null)
                throw new ArgumentNullException("srcImage");

            var result = new int[srcImage.Width, srcImage.Height];

            for (var y = 0; y < srcImage.Width; y++)
            {
                for (var x = 0; x < srcImage.Height; x++)
                {
                    Color srcPixel = srcImage.GetPixel(y, x);
                    result[y, x] = srcPixel.ToArgb();
                }
            }

            return result;
        }

    }
}
