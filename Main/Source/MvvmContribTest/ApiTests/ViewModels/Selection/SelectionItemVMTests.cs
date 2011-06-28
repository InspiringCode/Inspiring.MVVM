namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SelectionItemVMTests {
      [TestMethod]
      public void ToString_WhenCaptionSourceReturnsNull_ReturnsEmptyString() {
         var source = new SelectionSource { Caption = null };
         var vm = new ParentVM {
            AllItemsSource = new[] { source },
            SelectedItemSource = source
         };

         string caption = vm
            .GetValue(x => x.Selection)
            .SelectedItem
            .ToString();

         Assert.AreEqual(String.Empty, caption);
      }

      private class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Selection = v
                  .SingleSelection(x => x.SelectedItemSource)
                  .WithItems(x => x.AllItemsSource)
                  .WithCaption(x => x.Caption);
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {
            AllItemsSource = Enumerable.Empty<SelectionSource>();
         }

         public SelectionSource SelectedItemSource { get; set; }
         public IEnumerable<SelectionSource> AllItemsSource { get; set; }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<SingleSelectionVM<SelectionSource>> Selection { get; set; }
      }

      private class SelectionSource {
         public string Caption { get; set; }
      }
   }
}