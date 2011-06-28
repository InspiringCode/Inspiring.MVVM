namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public abstract class PathFixture {
      //
      // ASSERT HELPERS
      // 

      internal void AssertMatchIfNextSucceeds(PathDefinitionStep step, Path path, int expectedMatchLength = 1) {
         var a = new PathAssert(step, path);

         a.AssertMatchWith(PathAssert.SucceedingNextStep);

         a.AssertNoMatchWith(
            PathAssert.FailingNextStep,
            "The step returned a successful match even though the next step returned a failure."
         );

         a.AssertMatchLengthWith(PathAssert.SucceedingNextStep, expectedMatchLength);
         a.AssertMatchLengthWith(PathAssert.FailingNextStep, expectedMatchLength);

         a.AssertIteratorWasIncremented(expectedMatchLength);
      }

      internal void AssertNoMatch(PathDefinitionStep step, Path path) {
         var a = new PathAssert(step, path);

         a.AssertNoMatchWith(PathAssert.SucceedingNextStep);
         a.AssertMatchLengthWith(PathAssert.SucceedingNextStep, expectedLength: 0);
         a.AssertNextStepIsNotCalled();
      }

      internal void AssertException(PathDefinitionStep step, Path path) {
         AssertHelper.Throws<ArgumentException>(() =>
            step.Matches(GetDefinitionIterator(), path.GetIterator())
         );
      }

      internal class PathAssert {
         public static readonly PathDefinitionStep SucceedingNextStep = CreateStepStub(true);
         public static readonly PathDefinitionStep FailingNextStep = CreateStepStub(false);

         private PathDefinitionStep _step;
         private Path _path;

         public PathAssert(PathDefinitionStep step, Path path) {
            _step = step;
            _path = path;
         }

         public void AssertMatchWith(
            PathDefinitionStep nextStep,
            string message = "The step was expected to return a successful match."
         ) {
            var result = _step.Matches(GetDefinitionIterator(nextStep), _path.GetIterator());
            Assert.IsTrue(result.Success, message);
         }

         public void AssertNoMatchWith(
            PathDefinitionStep nextStep,
            string message = "The step was expected to return a failing match."
         ) {
            var result = _step.Matches(GetDefinitionIterator(nextStep), _path.GetIterator());
            Assert.IsFalse(result.Success, message);
         }

         public void AssertMatchLengthWith(
            PathDefinitionStep nextStep,
            int expectedLength,
            string message = "Expected a match length of {0} but was {1}."
         ) {
            var result = _step.Matches(GetDefinitionIterator(nextStep), _path.GetIterator());
            Assert.AreEqual(expectedLength, result.Length, message, expectedLength, result.Length);
         }

         public void AssertNextStepIsNotCalled(
            string message = "The step was not expected to call its next step."
         ) {
            var mock = new Mock<PathDefinitionStep>();

            mock
               .Setup(x => x.Matches(It.IsAny<PathDefinitionIterator>(), It.IsAny<PathIterator>()))
               .Callback(() => Assert.Fail(message));

            _step.Matches(GetDefinitionIterator(mock.Object), _path.GetIterator());
         }

         public void AssertIteratorWasIncremented(
            int expectedIncrement,
            string message = "The step was expected to increment the iterator passed to the next step {0} times but incremented it {1} times."
         ) {
            var mock = new Mock<PathDefinitionStep>();

            mock
               .Setup(x => x.Matches(It.IsAny<PathDefinitionIterator>(), It.IsAny<PathIterator>()))
               .Callback<PathDefinitionIterator, PathIterator>((s, it) => {
                  int actual = it.GetIndex();
                  Assert.AreEqual(expectedIncrement, actual, message, expectedIncrement, actual);
               });

            _step.Matches(GetDefinitionIterator(mock.Object), _path.GetIterator());
         }

         private static PathDefinitionStep CreateStepStub(bool success, int matchLength = 0) {
            var mock = new Mock<PathDefinitionStep>();

            var result = success ?
               PathMatch.Succeed(matchLength) :
               PathMatch.Fail(matchLength);

            mock
              .Setup(x => x.Matches(It.IsAny<PathDefinitionIterator>(), It.IsAny<PathIterator>()))
              .Returns(result);

            return mock.Object;
         }
      }

      private static PathDefinitionIterator GetDefinitionIterator(
         params PathDefinitionStep[] nextDefinitionSteps
      ) {
         return new PathDefinitionIterator(nextDefinitionSteps);
      }

      //
      // TEST VIEW MODELS
      // 

      protected sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
               d.Managers = v.Collection.Of<EmployeeVM>(d);
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }
      }

      protected sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
         public IVMPropertyDescriptor<IVMCollection<EmployeeVM>> Managers { get; set; }
      }

      protected sealed class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Name = v.Property.Of<string>();
               d.EndDate = v.Property.Of<DateTime>();
               d.SelectedCustomer = v.VM.Of<CustomerVM>();
               d.Customers = v.Collection.Of<CustomerVM>(CustomerVM.ClassDescriptor);
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }
      }

      protected sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<DateTime> EndDate { get; set; }
         public IVMPropertyDescriptor<CustomerVM> SelectedCustomer { get; set; }
         public IVMPropertyDescriptor<IVMCollection<CustomerVM>> Customers { get; set; }
      }

      protected sealed class CustomerVM : ViewModel<CustomerVMDescriptor> {
         public static readonly CustomerVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<CustomerVMDescriptor>()
            .For<CustomerVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Name = v.Property.Of<string>();
            })
            .Build();

         public CustomerVM()
            : base(ClassDescriptor) {
         }
      }

      protected sealed class CustomerVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }
   }
}