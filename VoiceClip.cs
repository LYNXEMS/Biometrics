using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometrics
{
    public class VoiceClip
    {
        public AudioFileReader audio;

        public VoiceClip (string file)
        {
            this.audio = new AudioFileReader(file);
        }

        
    }
}
