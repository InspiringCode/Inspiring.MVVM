namespace Inspiring.Mvvm.ViewModels.Core.Behaviors {
   public class DefaultProviders {
      public static readonly IBehaviorFactoryProvider SimpleProperty = new SimplePropertyProvider();
      public static readonly IBehaviorFactoryProvider ViewModelProperty = new ViewModelPropertyProvider();
      public static readonly IBehaviorFactoryProvider CollectionProperty = new CollectionPropertyFactoryProvider();
      public static readonly IBehaviorFactoryProvider ViewModel = new ViewModelProvider();
   }
}
