namespace Inspiring.Mvvm.Screens {
   using System;

   [AttributeUsage(AttributeTargets.Class)]
   public sealed class ScreenCreationBehaviorAttribute : Attribute {
      public ScreenCreationBehaviorAttribute(ScreenCreationBehavior creationBehavior) {
         CreationBehavior = creationBehavior;
      }

      public ScreenCreationBehavior CreationBehavior { get; private set; }
   }
}
