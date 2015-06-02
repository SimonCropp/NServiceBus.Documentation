﻿#region UsingLogging
using NServiceBus.Logging;

public class ClassUsingLogging
{
    public void SomeMethod()
    {
        //your code
        Logger.Debug("Something interesting happened.");
    }
    readonly static ILog Logger = LogManager.GetLogger(typeof(ClassUsingLogging)); // Can also use a string
}
#endregion
