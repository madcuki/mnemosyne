/***************************************************************************************
*    Title: CryptoRandom
*    Author: Stephen Toub and Shawn Farkas
*    Date: 09/2007
*    Source URL: https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/september/net-matters-tales-from-the-cryptorandom
*
***************************************************************************************/

using System;
using System.Security.Cryptography;

namespace Mnemosyne
{
    public class MnemRandom : Random
    {
        private RNGCryptoServiceProvider _rng;
        private byte[] _uint32_buffer;

        public MnemRandom()
        {
            _rng = new RNGCryptoServiceProvider();
            _uint32_buffer = new byte[4];
        }

        public override int Next()
        {
            _rng.GetBytes(_uint32_buffer);
            return BitConverter.ToInt32(_uint32_buffer, 0) & 0x7FFFFFFF;
        }

        public override int Next(int max_value)
        {
            if (max_value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max_value));
            }
            return Next(0, max_value);
        }

        public override int Next(int min_value, int max_value)
        {
            if (min_value > max_value)
            {
                throw new ArgumentOutOfRangeException(nameof(min_value));
            }
            if (min_value == max_value)
            {
                return min_value;
            }
            long diff = max_value - min_value;
            while (true)
            {
                _rng.GetBytes(_uint32_buffer);
                uint rand = BitConverter.ToUInt32(_uint32_buffer, 0);

                long max = 1 + (long)uint.MaxValue;
                long remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (int)(min_value + (rand % diff));
                }
            }
        }

        public override double NextDouble()
        {
            _rng.GetBytes(_uint32_buffer);
            uint rand = BitConverter.ToUInt32(_uint32_buffer, 0);
            return rand / (1.0 + uint.MaxValue);
        }

        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            _rng.GetBytes(buffer);
        }

        public byte[] GetNextBytes(int length)
        {
            byte[] number = new byte[length];
            NextBytes(number);

            return number;
        }
    }
}
