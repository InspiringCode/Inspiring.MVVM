namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDependencies {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System.Linq;
using System.Collections.Generic;

   [TestClass]
   public class IgnoreRefreshDependenciesTests {
      [TestMethod]
      public void Refresh_IgnoresCollectionDependencies() {
         var root = CreateRootVM(b => {
            b.OnChangeOf
               .Descendant(x => x.Child1)
               .Collection(x => x.Grandchildren)
               .Refresh
               .AndExecuteRefreshDependencies
               .Descendant(x => x.Child2)
               .Properties(x => x.Grandchildren);

            b.OnChangeOf
               .Descendant(x => x.Child2)
               .Descendant(x => x.Grandchildren)
               .Properties(x => x.StringProperty)
               .Refresh
               .Properties(x => x.Property1);
         });

         var trigger = new ChildVM();
         var monitor = new ChildVM();
         root.Child2.Grandchildren.Add(monitor);

         root.Child1.Grandchildren.Add(trigger);

         Assert.AreEqual(0, root.Property1RefreshCount);
      }

      [TestMethod]
      public void Refresh_IgnoresViewModelDependencies() {
         var root = CreateRootVM(b => {
            b.OnChangeOf
               .Descendant(x => x.Child1)
               .Descendant(x => x.Grandchild)
               .Refresh
               .Descendant(x => x.Child2)
               .Properties(x => x.Grandchild);
         });

         var trigger = new ChildVM();
         var monitor = new ChildVM();
         root.Child1.Grandchild = trigger;
         root.Child2.Grandchild = monitor;

         trigger.Refresh(x => x.StringProperty, executeRefreshDependencies: false);
         Assert.AreEqual(0, monitor.StringPropertyRefreshCount);

         root.Child1.Refresh(x => x.Grandchild);
         Assert.AreEqual(0, monitor.StringPropertyRefreshCount);
      }

      [TestMethod]
      public void Refresh_IgnoresPropertyDependencies() {
         var root = CreateRootVM(b => {
            b.OnChangeOf
               .Properties(x => x.Property1)
               .Refresh
               .Properties(x => x.Property2);
         });

         root.Refresh(x => x.Property1, executeRefreshDependencies: false);
         Assert.AreEqual(0, root.Property2RefreshCount);
      }

      [TestMethod]
      public void Refresh_IgnoresDependencyLoops() {
         var root = CreateRootVM(b => {
            b.OnChangeOf
               .Properties(x => x.Property1)
               .Refresh
               .Properties(x => x.Property2);

            b.OnChangeOf
               .Properties(x => x.Property2)
               .Refresh
               .Properties(x => x.Property1);
         });

         root.Refresh(x => x.Property1);
         Assert.AreEqual(0, root.Property2RefreshCount);

         root.Refresh(x => x.Property2);
         Assert.AreEqual(1, root.Property1RefreshCount);
      }

      [TestMethod]
      [Ignore] // TODO
      public void Refresh_WorksWithSelfRecursiveDependency() {
         var root = CreateRootVM(b => {
            b.OnChangeOf
               .Self.OrAnyDescendant
               .Refresh
               .Properties(x => x.Child1, x => x.Child2);
         });

         var gc1 = new ChildVM();
         var gc2 = new ChildVM();
         root.Child1.Grandchildren.Add(gc1);
         root.Child2.Grandchildren.Add(gc2);

         root.Child1.Refresh(x => x.Grandchildren, executeRefreshDependencies: false);

         Assert.AreEqual(1, gc1.StringPropertyRefreshCount);
         Assert.AreEqual(1, gc2.StringPropertyRefreshCount);
      }

      public static RootVM CreateRootVM(
         Action<IVMDependencyBuilder<RootVM, RootVMDescriptor>> dependencyConfigurator
      ) {
         RootVMDescriptor descriptor = VMDescriptorBuilder
            .OfType<RootVMDescriptor>()
            .For<RootVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Property1 = v.Property.Of<string>();
               d.Property2 = v.Property.Of<string>();

               d.Child1 = v.VM.Of<ChildVM>();
               d.Child2 = v.VM.Of<ChildVM>();
            })
            .WithDependencies(dependencyConfigurator)
            .WithBehaviors(b => {
               b.Property(x => x.Property1).AddBehavior(new RefreshSpyBehavior());
               b.Property(x => x.Property2).AddBehavior(new RefreshSpyBehavior());
            })
            .Build();

         var vm = new RootVM(descriptor);
         vm.SetValue(x => x.Child1, new ChildVM());
         vm.SetValue(x => x.Child2, new ChildVM());
         return vm;
      }

      public sealed class RootVM : TestViewModel<RootVMDescriptor> {
         public RootVM(RootVMDescriptor descriptor)
            : base(descriptor) {
         }

         public ChildVM Child1 {
            get { return GetValue(Descriptor.Child1); }
         }

         public ChildVM Child2 {
            get { return GetValue(Descriptor.Child2); }
         }

         public int Property1RefreshCount {
            get {
               return Descriptor
                  .Property1
                  .Behaviors
                  .GetNextBehavior<RefreshSpyBehavior>()
                  .RefreshCount;
            }
         }

         public int Property2RefreshCount {
            get {
               return Descriptor
                  .Property2
                  .Behaviors
                  .GetNextBehavior<RefreshSpyBehavior>()
                  .RefreshCount;
            }
         }
      }

      public sealed class RootVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Property1 { get; set; }
         public IVMPropertyDescriptor<string> Property2 { get; set; }
         public IVMPropertyDescriptor<ChildVM> Child1 { get; set; }
         public IVMPropertyDescriptor<ChildVM> Child2 { get; set; }
      }

      public sealed class ChildVM : TestViewModel<ChildVMDescriptor>, IHasSourceObject<object> {
         public ChildVM()
            : base(CreateDescriptor()) {

            Source = new Object();
            GrandchildrenSource = new List<object>();
         }

         public int StringPropertyRefreshCount {
            get {
               return Descriptor
                  .StringProperty
                  .Behaviors
                  .GetNextBehavior<RefreshSpyBehavior>()
                  .RefreshCount;
            }
            set {
               Descriptor
                  .StringProperty
                  .Behaviors
                  .GetNextBehavior<RefreshSpyBehavior>()
                  .RefreshCount = value;
            }
         }

         public static ChildVMDescriptor CreateDescriptor() {
            return VMDescriptorBuilder
               .OfType<ChildVMDescriptor>()
               .For<ChildVM>()
               .WithProperties((d, b) => {
                  var v = b.GetPropertyBuilder();

                  d.StringProperty = v.Property.Of<string>();
                  d.Grandchild = v.VM.Of<ChildVM>();
                  d.Grandchildren = v.Collection.Wraps(x => x.GrandchildrenSource).With<ChildVM>(d);
               })
               .WithBehaviors(b => {
                  b.Property(x => x.StringProperty).AddBehavior(new RefreshSpyBehavior());
               })
               .Build();
         }

         public ChildVM Grandchild {
            get { return GetValue(Descriptor.Grandchild); }
            set { SetValue(Descriptor.Grandchild, value); }
         }

         public IVMCollection<ChildVM> Grandchildren {
            get { return GetValue(Descriptor.Grandchildren); }
         }

         public object Source { get; set; }

         private List<Object> GrandchildrenSource { get; set; }
      }

      public sealed class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> Grandchild { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> Grandchildren { get; set; }
         public IVMPropertyDescriptor<string> StringProperty { get; set; }
      }

      private class RefreshEntry {
         public IViewModel VM { get; set; }
         public IVMPropertyDescriptor Property { get; set; }
      }

      private class RefreshSpyBehavior :
         Behavior,
         IRefreshBehavior {

         public int RefreshCount { get; set; }

         public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
            RefreshCount++;
            this.RefreshNext(context, executeRefreshDependencies);
         }
      }
   }
}