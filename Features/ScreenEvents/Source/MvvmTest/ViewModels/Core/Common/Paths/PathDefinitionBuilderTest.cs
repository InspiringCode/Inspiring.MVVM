namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.Common;

   [TestClass]
   public class PathDefinitionBuilderTest : PathFixture {
      private PathDefinitionBuilderContext Context { get; set; }
      private PathDefinitionBuilder<EmployeeVMDescriptor> Builder { get; set; }

      private QualifiedProperties SinglePath {
         get { return Context.Paths.Single(); }
      }

      [TestInitialize]
      public void Setup() {
         Context = new PathDefinitionBuilderContext();
         Builder = new PathDefinitionBuilder<EmployeeVMDescriptor>(Context);
      }

      [TestMethod]
      public void Descendants_AddsCorrectStepsToPath() {
         Builder
            .Descendant(x => x.Projects)
            .Descendant(x => x.SelectedCustomer);

         Assert.AreEqual(2, SinglePath.Path.Length);
      }

      [TestMethod]
      public void Properties_SetsSelectedPropertiesCorrectly() {
         Builder.Properties(x => x.Name, x => x.Projects);

         Assert.AreEqual(2, SinglePath.Properties.Length);
      }
   }
}