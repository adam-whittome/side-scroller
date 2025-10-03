using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor
{
    public class SoundManager
    {
        public Dictionary<string, SoundEffect> Music;
        public Dictionary<string, SoundEffect> SoundEffects;
        public SoundManager(Dictionary<string, SoundEffect> music, Dictionary<string, SoundEffect> soundEffects)
        {
            Music = music;
            SoundEffects = soundEffects;
        }
        public void PlaySoundEffect(string name)
        {
            SoundEffects[name].Play();
        }
        public void PlayMusic(string name)
        {
            SoundEffectInstance music = Music[name].CreateInstance();
            music.IsLooped = true;
            music.Play();
        }
    }
}
