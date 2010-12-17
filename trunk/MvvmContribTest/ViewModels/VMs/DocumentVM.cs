﻿namespace Inspiring.MvvmContribTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class DocumentVM : ViewModel<DocumentVMDescriptor>, ICanInitializeFrom<Document> {
      public static readonly DocumentVMDescriptor Descriptor = VMDescriptorBuilder
         .For<DocumentVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyBuilder();
            var d = c.GetPropertyBuilder(x => x.Document);

            return new DocumentVMDescriptor {
               Name = d.Mapped(x => x.Name).Property(),
               Keywords = d.Mapped(x => x).VM<KeywordSelectionVM>()
            };
         })
         .Build();

      public DocumentVM()
         : base() {
      }

      public Document Document { get; private set; }

      public void InitializeFrom(Document source) {
         Document = source;
      }

      public KeywordSelectionVM Keywords {
         get { return GetValue(Descriptor.Keywords); }
      }
   }

   internal sealed class DocumentVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
      public VMProperty<KeywordSelectionVM> Keywords { get; set; }
   }
}
