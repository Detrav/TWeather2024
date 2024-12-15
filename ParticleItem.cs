

using Godot;
using System;

namespace TWeather2024
{
    public class ParticleItem
    {
        private CustomParticlesInstance cpi;
        private float[] buffer;
        private int index;
        private int arrindex;
        private bool isVisible;

        public ParticleItem(CustomParticlesInstance customParticlesInstance, float[] buffer, int i)
        {
            this.cpi = customParticlesInstance;
            this.buffer = buffer;
            this.index = i;
            this.arrindex = i * 12;

            buffer[this.arrindex] = 1;
            buffer[this.arrindex + 5] = 1;
        }

        public float X
        {
            get => buffer[arrindex + 3];
            set => buffer[arrindex + 3] = value;
        }
        public float Y
        {
            get => buffer[arrindex + 7];
            set => buffer[arrindex + 7] = value;
        }
        public float SpeedX { get; set; }
        public float SpeedY { get; set; }
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;

                if (value)
                {
                    buffer[arrindex + 8] = 1;
                    buffer[arrindex + 9] = 1;
                    buffer[arrindex + 10] = 1;
                    buffer[arrindex + 11] = 1;
                }
                else
                {
                    buffer[arrindex + 8] = 0;
                    buffer[arrindex + 9] = 0;
                    buffer[arrindex + 10] = 0;
                    buffer[arrindex + 11] = 0;
                }
            }
        }

        //public void SetData(float[] buffer, int index)
        //{
        //    index *= 12;
        //    buffer[index] = 1; // m00
        //    buffer[index + 1] = 0; // m01
        //    buffer[index + 2] = 0; // unk
        //    buffer[index + 3] = X; // offset_x
        //    buffer[index + 4] = 0; // m10
        //    buffer[index + 5] = 1; // m11
        //    buffer[index + 6] = 0; // unk
        //    buffer[index + 7] = Y; // offset_y
        //    buffer[index + 8] = 1; // r
        //    buffer[index + 9] = 1; // g
        //    buffer[index + 10] = 1; // b
        //    buffer[index + 11] = 1; // a
        //}

        internal void Process(double delta)
        {
            X += (float)(SpeedX * delta);
            Y += (float)(SpeedY * delta);

            if (X < cpi.MinX)
            {
                X += cpi.Width;
            }
            else if (X > cpi.MaxX)
            {
                X -= cpi.Width;
            }

            if (Y > cpi.MaxY)
            {
                IsVisible = false;
            }

        }

        internal void GenerateRandomize()
        {
            X = cpi.MinX + (float)(cpi.Random.NextDouble() * cpi.Width);
            Y = cpi.MinY;

            var v = new Vector2(0, 100 + 100 * (float)cpi.Random.NextDouble()).Rotated((float)((0.5 - cpi.Random.NextDouble()) * Math.PI / 3));

            SpeedX = v.X;
            SpeedY = v.Y;

            IsVisible = true;
        }
    }
}
