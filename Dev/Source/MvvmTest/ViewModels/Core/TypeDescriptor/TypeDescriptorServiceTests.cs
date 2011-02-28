namespace Inspiring.MvvmTest.ViewModels.Core.TypeDescriptor {
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class TypeDescriptorServiceTests : TestBase {
      [TestMethod]
      public void PropertyDescriptors_ReturnsVMDescriptorProperties() {
         var behavior = EmployeeVM.ClassDescriptor.Behaviors.GetNextBehavior<TypeDescriptorBehavior>();

         PropertyDescriptor returnedDescriptor = behavior
            .PropertyDescriptors
            .Cast<PropertyDescriptor>()
            .Single();

         // Assert
         Assert.AreEqual("Name", returnedDescriptor.Name);
      }

      [TestMethod]
      public void PropertyDescriptors_GetTwice_ReturnsSameCollection() {
         var behavior = EmployeeVM.ClassDescriptor.Behaviors.GetNextBehavior<TypeDescriptorBehavior>();

         // Act
         var first = behavior.PropertyDescriptors;
         var second = behavior.PropertyDescriptors;

         Assert.AreSame(first, second);
      }

      public sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();
               d.Name = vm.Property.Of<string>();
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }
   }
}
