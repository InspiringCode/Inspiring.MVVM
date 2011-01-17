//namespace Inspiring.Mvvm.ViewModels.Core.GeneralBehaviors.PropertyBehaviors {
//   using System;
//   using System.Collections.Generic;
//   using System.Linq;
//   using Inspiring.Mvvm.Screens;

//   internal sealed class ChildScreenBehavior<TValue> :
//      Behavior,
//      IValueAccessorBehavior<TValue>
//      where TValue : IScreen {

//      public TValue GetValue(IBehaviorContext context) {
//         return this.GetValueNext<TValue>(context);
//      }

//      public void SetValue(IBehaviorContext context, TValue value) {
//         var parentScreen = (IScreen)context.VM;
//         var previousChild = this.GetValueNext<TValue>(context);

//         if (previousChild != null) {
//            parentScreen.Children.Remove(previousChild);
//         }

//         if (value != null) {
//            parentScreen.Children.Add(value);
//         }


//         throw new NotImplementedException();
//      }
//   }
//}
