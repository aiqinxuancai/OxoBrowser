using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    class SoundSetting
    {
        [DllImport("winmm.dll")]
        static extern int waveOutSetVolume(int _h, int _v);


        public static void SoundClose(bool _close)
        {
            if (_close)
            {
                waveOutSetVolume(0, 0); //关闭声音
            }
            else
            {
                waveOutSetVolume(0, -1); //开启声音
            }
        }


    }
}
