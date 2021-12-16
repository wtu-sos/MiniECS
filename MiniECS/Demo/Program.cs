using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ECS;

namespace Demo
{
    internal class Program
    {
        static ArchType art = new ArchType();
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

        struct Data2
        {
            [JsonInclude]
            public int A;
            [JsonInclude]
            public int B;
            [JsonInclude]
            public int C;
        }
        public class DebugSystem: ISystem
        {
            public void Exec(World world)
            {
                world.DebugInfo();
            }
        }
        public class UpdateDataSystem: ISystem
        {
            public void Exec(World world)
            {
                var r = world.Zip(art, (ref Data d1, ref Data1 d2, ref Data2 d3) =>
                {
                    d1.A += 1;
                    d1.B += 2;
                    d1.C += 3;

                    d2.A += 11;
                    d2.B += 12;
                    d2.C += 13;

                    d3.A += 21;
                    d3.B += 22;
                    d3.C += 23;
                });

                foreach (var d in r)
                {
                    if (!d)
                    {
                        break;
                    }
                }
            }
        }
        
        static void Main(string[] args)
        {
            print();

            var world = new World();
            world.AddSystem(new UpdateDataSystem());

            art.Add(typeof(Data));
            art.Add(typeof(Data1));
            art.Add(typeof(Data2));

            var start = new System.Diagnostics.Stopwatch();
            start.Start();
            for (int i = 0; i < 2900; i++)
            {
                //EntityKey key = null;
                world.CreateEntity(art);
                //key = world.Add(art, new Data() { A = i+3, B = i+4, C = i+6, });
                //world.Add(key, new Data1() { A = i+4, B = i+5, C = i+6, });
                //world.Add(key, new Data2() { A = i+4, B = i+5, C = i+6, });
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
                if (index > 100)
                {
                    break;
                }
            }
            
            world.AddSystem(new DebugSystem());
            world.Exec();

            Console.WriteLine("Hello World!");
        }
    }
}
