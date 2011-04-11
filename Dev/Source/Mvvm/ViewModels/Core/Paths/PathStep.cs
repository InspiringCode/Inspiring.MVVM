using System;
using Inspiring.Mvvm.Common;
namespace Inspiring.Mvvm.ViewModels.Core {

   public enum PathStepType {
      None,
      ViewModel,
      Collection,
      Property
   }

   public struct PathStep {
      private readonly PathStepType _type;
      private readonly IViewModel _viewModel;
      private readonly IVMCollection _collection;
      private readonly IVMPropertyDescriptor _property;

      public PathStep(IViewModel viewModel)
         : this() {
         _type = PathStepType.ViewModel;
         _viewModel = viewModel;
      }

      public PathStep(IVMCollection collection)
         : this() {
         _type = PathStepType.Collection;
         _collection = collection;
      }

      public PathStep(IVMPropertyDescriptor property)
         : this() {
         _type = PathStepType.Property;
         _property = property;
      }

      public PathStepType Type {
         get { return _type; }
      }

      public IViewModel ViewModel {
         get { return _viewModel; }
      }

      public IVMCollection Collection {
         get { return _collection; }
      }

      public IVMPropertyDescriptor Property {
         get { return _property; }
      }

      public override string ToString() {
         switch (_type) {
            case PathStepType.ViewModel:
               return TypeService.GetFriendlyName(_viewModel.GetType());
            case PathStepType.Collection:
               return TypeService.GetFriendlyName(_collection.GetType());
            case PathStepType.Property:
               return _property.PropertyName;
            default:
               throw new NotSupportedException();
         }
      }
   }
}
