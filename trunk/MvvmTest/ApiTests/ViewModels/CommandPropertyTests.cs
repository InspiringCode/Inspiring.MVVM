namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CommandPropertyTests {
      private SourceObject Source { get; set; }
      private TaskListVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         Source = new SourceObject();
         VM = new TaskListVM(Source);
      }

      [TestMethod]
      public void GetValue_RootPropertyFactory_ReturnsCommandInstance() {
         Assert.IsNotNull(VM.RootCommand);
      }

      [TestMethod]
      public void GetValue_RelativePropertyFactory_ReturnsCommandInstance() {
         Assert.IsNotNull(VM.RelativeCommand);
      }

      [TestMethod]
      public void Execute_RootPropertyFactory_CallsSource() {
         VM.RootCommand.Execute(null);
         Assert.AreEqual(1, Source.ExecuteInvokationCount);
      }

      [TestMethod]
      public void Execute_RelativePropertyFactory_CallsSource() {
         VM.RelativeCommand.Execute(null);
         Assert.AreEqual(1, Source.ExecuteInvokationCount);
      }

      [TestMethod]
      public void CanExecute_SourceReturnsTrue_CommandReturnsTrue() {
         Source.CanExecuteResult = true;
         bool result = VM.RelativeCommand.CanExecute(null);

         Assert.AreEqual(1, Source.CanExecuteInvocationCount);
         Assert.IsTrue(result);
      }

      [TestMethod]
      public void CanExecute_SourceReturnsFalse_CommandReturnsFalse() {
         Source.CanExecuteResult = false;
         bool result = VM.RelativeCommand.CanExecute(null);

         Assert.AreEqual(1, Source.CanExecuteInvocationCount);
         Assert.IsFalse(result);
      }

      public class SourceObject {
         public int ExecuteInvokationCount { get; set; }
         public int CanExecuteInvocationCount { get; set; }
         public bool CanExecuteResult { get; set; }

         public void ExecuteAction() {
            ExecuteInvokationCount++;
         }

         public bool CanExecuteAction() {
            CanExecuteInvocationCount++;
            return CanExecuteResult;
         }
      }

      public sealed class TaskListVM : ViewModel<TaskListVMDescriptor> {
         public static readonly TaskListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TaskListVMDescriptor>()
            .For<TaskListVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var s = c.GetPropertyBuilder(x => x.Source);

               d.RootCommand = v.Command(x => x.Source.ExecuteAction());
               d.RelativeCommand = s.Command(
                  x => x.ExecuteAction(),
                  x => x.CanExecuteAction()
               );
            })
            .Build();

         public TaskListVM(SourceObject sourceObject)
            : base(ClassDescriptor) {
            Source = sourceObject;
         }

         public SourceObject Source { get; private set; }

         public ICommand RootCommand {
            get { return GetValue(Descriptor.RootCommand); }
         }

         public ICommand RelativeCommand {
            get { return GetValue(Descriptor.RelativeCommand); }
         }
      }

      public sealed class TaskListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ICommand> RootCommand { get; set; }
         public IVMPropertyDescriptor<ICommand> RelativeCommand { get; set; }
      }
   }
}