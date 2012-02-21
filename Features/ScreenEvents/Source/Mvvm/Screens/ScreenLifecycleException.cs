namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Runtime.Serialization;

   [Serializable]
   public sealed class ScreenLifecycleException : Exception {
      public ScreenLifecycleException() { }
      public ScreenLifecycleException(string message) : base(message) { }
      public ScreenLifecycleException(string message, Exception inner) : base(message, inner) { }
      protected ScreenLifecycleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
