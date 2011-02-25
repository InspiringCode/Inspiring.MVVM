namespace Inspiring.MvvmContribTest.ViewModels {
   using System;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EnumLocalizerTest {
      [TestInitialize]
      public void Setup() {
         EnumLocalizer.AddLocalizationResource(EnumLocalizations.ResourceManager);
      }

      [TestMethod]
      public void GetCaption() {
         string actual = EnumLocalizer.GetCaption(PersonStatus.Dismissed);
         Assert.AreEqual(EnumLocalizations.PersonStatus_Dismissed, actual);

         actual = EnumLocalizer.GetCaption(PersonStatus.None);
         Assert.AreEqual(EnumLocalizations.PersonStatus_None, actual);
      }

      [TestMethod]
      public void GetCaptionOfNonExisting() {
         AssertHelper.Throws<ArgumentException>(
            () => EnumLocalizer.GetCaption(TestEnum.First)
         ).Containing(ExceptionTexts
            .EnumLocalizationNotFound
            .FormatWith("TestEnum", "First")
         );
      }

      private enum TestEnum {
         First
      }
   }
}