using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    class Patient
    {
        public required string name { get; set; }
        public int illness { get; set; }
    }

    static object lockObj = new object();
    static Queue<Patient> queue_docs = new Queue<Patient>();
    static Queue<Patient> queue_patients = new Queue<Patient>();
    static int N = 5; // количество мест в смотровой
    static int M = 2; // количество докторов
    static int T = 2000; // время приема у доктора
    static int ill_or_not_in_docs;

   
    static async Task Main()
    {
        async Task PatientArrival()
        {
            Random rnd = new Random();
            int patient_num = 1;
            while (true)
            {
                await Task.Delay(rnd.Next(1000, 4000));
                string patient = "Patient " + patient_num.ToString();
                var ill = rnd.Next(0, 2);
                patient_num++;
                
                lock (lockObj)
                {
                    var temp_str = ill == 1 ? "is ill" : "is healthy";
                    Console.WriteLine($"New {patient} arrived and {temp_str}.");
                    if (queue_docs.Count == 0)
                    {
                         ill_or_not_in_docs = ill;
                    }
                
                    if (queue_docs.Count < N && ill == ill_or_not_in_docs)
                    {
                        queue_docs.Enqueue(new Patient { name = patient, illness = 1 });
                        var ill_str = ill_or_not_in_docs == 1 ? "is ill" : "is healthy";
                        Console.WriteLine($"{patient} entered the waiting room and {ill_str}.");
                    }
                    else
                    {
                        queue_patients.Enqueue(new Patient { name = patient, illness = ill });
                        Console.WriteLine($"{patient} joined the queue.");
                        if (ill == 1)
                        {
                            async Task P()
                            {
                                await Task.Delay(3000);
                                lock (lockObj)
                                { 
                                    var temp = new Patient { name = patient, illness = 1 };
                                    if (queue_patients.Contains(temp))
                                    {
                                        Queue<Patient> newQueue = new Queue<Patient>(queue_patients.Count);
                                        foreach (var item in queue_patients)
                                        {
                                            newQueue.Enqueue(new Patient { name = item.name, illness = 1 });
                                        }
                                        queue_patients = newQueue;
                                    }
                                }
                                await P();
                            }
                        }
                    }
                 }
            }
        };



        async Task PatientQueue()
        {
            while (true)
            {
                var pat = queue_patients.Peek();
                if (queue_docs.Count < N && pat.illness == ill_or_not_in_docs)
                {
                    lock (lockObj)
                    {
                        pat = queue_patients.Dequeue();
                        queue_docs.Enqueue(pat);
                        var ill_str = ill_or_not_in_docs == 1 ? "is ill" : "is healthy";
                        Console.WriteLine($"{pat.name} entered the waiting room and {ill_str}.");
                    }
                }
            }
        };

        async Task DoctorTreatment (int delay)
        {
            Random rnd = new Random();
            await Task.Delay(delay);
           
            var random_help = rnd.Next(1000, T);
            bool flag = false;
            if (random_help > T - 500)
            {
                await Task.Delay(random_help);
                flag = true;
            }
            Patient patient;
            
                if (queue_docs.Count > 0)
                {
                    patient = queue_docs.Dequeue();
                    ill_or_not_in_docs = patient.illness;
                    Console.WriteLine($"Doctor treated {patient.name} for {delay} time units.");

                    if (flag == true)
                    {
                        Console.WriteLine($"Doctor helped to treat {patient.name} for {delay} time units.");
                    }
                }
                else
                {
                    Console.WriteLine("No patients to treat.");
                }
        };

        async Task Doctors()
        {
            Random rnd = new Random();
            var tasks = new Task[M];
            while (true)
            {
                for (int i = 0; i < M; i++)
                {
                    if (queue_docs.Count > 0)
                    {
                        lock (lockObj)
                        {
                            var time = rnd.Next(1000, T);
                            tasks[i] = DoctorTreatment(time);
                        }
                    await tasks[i];
                    }
                    else
                    {
                        break;
                    }
                }
            }
        };

        var patientTask = PatientArrival();
        var doctorTask = Doctors();
        var patientQ = PatientQueue();

        await Task.WhenAll(patientTask, doctorTask, patientQ);

        Console.ReadLine();
    }
}
