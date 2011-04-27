﻿namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Diagnostics.Contracts;

   public abstract class BinderExpression {
      private BinderContext _context;

      public BinderExpression(BinderContext context) {

         _context = context;
      }

      public static void ExposeContext(object binderExpression, Action<BinderContext> contextAction) {
         Contract.Requires<ArgumentNullException>(binderExpression != null);
         Contract.Requires<ArgumentException>(
            binderExpression is BinderExpression,
            ExceptionTexts.ParameterMustBeABinderExpression
         );

         BinderExpression exp = (BinderExpression)binderExpression;
         contextAction(exp._context);
      }

      //public void ExposeContext(Action<BinderContext> contextAction) {

      //   contextAction(_context);
      //}
   }
}