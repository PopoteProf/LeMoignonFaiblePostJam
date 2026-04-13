using System;

public static class AudioBus
{
    public static Action<AudioElement> OnPlayAudioElementSFX;
    public static Action<AudioElement> OnPlayAudioElementMusic;
    public static Action<AudioElement> OnPlayAudioElementAmbiance;
}