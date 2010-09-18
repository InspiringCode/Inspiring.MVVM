namespace Inspiring.MvvmContribTest.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SingleSelectionTest {
      [TestMethod]
      public void TestMethod1() {

      }

      private sealed class PersonVM : ViewModel<PersonVMDescriptor>, ICanInitializeFrom<Person> {
         public static readonly PersonVMDescriptor Descriptor = VMDescriptorBuilder
            .For<PersonVM>()
            .CreateDescriptor(c => {
               var vm = c.GetPropertyFactory();
               var p = c.GetPropertyFactory(x => x.Person);

               return new PersonVMDescriptor {
                  Status = vm.SingleSelection()
                     .WithItems(x => x.AllStatus, x => x.IsSelectable)
                     .WithSelection(x => x.Person.CurrentStatus)
                     .Of(i => new StatusSelectionItemVMDescriptor {
                        Name = i.Mapped(x => x.Name),
                        Description = i.Mapped(x => x.Description)
                     })
               };
            })
            .Build();

         public PersonVM()
            : base(Descriptor) {
         }

         public Person Person { get; private set; }

         private IEnumerable<PersonStatus> AllStatus { get; set; }

         public void InitializeFrom(Person source) {
            Person = source;
         }
      }

      private sealed class PersonVMDescriptor : VMDescriptor {
         public VMProperty<SingleSelectionVM<PersonStatus, SelectionItemVM<PersonStatus>>> Status { get; set; }
      }

      private class Person {
         public PersonStatus CurrentStatus { get; set; }
      }

      private class PersonStatus {
         public string Name { get; set; }
         public string Description { get; set; }
         public bool IsSelectable { get; set; }
      }

      private class StatusSelectionItemVMDescriptor : VMDescriptor {
         public VMProperty<string> Name { get; set; }
         public VMProperty<string> Description { get; set; }
      }
   }
}