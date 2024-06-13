using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using ApplicationCache;

namespace CacheDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = new ApplicationCache<string>(TimeSpan.FromSeconds(3), 4);
            
            cache.Save("key1", "data1");
            cache.Save("key2", "data2");
            cache.Save("key3", "data3");

            Console.WriteLine(cache.Get("key1"));
            Console.WriteLine(cache.Get("key2"));
            Console.WriteLine(cache.Get("key3"));

            try
            {
                cache.Save("key2", "newData2");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            Thread.Sleep(6000);

            try
            {
                Console.WriteLine(cache.Get("key1"));
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}

