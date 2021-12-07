using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ECS;

namespace Demo
{
    internal class Program
    {
        static void print()
        {
            string v = null;
            v?.ToLower();
            Console.WriteLine("print");
        }

        struct Data
        {
            [JsonInclude]
            public int A;
            [JsonInclude]
            public int B;
            [JsonInclude]
            public int C;
        }
        
        struct Data1
        {
            [JsonInclude]
            public int A;
            [JsonInclude]
            public int B;
            [JsonInclude]
            public int C;
        }

        public class UpdateDataSystem: ISystem
        {
            public void Exec(World world)
            {
                var art = new ArchType();
                art.Add(typeof(Data));
                art.Add(typeof(Data1));

                world.View<Data>(art).Zip(world.View<Data1>(art), (d1, d2) =>
                {
                    d1.A += 1;
                    d1.B += 2;
                    d1.C += 3;

                    d2.A += 11;
                    d2.B += 12;
                    d2.C += 13;
                });
            }
        }
        
        static void Main(string[] args)
        {
            print();

            var world = new World();
            world.AddSystem(new UpdateDataSystem());

            var art = new ArchType();
            art.Add(typeof(Data));
            art.Add(typeof(Data1));

            var start = new System.Diagnostics.Stopwatch();
            start.Start();
            for (int i = 0; i < 2900; i++)
            {
                EntityKey key = null;
                key = world.Add(art, new Data() { A = i+3, B = i+4, C = i+6, });
                world.Add(key, new Data1() { A = i+4, B = i+5, C = i+6, });
            }
            start.Stop();
            Console.WriteLine($"elasped : {start.ElapsedMilliseconds}");

            int index = 0;
            while(true)
            {
                start.Restart();
                for (int i = 0; i < 10000; ++i)
                    world.Exec();
                start.Stop();
                Console.WriteLine($" {index++} change data elasped : {start.ElapsedMilliseconds}");
            }
            
            /*
            var at = new ArchData<Data>();
            at.Add(new Data() { A = 1, B = 2, C = 3, });
            at.Add(new Data() { A = 2, B = 4, C = 3, });
            at.Add(new Data() { A = 3, B = 6, C = 3, });
            at.Remove(0, 1);
            at.Add(new Data() { A = 2, B = 4, C = 3, });
            at.Remove(0, 1);
            at.Add(new Data() { A = 3, B = 6, C = 3, });
            
            Console.WriteLine(at.DebugInfo());
            */
            
            Console.WriteLine("Hello World!");
        }
    }
}
