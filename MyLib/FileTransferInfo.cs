using System;

namespace MyLib
{
    [Serializable] // атрибут для серіалізації об'єкта класа
    public class FileTransferInfo
    {
        // вміст файла
        public byte[] Data { get; set; }
    }
}
