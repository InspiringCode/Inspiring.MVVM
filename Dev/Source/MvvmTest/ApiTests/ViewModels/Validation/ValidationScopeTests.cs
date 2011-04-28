﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationScopeTests : TestBase {
      private const string LoadedChildError = "LoadedChildError";
      private const string LoadedGrandChildError = "LoadedGrandchildError";
      private const string UnloadedChildError = "UnloadedChildError";
      private const string UnloadedGrandChildError = "UnloadedGrandchildError";

      private RootVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new RootVM {
            LoadedChildSource = new ChildSource {
               ChildPropertyError = LoadedChildError,
               Items = new List<GrandchildSource> {
                  new GrandchildSource { GrandchildError = LoadedGrandChildError }
               }
            },
            UnloadedChildSource = new ChildSource {
               ChildPropertyError = UnloadedChildError,
               Items = new List<GrandchildSource> {
                  new GrandchildSource { GrandchildError = UnloadedGrandChildError }
               }
            }
         };

         VM.Load(x => x.LoadedChild);
         VM.GetValue(x => x.LoadedChild).Load(x => x.Items);

         Assert.IsTrue(VM.IsValid, "Test prerequisite failed");
      }

      [TestMethod]
      public void RevalidateSelf_DoesNotLoadDescendants() {
         VM.Revalidate(ValidationScope.Self);
         Assert.IsFalse(VM.IsLoaded(x => x.UnloadedChild));
      }

      [TestMethod]
      public void RevalidateAllDescendants_LoadsAndValidatesAllDescendants() {
         VM.Revalidate(ValidationScope.SelfAndAllDescendants);

         Assert.IsTrue(VM.IsLoaded(x => x.UnloadedChild));

         ValidationAssert.ErrorMessages(
            VM.ValidationResult,
            LoadedChildError,
            LoadedGrandChildError,
            UnloadedChildError,
            UnloadedGrandChildError
         );
      }

      [TestMethod]
      public void RevalidateLoadedDescendants_RevalidatesOnlyLoadedDescendants() {
         VM.Revalidate(ValidationScope.SelfAndLoadedDescendants);

         Assert.IsFalse(VM.IsLoaded(x => x.UnloadedChild));

         ValidationAssert.ErrorMessages(
            VM.ValidationResult,
            LoadedChildError,
            LoadedGrandChildError
         );
      }

      [TestMethod]
      public void GetValue_OfUnvalidatedAndUnloadedDescendant_DoesNotValidateDescendant() {
         VM.GetValue(x => x.UnloadedChild);
         ValidationAssert.IsValid(VM);
      }

      [TestMethod]
      public void GetValueOfUnloadedChild_WhenRevalidateLoadedDescendantsWasCalledBefore_ValidatesChild() {
         VM.Revalidate(ValidationScope.SelfAndLoadedDescendants);
         VM.GetValue(x => x.UnloadedChild);

         ValidationAssert.ErrorMessages(
            VM.ValidationResult,
            LoadedChildError,
            LoadedGrandChildError,
            UnloadedChildError
         );
      }

      [TestMethod]
      public void GetValueOfUnloadedChild_WhenRevalidateLoadedDescendantsWasCalledBefore_DoesNotLoadGrandchild() {
         VM.Revalidate(ValidationScope.SelfAndLoadedDescendants);
         VM.GetValue(x => x.UnloadedChild);

         bool childrenOfUnlaodedChildWereLoaded = VM
            .GetValue(x => x.UnloadedChild)
            .IsLoaded(x => x.Items);

         Assert.IsFalse(childrenOfUnlaodedChildWereLoaded);
      }

      [TestMethod]
      public void GetValueOfUnloadedGrandChild_WhenRevalidateLoadedDescendantsWasCalledBefore_ValidatesGrandchild() {
         VM.Revalidate(ValidationScope.SelfAndLoadedDescendants);

         VM.GetValue(x => x.UnloadedChild);
         VM.GetValue(x => x.UnloadedChild).GetValue(x => x.Items);

         ValidationAssert.ErrorMessages(
            VM.ValidationResult,
            LoadedChildError,
            LoadedGrandChildError,
            UnloadedChildError,
            UnloadedGrandChildError
         );
      }


      //
      //   S O U R C E   C L A S S E S
      //

      private class ChildSource {
         public ChildSource() {
            Items = new List<GrandchildSource>();
         }

         public string ChildProperty { get; set; }
         public string ChildPropertyError { get; set; }

         public List<GrandchildSource> Items { get; set; }
      }

      private class GrandchildSource {
         public string GrandchildError { get; set; }
      }


      //
      //   V I E W   M O D E L S
      //

      private sealed class RootVM : TestViewModel<RootVMDescriptor> {
         public static readonly RootVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<RootVMDescriptor>()
            .For<RootVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.LoadedChild = v.VM.Wraps(x => x.LoadedChildSource).With<ChildVM>();
               d.UnloadedChild = v.VM.Wraps(x => x.UnloadedChildSource).With<ChildVM>();

            })
            .Build();

         public RootVM()
            : base(ClassDescriptor) {
         }

         public ChildSource LoadedChildSource { get; set; }
         public ChildSource UnloadedChildSource { get; set; }
      }

      private sealed class RootVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> LoadedChild { get; set; }
         public IVMPropertyDescriptor<ChildVM> UnloadedChild { get; set; }
      }

      private sealed class ChildVM : TestViewModel<ChildVMDescriptor>, IHasSourceObject<ChildSource> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var s = b.GetPropertyBuilder(x => x.Source);

               d.ChildProperty = s.Property.MapsTo(x => x.ChildProperty);

               d.Items = s
                  .Collection
                  .Wraps(x => x.Items)
                  .With<GrandchildVM>(GrandchildVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.Check(x => x.ChildProperty).Custom(args => {
                  args.AddError(args.Owner.Source.ChildPropertyError);
               });
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public ChildSource Source { get; set; }
      }

      private sealed class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ChildProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<GrandchildVM>> Items { get; set; }
      }

      private sealed class GrandchildVM : TestViewModel<GrandchildVMDescriptor>, IHasSourceObject<GrandchildSource> {
         public static readonly GrandchildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<GrandchildVMDescriptor>()
            .For<GrandchildVM>()
            .WithProperties((d, b) => {
               var s = b.GetPropertyBuilder(x => x.Source);
            })
            .WithValidators(b => {
               b.CheckViewModel(args => {
                  args.AddError(args.Owner.Source.GrandchildError);
               });
            })
            .Build();

         public GrandchildVM()
            : base(ClassDescriptor) {
         }

         public GrandchildSource Source { get; set; }
      }

      private sealed class GrandchildVMDescriptor : VMDescriptor {
      }
   }
}