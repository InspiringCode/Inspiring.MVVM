namespace Inspiring.Mvvm.Views.Binder {
   using System;
   
   public class BinderExpression {
      private BinderContext _context;

      public BinderExpression(BinderContext context) {

         _context = context;
      }

      public static void ExposeContext(object binderExpression, Action<BinderContext> contextAction) {
         Check.NotNull(binderExpression, nameof(binderExpression));
         Check.Requires(binderExpression is BinderExpression);

         BinderExpression exp = (BinderExpression)binderExpression;
         contextAction(exp._context);
      }

      //public void ExposeContext(Action<BinderContext> contextAction) {

      //   contextAction(_context);
      //}
   }
}
