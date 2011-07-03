using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Inspiring.Mvvm.Screens;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Views {
   [TestClass]
   public class ViewBinderTest {
      // [TestMethod] // TODO
      public void BindScreen() {
         PersonScreenView screenView = new PersonScreenView();
         TextBox targetElement = new TextBox();

         ViewBinder.BindScreen<PersonScreen>(screenView, c => {
            c.BindVM(x => x.VM, b => {
               b.Property(x => x.FirstName).To(targetElement).On(TextBox.TextProperty);
            });
         });

         Binding binding = BindingOperations.GetBinding(targetElement, TextBox.TextProperty);
         Assert.AreEqual("VM.FirstName", binding.Path.Path);
         Assert.IsNull(binding.Source);
      }

      // [TestMethod] // TODO
      public void BindCollection() {
         Inspiring.MvvmTest.ViewModels.PersonVM vm = null;
         PersonVMView view = new PersonVMView();
         DataGrid grid = new DataGrid();

         var nameColumn = new DataGridTextColumn();
         var memberCountColumn = new DataGridTextColumn();

         grid.Columns.Add(nameColumn);
         grid.Columns.Add(memberCountColumn);

         ViewBinder.BindVM(view, b => {
            b.Collection(x => x.Projects).To(grid, i => {
               i.Property(x => x.Name).To(nameColumn);
               i.Property(x => x.MemberCount).To(memberCountColumn);
            });
         });

         Binding itemsSourceBinding = BindingOperations.GetBinding(grid, DataGrid.ItemsSourceProperty);
         Assert.IsNotNull(itemsSourceBinding);
         Assert.IsNull(itemsSourceBinding.Source);
         Assert.AreEqual("Projects", itemsSourceBinding.Path.Path);

         Binding nameBinding = (Binding)nameColumn.Binding;
         Binding memberCountBinding = (Binding)memberCountColumn.Binding;

         Assert.AreEqual("Name", nameBinding.Path.Path);
         Assert.AreEqual("MemberCount", memberCountBinding.Path.Path);
         Assert.IsNull(nameBinding.Source);
         Assert.IsNull(memberCountBinding.Source);
      }

      private class PersonScreenView : IView<PersonScreen> {

         public PersonScreen Model {
            set { throw new System.NotImplementedException(); }
         }
      }

      private class PersonVMView : IView<Inspiring.MvvmTest.ViewModels.PersonVM> {

         public Inspiring.MvvmTest.ViewModels.PersonVM Model {
            set { throw new System.NotImplementedException(); }
         }
      }

      private class PersonScreen : ScreenBase {
         public Inspiring.MvvmTest.ViewModels.PersonVM VM { get; private set; }
      }


      [TestMethod]
      public void BindVM_DescriptorInterface_Succeeds() {
         var view = new PersonInterfaceView();

         ViewBinder.BindVM(view, b => {

         });
      }

      private interface IPersonDescriptor : IVMDescriptor {
         IVMPropertyDescriptor<string> Name { get; }
      }

      private class PersonVMDescriptor : VMDescriptor, IPersonDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }

      private class PersonVM : ViewModel<PersonVMDescriptor> {

      }

      private class PersonInterfaceView : DependencyObject, IView<IViewModelExpression<IPersonDescriptor>> {


         public IViewModelExpression<IPersonDescriptor> Model {
            set { throw new System.NotImplementedException(); }
         }
      }
   }
}
