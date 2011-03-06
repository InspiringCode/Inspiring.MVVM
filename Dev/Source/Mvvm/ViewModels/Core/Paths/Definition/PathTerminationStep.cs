namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal sealed class PathTerminationStep : PathDefinitionStep {
      public static readonly PathTerminationStep Instance = new PathTerminationStep();


      public override PathMatch Matches(PathIterator path) {
         throw new NotImplementedException();
      }
   }
}
