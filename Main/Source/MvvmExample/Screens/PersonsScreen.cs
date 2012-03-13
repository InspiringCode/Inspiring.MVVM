namespace Inspiring.MvvmExample.Screens {
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Common;

   public sealed class PersonsScreen : ScreenBase {
      public PersonsScreen(EventAggregator aggregator)
         : base(aggregator) {
      }
   }
}
