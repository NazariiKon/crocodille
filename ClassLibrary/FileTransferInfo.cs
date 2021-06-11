using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib
{
    [Serializable] // атрибут для серіалізації об'єкта класа
    public class FileTransferInfo
    {
        // вміст файла
        public byte[] Data { get; set; }
        // його розмір
        public int Size { get; set; }
    }
}
