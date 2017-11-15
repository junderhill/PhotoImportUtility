using System;
using System.Collections.Generic;

namespace PhotoImport
{
    internal class UserRespondsPositively : IUserRespondsPositively
    {
        internal IUserIO UserIO = new UserIOWrapper();

        private static readonly List<string> PositiveResponses = new List<string>()
        {
            "yes",
            "y",
            "1"
        };

        public bool ToQuestion(string question)
        {
            Console.WriteLine(question);

            var response = UserIO.ReadLine().ToLower().Replace(" ", "");

            return PositiveResponses.Contains(response);
        }
    }
}