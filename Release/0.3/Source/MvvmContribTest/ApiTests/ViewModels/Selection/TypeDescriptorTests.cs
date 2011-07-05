namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class TypeDescriptorTests {
      [TestMethod]
      public void BrowsableViewModelList_ReturnsCorrectProperties() {
         var list = new BrowsableViewModelList<ParentVM>(new ParentVM());
         var allProperties = list.GetItemProperties(null);
         var singleSelectionProperty = allProperties
            .Cast<PropertyDescriptor>()
            .Single(x => x.Name == "Selection");

         var selectionPropertyNames = list
            .GetItemProperties(new[] { singleSelectionProperty })
            .Cast<PropertyDescriptor>()
            .Select(x => x.Name)
            .ToArray();

         CollectionAssert.Contains(selectionPropertyNames, "SelectedItem");
      }

      [TestMethod]
      public void TypeDescriptorGetProperties_ReturnsCorrectProperties() {
         var list = new BrowsableViewModelList<ParentVM>(new ParentVM());
         var selectionPropertyNames = TypeDescriptor
            .GetProperties(typeof(SingleSelectionVM<SelectionSource>))
            .Cast<PropertyDescriptor>()
            .Select(x => x.Name)
            .ToArray();
         
         CollectionAssert.Contains(selectionPropertyNames, "SelectedItem");
      }

      private class ParentVM : ViewModel<ParentVMDescriptor> {
         [ClassDescriptor]
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