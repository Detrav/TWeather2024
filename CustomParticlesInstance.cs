using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeather2024
{
    public class CustomParticlesInstance
    {
        private readonly ParticleItem[] items;
        private readonly float[] buffer;
        private int minX;
        private int maxX;
        private int minY;
        private int maxY;

        public int Height { get; private set; }
        public int Width { get; private set; }
        public int Count { get; }
        public double GenerateSpeed = 100;
        private double nextGeneration = 0;
        public int MinX
        {
            get => minX;
            set
            {
                minX = value;
                Width = MaxX - MinX;
            }
        }
        public int MaxX
        {
            get => maxX;
            set
            {
                maxX = value;
                Width = MaxX - MinX;
            }
        }
        public int MinY
        {
            get => minY;
            set
            {
                minY = value;
                Height = MaxY - MinY;
            }
        }
        public int MaxY
        {
            get => maxY;
            set
            {
                maxY = value;
                Height = MaxY - MinY;
            }
        }

        public Random Random { get; } = new Random();

        public CustomParticlesInstance(int count = 1000)
        {
            this.Count = count;

            buffer = new float[count * 12];
            items = new ParticleItem[count];

            for (int i = 0; i < count; i++)
            {
                items[i] = new ParticleItem(this, buffer, i);
            }
        }

        public float[] GetBuffer()
        {
            return buffer;
        }

        public void ProcessCurrent(double delta)
        {
            if (nextGeneration < 1)
            {
                nextGeneration += GenerateSpeed * delta;
            }
            int generateCount = 0;

            if (nextGeneration > 1)
            {
                generateCount = (int)nextGeneration;
                nextGeneration -= generateCount;
            }

            foreach (var item in items)
            {
                if (item.IsVisible)
                {
                    item.Process(delta);
                }
                else if (generateCount > 0)
                {
                    item.GenerateRandomize();
                    generateCount--;
                }
            }
        }
    }
}
