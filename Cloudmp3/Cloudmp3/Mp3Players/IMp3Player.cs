using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudmp3.Mp3Players
{
    public interface IMp3Player
    {
        void Play(string path = null);
        void Stop();
        void Pause();
    }
}
