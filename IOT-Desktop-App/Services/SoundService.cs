using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace IOT_Dashboard.Services
{
    /// <summary>
    /// Sound Service - Plays alert sounds (non-blocking, no infinite loops)
    /// </summary>
    public class SoundService
    {
        private bool _soundEnabled = true;
        private SoundPlayer _currentSound = null;
        
        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                _soundEnabled = value;
                if (!value && _currentSound != null)
                {
                    StopSound();
                }
                Console.WriteLine($"[Sound] Sound {(value ? "ENABLED" : "DISABLED")}");
            }
        }
        
        /// <summary>
        /// Play alert sound (critical = error sound, warning = exclamation sound)
        /// </summary>
        public void PlayAlertSound(bool isCritical)
        {
            if (!_soundEnabled) return;
            
            try
            {
                // Stop any currently playing sound
                StopSound();
                
                // Use Windows system sounds (built-in, reliable)
                if (isCritical)
                {
                    SystemSounds.Hand.Play(); // Error sound (more urgent)
                    Console.WriteLine("[Sound] 🔊 Critical alert sound played");
                }
                else
                {
                    SystemSounds.Exclamation.Play(); // Warning sound
                    Console.WriteLine("[Sound] 🔊 Warning alert sound played");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] ❌ Error playing sound: {ex.Message}");
                // Don't crash - sound is non-critical
            }
        }
        
        /// <summary>
        /// Play custom sound from file (optional - for custom .wav files)
        /// </summary>
        public void PlayCustomSound(string wavFilePath)
        {
            if (!_soundEnabled) return;
            if (string.IsNullOrWhiteSpace(wavFilePath) || !File.Exists(wavFilePath))
            {
                Console.WriteLine($"[Sound] ⚠️ Sound file not found: {wavFilePath}");
                return;
            }
            
            try
            {
                StopSound();
                
                _currentSound = new SoundPlayer(wavFilePath);
                _currentSound.Play(); // Non-blocking, plays once
                Console.WriteLine($"[Sound] 🔊 Custom sound played: {wavFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] ❌ Error playing custom sound: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Stop any currently playing sound
        /// </summary>
        public void StopSound()
        {
            try
            {
                if (_currentSound != null)
                {
                    _currentSound.Stop();
                    _currentSound.Dispose();
                    _currentSound = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] ❌ Error stopping sound: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Cleanup resources
        /// </summary>
        public void Dispose()
        {
            StopSound();
        }
    }
}

