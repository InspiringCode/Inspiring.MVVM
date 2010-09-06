using System;
namespace Inspiring.Mvvm.Views.Binder {

   public abstract class BinderExpression {
      private BinderContext _context;

      public BinderExpression(BinderContext context) {

         _context = context;
      }

      public static void ExposeContext(object binderExpression, Action<BinderContext> contextAction) {

      }

      //public void ExposeContext(Action<BinderContext> contextAction) {

      //   contextAction(_context);
      //}
   }
}
