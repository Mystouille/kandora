﻿using System;

namespace kandora.bot.exceptions
{
    public class SignGameException: Exception
    {
        public SignGameException(string message) : base(message)
        {
        }

        public SignGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SignGameException()
        {
        }
    }
}
