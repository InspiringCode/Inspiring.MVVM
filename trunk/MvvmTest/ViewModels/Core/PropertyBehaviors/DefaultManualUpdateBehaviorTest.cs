﻿namespace Inspiring.MvvmTest.ViewModels.Core.PropertyBehaviors {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class DefaultManualUpdateBehaviorTest {
      [TestMethod]
      public void UpdateFromSource_OnPropertyChangedGetsCalled() {
         // Arrange
         VMProperty<string> property = new VMProperty<string>();

         BehaviorInitializationContext initContext = new BehaviorInitializationContext(
            new FieldDefinitionCollection(),
            property
         );

         IManuelUpdateBehavior behavior = new DefaultManualUpdateBehavior<string>();
         behavior.Initialize(initContext);

         var contextMock = new Mock<IBehaviorContext>();

         // Act
         behavior.UpdateFromSource(contextMock.Object);

         // Assert
         contextMock.Verify(x => x.RaisePropertyChanged(property), Times.Once());
      }
   }
}