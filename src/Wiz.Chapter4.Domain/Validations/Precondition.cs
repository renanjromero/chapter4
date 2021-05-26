using System;

namespace Wiz.Chapter4.Domain.Validations
{
    public static class Precondition
    {
        public static void Requires(bool condition)
        {
            if (!condition)
                throw new InvalidOperationException();
        }
    }
}