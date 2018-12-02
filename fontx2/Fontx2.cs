using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

namespace fontx2
{
    class Fontx2
    {
        string identifier;
        string name;
        byte width;
        byte height;
        byte codeType;
        Hashtable bitmap_list;

        public bool parse(byte[] data)
        {
            int index = 0;
            int dataLength = data.Length;
            Encoding enc = Encoding.GetEncoding("Shift_JIS");
            string str = enc.GetString(data, index, 6);
            if (str.Equals("FONTX2"))
                identifier = str;
            else
                return false;
            index += 6;
            name = enc.GetString(data, index, 8);
            index += 8;
            width = data[index++];
            height = data[index++];
            codeType = data[index++];
            if (codeType == 0)
            {
                bitmap_list = new Hashtable();
                int keyCode = 0;
                while (index < dataLength)
                {
                    Bitmap bitmap = new Bitmap(width, height);
                    for (int y = 0; y < height; y++)
                    {
                        int shift_amount = 0;
                        for (int x = 0; x < width; )
                        {
                            if ((data[index] & (0x80 >> shift_amount)) != 0)
                                bitmap.SetPixel(x, y, Color.Black);
                            else
                                bitmap.SetPixel(x, y, Color.White);

                            shift_amount++;
                            x++;
                            if ((shift_amount >= 8) && (x < width))
                            {
                                shift_amount = 0;
                                index++;
                            }
                        }
                        index++;
                    }

                    bitmap_list.Add(keyCode++, bitmap);
                }
            }
            else
            {
                bitmap_list = new Hashtable();

                int code_area_num = data[index++];
                int code_area_index = index;
                index += (code_area_num * 4);

                for (int count = 0; count < code_area_num; count++ )
                {
                    int begin = (int)(BitConverter.ToUInt16(data, code_area_index));
                    int end = (int)(BitConverter.ToUInt16(data, code_area_index + 2));

                    for (int code_count = begin; code_count <= end; code_count++)
                    {
                        Bitmap bitmap = new Bitmap(width, height);
                        for (int y = 0; y < height; y++)
                        {
                            int shift_amount = 0;
                            for (int x = 0; x < width; )
                            {
                                if ((data[index] & (0x80 >> shift_amount)) != 0)
                                    bitmap.SetPixel(x, y, Color.Black);
                                else
                                    bitmap.SetPixel(x, y, Color.White);

                                shift_amount++;
                                x++;
                                if (shift_amount >= 8)
                                {
                                    shift_amount = 0;

                                    if (x < width)
                                    {
                                        index++;
                                    }
                                }
                            }
                            index++;
                        }

                        bitmap_list.Add(code_count, bitmap);
                    }

                    code_area_index += 4;
                }
            }

            return true;
        }

        public Hashtable BitmapList
        {
            get{ return bitmap_list;}
        }

        public byte Width
        {
            get{ return width;}
        }

        public byte Height
        {
            get { return height; }
        }

        public byte CodeType
        {
            get { return codeType; }
        }

        public string FontName
        {
            get { return name; }
        }
    }
}
