using System;

namespace PhotoImport
{
    public class UserIOWrapper : IUserIO
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }

    public interface IUserIO
    {
        string ReadLine();
    }
}