using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudmp3.Mp3Players
{
    public class StreamMp3Player
    {
        public enum Mp3PlayerState
        {
            Playing,
            Stopped,
            Paused,

        }

        private Thread _downLoadThread;
        private MemoryStream _cloudStream;
        private HttpWebResponse _webResponse; 

    }
}
