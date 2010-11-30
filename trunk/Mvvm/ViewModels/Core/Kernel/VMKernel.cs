namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels;

   public sealed class VMKernel : IBehaviorContext {
      private readonly IViewModel _vm;
      private readonly VMDescriptorBase _descriptor;
      private FieldValueHolder _fieldValues;

      public VMKernel(IViewModel vm, VMDescriptorBase descriptor, IServiceLocator serviceLocator) {
         Contract.Requires<ArgumentNullException>(vm != null);
         Contract.Requires<ArgumentNullException>(descriptor != null);
         Contract.Requires<ArgumentNullException>(serviceLocator != null);

         _vm = vm;
         _descriptor = descriptor;
         ServiceLocator = serviceLocator;
      }

      public IViewModel Parent { get; set; }

      public IServiceLocator ServiceLocator {
         get;
         private set;
      }

      IViewModel IBehaviorContext.VM {
         get { return _vm; }
      }

      FieldValueHolder IBehaviorContext.FieldValues {
         get {
            if (_fieldValues == null) {
               _fieldValues = _descriptor.Fields.CreateValueHolder();
            }

            return _fieldValues;
         }
      }

      void IBehaviorContext.NotifyChange(ChangeArgs args) {
         NotifyChange(args, InstancePath.Empty);
      }


      void IBehaviorContext.NotifyValidating(_ValidationArgs args) {
         HandleNotifyValidating(args);

         if (Parent != null) {
            Parent.Kernel.NotifyValidating(args);
         }
      }

      public ValidationState GetValidationState(IVMProperty forProperty) {
         return forProperty.GetValidationState(this);
      }

      public ValidationState GetValidationState() {
         return _descriptor.GetValidationState(this);
      }

      public IVMProperty GetProperty(string propertyName) {
         throw new NotImplementedException();
         //return _descriptor.Properties[propertyName];
      }

      public void UpdateFromSource() {
         throw new NotImplementedException();
         //IManuelUpdateBehavior behavior;
         //_descriptor
         //   .Properties
         //   .Where(x => x.Behaviors.TryGetBehavior(out behavior))
         //   .ForEach(UpdateFromSource);
      }

      public void UpdateFromSource(VMPropertyBase property) {
         throw new NotImplementedException();
         //property.Behaviors
         //   .GetNextBehavior<IManuelUpdateBehavior>()
         //   .UpdateFromSource(this);
      }

      // TODO: Test and refactor me.
      protected void UpdateSource() {
         throw new NotImplementedException();
         //RequireDescriptor();
         //IManuelUpdateBehavior behavior;
         //_descriptor
         //   .Properties
         //   .Where(x => x.Behaviors.TryGetBehavior(out behavior))
         //   .ForEach(UpdateFromSource);
      }

      protected void UpdateSource(VMPropertyBase property) {
         throw new NotImplementedException();
         //property.Behaviors
         //   .GetNextBehavior<IManuelUpdateBehavior>()
         //   .UpdateSource(this);
      }

      private void NotifyChange(ChangeArgs args, InstancePath changedPath) {
         bool selfChanged = changedPath.IsEmpty;
         changedPath = changedPath.PrependVM(_vm);

         if (selfChanged && args.ChangeType == ChangeType.PropertyChanged) {
            args
               .ChangedProperty
               .Behaviors
               .TryCall<IHandlePropertyChangedBehavior>(b => 
                  b.HandlePropertyChanged(this)
               );
         }

         ViewModelBehavior behavior;
         if (_descriptor.Behaviors.TryGetBehavior(out behavior)) {
            if (selfChanged) {
               behavior.OnSelfChanged(this, args);
            } else {
               behavior.OnChildChanged(this, args, changedPath);
            }

            behavior.OnChanged(this, args, changedPath);
         }

         if (Parent != null) {
            Parent.Kernel.NotifyChange(args, changedPath);
         }

         if (args.ChangeType == ChangeType.PropertyChanged) {
            _vm.RaisePropertyChanged(args.ChangedProperty.PropertyName);
         }
      }

      private void HandleNotifyValidating(_ValidationArgs args) {
         _descriptor.Behaviors.TryCall<ViewModelBehavior>(b =>
            b.OnValidating(this, args)
         );
      }

      private void NotifyValidating(_ValidationArgs args) {
         args = args.PrependTargetPath(with: _vm);
         HandleNotifyValidating(args);

         if (Parent != null) {
            Parent.Kernel.NotifyValidating(args);
         }
      }

   }
}
