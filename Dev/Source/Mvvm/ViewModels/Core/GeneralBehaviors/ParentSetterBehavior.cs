﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   internal sealed class ParentSetterBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>
      where TValue : IViewModel {

      private bool _setParentOnGetValue;
      private bool _setParentOnSetValue;

      public ParentSetterBehavior(
         bool setParentOnGetValue,
         bool setParentOnSetValue
      ) {
         Contract.Requires(setParentOnGetValue || setParentOnSetValue);

         _setParentOnGetValue = setParentOnGetValue;
         _setParentOnSetValue = setParentOnSetValue;
      }

      /// <inheritdoc />
      public TValue GetValue(IBehaviorContext context) {
         TValue childVM = this.GetValueNext<TValue>(context);

         if (_setParentOnGetValue && childVM != null) {
            childVM.Kernel.Parents.Add(context.VM);
         }

         return childVM;
      }

      /// <inheritdoc />
      public void SetValue(IBehaviorContext context, TValue value) {
         // Note 1: It is not a good idea to reset the 'Parent' of the previous
         // VM to null, because it may also be contained in a VM collection and
         // would loose its parent connection in this case.

         // Note 2: We do not set the parent if it is already set because the VM
         // may already be the child of a different VM.

         if (_setParentOnSetValue && value != null) {
            value.Kernel.Parents.Add(context.VM);
         }

         this.SetValueNext(context, value);
      }
   }
}