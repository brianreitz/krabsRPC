using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.O365.Security.ETW;

namespace krabsRPC
{
    class Program
    {
        static void Main(string[] args)
        {
            var filter = new EventFilter(
                Filter.EventIdIs(5)
                //.Or(Filter.EventIdIs(6))
                );

            // Microsoft-Windows-RPC EventID 5 - Client RPC call started
            // EventID 6 - Server RPC call started.

            filter.OnEvent += (IEventRecord r) =>
            {
                var endpoint = r.GetUnicodeString("Endpoint");
                var opNum = r.GetUInt32("ProcNum");
                var protocol = r.GetUInt32("Protocol");
                Console.WriteLine($"RPC Event {r.Id}");
                Console.WriteLine($"Endpoint: {endpoint}");
                Console.WriteLine($"Protocol {protocol,0:X}");
                Console.WriteLine($"OpNum: {opNum}");
            };

            var provider = new Provider("Microsoft-Windows-RPC");
            provider.AddFilter(filter);

            var trace = new UserTrace();
            trace.Enable(provider);

            Console.CancelKeyPress += (sender, eventArg) =>
            {
                if (trace != null)
                {
                    trace.Stop();
                }
            };

            trace.Start();
        }
    }
}
