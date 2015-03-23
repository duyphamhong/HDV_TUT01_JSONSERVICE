using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Networks
{
    public class TcpHeaderFactory
    {
        const int headerCount = 4;

        private MemoryStream ms = new MemoryStream();
        public TcpHeaderFactory()
        {
        }

        public void PumpData(byte[] buffer, int start, int count)
        {
            ms.Write(buffer, start, count);
        }

        public bool TryProcess(out string message)
        {
            message = string.Empty;

            //Reset về đầu stream
            ms.Position = 0;

            //Đọc header
            BinaryReader reader = new BinaryReader(ms);
            if (ms.Length >= headerCount)
            {
                int contentLength = reader.ReadInt32();
                if (ms.Length >= headerCount + contentLength)
                {
                    byte[] messageBuffer = reader.ReadBytes(contentLength);
                    message = Encoding.UTF8.GetString(messageBuffer);

                    var oldStream = ms;
                    oldStream.Close();

                    ms = new MemoryStream();
                }
            }
            
            return !string.IsNullOrEmpty(message);
        }

        public static byte[] ToPacket(byte[] content)
        {
            byte[] packet = new byte[headerCount + content.Length];

            using (BinaryWriter writer = new BinaryWriter(new MemoryStream(packet)))
            {
                writer.Write(content.Length);
                writer.Write(content);
            }
            return packet;
        }
    }
}
