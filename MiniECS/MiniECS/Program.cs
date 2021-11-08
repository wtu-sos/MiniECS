using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECS
{
    class Program
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
        
        static void Main(string[] args)
        {
            print();

            var world = new World();
            var art = new ArchType();
            art.Add(typeof(Data));
            
            world.Add(art, new Data() { A = 1, B = 2, C = 3, });
            world.Add(art, new Data() { A = 2, B = 3, C = 3, });
            world.Add(art, new Data() { A = 3, B = 4, C = 6, });
            world.Add(art, new Data() { A = 4, B = 5, C = 6, });

            foreach (var d in world.View<Data>(art))
            {
                Console.WriteLine($"{JsonSerializer.Serialize(d)}");
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