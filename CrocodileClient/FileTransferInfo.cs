using System;
using System.Collections.Generic;
using System.Text;

namespace CrocodileClient
{
    // Інформація про файл, який відправляється
    [Serializable] 
    public class FileTransferInfo
    {
        // вміст файла
        public byte[] Data { get; set; }
    }
}
