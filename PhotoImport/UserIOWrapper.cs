using System;

namespace PhotoImport
{
    public class UserIOWrapper : IUserIO
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }

    public interface IUserIO
    {
        string ReadLine();
        void WriteLine(string line);
    }
}