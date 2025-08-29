using UnityEngine;

namespace Game.Data
{
    public static class GlobalDefinitions
    {
        public static Size GetSize(float size)
        {
            if (size >= 65536) return Size.s65536;
            if (size >= 32768) return Size.s32768;
            if (size >= 16384) return Size.s16384;
            if (size >= 8192) return Size.s8192;
            if (size >= 4096) return Size.s4096;
            if (size >= 2048) return Size.s2048;
            if (size >= 1024) return Size.s1024;
            if (size >= 512) return Size.s512;
            if (size >= 256) return Size.s256;
            if (size >= 128) return Size.s128;
            if (size >= 64) return Size.s64;
            if (size >= 32) return Size.s32;
            if (size >= 16) return Size.s16;
            if (size >= 8) return Size.s8;
            if (size >= 4) return Size.s4;
            if (size >= 2) return Size.s2;
            else return Size.s1;
        }

        public static int GetSizeInt(Size size)
        {
            if ((int)size >= 17) return 65536;
            if ((int)size >= 16) return 32768;
            if ((int)size >= 15) return 16384;
            if ((int)size >= 14) return 8192;
            if ((int)size >= 13) return 4096;
            if ((int)size >= 12) return 2048;
            if ((int)size >= 11) return 1024;
            if ((int)size >= 10) return 512;
            if ((int)size >= 9) return 256;
            if ((int)size >= 8) return 128;
            if ((int)size >= 7) return 64;
            if ((int)size >= 6) return 32;
            if ((int)size >= 5) return 16;
            if ((int)size >= 4) return 8;
            if ((int)size >= 3) return 4;
            if ((int)size >= 2) return 2;
            else return 1;
        }
    }
    public enum Size
    {
        s1 = 1,
        s2 = 2,
        s4 = 3,
        s8 = 4,
        s16 = 5,
        s32 = 6,
        s64 = 7,
        s128 = 8,
        s256 = 9,
        s512 = 10,
        s1024 = 11,
        s2048 = 12,
        s4096 = 13,
        s8192 = 14,
        s16384 = 15,
        s32768 = 16,
        s65536 = 17
    }
}
