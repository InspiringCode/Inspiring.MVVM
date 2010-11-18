﻿namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   [Obsolete]
   public sealed class _ViewModelValidationArgs {
      internal _ViewModelValidationArgs(
         ViewModel validationTarget,
         ViewModel changedVM,
         VMPropertyBase changedProperty
      ) {
         Contract.Requires(validationTarget != null);
         Contract.Requires(changedVM != null);

         ValidationTarget = validationTarget;
         ChangedVM = changedVM;
         ChangedProperty = changedProperty;

         Errors = new List<string>();
      }

      public ViewModel ValidationTarget { get; private set; }
      public ViewModel ChangedVM { get; private set; }
      public VMPropertyBase ChangedProperty { get; private set; }

      internal List<string> Errors { get; private set; }

      public void AddError(string errorMessage) {
         Errors.Add(errorMessage);
      }
   }
}