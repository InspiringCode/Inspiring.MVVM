namespace Inspiring.Mvvm.ViewModels {
   using System;

   /// <summary>
   ///   Marks a static field of type <see cref="VMDescriptor"/> that holds the
   ///   descriptor that should be used by the <see cref="System.ComponentModel.TypeDescriptionProvider"/>
   ///   if the view model is used in a designer for example.
   /// </summary>
   [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
   public sealed class ClassDescriptorAttribute : Attribute {
   }
}
