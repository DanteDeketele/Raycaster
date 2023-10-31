using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Raycaster
{
    internal class AudioUnpacker
    {
        public static Dictionary<string, SoundEffect> GetSounds(string folderName)
        {
            Dictionary<string, SoundEffect> soundDictionary = new Dictionary<string, SoundEffect>();

            string[] soundFiles = Directory.EnumerateFiles(folderName, "*.*")
    .Where(file => file.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
    .ToArray();

            foreach (var item in soundFiles)
            {
                Debug.WriteLine(item);
            }

            foreach (string soundFile in soundFiles)
            {
                string soundName = Path.GetFileNameWithoutExtension(soundFile);
                byte[] soundData = File.ReadAllBytes(soundFile);

                SoundEffect soundEffect;
                if (soundFile.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    soundEffect = LoadSoundEffectFromMp3(soundData);
                }
                else if (soundFile.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                {
                    soundEffect = LoadSoundEffectFromWav(soundData);
                }
                else
                {
                    // Unsupported file type
                    continue;
                }

                soundDictionary[soundName] = soundEffect;
            }

            return soundDictionary;
        }

        private static SoundEffect LoadSoundEffectFromMp3(byte[] mp3Data)
        {
            using (MemoryStream mp3Stream = new MemoryStream(mp3Data))
            {
                using (var mp3Reader = new Mp3FileReader(mp3Stream))
                {
                    // Create a temporary WAV file
                    string tempWavFile = Path.GetTempFileName();

                    using (var waveStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
                    {
                        using (var wavWriter = new WaveFileWriter(tempWavFile, waveStream.WaveFormat))
                        {
                            waveStream.CopyTo(wavWriter);
                        }
                    }

                    // Load the temporary WAV file as a SoundEffect
                    using (FileStream wavFileStream = new FileStream(tempWavFile, FileMode.Open))
                    {
                        SoundEffect soundEffect = SoundEffect.FromStream(wavFileStream);
                        return soundEffect;
                    }
                }
            }
        }

        private static SoundEffect LoadSoundEffectFromWav(byte[] wavData)
        {
            using (MemoryStream wavStream = new MemoryStream(wavData))
            {
                SoundEffect soundEffect = SoundEffect.FromStream(wavStream);
                return soundEffect;
            }
        }
    }
}
