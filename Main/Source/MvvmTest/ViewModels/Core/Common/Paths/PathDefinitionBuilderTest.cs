namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspiring.Mvvm.ViewModels.Core;
using Inspiring.Mvvm.Common;

   [TestClass]
   public class PathDefinitionBuilderTest : PathFixture {
      private Reference<PathDefinition> SelectedPath { get; set; }
      private Reference<IPropertySelector[]> SelectedProperties { get; set; }
      private PathDefinitionBuilder<EmployeeVMDescriptor> Builder { get; set; }

      [TestInitialize]
      public void Setup() {
         SelectedPath = new Reference<PathDefinition>(PathDefinition.Empty);
         SelectedProperties = new Reference<IPropertySelector[]>();
         Builder = new PathDefinitionBuilder<EmployeeVMDescriptor>(SelectedPath, SelectedProperties);
      }

      [TestMethod]
      public void Descendants_AddsCorrectStepsToPath() {
         Builder
            .Descendant(x => x.Projects)
            .Descendant(x => x.SelectedCustomer);

         Assert.AreEqual(2, SelectedPath.Value.Length);
      }

      [TestMethod]
      public void Properties_SetsSelectedPropertiesCorrectly() {
         Builder.Properties(x => x.Name, x => x.Projects);

         Assert.AreEqual(2, SelectedProperties.Value.Length);
      }
   }
}