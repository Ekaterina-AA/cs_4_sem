using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static Program;

class Program
{
    public class Person
    {
        public string Name;
        public bool Infected;
        public bool Immune;
        public List<Person> Contacts { get; set; }

        public Person()
        {
            Contacts = new List<Person>();
        }
    }

    public void ReadPeopleFromFile(string filePath)
    {
        try
        {
            people = new List<Person>(); 
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line != null)
                    {
                        string[] words = line.Split(' ');

                        Person person = new Person();
                        int temp = people.FindIndex(per => per.Name == words[0]);
                        if (temp != -1)
                        {
                            person = people[temp];
                        }
                        else
                        {
                            person.Name = words[0];
                            person.Infected = false;
                            person.Immune = false;
                            people.Add(person);
                        }

                        for (int i = 1; i < words.Length; i++)
                        {
                            int index = people.FindIndex(per => per.Name == words[i]);
                            if (index != -1)
                            {
                                var contact = people[index];
                                person.Contacts.Add(contact);
                            }
                            else
                            {
                                Person contact_to_add = new Person();
                                contact_to_add.Name = words[i];
                                contact_to_add.Infected = false;
                                person.Immune = false;

                                people.Add(contact_to_add);
                                person.Contacts.Add(contact_to_add);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("file error: " + e.Message);
        }
    }

    public List<Person> people;
    public double pos_of_infection;
    public double pos_of_recovery;

    static void Main()
    {
        //генерация файла с людьми
        /*
        int patient_num = 1;

        using (StreamWriter file = new StreamWriter("5.txt"))
        {
            Random rnd = new Random();
            
            for (int i = 0; i < 50000; i++)
            {
                string patient = "Patient" + patient_num.ToString();
                patient_num++;

                int amount_of_contacts = rnd.Next(1,5);
                var contacts = new Dictionary<int, int>();
                for (int i2 = 0; i2 < amount_of_contacts; i2++)
                {
                    int temp = rnd.Next(1,50000);
                    contacts[temp] = 0;
                }

                file.Write(patient + " ");

                foreach (var key in contacts.Keys)
                {
                    file.Write("Patient" + key.ToString() + " ");
                }
                file.WriteLine();
            }
        }
        */

        try
        {
            string file = "5.txt";
            Program program = new Program();
            program.people = new List<Person>();

            program.ReadPeopleFromFile(file);

            int days = 100;

            Random random = new Random();
            var rand_person = random.Next(0, program.people.Count);
            program.people[rand_person].Infected = true;
            Console.WriteLine($"{program.people[rand_person].Name} was infected for the first time");

            for (int i = 1; i <= days; i++)
            {
                foreach (var person in program.people)
                {
                    if (person.Infected == true)
                    {
                        if (random.Next(0, 100) < 10)
                        {
                            person.Infected = false;
                            person.Immune = true;
                            break;
                        }
                    }
                    else if (person.Infected == false && person.Immune == false)
                    {
                        foreach (var contact in person.Contacts)
                        {
                            if (contact.Infected)
                            {
                                if (random.Next(0, 100) < 80)
                                {
                                    person.Infected = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            int infected = 0;
            int healthy = 0;
            foreach (var person in program.people)
            {
                if (person.Infected)
                {
                    infected++;
                    //Console.WriteLine($"\n{person.Name} is infected");
                }
                else
                {
                    healthy++;
                    //Console.WriteLine($"\n{person.Name} is healthy");
                }
            }
            Console.WriteLine($"\n{infected} people are infected");
            Console.WriteLine($"\n{healthy} people are healthy");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
        
}
