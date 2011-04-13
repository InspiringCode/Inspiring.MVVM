//namespace Inspiring.Mvvm.ViewModels.Core.BehaviorFactoryProviders.Bla {
//   using System;
//   using Inspiring.Mvvm.Common;


//   public abstract class PropertyBehaviorFactoryProviderBase : IBehaviorFactoryProvider {
//      public BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject, TValueSource>()
//         where TOwnerVM : IViewModel {

//         return new BehaviorFactory();
//      }
//   }

//   public abstract class ChildBehaviorFactoryProvider : IBehaviorFactoryProvider {
//      public abstract BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>()
//         where TOwnerVM : IViewModel
//         where TChildVM : IViewModel;

//      // Helper functions
//   }

//   public abstract class ChildWithSourceBehaviorFactoryProvider : IBehaviorFactoryProvider {
//      public abstract BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>()
//         where TOwnerVM : IViewModel
//         where TChildVM : IViewModel, IHasSourceObject<TChildSource>;

//      // Helper functions
//   }

//   public class PropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase {
//      public static readonly PropertyBehaviorFactoryProvider Default
//         = new PropertyBehaviorFactoryProvider();

//      public BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject, TValueSource>()
//         where TOwnerVM : IViewModel {

//         return new BehaviorFactory();
//         // Add common property behaviors
//         // Add property behaviors
//      }
//   }

//   public class ViewModelPropertyBehaviorFactoryProvider : ChildBehaviorFactoryProvider {
//      public override BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>() {
//         // Add common property behaviors
//         // Add view model behaviors
//         throw new NotImplementedException();
//      }
//   }

//   public class ViewModelPropertyWithSourceBehaviorFactoryProvider : ChildWithSourceBehaviorFactoryProvider {
//      public override BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>() {
//         throw new NotImplementedException();
//      }
//   }




//   public class CollectionPropertyBehaviorFactoryProvider : ChildBehaviorFactoryProvider {
//      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>() {
//         // Add common property behaviors
//         // Add view model behaviors
//         throw new NotImplementedException();
//      }

//      public override BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>() {
//         throw new NotImplementedException();
//      }
//   }

//   public class CommandPropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase { // TODO???

//      public BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject, TValueSource>() {
//         throw new NotImplementedException();
//      }
//   }


//   public class CollectionBehaviorFactoryProvider : ChildBehaviorFactoryProvider {
//      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>() {
//         throw new NotImplementedException();
//      }

//      public override BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>() {
//         throw new NotImplementedException();
//      }
//   }

//   public class CommandBehaviorFactoryProvider : ChildBehaviorFactoryProvider {
//      public BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject, TChildSource>() {
//         throw new NotImplementedException();
//      }

//      public override BehaviorFactory GetFactory<TOwnerVM, TChildVM, TSourceObject>() {
//         throw new NotImplementedException();
//      }
//   }


//   public class ViewModelBehaviorFactoryProvider : IBehaviorFactoryProvider {
//      public static readonly ViewModelBehaviorFactoryProvider Default
//         = new ViewModelBehaviorFactoryProvider();

//      public virtual BehaviorFactory GetFactory<TVM>() where TVM : IViewModel {
//         return new BehaviorFactory();
//         // Add view model behaviors
//      }
//   }




//   //public class PropertyBehaviorFactoryProviderBase : IBehaviorFactoryProvider {
//   //   public BehaviorFactory GetFactory<TOwnerVM, TValue>()
//   //      where TOwnerVM : IViewModel {

//   //      return new BehaviorFactory();
//   //   }
//   //}

//   //public class PropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase {
//   //   public static readonly PropertyBehaviorFactoryProvider Default
//   //      = new PropertyBehaviorFactoryProvider();

//   //   public virtual new BehaviorFactory GetFactory<TOwnerVM, TValue>()
//   //      where TOwnerVM : IViewModel {

//   //      return base
//   //         .GetFactory<TOwnerVM, TValue>();
//   //   }
//   //}

//   //public class ViewModelPropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase {
//   //   public virtual new BehaviorFactory GetFactory<TOwnerVM, TValue>()
//   //      where TOwnerVM : IViewModel
//   //      where TValue : IViewModel {

//   //      return base
//   //         .GetFactory<TOwnerVM, TValue>();
//   //   }
//   //}

//   //public class CollectionPropertyBehaviorFactoryProvider : PropertyBehaviorFactoryProviderBase {
//   //   public virtual new BehaviorFactory GetFactory<TOwnerVM, TItemVM>()
//   //      where TOwnerVM : IViewModel
//   //      where TItemVM : IViewModel {

//   //      return base
//   //         .GetFactory<TOwnerVM, IVMCollection<TItemVM>>();
//   //   }
//   //}

//   //public class ViewModelBehaviorFactoryProvider : IBehaviorFactoryProvider {
//   //   public static readonly ViewModelBehaviorFactoryProvider Default
//   //      = new ViewModelBehaviorFactoryProvider();

//   //   public virtual BehaviorFactory GetFactory<TVM>() where TVM : IViewModel {
//   //      return new BehaviorFactory();
//   //   }
//   //}

//   //public class CommandBehaviorFactoryProvider : IBehaviorFactoryProvider {
//   //   public virtual BehaviorFactory GetFactory<TOwnerVM, TSourceObject>() where TOwnerVM : IViewModel {
//   //      return new BehaviorFactory();
//   //   }
//   //}
//}
