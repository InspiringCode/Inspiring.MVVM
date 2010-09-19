﻿namespace Inspiring.Mvvm.ViewModels {
   public class SelectionItemVM<TSourceItem> :
      ViewModel<VMDescriptor>,
      ICanInitializeFrom<TSourceItem> {

      public TSourceItem SourceItem { get; private set; }

      public void InitializeFrom(TSourceItem source) {
         SourceItem = source;
      }
   }

   public class SelectionItemVMDescriptor : VMDescriptor {
      public VMProperty<string> Caption { get; set; }
   }
}
