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
    public class StreamMp3Player : IMp3Player
    {
        public enum Mp3PlayerState
        {
            Playing,
            Stopped,
            Paused,
        }

        private Thread _downLoadThread;
        private Thread _playThread;
        private MemoryStream _cloudStream;
        private HttpWebResponse _webResponse;


        public void Play(string path = null)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }
    }
}
