﻿//using System;
//using System.Collections.Generic;
//using Inspiring.Mvvm.ViewModels;
//using Inspiring.Mvvm.ViewModels.Core;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace Inspiring.MvvmTest.ViewModels.Behaviors {
//   [TestClass]
//   public class VMCollectionPropertyTest : TestBase {
//      private Mock<IPropertyAccessorBehavior<IEnumerable<Person>>> _sourceAccessor;
//      private Mock<IPropertyAccessorBehavior<VMCollection<PersonVM>>> _vmCollectionAccesor;
//      private IBehaviorContext _context;
//      private VMCollection<PersonVM> _vmCollection;
//      private CollectionPopulatorBehavior<PersonVM, PersonVM, Person> _populator;
//      private int _vmCollectionAccessorCalls;
//      private int _sourceAccessorCalls;
//      private FieldDefinitionCollection _fieldDefinitions;
//      private FieldValueHolder _fieldValues;

//      private Person[] _sourceCollection = new Person[] {
//         new Person { FirstName = "First" },
//         new Person { FirstName = "Second" }
//      };

//      [TestInitialize]
//      public void Setup() {
//         _fieldDefinitions = new FieldDefinitionCollection();
//         _fieldValues = _fieldDefinitions.CreateValueHolder();
//         _vmCollection = new VMCollection<PersonVM>(null, PersonVM.Descriptor);
//         SetupSourceAccesor();
//         SetupContext();
//         SetupVMCollectionAccessor();
//         SetupPopulator();
//         _vmCollectionAccessorCalls = 0;
//         _sourceAccessorCalls = 0;
//      }

//      // [TestMethod] // TODO
//      public void GetValue() {
//         _populator.Successor = _vmCollectionAccesor.Object;

//         VerifySourceAccessorNotCalled();
//         VerifyVMCollectionAccessorNotCalled();

//         VMCollection<PersonVM> coll = _populator.GetValue(_context, ValueStage.PostValidation);
//         Assert.AreEqual(_vmCollection, coll);
//         Assert.AreEqual(2, coll.Count);
//         Assert.AreEqual(_sourceCollection[0], coll[0].Person);
//         Assert.AreEqual(_sourceCollection[1], coll[1].Person);

//         VerifySourceAccessorCalled();
//         VerifyVMCollectionAccessorCalled();

//         coll = _populator.GetValue(_context, ValueStage.PostValidation);
//         Assert.AreEqual(_vmCollection, coll);
//         Assert.AreEqual(2, coll.Count);

//         VerifySourceAccessorCalled();
//         VerifyVMCollectionAccessorCalled();
//      }

//      [TestMethod]
//      public void SetValue() {
//         AssertHelper.Throws<NotSupportedException>(() => {
//            _populator.SetValue(_context, new VMCollection<PersonVM>(null, PersonVM.Descriptor));
//         });
//      }

//      // [TestMethod] // TODO
//      public void TestCollectionProperty() {
//         var prop = new VMCollectionProperty<PersonVM>();
//         prop.Behaviors = new Behavior { Successor = _populator };
//         //_.Initialize(new BehaviorInitializationContext("Test", _fieldDefinitions));

//         VerifySourceAccessorNotCalled();
//         VerifyVMCollectionAccessorNotCalled();

//         VMCollection<PersonVM> coll = prop.GetValue(_context);
//         Assert.AreNotEqual(_vmCollection, coll);
//         Assert.AreEqual(2, coll.Count);
//         Assert.AreEqual(_sourceCollection[0], coll[0].Person);
//         Assert.AreEqual(_sourceCollection[1], coll[1].Person);

//         VerifySourceAccessorCalled();
//         VerifyVMCollectionAccessorNotCalled();

//         VMCollection<PersonVM> secondColl = prop.GetValue(_context);
//         Assert.AreEqual(coll, secondColl);
//         Assert.AreEqual(2, coll.Count);

//         VerifySourceAccessorNotCalled();
//         VerifyVMCollectionAccessorNotCalled();
//      }

//      private void VerifySourceAccessorCalled() {
//         _sourceAccessorCalls++;
//         _sourceAccessor.Verify(
//            x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>()),
//            Times.Exactly(_sourceAccessorCalls)
//         );
//      }

//      private void VerifyVMCollectionAccessorCalled() {
//         _vmCollectionAccessorCalls++;
//         _vmCollectionAccesor.Verify(
//            x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>()),
//            Times.Exactly(_vmCollectionAccessorCalls)
//         );
//      }

//      private void VerifySourceAccessorNotCalled() {
//         _sourceAccessor.Verify(
//            x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>()),
//            Times.Exactly(_sourceAccessorCalls)
//         );
//      }

//      private void VerifyVMCollectionAccessorNotCalled() {
//         _vmCollectionAccesor.Verify(
//            x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>()),
//            Times.Exactly(_vmCollectionAccessorCalls)
//         );
//      }



//      private void SetupSourceAccesor(IEnumerable<Person> sourceCollection = null) {
//         sourceCollection = sourceCollection ?? _sourceCollection;
//         _sourceAccessor = new Mock<IPropertyAccessorBehavior<IEnumerable<Person>>>(MockBehavior.Strict);
//         _sourceAccessor
//            .Setup(x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>()))
//            .Returns(sourceCollection);

//         _sourceAccessor
//            .Setup(x => x.Successor)
//            .Returns((IBehavior)null);
//      }

//      private void SetupContext() {
//         var mock = new Mock<IBehaviorContext>();
//         mock.Setup(x => x.FieldValues).Returns(_fieldValues);
//         _context = mock.Object;
//      }

//      private void SetupVMCollectionAccessor() {
//         _vmCollectionAccesor = new Mock<IPropertyAccessorBehavior<VMCollection<PersonVM>>>(MockBehavior.Strict);
//         _vmCollectionAccesor.Setup(x => x.GetValue(It.IsAny<IBehaviorContext>(), It.IsAny<ValueStage>())).Returns(_vmCollection);
//         _vmCollectionAccesor
//            .Setup(x => x.Successor)
//            .Returns(_sourceAccessor.Object);
//      }

//      private void SetupPopulator() {
//         _populator = new CollectionPopulatorBehavior<PersonVM, PersonVM, Person>();
//         _populator.Successor = _sourceAccessor.Object;
//      }
//   }
//}