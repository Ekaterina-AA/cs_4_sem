using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Tokenizer : IDisposable, IEnumerable<string>
{
    private HashSet<char> delimiters;
    private string filePath;
    private StreamReader? reader;

    public Tokenizer(HashSet<char> delimiters, string filePath)
    {
        this.delimiters = delimiters;
        this.filePath = filePath;
        reader = new StreamReader(filePath);
    }

    public void Dispose()
    {
        if (reader != null)
        {
            reader.Dispose();
            GC.SuppressFinalize(this);//
        }
    }

    public IEnumerator<string> GetEnumerator()
    {
        if (reader == null)
        {
            throw new ObjectDisposedException("Tokenizer");
        }

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            foreach (var token in line.Split(delimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                yield return token;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    ~Tokenizer()
    {
        if (reader != null)
        {
            reader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

class Program
{
    static void Main()
    {
        HashSet<char> delimiters = new HashSet<char> { ' ', ',', '.', '!', '?', '(', ')' };
        string filePath = "example.txt";

        using (var tokenizer = new Tokenizer(delimiters, filePath))
        {
            foreach (var token in tokenizer)
            {
                Console.WriteLine(token);
            }
        }
    }
}
